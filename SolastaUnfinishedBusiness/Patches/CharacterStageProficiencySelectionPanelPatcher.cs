using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using static HeroDefinitions.PointsPoolType;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterStageProficiencySelectionPanelPatcher
{
    private static LearnStepItem CurrentStepItem(CharacterStageProficiencySelectionPanel __instance)
    {
        var table = __instance.learnStepsTable;
        LearnStepItem item = null;

        for (var i = 0; i < table.childCount; i++)
        {
            var child = table.GetChild(i);

            if (!child.gameObject.activeSelf || i != __instance.currentLearnStep)
            {
                continue;
            }

            item = child.GetComponent<LearnStepItem>();
            break;
        }

        return item;
    }

    [HarmonyPatch(typeof(CharacterStageProficiencySelectionPanel),
        nameof(CharacterStageProficiencySelectionPanel.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix(CharacterStageProficiencySelectionPanel __instance)
        {
            //PATCH: support for skipping skill and tool proficiency picking if you picked all available, but still have points remaining
            var item = CurrentStepItem(__instance);

            if (!item)
            {
                return;
            }

            var hero = __instance.currentHero;
            var buildingData = hero.GetHeroBuildingData();
            var service = ServiceRepository.GetService<ICharacterBuildingService>();
            var needSkip = false;
            var pool = service.GetPointPoolOfTypeAndTag(buildingData, item.PoolType, item.Tag);

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (item.PoolType)
            {
                case Skill:
                {
                    if (DatabaseRepository.GetDatabase<SkillDefinition>()
                        .All(s => service.IsSkillKnownOrTrained(buildingData, s)))
                    {
                        needSkip = true;
                    }

                    break;
                }
                case Tool:
                {
                    if (DatabaseRepository
                        //get all restricted tools
                        .GetDatabase<ToolTypeDefinition>()
                        //remove ones already known or trained this level
                        .Where(s =>
                            pool.RestrictedChoices == null ||
                            pool.RestrictedChoices.Count == 0 ||
                            pool.RestrictedChoices.Contains(s.Name))
                        .All(s => service.IsToolTypeKnownOrTrained(buildingData, s)))
                    {
                        needSkip = true;
                    }

                    break;
                }
            }

            if (!needSkip)
            {
                return;
            }

            item.ignoreAvailable = true;
            item.Refresh(LearnStepItem.Status.InProgress);
        }
    }

    [HarmonyPatch(typeof(CharacterStageProficiencySelectionPanel),
        nameof(CharacterStageProficiencySelectionPanel.OnLearnAutoImpl))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnLearnAutoImpl_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterStageProficiencySelectionPanel __instance, Random rng)
        {
            //PATCH: support for skipping skill and tool proficiency picking if you picked all available, but still have points remaining
            if (rng != null)
            {
                return true;
            }

            var item = CurrentStepItem(__instance);

            if (!item || !item.ignoreAvailable || (item.PoolType != Skill && item.PoolType != Tool))
            {
                return true;
            }

            var hero = __instance.currentHero;
            var buildingData = hero.GetHeroBuildingData();

            var heroBuildingCommandService = ServiceRepository.GetService<IHeroBuildingCommandService>();

            heroBuildingCommandService.AcknowledgePreviousCharacterBuildingCommandLocally(() =>
            {
                __instance.CharacterBuildingService
                    .GetPoolPointsOfTypeAndTag(buildingData, item.PoolType, item.Tag, out _, out _);
                __instance.OnPreRefresh();
                __instance.RefreshNow();
                __instance.MoveToNextLearnStep();
                __instance.ResetWasClickedFlag();
            });

            return false;
        }
    }

    //PATCH: allow refreshing custom metamagic options to avoid requires restart when tweaking mod ui options
    [HarmonyPatch(typeof(CharacterStageProficiencySelectionPanel),
        nameof(CharacterStageProficiencySelectionPanel.EnterStage))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class EnterStage_Patch
    {
        [UsedImplicitly]
        public static void Prefix(CharacterStageProficiencySelectionPanel __instance)
        {
            GameUiContext.RefreshMetamagicOffering(__instance.metamagicSubPanel);
        }
    }
}
