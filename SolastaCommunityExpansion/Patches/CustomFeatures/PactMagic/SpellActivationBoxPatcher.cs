using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using UnityEngine.UI;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    // Don't present the upcast menu on SC Warlock
    //[HarmonyPatch(typeof(SpellActivationBox), "BindSpell")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellActivationBox_Bind
    {
        internal static void Postfix(
            RulesetSpellRepertoire spellRepertoire,
            ref bool ___hasUpcast,
            Button ___upcastButton,
            Image ___upcastUpImage,
            Image ___upcastPlusImage,
            Image ___frame,
            List<int> ___higherLevelSlots)
        {
            var isWarlockSpell = SharedSpellsContext.IsWarlock(spellRepertoire.SpellCastingClass);

            if (isWarlockSpell)
            {
                var heroWithSpellRepertoire = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);
                var sharedSpellLevel = SharedSpellsContext.GetSharedSpellLevel(heroWithSpellRepertoire);
                var warlockSpellLevel = SharedSpellsContext.GetWarlockSpellLevel(heroWithSpellRepertoire);

                ___higherLevelSlots.Clear();
 
                for (var i = warlockSpellLevel + 1; i <= sharedSpellLevel; i++)
                {
                    ___higherLevelSlots.Add(i);
                }

                ___hasUpcast = ___higherLevelSlots.Count > 0;
                ___upcastButton.gameObject.SetActive(___hasUpcast);
                //___upcastUpImage.gameObject.SetActive(___hasUpcast);
                //___upcastPlusImage.gameObject.SetActive(___hasUpcast);
                //___frame.gameObject.SetActive(___hasUpcast);
            }
        }
    }
}
