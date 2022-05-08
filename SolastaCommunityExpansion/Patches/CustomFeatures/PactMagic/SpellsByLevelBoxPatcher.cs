using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.PactMagic
{
    // Guarantee Warlock Spell Level will be used whenever possible on SC Warlocks
    [HarmonyPatch(typeof(SpellsByLevelBox), "OnActivateStandardBox")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellsByLevelBox_OnActivateStandardBox
    {
        public static bool Prefix(
            int index,
            SpellsByLevelBox.SpellCastEngagedHandler ___spellCastEngaged,
            Dictionary<int, SpellDefinition> ___spellsByIndex,
            RulesetCharacter ___caster,
            RulesetSpellRepertoire ___spellRepertoire,
            List<SpellActivationBox> ___activationBoxes)
        {
            // PATCH HERE
            Global.CastedSpell = ___spellsByIndex[index];
            Global.CastedSpellRepertoire = ___spellRepertoire;

            if (___spellCastEngaged == null)
            {
                return false;
            }

            var spellDefinition = ___spellsByIndex[index];
            var spellLevel = spellDefinition.SpellLevel;
            var hero = ___caster as RulesetCharacterHero;

            // PATCH HERE
            if (SharedSpellsContext.IsWarlock(___spellRepertoire.SpellCastingClass)
                && !SharedSpellsContext.IsMulticaster(hero)
                && spellDefinition.SpellLevel > 0
                && ___spellRepertoire.CanUpcastSpell(spellDefinition))
            {
                spellLevel = SharedSpellsContext.GetWarlockSpellLevel(hero);
            }
            // END PATCH

            if (spellDefinition.SpellsBundle)
            {
                SubspellSelectionModal screen = Gui.GuiService.GetScreen<SubspellSelectionModal>();
                screen.Bind(spellDefinition, ___caster, ___spellRepertoire, ___spellCastEngaged, spellLevel, ___activationBoxes[index].RectTransform);
                screen.Show();
            }
            else
            {
                ___spellCastEngaged(___spellRepertoire, spellDefinition, spellLevel);
            }

            return false;
        }
    }

    // Guarantee Warlock Spell Level will be used whenever possible on SC Warlocks
    [HarmonyPatch(typeof(SpellsByLevelBox), "OnActivateAdvancedBox")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellsByLevelBox_OnActivateAdvancedBox
    {
        public static void Prefix(
            int index,
            RulesetSpellRepertoire ___spellRepertoire,
            Dictionary<int, SpellDefinition> ___spellsByIndex)
        {
            Global.CastedSpell = ___spellsByIndex[index];
            Global.CastedSpellRepertoire = ___spellRepertoire;
        }
    }
}
