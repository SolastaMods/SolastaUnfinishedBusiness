using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Multiclass.Models
{
    internal static class InspectionPanelContext
    {
        internal const InputCommands.Id PLAIN_UP = (InputCommands.Id)22220005;
        internal const InputCommands.Id PLAIN_DOWN = (InputCommands.Id)22220006;

        private static RulesetCharacterHero selectedHero { get; set; }

        private static int selectedClass { get; set; }

        private static readonly List<string> classesWithDeity = new() { RuleDefinitions.ClericClass, RuleDefinitions.PaladinClass };

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

        internal static bool IsMulticlass => selectedHero?.ClassesAndLevels.Count > 1;

        private static bool RequiresDeity => selectedHero?.DeityDefinition != null && classesWithDeity.Contains(SelectedClass.Name);

        internal static void Load()
        {
            var inputService = ServiceRepository.GetService<IInputService>();

            inputService.RegisterCommand(PLAIN_UP, 273, -1, -1, -1, -1, -1);
            inputService.RegisterCommand(PLAIN_DOWN, 274, -1, -1, -1, -1, -1);
        }

        public static string GetSelectedClassSearchTerm(string original)
        {
            return original + SelectedClass.Name;
        }

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

            if (RequiresDeity)
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

        private static List<FightingStyleDefinition> GetTrainedFightingStyles()
        {
            var fightingStyleIdx = 0;
            var classLevelFightingStyle = new Dictionary<string, FightingStyleDefinition>();
            var classBadges = new List<FightingStyleDefinition>();

            foreach (var activeFeature in selectedHero.ActiveFeatures.Where(x => x.Key.Contains(AttributeDefinitions.TagClass)))
            {
                foreach (var featureDefinition in activeFeature.Value.Where(x => x is FeatureDefinitionFightingStyleChoice featureDefinitionFightingStyleChoice))
                {
                    classLevelFightingStyle.Add(activeFeature.Key, selectedHero.TrainedFightingStyles[fightingStyleIdx++]);
                }
            }

            foreach (var tuple in classLevelFightingStyle.Where(x => x.Key.Contains(SelectedClass.Name)))
            {
                classBadges.Add(tuple.Value);
            }

            return classBadges;
        }

        internal static void PickNextHeroClass()
        {
            selectedClass = selectedClass < selectedHero.ClassesAndLevels.Count - 1 ? selectedClass + 1 : 0;
        }
    }
}
