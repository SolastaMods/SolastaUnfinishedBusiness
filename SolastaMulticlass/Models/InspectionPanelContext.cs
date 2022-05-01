using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using UnityEngine;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;

namespace SolastaMulticlass.Models
{
    internal static class InspectionPanelContext
    {
        internal const InputCommands.Id PLAIN_UP = (InputCommands.Id)22220005;
        internal const InputCommands.Id PLAIN_DOWN = (InputCommands.Id)22220006;

        private static RulesetCharacterHero selectedHero;

        private static int selectedClass;

        internal static RulesetCharacterHero SelectedHero
        {
            get => selectedHero;
            set
            {
                selectedHero = value;
                selectedClass = 0;
            }
        }

        internal static CharacterClassDefinition SelectedClass => selectedHero?.ClassesAndLevels.Keys.ElementAt(selectedClass);

        internal static void Load()
        {
            var inputService = ServiceRepository.GetService<IInputService>();

            inputService.RegisterCommand(PLAIN_UP, 273, -1, -1, -1, -1, -1);
            inputService.RegisterCommand(PLAIN_DOWN, 274, -1, -1, -1, -1, -1);
        }

        public static string GetSelectedClassSearchTerm(string original) => original + SelectedClass.Name;

        public static void EnumerateClassBadges(CharacterInformationPanel __instance)
        {
            var badgeDefinitions = __instance.GetField<CharacterInformationPanel, List<BaseDefinition>>("badgeDefinitions");
            var classBadgesTable = __instance.GetField<CharacterInformationPanel, RectTransform>("classBadgesTable");
            var classBadgePrefab = __instance.GetField<CharacterInformationPanel, GameObject>("classBadgePrefab");

            badgeDefinitions.Clear();

            foreach (var classesAndSubclass in SelectedHero.ClassesAndSubclasses.Where(x => x.Key == SelectedClass))
            {
                badgeDefinitions.Add(classesAndSubclass.Value);
            }

            if (selectedHero?.DeityDefinition != null && (SelectedClass == Paladin || SelectedClass == Cleric))
            {
                badgeDefinitions.Add(SelectedHero.DeityDefinition);
            }

            foreach (var trainedFightingStyle in GetTrainedFightingStyles())
            {
                badgeDefinitions.Add(trainedFightingStyle);
            }

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
            var classLevelFightingStyle = new Dictionary<string, FightingStyleDefinition>();
            var classBadges = new HashSet<FightingStyleDefinition>();

            foreach (var activeFeature in selectedHero.ActiveFeatures
                .Where(x => x.Key.Contains(AttributeDefinitions.TagClass)))
            {
                foreach (var featureDefinition in activeFeature.Value
                    .OfType<FeatureDefinitionFightingStyleChoice>())
                {
                    classLevelFightingStyle.Add(activeFeature.Key, selectedHero.TrainedFightingStyles[fightingStyleIdx++]);
                }
            }

            foreach (var kvp in classLevelFightingStyle
                .Where(x => x.Key.Contains(SelectedClass.Name)))
            {
                classBadges.Add(kvp.Value);
            }

            return classBadges;
        }

        internal static void PickNextHeroClass()
        {
            selectedClass = (selectedClass + 1) % selectedHero.ClassesAndLevels.Count;
        }
    }
}
