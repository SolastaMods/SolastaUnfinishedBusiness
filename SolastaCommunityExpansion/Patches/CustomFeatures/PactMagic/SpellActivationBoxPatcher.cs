using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    // Don't present the upcast menu on SC Warlock
    [HarmonyPatch(typeof(SpellActivationBox), "BindSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellActivationBox_Bind
    {
        internal static void Postfix(
            RulesetSpellRepertoire spellRepertoire, 
            SpellDefinition spellDefinition,
            bool ___hasUpcast, 
            Button ___upcastButton,
            List<int> ___higherLevelSlots)
        {
            var isWarlockSpell = SharedSpellsContext.IsWarlock(spellRepertoire.SpellCastingClass);

            if (___hasUpcast && isWarlockSpell)
            {
                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);
                var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                ___upcastButton.gameObject.SetActive(sharedSpellLevel > warlockSpellLevel);
                ___higherLevelSlots.Clear();

                for (var i = warlockSpellLevel + 1; i <= sharedSpellLevel; i++)
                {
                    ___higherLevelSlots.Add(i);
                }
            }
        }
    }
}
