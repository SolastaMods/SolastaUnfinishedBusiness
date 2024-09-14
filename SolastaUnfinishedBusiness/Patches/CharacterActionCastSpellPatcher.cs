using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterActionCastSpellPatcher
{
    private static bool RequiresConcentration(
        SpellDefinition spellDefinition,
        CharacterActionCastSpell characterActionCastSpell)
    {
        var rulesetCharacter = characterActionCastSpell.ActingCharacter.RulesetCharacter;
        var rulesetEffectSpell = characterActionCastSpell.ActiveSpell;

        return spellDefinition.RequiresConcentration &&
               rulesetCharacter.GetSubFeaturesByType<IModifyConcentrationRequirement>()
                   .All(modifyConcentrationRequirement =>
                       modifyConcentrationRequirement.RequiresConcentration(rulesetCharacter, rulesetEffectSpell));
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
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: enforces cantrips to be cast at character level (MULTICLASS)
            //replaces repertoire's SpellCastingLevel with character level for cantrips
            var requiresConcentrationMethod = typeof(SpellDefinition).GetMethod("get_RequiresConcentration");
            var myRequiresConcentrationMethod =
                new Func<SpellDefinition, CharacterActionCastSpell, bool>(RequiresConcentration)
                    .Method;

            return instructions.ReplaceCalls(requiresConcentrationMethod,
                "CharacterActionCastSpell.StartConcentrationAsNeeded",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myRequiresConcentrationMethod));
        }

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
                    || __instance.ActiveSpell.EffectLevel < 5;
            }

            return true;
        }
    }

    //PATCH: implement IPreventRemoveConcentrationOnPowerUse
    [HarmonyPatch(typeof(CharacterActionCastSpell), nameof(CharacterActionCastSpell.RemoveConcentrationAsNeeded))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RemoveConcentrationAsNeeded_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var requiresConcentrationMethod = typeof(SpellDefinition).GetMethod("get_RequiresConcentration");
            var myRequiresConcentrationMethod =
                new Func<SpellDefinition, CharacterActionCastSpell, bool>(RequiresConcentration)
                    .Method;

            return instructions.ReplaceCalls(requiresConcentrationMethod,
                "CharacterActionCastSpell.RemoveConcentrationAsNeeded",
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, myRequiresConcentrationMethod));
        }

        [UsedImplicitly]
        public static bool Prefix(CharacterActionCastSpell __instance)
        {
            var character = __instance.ActingCharacter.RulesetCharacter;

            return !CharacterActionExtensions
                .ShouldKeepConcentrationOnPowerUseOrSpend(character); // abort if should keep
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
            ForceGlobalUniqueEffects.TerminateMatchingUniqueEffect(
                __instance.ActingCharacter.RulesetCharacter,
                __instance.ActiveSpell);

            return false;
        }
    }

    //PATCH: allow check reactions on cast spell regardless of success / failure
    [HarmonyPatch(typeof(CharacterActionCastSpell), nameof(CharacterActionCastSpell.CounterEffectAction))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class CounterEffectAction_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ref IEnumerator __result, CharacterActionCastSpell __instance,
            CharacterAction counterAction)
        {
            __result = Process(__instance, counterAction);

            return false;
        }

        private static IEnumerator Process(
            CharacterActionCastSpell characterActionCastSpell,
            CharacterAction counterAction)
        {
            var targetAction = characterActionCastSpell.ActionParams.TargetAction;

            if (targetAction == null)
            {
                yield break;
            }

            var actingCharacter = characterActionCastSpell.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var actionParams = characterActionCastSpell.ActionParams;
            var targetActionParams = targetAction.ActionParams;
            var counteredSpell = targetActionParams.RulesetEffect as RulesetEffectSpell;
            var counteredSpellDefinition = counteredSpell!.SpellDefinition;
            var slotLevel = counteredSpell.SlotLevel;
            var actionModifier = actionParams.ActionModifiers[0];

            foreach (var counterForm in actionParams.RulesetEffect.EffectDescription.EffectForms
                         .Where(effectForm => effectForm.FormType == EffectForm.EffectFormType.Counter)
                         .Select(effectForm => effectForm.CounterForm))
            {
                if (counterForm.AutomaticSpellLevel + characterActionCastSpell.addSpellLevel >= slotLevel)
                {
                    targetAction.Countered = true;
                }
                else if (counterForm.CheckBaseDC != 0)
                {
                    var checkDC = counterForm.CheckBaseDC + slotLevel;

                    rulesetCharacter
                        .EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(rulesetCharacter.FeaturesToBrowse);

                    foreach (var featureDefinition in rulesetCharacter.FeaturesToBrowse)
                    {
                        var definitionMagicAffinity = (FeatureDefinitionMagicAffinity)featureDefinition;

                        if (definitionMagicAffinity.CounterspellAffinity == AdvantageType.None)
                        {
                            continue;
                        }

                        var advTrend = definitionMagicAffinity.CounterspellAffinity == AdvantageType.Advantage
                            ? 1
                            : -1;

                        actionModifier.AbilityCheckAdvantageTrends.Add(new TrendInfo(
                            advTrend, FeatureSourceType.CharacterFeature, definitionMagicAffinity.Name, null));
                    }

                    if (counteredSpell.CounterAffinity != AdvantageType.None)
                    {
                        var advTrend = counteredSpell.CounterAffinity == AdvantageType.Advantage
                            ? 1
                            : -1;

                        actionModifier.AbilityCheckAdvantageTrends.Add(new TrendInfo(
                            advTrend, FeatureSourceType.CharacterFeature, counteredSpell.CounterAffinityOrigin, null));
                    }

                    var proficiencyName = string.Empty;

                    if (counterForm.AddProficiencyBonus)
                    {
                        proficiencyName = "ForcedProficiency";
                    }

                    var abilityCheckRoll = actingCharacter.RollAbilityCheck(
                        characterActionCastSpell.activeSpell.SpellRepertoire.SpellCastingAbility,
                        proficiencyName,
                        checkDC,
                        AdvantageType.None,
                        actionModifier,
                        false,
                        0,
                        out var outcome,
                        out var successDelta,
                        true);

                    var abilityCheckData = new AbilityCheckData
                    {
                        AbilityCheckRoll = abilityCheckRoll,
                        AbilityCheckRollOutcome = outcome,
                        AbilityCheckSuccessDelta = successDelta,
                        AbilityCheckActionModifier = actionModifier,
                        Action = characterActionCastSpell
                    };

                    yield return TryAlterOutcomeAttributeCheck
                        .HandleITryAlterOutcomeAttributeCheck(actingCharacter, abilityCheckData);

                    characterActionCastSpell.AbilityCheckRoll = abilityCheckData.AbilityCheckRoll;
                    characterActionCastSpell.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckRollOutcome;
                    characterActionCastSpell.AbilityCheckSuccessDelta = abilityCheckData.AbilityCheckSuccessDelta;

                    if (counterAction.AbilityCheckRollOutcome == RollOutcome.Success)
                    {
                        targetAction.Countered = true;
                    }
                }

                if (!targetAction.Countered ||
                    rulesetCharacter.SpellCounter == null)
                {
                    continue;
                }

                var unknown = string.IsNullOrEmpty(counteredSpell.IdentifiedBy);

                characterActionCastSpell.ActingCharacter.RulesetCharacter.SpellCounter(
                    rulesetCharacter,
                    targetAction.ActingCharacter.RulesetCharacter,
                    counteredSpellDefinition,
                    targetAction.Countered,
                    unknown);
            }
        }
    }
}
