using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.RecipeDefinitions;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationManagerPatcher
{
    //PATCH: EnableSaveByLocation
    [HarmonyPatch(typeof(GameLocationManager), nameof(GameLocationManager.LoadLocationAsync))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class LoadLocationAsync_Patch
    {
        [UsedImplicitly]
        public static void Prefix(
            GameLocationManager __instance,
            string userLocationName,
            string userCampaignName)
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return;
            }

            var sessionService = ServiceRepository.GetService<ISessionService>();

            if (sessionService is { Session: { } })
            {
                // Record which campaign/location the latest load game belongs to
                var selectedCampaignService = SaveByLocationContext.ServiceRepositoryEx
                    .GetOrCreateService<SaveByLocationContext.SelectedCampaignService>();

                selectedCampaignService.SetCampaignLocation(userCampaignName, userLocationName);
            }

            __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
        }
    }

    [HarmonyPatch(typeof(GameLocationManager), nameof(GameLocationManager.ReadyLocation))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class ReadyLocation_Patch
    {
        [UsedImplicitly]
        public static void Postfix(GameLocationManager __instance)
        {
            //BUGFIX: enforce learn same recipes as official campaigns get on a Load
            var gameLoreService = ServiceRepository.GetService<IGameLoreService>();

            if (!gameLoreService.KnownRecipes.Contains(RecipeBasic_Arrows))
            {
                gameLoreService.LearnRecipe(RecipeBasic_Arrows);
            }

            if (!gameLoreService.KnownRecipes.Contains(RecipeBasic_Bolts))
            {
                gameLoreService.LearnRecipe(RecipeBasic_Bolts);
            }
            //END BUGFIX

            //BUGFIX: remove carefully tracked dynamic item properties that have effect guid, but effect is removed
            //fixes Inventor's Infusions sometimes breaking and lingering forever without ability to remove them
            ExtraCarefulTrackedItem.FixDynamicPropertiesWithoutEffect();

            //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
            if (!Main.Settings.HideExitsAndTeleportersGizmosIfNotDiscovered || Gui.GameLocation.UserLocation == null)
            {
                return;
            }

            var worldGadgets = __instance.WorldLocation.WorldSectors.SelectMany(x => x.WorldGadgets);

            foreach (var worldGadget in worldGadgets)
            {
                GameUiContext.SetTeleporterGadgetActiveAnimation(worldGadget);
            }
        }
    }

    [HarmonyPatch(typeof(GameLocationManager), nameof(GameLocationManager.StopCharacterEffectsIfRelevant))]
    [UsedImplicitly]
    public static class StopCharacterEffectsIfRelevant_Patch
    {
        [NotNull]
        [UsedImplicitly]
        public static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: prevent some effects from being removed when entering new location
            var terminateMethod = typeof(RulesetEffect).GetMethod("Terminate");
            var maybeTerminateMethod = new Action<RulesetEffect, bool, bool>(MaybeTerminate).Method;

            return instructions.ReplaceCall(terminateMethod,
                1, "GameLocationManager.StopCharacterEffectsIfRelevant",
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, maybeTerminateMethod));
        }

        private static void MaybeTerminate([NotNull] RulesetEffect effect, bool self, bool willEnterChainedLocation)
        {
            var baseDefinition = effect.SourceDefinition;

            if (baseDefinition != null)
            {
                var skip = baseDefinition.GetFirstSubFeatureOfType<ISKipEffectRemovalOnLocationChange>();

                if (skip != null && skip.Skip(willEnterChainedLocation))
                {
                    return;
                }

                var effectDescription = effect.EffectDescription;

                if (willEnterChainedLocation
                    && RuleDefinitions.MatchesMagicType(effectDescription, RuleDefinitions.MagicType.SummonsCreature))
                {
                    return;
                }
            }

            effect.Terminate(self);
        }
    }
}
