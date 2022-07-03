using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.LevelUp;

// filters how spells are displayed during level up
[HarmonyPatch(typeof(SpellsByLevelGroup), "BindLearning")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class SpellsByLevelGroup_BindLearning
{
    internal static void FilterMulticlassBleeding(
        SpellsByLevelGroup __instance,
        RulesetCharacterHero caster,
        List<SpellDefinition> allSpells,
        List<SpellDefinition> auToPreparedSpells)
    {
        // avoids auto prepared spells from other classes to bleed in
        var allowedAutoPreparedSpells = LevelUpContext.GetAllowedAutoPreparedSpells(caster)
            .Where(x => x.SpellLevel == __instance.SpellLevel).ToList();

        auToPreparedSpells.SetRange(allowedAutoPreparedSpells);

        // displays known spells from other classes
        var allowedSpells = LevelUpContext.GetAllowedSpells(caster)
            .Where(x => x.SpellLevel == __instance.SpellLevel).ToList();

        if (Main.Settings.DisplayAllKnownSpellsDuringLevelUp)
        {
            var otherClassesKnownSpells = LevelUpContext.GetOtherClassesKnownSpells(caster)
                .Where(x => x.SpellLevel == __instance.SpellLevel).ToList();

            allSpells.RemoveAll(x => !allowedSpells.Contains(x) && !otherClassesKnownSpells.Contains(x));

            // try add to avoid duplicates
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

    private static void CollectAllAutoPreparedSpells(
        SpellsByLevelGroup __instance,
        RulesetCharacterHero hero,
        List<SpellDefinition> allSpells,
        List<SpellDefinition> auToPreparedSpells)
    {
        // Collect all the auto prepared spells.
        // Also filter the prepped spells by level this group is displaying.
        hero.EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(hero.FeaturesToBrowse);

        foreach (var autoPreparedSpells in hero.FeaturesToBrowse.OfType<FeatureDefinitionAutoPreparedSpells>())
        {
            foreach (var spell in from preparedSpellsGroup in autoPreparedSpells.AutoPreparedSpellsGroups
                     from spell in preparedSpellsGroup.SpellsList
                     let flag = !auToPreparedSpells.Contains(spell) && __instance.SpellLevel == spell.SpellLevel
                     where flag
                     select spell)
            {
                auToPreparedSpells.Add(spell);

                // If a spell is not in all spells it won't be shown in the UI.
                // Add the auto prepared spells here to make sure the user sees them.
                allSpells.TryAdd(spell);
            }
        }
    }

    public static bool Prefix(
        SpellsByLevelGroup __instance,
        ICharacterBuildingService characterBuildingService,
        SpellListDefinition spellListDefinition,
        List<string> restrictedSchools,
        int spellLevel,
        SpellBox.SpellBoxChangedHandler spellBoxChanged,
        List<SpellDefinition> knownSpells,
        List<SpellDefinition> unlearnedSpells,
        string spellTag,
        bool canAcquireSpells,
        bool unlearn)
    {
        var heroBuildingData = characterBuildingService.CurrentLocalHeroCharacter?.GetHeroBuildingData();

        __instance.spellsTable.gameObject.SetActive(true);
        __instance.slotStatusTable.gameObject.SetActive(true);

        __instance.SpellLevel = spellLevel;


        var allSpells = spellListDefinition.SpellsByLevel[spellListDefinition.HasCantrips ? spellLevel : spellLevel - 1]
            .Spells
            .Where(spell => restrictedSchools.Count == 0 || restrictedSchools.Contains(spell.SchoolOfMagic)).ToList();

        foreach (var andAcquiredSpell in characterBuildingService.EnumerateKnownAndAcquiredSpells(heroBuildingData,
                     string.Empty).Where(andAcquiredSpell =>
                     andAcquiredSpell.SpellLevel == spellLevel && !allSpells.Contains(andAcquiredSpell)))
        {
            allSpells.Add(andAcquiredSpell);
        }

        if (!spellTag.Contains(AttributeDefinitions.TagRace)) // this is a patch over original TA code
        {
            characterBuildingService.CurrentLocalHeroCharacter
                .EnumerateFeaturesToBrowse<FeatureDefinitionMagicAffinity>(
                    __instance.features);

            foreach (var spell in from FeatureDefinitionMagicAffinity feature in __instance.features
                     where feature.ExtendedSpellList != null
                     from spell in feature.ExtendedSpellList
                         .SpellsByLevel[spellListDefinition.HasCantrips ? spellLevel : spellLevel - 1].Spells
                     where !allSpells.Contains(spell) &&
                           (restrictedSchools.Count == 0 || restrictedSchools.Contains(spell.SchoolOfMagic))
                     select spell)
            {
                allSpells.Add(spell);
            }
        }

        var autoPrepareTag = string.Empty;

        __instance.autoPreparedSpells.Clear();

        if (__instance.SpellLevel > 0)
        {
            characterBuildingService.CurrentLocalHeroCharacter
                .EnumerateFeaturesToBrowse<FeatureDefinitionAutoPreparedSpells>(__instance.features);

            foreach (var feature in __instance.features.OfType<FeatureDefinitionAutoPreparedSpells>())
            {
                autoPrepareTag = feature.AutoPreparedTag;

                foreach (var spells in from preparedSpellsGroup in feature.AutoPreparedSpellsGroups
                         from spells in preparedSpellsGroup.SpellsList
                         where spells.SpellLevel == __instance.SpellLevel
                         select spells)
                {
                    __instance.autoPreparedSpells.Add(spells);
                }

                foreach (var autoPreparedSpell in __instance.autoPreparedSpells.Where(autoPreparedSpell =>
                             !allSpells.Contains(autoPreparedSpell)))
                {
                    allSpells.Add(autoPreparedSpell);
                }
            }
        }

        var service = ServiceRepository.GetService<IGamingPlatformService>();

        for (var index = allSpells.Count - 1; index >= 0; --index)
        {
            if (!service.IsContentPackAvailable(allSpells[index].ContentPack))
            {
                allSpells.RemoveAt(index);
            }
        }

        CollectAllAutoPreparedSpells(__instance, characterBuildingService.CurrentLocalHeroCharacter, allSpells,
            __instance.autoPreparedSpells);

        FilterMulticlassBleeding(__instance, characterBuildingService.CurrentLocalHeroCharacter, allSpells,
            __instance.autoPreparedSpells);

        __instance.CommonBind(null, unlearn ? SpellBox.BindMode.Unlearn : SpellBox.BindMode.Learning, spellBoxChanged,
            allSpells, null, __instance.autoPreparedSpells, unlearnedSpells, autoPrepareTag);

        if (unlearn)
        {
            __instance.RefreshUnlearning(characterBuildingService, knownSpells, unlearnedSpells, spellTag,
                canAcquireSpells && spellLevel > 0);
        }
        else
        {
            __instance.RefreshLearning(characterBuildingService, knownSpells, unlearnedSpells, spellTag,
                canAcquireSpells);
        }

        __instance.slotStatusTable.Bind(null, spellLevel, null, false);

        return false;
    }
}
