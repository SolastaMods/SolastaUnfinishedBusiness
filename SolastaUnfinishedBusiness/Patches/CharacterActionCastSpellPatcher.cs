using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionCastSpellPatcher
{
    [HarmonyPatch(typeof(CharacterActionCastSpell), nameof(CharacterActionCastSpell.ApplyMagicEffect))]
    [HarmonyPatch(
        new[]
        {
            typeof(GameLocationCharacter), typeof(ActionModifier), typeof(int), typeof(int),
            typeof(RuleDefinitions.RollOutcome), typeof(bool), typeof(RuleDefinitions.RollOutcome), typeof(int),
            typeof(int), typeof(bool), typeof(bool)
        },
        new[]
        {
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal,
            ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Ref, ArgumentType.Out,
            ArgumentType.Out
        })]
    [UsedImplicitly]
    public static class ApplyMagicEffect_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionCastSpell __instance,
            GameLocationCharacter target,
            ActionModifier actionModifier,
            int targetIndex,
            int targetCount,
            RuleDefinitions.RollOutcome outcome,
            bool rolledSaveThrow,
            RuleDefinitions.RollOutcome saveOutcome,
            int saveOutcomeDelta,
            ref int damageReceived,
            ref bool damageAbsorbedByTemporaryHitPoints,
            ref bool terminateEffectOnTarget
        )
        {
            //PATCH: re-implements base method to allow `IModifySpellEffectLevel` to provide customized spell effect level

            var activeSpell = __instance.ActiveSpell;
            var effectLevelProvider = activeSpell.SpellDefinition.GetFirstSubFeatureOfType<IModifySpellEffectLevel>();

            if (effectLevelProvider == null)
            {
                return true;
            }

            var actingCharacter = __instance.ActingCharacter;
            var effectLevel = effectLevelProvider.GetEffectLevel(actingCharacter.RulesetActor, activeSpell);

            //Re-implementing CharacterActionMagicEffect.ApplyForms
            var formsParams = new RulesetImplementationDefinitions.ApplyFormsParams();

            formsParams.FillSourceAndTarget(actingCharacter.RulesetCharacter, target.RulesetActor);
            formsParams.FillFromActiveEffect(activeSpell);
            formsParams.FillSpecialParameters(
                rolledSaveThrow,
                __instance.AddDice,
                __instance.AddHP,
                __instance.AddTempHP,
                effectLevel,
                actionModifier,
                saveOutcome,
                saveOutcomeDelta,
                outcome == RuleDefinitions.RollOutcome.CriticalSuccess,
                targetIndex,
                targetCount,
                __instance.TargetItem
            );
            formsParams.effectSourceType = RuleDefinitions.EffectSourceType.Spell;
            formsParams.targetSubstitute = __instance.ActionParams.TargetSubstitute;

            var spellEffectDescription = activeSpell.EffectDescription;
            var rangeType = spellEffectDescription.RangeType;

            if (rangeType is RuleDefinitions.RangeType.MeleeHit or RuleDefinitions.RangeType.RangeHit)
            {
                formsParams.attackOutcome = outcome;
            }

            var actualEffectForms = __instance.actualEffectForms;

            damageReceived = ServiceRepository.GetService<IRulesetImplementationService>()
                .ApplyEffectForms(actualEffectForms[targetIndex],
                    formsParams,
                    __instance.effectiveDamageTypes,
                    out damageAbsorbedByTemporaryHitPoints,
                    effectApplication: spellEffectDescription.EffectApplication,
                    filters: spellEffectDescription.EffectFormFilters,
                    terminateEffectOnTarget: out terminateEffectOnTarget);

            return false;
        }
    }

    [HarmonyPatch(typeof(CharacterActionCastSpell), nameof(CharacterActionCastSpell.GetAdvancementData))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class GetAdvancementData_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: enforces cantrips to be cast at character level (MULTICLASS)
            //replaces repertoire's SpellCastingLevel with character level for cantrips
            var spellCastingLevelMethod = typeof(RulesetSpellRepertoire).GetMethod("get_SpellCastingLevel");
            var spellCastingLevel =
                new Func<RulesetSpellRepertoire, CharacterActionCastSpell, int>(MulticlassContext.SpellCastingLevel)
                    .Method;

            return instructions.ReplaceCalls(spellCastingLevelMethod,
                "CharacterActionCastSpell.GetAdvancementData",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, spellCastingLevel));
        }
    }

    [HarmonyPatch(typeof(CharacterActionCastSpell), nameof(CharacterActionCastSpell.StartConcentrationAsNeeded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class StartConcentrationAsNeeded_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterActionCastSpell __instance)
        {
            //PATCH: BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove
            if (Main.Settings.BestowCurseNoConcentrationRequiredForSlotLevel5OrAbove)
            {
                // Per SRD Bestow Curse does not need concentration when cast with slot level 5 or above
                // If the active spell is a sub-spell of Bestow Curse and the slot level is >= 5 don't run StartConcentrationAsNeeded
                return
                    !__instance.ActiveSpell.SpellDefinition.IsSubSpellOf(DatabaseHelper.SpellDefinitions.BestowCurse)
                    || __instance.ActiveSpell.SlotLevel < 5;
            }

            return true;
        }
    }

    //PATCH: implement IPreventRemoveConcentrationWithPowerUse
    [HarmonyPatch(typeof(CharacterActionCastSpell), nameof(CharacterActionCastSpell.RemoveConcentrationAsNeeded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveConcentrationAsNeeded_Patch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            var currentAction = Global.CurrentAction;

            return currentAction is not CharacterActionUsePower characterActionUsePower || characterActionUsePower
                    .activePower.PowerDefinition.GetFirstSubFeatureOfType<IPreventRemoveConcentrationWithPowerUse>() ==
                null;
        }
    }

    [HarmonyPatch(typeof(CharacterActionCastSpell), nameof(CharacterActionCastSpell.HandleEffectUniqueness))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleEffectUniqueness_Patch
    {
        [UsedImplicitly]
        public static bool Prefix([NotNull] CharacterActionCastSpell __instance)
        {
            //PATCH: terminates all matching spells and powers of same group
            GlobalUniqueEffects.TerminateMatchingUniqueEffect(
                __instance.ActingCharacter.RulesetCharacter,
                __instance.ActiveSpell);

            return false;
        }
    }
}
