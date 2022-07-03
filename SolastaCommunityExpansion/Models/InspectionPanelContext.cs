using System.Collections.Generic;
using System.Linq;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaCommunityExpansion.Models;

public static class InspectionPanelContext
{
    internal static int SelectedClassIndex { get; set; }

    public static CharacterClassDefinition SelectedClass =>
        Global.InspectedHero?.ClassesAndLevels.Keys.ElementAtOrDefault(SelectedClassIndex);

    public static string GetSelectedClassSearchTerm(string original)
    {
        var selectedClass = SelectedClass;
        return original
               + (selectedClass == null
                   ? string.Empty
                   : selectedClass.Name);
    }

    public static void EnumerateClassBadges(CharacterInformationPanel __instance)
    {
        var badgeDefinitions =
            __instance.badgeDefinitions;
        var classBadgesTable = __instance.classBadgesTable;
        var classBadgePrefab = __instance.classBadgePrefab;

        badgeDefinitions.Clear();

        badgeDefinitions.AddRange(Global.InspectedHero.ClassesAndSubclasses.Where(x => x.Key == SelectedClass)
            .Select(classesAndSubclass => classesAndSubclass.Value));

        if (Global.InspectedHero.DeityDefinition != null && (SelectedClass == Paladin || SelectedClass == Cleric))
        {
            badgeDefinitions.Add(Global.InspectedHero.DeityDefinition);
        }

        badgeDefinitions.AddRange(GetTrainedFightingStyles());

        while (classBadgesTable.childCount < badgeDefinitions.Count)
        {
            Gui.GetPrefabFromPool(classBadgePrefab, classBadgesTable);
        }

        var index = 0;

        foreach (var badgeDefinition in badgeDefinitions)
        {
            var child = classBadgesTable.GetChild(index);

            child.gameObject.SetActive(true);
            child.GetComponent<CharacterInformationBadge>().Bind(badgeDefinition, classBadgesTable);
            ++index;
        }

        for (; index < classBadgesTable.childCount; ++index)
        {
            var child = classBadgesTable.GetChild(index);

            child.GetComponent<CharacterInformationBadge>().Unbind();
            child.gameObject.SetActive(false);
        }
    }

    private static HashSet<FightingStyleDefinition> GetTrainedFightingStyles()
    {
        var fightingStyleIdx = 0;
        var classBadges = new HashSet<FightingStyleDefinition>();

        var classLevelFightingStyle =
            (from activeFeature in
                    Global.InspectedHero.ActiveFeatures.Where(x => x.Key.Contains(AttributeDefinitions.TagClass))
                from featureDefinition in activeFeature.Value.OfType<FeatureDefinitionFightingStyleChoice>()
                select activeFeature).ToDictionary(activeFeature => activeFeature.Key,
                activeFeature => Global.InspectedHero.TrainedFightingStyles[fightingStyleIdx++]);

        foreach (var kvp in classLevelFightingStyle
                     .Where(x => x.Key.Contains(SelectedClass.Name)))
        {
            classBadges.Add(kvp.Value);
        }

        return classBadges;
    }
}
