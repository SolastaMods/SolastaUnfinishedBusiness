using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using static RuleDefinitions;
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

            if (sessionService is { Session: not null })
            {
                // Record which campaign/location the latest load game belongs to
                var selectedCampaignService = SaveByLocationContext.ServiceRepositoryEx
                    .GetOrCreateService<SaveByLocationContext.SelectedCampaignService>();
                string name = null;
                SaveByLocationContext.LocationType type;

                if (!string.IsNullOrEmpty(userCampaignName))
                {
                    type = SaveByLocationContext.LocationType.CustomCampaign;
                    name = ServiceRepository.GetService<ISessionService>()?.Session?.UserCampaign?.Title;
                }
                else if (!string.IsNullOrEmpty(userLocationName))
                {
                    type = SaveByLocationContext.LocationType.UserLocation;
                    if (ServiceRepository.GetService<IUserLocationPoolService>()
                        .TryGetUserLocation(userLocationName, out var userLocation))
                    {
                        name = userLocation.Title;
                    }
                }
                else
                {
                    type = SaveByLocationContext.LocationType.StandardCampaign;
                    var campaignName = ServiceRepository.GetService<ISessionService>()?.Session?.CampaignDefinitionName;
                    var campaignDB = DatabaseRepository.GetDatabase<CampaignDefinition>();
                    if (campaignDB.TryGetElement(campaignName, out var campaign))
                    {
                        name = Gui.Localize(campaign.GuiPresentation.Title);
                    }
                }

                if (name != null)
                {
                    selectedCampaignService.SetCampaignLocation(type, name);
                }
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
            //PATCH: ensure Sorcerous Wild Magic will have Tides of Chaos pool initiated correctly
            SorcerousWildMagic.InitTidesOfChaos();

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

            //PATCH: remove carefully tracked dynamic item properties that have effect guid, but effect is removed
            //fixes Inventor's Infusions sometimes breaking and lingering forever without ability to remove them
            TrackItemsCarefully.FixDynamicPropertiesWithoutEffect();

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

            if (baseDefinition)
            {
                var skip = baseDefinition.GetFirstSubFeatureOfType<IPreventRemoveEffectOnLocationChange>();

                if (skip != null && skip.Skip(willEnterChainedLocation))
                {
                    return;
                }

                var effectDescription = effect.EffectDescription;

                if (willEnterChainedLocation
                    && MatchesMagicType(effectDescription, MagicType.SummonsCreature))
                {
                    return;
                }
            }

            effect.Terminate(self);
        }
    }
}
