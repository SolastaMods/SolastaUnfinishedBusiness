using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.SrdAndHouseRules.SorcererTwinnedLogic
{
    // fix twinned spells offering
    [HarmonyPatch(typeof(RulesetImplementationManagerLocation), "IsMetamagicOptionAvailable")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class RulesetImplementationManagerLocation_IsMetamagicOptionAvailable
    {
        private static readonly string[] AllowedSpellsIfHeroBelowLevel5 =
        {
            "EldritchBlast", "EldritchBlastGraspingHand", "EldritchBlastRepellingBlast"
        };

        private static readonly string[] AllowedSpellsIfNotUpcast =
        {
            // level 1
            "CharmPerson", "Longstrider",

            // level 2
            "Blindness", "HoldPerson", "Invisibility",

            // level 3
            "Fly",

            // level 4
            "Banishment",

            // level 5
            "HoldMonster"
        };

        internal static void Postfix(
            ref bool __result,
            RulesetEffectSpell rulesetEffectSpell,
            RulesetCharacter caster,
            MetamagicOptionDefinition metamagicOption,
            ref string failure)
        {
            if (!Main.Settings.FixSorcererTwinnedLogic
                || metamagicOption != SolastaModApi.DatabaseHelper.MetamagicOptionDefinitions.MetamagicTwinnedSpell
                || caster is not RulesetCharacterHero hero)
            {
                return;
            }

            var spellDefinition = rulesetEffectSpell.SpellDefinition;

            if (Array.IndexOf(AllowedSpellsIfHeroBelowLevel5, spellDefinition) == -1
                && Array.IndexOf(AllowedSpellsIfNotUpcast, spellDefinition) == -1)
            {
                return;
            }

            var isAllowedIfNotUpCastSpell = Array.Exists(AllowedSpellsIfNotUpcast, x => x == spellDefinition.Name);
            var isAllowedIfHeroBelowLevel5Spell =
                Array.Exists(AllowedSpellsIfHeroBelowLevel5, x => x == spellDefinition.Name);

            var spellLevel = spellDefinition.SpellLevel;
            var slotLevel = rulesetEffectSpell.SlotLevel;

            var rulesetSpellRepertoire = rulesetEffectSpell.SpellRepertoire;
            var classLevel = rulesetSpellRepertoire.KnownCantrips.Contains(spellDefinition)
                ? hero.ClassesHistory.Count
                : rulesetEffectSpell.GetClassLevel(caster);

            if ((isAllowedIfNotUpCastSpell && spellLevel == slotLevel) ||
                (isAllowedIfHeroBelowLevel5Spell && classLevel < 5))
            {
                return;
            }

            var postfix = "";

            if (!isAllowedIfHeroBelowLevel5Spell)
            {
                postfix = " above level 4";
            }
            else if (!isAllowedIfNotUpCastSpell)
            {
                postfix = " and upcasted";
            }

            failure = $"Cannot be twinned{postfix}";

            __result = false;
        }
    }
}
