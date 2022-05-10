using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    // Don't present the upcast menu on SC Warlock
    [HarmonyPatch(typeof(SpellActivationBox), "BindSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellActivationBox_Bind
    {
        internal static readonly List<int> HigherLevelSlots = new();

        internal static void Prefix(RulesetSpellRepertoire spellRepertoire)
        {
            var isWarlockSpell = SharedSpellsContext.IsWarlock(spellRepertoire.SpellCastingClass);

            HigherLevelSlots.Clear();

            if (!isWarlockSpell)
            {
                return;
            }

            var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);
            var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
            var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

            for (var i = warlockSpellLevel + 1; i <= sharedSpellLevel; i++)
            {
                HigherLevelSlots.Add(i);
            }
        }

        public static void Postfix(
            RulesetSpellRepertoire spellRepertoire,
            bool ___hasUpcast, 
            Button ___upcastButton)
        {
            var isWarlockSpell = SharedSpellsContext.IsWarlock(spellRepertoire.SpellCastingClass);

            if (___hasUpcast && isWarlockSpell)
            {
                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);
                var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                ___upcastButton.gameObject.SetActive(sharedSpellLevel > warlockSpellLevel);
            }
        }
    }

    [HarmonyPatch(typeof(SlotAdvancementPanel), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SlotAdvancementPanel_Bind
    {
        public static void Prefix(List<int> higherLevelSlots)
        {
            if (SpellActivationBox_Bind.HigherLevelSlots.Count > 0)
            {
                higherLevelSlots.SetRange(SpellActivationBox_Bind.HigherLevelSlots);
            }
        }
    }
}
