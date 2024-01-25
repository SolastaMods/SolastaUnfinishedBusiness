using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomGenericBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

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
}
