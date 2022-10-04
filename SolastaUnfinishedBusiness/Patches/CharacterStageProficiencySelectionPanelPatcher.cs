using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using static HeroDefinitions.PointsPoolType;

namespace SolastaUnfinishedBusiness.Patches;

public static class CharacterStageProficiencySelectionPanelPatcher
{
    private static LearnStepItem CurrentStepItem(CharacterStageProficiencySelectionPanel __instance)
    {
        var table = __instance.learnStepsTable;
        LearnStepItem item = null;
        for (var i = 0; i < table.childCount; i++)
        {
            var child = table.GetChild(i);
            if (child.gameObject.activeSelf && i == __instance.currentLearnStep)
            {
                item = child.GetComponent<LearnStepItem>();
                break;
            }
        }

        return item;
    }

    [HarmonyPatch(typeof(CharacterStageProficiencySelectionPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class Refresh_Patch
    {
        public static void Postfix(CharacterStageProficiencySelectionPanel __instance)
        {
            //PATCH: support for skipping skill and tool proficiency picking if you picked all available, but still have points remaining
            var item = CurrentStepItem(__instance);

            if (item == null)
            {
                return;
            }

            var hero = __instance.currentHero;
            var buildingData = hero.GetHeroBuildingData();
            var service = ServiceRepository.GetService<ICharacterBuildingService>();
            var needSkip = false;
            var pool = service.GetPointPoolOfTypeAndTag(buildingData, item.PoolType, item.Tag);

            if (item.PoolType == Skill)
            {
                //get all skils - unlike tools if you run out ofn restricted skills to pick, game allows picking any skill
                if (DatabaseRepository.GetDatabase<SkillDefinition>()
                        //remove skills already knows or trained this level
                        .Where(s => !service.IsSkillKnownOrTrained(buildingData, s))
                        .Count() == 0)
                {
                    needSkip = true;
                }
            }
            else if (item.PoolType == Tool)
            {
                if (DatabaseRepository.GetDatabase<ToolTypeDefinition>()
                        //get all restricted tools
                        .Where(s => pool.RestrictedChoices == null
                                    || pool.RestrictedChoices.Empty()
                                    || pool.RestrictedChoices.Contains(s.Name))
                        //remove ones already known or trained this level
                        .Where(s => !service.IsToolTypeKnownOrTrained(buildingData, s))
                        .Count() == 0)
                {
                    needSkip = true;
                }
            }

            if (needSkip)
            {
                item.ignoreAvailable = true;
                item.Refresh(LearnStepItem.Status.InProgress);
            }
        }
    }

    [HarmonyPatch(typeof(CharacterStageProficiencySelectionPanel), "OnLearnAutoImpl")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnLearnAutoImpl_Patch
    {
        public static bool Prefix(CharacterStageProficiencySelectionPanel __instance, Random rng)
        {
            //PATCH: support for skipping skill and tool proficiency picking if you picked all available, but still have points remaining
            if (rng != null)
            {
                return true;
            }

            var item = CurrentStepItem(__instance);

            if (item == null || !item.ignoreAvailable || (item.PoolType != Skill && item.PoolType != Tool))
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
}
