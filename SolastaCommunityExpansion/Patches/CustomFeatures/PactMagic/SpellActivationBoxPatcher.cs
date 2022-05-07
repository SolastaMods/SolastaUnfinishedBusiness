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
        public static void Postfix(RulesetSpellRepertoire spellRepertoire, bool ___hasUpcast, Button ___upcastButton)
        {
            if (___hasUpcast && SharedSpellsContext.IsWarlock(spellRepertoire.SpellCastingClass))
            {
                var hero = SharedSpellsContext.GetHero(spellRepertoire.CharacterName);

                ___upcastButton.gameObject.SetActive(SharedSpellsContext.IsMulticaster(hero));
            }
        }
    }
}
