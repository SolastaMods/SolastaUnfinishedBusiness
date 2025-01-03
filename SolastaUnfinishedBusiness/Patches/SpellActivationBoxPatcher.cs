using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class SpellActivationBoxPatcher
{
    [HarmonyPatch(typeof(SpellActivationBox), nameof(SpellActivationBox.BindSpell))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
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
            var pactMaxSlots = SharedSpellsContext.GetWarlockMaxSlots(hero);
            var pactUsedSlots = SharedSpellsContext.GetWarlockUsedSlots(hero);
            var pactAvailableSlots = pactMaxSlots - pactUsedSlots;

            return featureDefinitionCastSpell.UniqueLevelSlots &&
                   // this ensures game does std slot calculation when out of pact slots
                   pactAvailableSlots > 0 &&
                   // this ensures game does std slot calculation if we can upcast warlock spells
                   sharedSpellLevel <= warlockSpellLevel;
        }

        [UsedImplicitly]
        public static void MyGetSlotsNumber(
            RulesetSpellRepertoire repertoire,
            int spellLevel, 
            out int remaining,
            out int max,
            SpellActivationBox spellActivationBox)
        {
            if (Main.Settings.UseAlternateSpellPointsSystem)
            {
                var canCastSpell = SpellPointsContext.CanCastSpellOfLevel(repertoire.GetCaster(), spellLevel);
                
                max = 1; // irrelevant
                remaining = canCastSpell ? 1 : 0;

                if (!canCastSpell)
                {
                    spellActivationBox.hasUpcast = false;
                }
            }
            else
            {
                repertoire.GetSlotsNumber(spellLevel, out remaining, out max);
            }
        }

        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            var uniqueLevelSlotsMethod = typeof(FeatureDefinitionCastSpell).GetMethod("get_UniqueLevelSlots");
            var myUniqueLevelSlotsMethod =
                new Func<FeatureDefinitionCastSpell, RulesetCharacterHero, bool>(UniqueLevelSlots).Method;

            var getSlotsNumberMethod = typeof(RulesetSpellRepertoire).GetMethod("GetSlotsNumber");
            var myGetSlotsNumberMethod = typeof(BindSpell_Patch).GetMethod("MyGetSlotsNumber");

            return instructions
                .ReplaceCalls(getSlotsNumberMethod, "SpellActivationBox.BindSpell.GetSlotsNumber",
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, myGetSlotsNumberMethod))
                .ReplaceCalls(uniqueLevelSlotsMethod, "SpellActivationBox.BindSpell.UniqueLevelSlots",
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new CodeInstruction(OpCodes.Call, myUniqueLevelSlotsMethod));
        }
    }

    //PATCH: register on acting character if SHIFT is pressed on spell box activation
    [HarmonyPatch(typeof(SpellActivationBox), nameof(SpellActivationBox.OnActivateCb))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnActivateCb_Patch
    {
        [UsedImplicitly]
        public static void Prefix(SpellActivationBox __instance)
        {
            if (__instance.spellRepertoire == null)
            {
                return;
            }

            var rulesetCaster = __instance.spellRepertoire.GetCaster();
            var caster = GameLocationCharacter.GetFromActor(rulesetCaster);

            caster?.RegisterShiftState();
        }
    }
}
