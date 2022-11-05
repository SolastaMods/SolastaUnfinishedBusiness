using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class SpellActivationBoxPatcher
{
    [HarmonyPatch(typeof(SpellActivationBox), "BindSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class BindSpell_Patch
    {
        private static bool UniqueLevelSlots(
            FeatureDefinitionCastSpell featureDefinitionCastSpell,
            RulesetCharacter character)
        {
            //PATCH: offers upcast using higher spell slots on Warlock repertoire
            if (character is not RulesetCharacterHero hero || !SharedSpellsContext.IsMulticaster(hero))
            {
                return featureDefinitionCastSpell.UniqueLevelSlots;
            }

            var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(hero);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);

            return featureDefinitionCastSpell.UniqueLevelSlots && sharedSpellLevel <= warlockSpellLevel;
        }

        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var uniqueLevelSlotsMethod = typeof(FeatureDefinitionCastSpell).GetMethod("get_UniqueLevelSlots");
            var myUniqueLevelSlotsMethod =
                new Func<FeatureDefinitionCastSpell, RulesetCharacterHero, bool>(UniqueLevelSlots).Method;

            return instructions.ReplaceCalls(uniqueLevelSlotsMethod, "SpellActivationBoxPatcher.BindSpell_Patch",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, myUniqueLevelSlotsMethod));
        }
    }
}
