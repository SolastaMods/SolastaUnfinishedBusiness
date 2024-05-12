using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationTargetingManagerPatcher
{
    //BUGFIX: Chain Lightning allow targeting enemies that cannot be perceived creating soft lock situations
    [HarmonyPatch(typeof(GameLocationTargetingManager), "IGameLocationTargetingService.ComputeAndSortSubtargets")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ComputeAndSortSubtargets_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(
            GameLocationTargetingManager __instance,
            GameLocationCharacter caster,
            RulesetEffect rulesetEffect,
            GameLocationCharacter mainTarget,
            List<GameLocationCharacter> subTargets)
        {
            ComputeAndSortSubtargets(__instance, caster, rulesetEffect, mainTarget, subTargets);

            return false;
        }

        private static void ComputeAndSortSubtargets(
            GameLocationTargetingManager __instance,
            GameLocationCharacter caster,
            RulesetEffect rulesetEffect,
            GameLocationCharacter mainTarget,
            List<GameLocationCharacter> subTargets)
        {
            subTargets.Clear();

            if (rulesetEffect == null ||
                rulesetEffect.EffectDescription.TargetType != RuleDefinitions.TargetType.ArcFromIndividual ||
                rulesetEffect.EffectDescription.TargetParameter <= 0 ||
                rulesetEffect.EffectDescription.TargetParameter2 <= 0)
            {
                return;
            }

            var maxTargets = rulesetEffect.ComputeTargetParameter();
            var range = rulesetEffect.ComputeTargetParameter2();

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            foreach (var validEntity in ServiceRepository
                         .GetService<IGameLocationCharacterService>().AllValidEntities
                         .Where(x => x.RulesetActor is not RulesetCharacterEffectProxy))
            {
                if (validEntity.RulesetActor is RulesetGadget)
                {
                    var gadgetService = ServiceRepository.GetService<IGameLocationGadgetService>();
                    var guid = validEntity.RulesetGadget.Guid;

                    if (gadgetService.TryGetGadgetByGadgetGuid(guid, out var gameGadget))
                    {
                        var factoryService = ServiceRepository.GetService<IWorldLocationEntityFactoryService>();

                        if (factoryService.TryFindWorldGadget(gameGadget, out var worldGadget) &&
                            !worldGadget.HasTargetingFlow())
                        {
                            continue;
                        }
                    }
                }

                if (validEntity != mainTarget &&
                    //BEGIN PATCH
                    caster.CanPerceiveTarget(validEntity) &&
                    //END PATCH
                    battleService.IsWithinXCells(mainTarget, validEntity, range))
                {
                    subTargets.Add(validEntity);
                }
            }

            __instance.compareCenter = mainTarget.LocationPosition;
            subTargets.Sort(__instance);

            while (subTargets.Count > maxTargets)
            {
                var flag = false;
                for (var index = subTargets.Count - 1; index >= 0; --index)
                {
                    if (subTargets[index].IsOppositeSide(caster.Side))
                    {
                        continue;
                    }

                    subTargets.RemoveAt(index);
                    flag = true;
                    break;
                }

                if (!flag)
                {
                    break;
                }
            }

            while (subTargets.Count > maxTargets)
            {
                subTargets.RemoveRange(maxTargets, subTargets.Count - maxTargets);
            }
        }
    }
}
