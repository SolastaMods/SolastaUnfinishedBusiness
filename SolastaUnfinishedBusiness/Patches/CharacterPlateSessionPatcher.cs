using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CharacterPlateSessionPatcher
{
    [HarmonyPatch(typeof(CharacterPlateSession), nameof(CharacterPlateSession.Refresh))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static bool Prefix(CharacterPlateSession __instance)
        {
            Refresh(__instance);

            return false;
        }

        // mainly vanilla code except for patch block
        private static void Refresh(CharacterPlateSession __instance)
        {
            __instance.emptyLabel.gameObject.SetActive(__instance.GuiCharacter == null);
            __instance.characterName.gameObject.SetActive(__instance.GuiCharacter != null);
            __instance.characterClassAndLevel.gameObject.SetActive(__instance.GuiCharacter != null);
            __instance.levelUpIcon.gameObject.SetActive(
                __instance.GuiCharacter != null &&
                __instance.ForceLevelUpTo > __instance.GuiCharacter.CharacterLevel);
            __instance.reservedCharacterStatusGroup.gameObject.SetActive(false);

            if (__instance.GuiCharacter != null)
            {
                __instance.characterName.Text = __instance.GuiCharacter.Name;
                __instance.characterClassAndLevel.Text = __instance.GuiCharacter.ClassAndLevel;
                __instance.GuiCharacter.AssignPortraitImage(__instance.characterPortrait);
            }

            var service = ServiceRepository.GetService<INetworkingService>();
            var flag1 = service.LocalPlayerNumber != __instance.PlayerSlot.PlayerId;

            __instance.selectCharacterButton.gameObject.SetActive(!flag1 && __instance.GuiCharacter == null);

            if (!__instance.loadingMultiplayerSaveMode)
            {
                __instance.dismissCharacterButton.gameObject.SetActive(!flag1 && __instance.GuiCharacter != null);
                __instance.playerRequestsGroup.gameObject.SetActive(false);
            }
            else
            {
                __instance.dismissCharacterButton.gameObject.SetActive(service.IsMasterClient || !flag1);
                __instance.playerRequestsGroup.gameObject.SetActive(true);
            }

            __instance.dismissCharacterButton.interactable = !__instance.locked;
            __instance.dismissCharacterButtonTooltip.Content = !__instance.locked
                ? "MainMenu/&ChangeCharacterSlotDescription"
                : Gui.FormatFailure("MainMenu/&ChangeCharacterSlotDescription", "Failure/&FailureFlagLockedCharacter");
            __instance.requestCharacterButton.gameObject.SetActive(
                __instance.loadingMultiplayerSaveMode & flag1 && !service.IsMasterClient);

            if (!service.InOnlineRoom)
            {
                __instance.remoteCharacterStatusLabel.gameObject.SetActive(false);
                __instance.reservedCharacterStatusGroup.gameObject.SetActive(false);
                __instance.playerInfoStatusGroup.gameObject.SetActive(false);
                __instance.playerDropdown.gameObject.SetActive(false);
                __instance.playerGamepadSelector.gameObject.SetActive(false);
                __instance.playerInfoGroup.Unbind();
                __instance.playerInfoGroup.gameObject.SetActive(false);
            }
            else
            {
                if (__instance.loadingMultiplayerSaveMode)
                {
                    __instance.playerInfoGroup.Unbind();
                    __instance.playerInfoGroup.gameObject.SetActive(false);

                    if (Gui.GamepadActive)
                    {
                        __instance.playerDropdown.gameObject.SetActive(false);
                        __instance.playerGamepadSelector.gameObject.SetActive(true);
                        __instance.playerGamepadSelector.interactable =
                            __instance.loadingMultiplayerSaveMode && service.IsMasterClient;
                    }
                    else
                    {
                        __instance.playerGamepadSelector.gameObject.SetActive(false);
                        __instance.playerDropdown.gameObject.SetActive(true);
                        __instance.playerDropdown.interactable =
                            __instance.loadingMultiplayerSaveMode && service.IsMasterClient;
                    }
                }
                else
                {
                    __instance.playerGamepadSelector.gameObject.SetActive(false);
                    __instance.playerDropdown.gameObject.SetActive(false);
                    __instance.playerInfoGroup.gameObject.SetActive(true);
                }

                __instance.remoteCharacterStatusLabel.gameObject.SetActive(
                    !__instance.loadingMultiplayerSaveMode & flag1);

                if (flag1)
                {
                    if (!__instance.loadingMultiplayerSaveMode)
                    {
                        var remotePlayerStatus = __instance.GuiCharacter == null
                            ? NetworkingDefinitions.RemotePlayerStatus.SelectingCharacter
                            : NetworkingDefinitions.RemotePlayerStatus.Ready;
                        __instance.remoteCharacterStatusLabel.Text =
                            $"MainMenu/&RemotePlayerStatus{remotePlayerStatus}Title";
                        __instance.playerInfoStatusGroup.gameObject.SetActive(true);
                        __instance.playerInfoGroup.Unbind();
                        __instance.playerInfoGroup.Bind(
                            __instance.PlayerSlot, false, false, false, false, true, false, false);
                    }
                    else
                    {
                        __instance.playerInfoStatusGroup.gameObject.SetActive(false);

                        if (!__instance.loadingMultiplayerSaveMode)
                        {
                            __instance.playerInfoGroup.Unbind();
                            __instance.playerInfoGroup.Bind(
                                __instance.PlayerSlot, false, false, false, false, true, false, false);
                        }
                        else
                        {
                            __instance.RefreshPlayerDropdownOpts();
                        }
                    }
                }
                else
                {
                    if (service.IsMasterClient && !__instance.loadingMultiplayerSaveMode)
                    {
                        // BEGIN PATCH
                        var max = (Main.Settings.OverridePartySize - 1) / 2;
                        // var flag2 = __instance.index > 1;
                        var flag2 = __instance.index > max;
                        //END PATCH

                        __instance.reservedCharacterStatusGroup.gameObject.SetActive(flag2);
                        if (flag2 && __instance.selectCharacterButton.gameObject.activeSelf)
                        {
                            __instance.selectCharacterButton.gameObject.SetActive(false);
                        }
                    }

                    __instance.playerInfoStatusGroup.gameObject.SetActive(false);
                    if (!__instance.loadingMultiplayerSaveMode)
                    {
                        __instance.playerInfoGroup.Unbind();
                        __instance.playerInfoGroup.Bind(__instance.PlayerSlot, false, false, false, false, true, false,
                            false);
                    }
                    else
                    {
                        __instance.RefreshPlayerDropdownOpts();
                    }
                }
            }
        }
    }
}
