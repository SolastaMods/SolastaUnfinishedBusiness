using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageFightingStyleSelectionPanelPatcher
{
    [HarmonyPatch(typeof(CharacterStageFightingStyleSelectionPanel),
        nameof(CharacterStageFightingStyleSelectionPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CharacterStageFightingStyleSelectionPanel __instance)
        {
            //PATCH: changes the fighting style layout to allow more offerings
            var table = __instance.fightingStylesTable;
            var gridLayoutGroup = table.GetComponent<GridLayoutGroup>();
            var rectTransform = table.GetComponent<RectTransform>();
            var count = __instance.compatibleFightingStyles.Count;

            switch (count)
            {
                case > 15:
                {
                    gridLayoutGroup.constraintCount = 4;
                    rectTransform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
                    break;
                }
                case > 12:
                    gridLayoutGroup.constraintCount = 3;
                    rectTransform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
                    break;
                case > 6:
                    gridLayoutGroup.constraintCount = 3;
                    rectTransform.localScale = Vector3.one;
                    break;
                default:
                    gridLayoutGroup.constraintCount = 2;
                    rectTransform.localScale = Vector3.one;
                    break;
            }

            //PATCH: sorts the fighting style panel by Title
            if (!Main.Settings.EnableSortingFightingStyles)
            {
                return;
            }

            __instance.compatibleFightingStyles
                .Sort((a, b) =>
                    String.Compare(a.FormatTitle(), b.FormatTitle(), StringComparison.CurrentCultureIgnoreCase));
        }
    }

    [HarmonyPatch(typeof(CharacterStageFightingStyleSelectionPanel),
        nameof(CharacterStageFightingStyleSelectionPanel.TryGetFightingStyleChoiceFeature))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class TryGetFightingStyleChoiceFeature_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            [NotNull] CharacterStageFightingStyleSelectionPanel __instance,
            ref bool __result,
            ref FeatureDefinitionFightingStyleChoice fightingStyleChoiceFeature)
        {
            //PATCH: allow fighting styles to be granted from subs
            if (fightingStyleChoiceFeature)
            {
                return;
            }

            var hero = __instance.currentHero;
            var lastGainedSubclass = LevelUpContext.GetSelectedSubclass(hero);

            if (!lastGainedSubclass)
            {
                return;
            }

            var tag = AttributeDefinitions.GetSubclassTag(
                __instance.lastGainedClassDefinition,
                __instance.lastGainedClassLevel, lastGainedSubclass);

            if (hero.ActiveFeatures.TryGetValue(tag, out var value))
            {
                fightingStyleChoiceFeature = value.OfType<FeatureDefinitionFightingStyleChoice>()
                    .FirstOrDefault();
            }

            __result = fightingStyleChoiceFeature;
        }
    }
}
