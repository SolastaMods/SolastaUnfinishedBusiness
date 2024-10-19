using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ViewLocationPatcher
{
    [HarmonyPatch(typeof(ViewLocation), nameof(ViewLocation.OnCommandJustActivated))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnCommandJustActivated_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(ViewLocation __instance, Queue<int> queuedCommands)
        {
            if (queuedCommands.Count <= 0)
            {
                return false;
            }

            var id = (InputCommands.Id)queuedCommands.Peek();
            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService is not { IsBattleInProgress: true })
            {
                return false;
            }

            var playerController = ServiceRepository.GetService<IPlayerControllerService>().ActivePlayerController;

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (id)
            {
                case (InputCommands.Id)InputContext.InputCommandsExtra.SelectCharacter5:
                    __instance.CameraControllerLocation.FollowCharacterForBattle(
                        playerController.ControlledCharacters[4]);
                    queuedCommands.Dequeue();
                    return false;
                case (InputCommands.Id)InputContext.InputCommandsExtra.SelectCharacter6:
                    __instance.CameraControllerLocation.FollowCharacterForBattle(
                        playerController.ControlledCharacters[5]);
                    queuedCommands.Dequeue();
                    return false;
                default:
                    return true;
            }
        }
    }
}
