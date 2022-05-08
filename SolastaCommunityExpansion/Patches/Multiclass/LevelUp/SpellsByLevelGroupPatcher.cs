using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.Multiclass.LevelUp
{
    // filters how spells are displayed during level up
    [HarmonyPatch(typeof(SpellsByLevelGroup), "CommonBind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SpellsByLevelGroup_CommonBind
    {
        /*
         * So this patch seems like it should be unnecessary, however when collecting the autoprepared spells TA only looks at the FIRST
         * feature returned from enumerate features rather than iterate over all of them. This means only 1 feature worth of auto prepared
         * spells are shown to the user during level up. For the inspection/spell preparation binding a different method to collect the auto
         * prepared spells is used which works properly.
         */
        private static void CollectAllAutoPreparedSpells(
            SpellsByLevelGroup __instance,
            SpellBox.BindMode bindMode,
            RulesetCharacter caster,
            List<SpellDefinition> allSpells,
            List<SpellDefinition> auToPreparedSpells)
        {
            if (!Main.Settings.ShowAllAutoPreparedSpells)
            {
                return;
            }

            // Collect all the auto prepared spells.
            // Also filter the prepped spells by level this group is displaying.
            caster.EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(caster.FeaturesToBrowse);

            foreach (var autoPreparedSpells in caster.FeaturesToBrowse.OfType<FeatureDefinitionAutoPreparedSpells>())
            {
                foreach (FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup preparedSpellsGroup in autoPreparedSpells.AutoPreparedSpellsGroups)
                {
                    foreach (SpellDefinition spell in preparedSpellsGroup.SpellsList)
                    {
                        var flag = !auToPreparedSpells.Contains(spell) && __instance.SpellLevel == spell.SpellLevel;

                        if (flag)
                        {
                            auToPreparedSpells.Add(spell);

                            // If a spell is not in all spells it won't be shown in the UI.
                            // Add the auto prepared spells here to make sure the user sees them.
                            allSpells.TryAdd(spell);
                        }
                    }
                }
            }
        }

        // there is indeed a camel case typo on auto prepared spells parameter
        internal static void Prefix(
            SpellsByLevelGroup __instance,
            SpellBox.BindMode bindMode,
            List<SpellDefinition> allSpells,
            List<SpellDefinition> auToPreparedSpells)
        {
            if (bindMode == SpellBox.BindMode.Preparation || bindMode == SpellBox.BindMode.Inspection)
            {
                return;
            }

            var hero = Global.ActiveLevelUpHero;

            if (hero == null)
            {
                return;
            }

            CollectAllAutoPreparedSpells(__instance, bindMode, hero, allSpells, auToPreparedSpells);

            if (!Main.Settings.EnableMulticlass)
            {
                return;
            }

            // avoids auto prepared spells from other classes to bleed in
            var allowedAutoPreparedSpells = LevelUpContext.GetAllowedAutoPreparedSpells(hero)
                .Where(x => x.SpellLevel == __instance.SpellLevel).ToList();

            auToPreparedSpells.SetRange(allowedAutoPreparedSpells);

            // displays known spells from other classes
            var allowedSpells = LevelUpContext.GetAllowedSpells(hero)
                .Where(x => x.SpellLevel == __instance.SpellLevel).ToList();

            if (Main.Settings.DisplayAllKnownSpellsDuringLevelUp)
            {
                var otherClassesKnownSpells = LevelUpContext.GetOtherClassesKnownSpells(hero)
                    .Where(x => x.SpellLevel == __instance.SpellLevel).ToList();

                allSpells.RemoveAll(x => !allowedSpells.Contains(x) && !otherClassesKnownSpells.Contains(x));

                // try add to avoid dups
                foreach (var spell in otherClassesKnownSpells)
                {
                    allSpells.TryAdd(spell);

                    if (!Main.Settings.EnableRelearnSpells || !allowedSpells.Contains(spell))
                    {
                        auToPreparedSpells.TryAdd(spell);
                    }
                }
            }
            // remove spells bleed from other classes
            else
            {
                allSpells.RemoveAll(x => !allowedSpells.Contains(x));
            }
        }
    }
}
