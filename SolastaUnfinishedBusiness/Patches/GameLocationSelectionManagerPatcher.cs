using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class GameLocationSelectionManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationSelectionManager), nameof(GameLocationSelectionManager.OnCommandJustActivated))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnCommandJustActivated_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(GameLocationSelectionManager __instance, Queue<int> queuedCommands)
        {
            OnCommandJustActivated(__instance, queuedCommands);

            return false;
        }

        private static void OnCommandJustActivated(GameLocationSelectionManager __instance, Queue<int> queuedCommands)
        {
            if (queuedCommands.Count <= 0)
            {
                return;
            }

            var id = (InputCommands.Id)queuedCommands.Peek();
            var service = ServiceRepository.GetService<IGameLocationBattleService>();

            if (service == null || service.IsBattleInProgress)
            {
                return;
            }

            var playerController = ServiceRepository.GetService<IPlayerControllerService>().ActivePlayerController;

            if (playerController?.ControlledCharacters == null ||
                playerController.ControlledCharacters.Count == 0)
            {
                return;
            }

            if (id == InputCommands.Id.SelectAllCharacters)
            {
                __instance.SelectMultipleCharacters(playerController.ControlledCharacters, true);
            }
            else
            {
                var flag = false;
                var index = -1;

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (id)
                {
                    case InputCommands.Id.SelectCharacter1:
                        index = 0;
                        queuedCommands.Dequeue();
                        break;
                    case InputCommands.Id.SelectCharacter2:
                        index = 1;
                        queuedCommands.Dequeue();
                        break;
                    case InputCommands.Id.SelectCharacter3:
                        index = 2;
                        queuedCommands.Dequeue();
                        break;
                    case InputCommands.Id.SelectCharacter4:
                        index = 3;
                        queuedCommands.Dequeue();
                        break;
                    //BEGIN PATCH
                    case (InputCommands.Id)InputContext.InputCommandsExtra.SelectCharacter5:
                        index = 4;
                        queuedCommands.Dequeue();
                        break;
                    case (InputCommands.Id)InputContext.InputCommandsExtra.SelectCharacter6:
                        index = 5;
                        queuedCommands.Dequeue();
                        break;
                    //END PATCH
                    case InputCommands.Id.SelectNextCharacter:
                    {
                        if (__instance.IsCameraFree())
                        {
                            __instance.selectedCharacters.Do(selectedCharacter =>
                                __instance.SelectCharacter(selectedCharacter, false, true));

                            index = -1;
                        }
                        else
                        {
                            index = 0;
                            var a = __instance.selectedCharacters.Aggregate(-1, (current, selectedCharacter) =>
                                Mathf.Max(current, playerController.ControlledCharacters.IndexOf(selectedCharacter)));

                            if (a >= 0)
                            {
                                index = (a + 1) % playerController.ControlledCharacters.Count;
                            }

                            flag = true;
                        }

                        queuedCommands.Dequeue();
                        break;
                    }
                }

                if (index < 0 || index >= playerController.ControlledCharacters.Count)
                {
                    return;
                }

                var controlledCharacter = playerController.ControlledCharacters[index];

                __instance.DeselectAll();
                __instance.SelectCharacter(controlledCharacter);

                if (!flag)
                {
                    return;
                }

                __instance.SelectCharacter(controlledCharacter);
            }
        }
    }
}
