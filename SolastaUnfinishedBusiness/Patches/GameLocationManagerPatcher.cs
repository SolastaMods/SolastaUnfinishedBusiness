using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class GameLocationManagerPatcher
{
    //PATCH: EnableSaveByLocation
    [HarmonyPatch(typeof(GameLocationManager), "LoadLocationAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class LoadLocationAsync_Patch
    {
        public static void Prefix(GameLocationManager __instance,
            string locationDefinitionName, string userLocationName, string userCampaignName)
        {
            if (!Main.Settings.EnableSaveByLocation)
            {
                return;
            }

            Main.Log(
                $"LoadLocationAsync-Params: ld={locationDefinitionName}, ul={userLocationName}, uc={userCampaignName}");

            var sessionService = ServiceRepository.GetService<ISessionService>();

            if (sessionService is {Session: { }})
            {
                // Record which campaign/location the latest load game belongs to

#if DEBUG
                var session = sessionService.Session;

                Main.Log(
                    $"Campaign-ss: Campaign={session.CampaignDefinitionName}, Location: {session.UserLocationName}");
#endif
                var selectedCampaignService = SaveByLocationContext.ServiceRepositoryEx
                    .GetOrCreateService<SaveByLocationContext.SelectedCampaignService>();


                selectedCampaignService.SetCampaignLocation(userCampaignName, userLocationName);
            }

            __instance.StartCoroutine(ServiceRepository.GetService<IGameSerializationService>()?.EnumerateSavesGames());
        }
    }

    //PATCH: HideExitsAndTeleportersGizmosIfNotDiscovered
    [HarmonyPatch(typeof(GameLocationManager), "ReadyLocation")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class ReadyLocation_Patch
    {
        public static void Postfix(GameLocationManager __instance)
        {
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

    [HarmonyPatch(typeof(GameLocationManager), "StopCharacterEffectsIfRelevant")]
    internal static class GameLocationManager_StopCharacterEffectsIfRelevant
    {
        [NotNull]
        internal static IEnumerable<CodeInstruction> Transpiler([NotNull] IEnumerable<CodeInstruction> instructions)
        {
            //PATCH: prevent some effects from being removed when entering new location
            var maybeTerminate = new Action<RulesetEffect, bool, bool>(MaybeTerminate).Method;
            foreach (var instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand.ToString().Contains("Terminate"))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, maybeTerminate);
                }
                else
                {
                    yield return instruction;
                }
            }
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
