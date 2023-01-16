using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class MultiplayerStatusModalPatcher
{
    [HarmonyPatch(typeof(MultiplayerStatusModal), nameof(MultiplayerStatusModal.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow1_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] MultiplayerStatusModal __instance)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            switch (__instance)
            {
                case MultiplayerWaitModal multiplayerWaitModal:
                {
                    if (multiplayerWaitModal.notReadyPlayerInfoGroups.Count > 0)
                    {
                        while (multiplayerWaitModal.notReadyPlayerInfoGroups.Count < Main.Settings.OverridePartySize)
                        {
                            var newItem = Object.Instantiate(
                                multiplayerWaitModal.notReadyPlayerInfoGroups[0].gameObject,
                                multiplayerWaitModal.notReadyPlayerInfoGroups[0].transform.parent);

                            var playerInfoGroup = newItem.GetComponent<PlayerInfoGroup>();

                            multiplayerWaitModal.notReadyPlayerInfoGroups.Add(playerInfoGroup);
                        }
                    }

                    if (multiplayerWaitModal.readyPlayerInfoGroups.Count > 0)
                    {
                        while (multiplayerWaitModal.readyPlayerInfoGroups.Count < Main.Settings.OverridePartySize)
                        {
                            var newItem = Object.Instantiate(
                                multiplayerWaitModal.readyPlayerInfoGroups[0].gameObject,
                                multiplayerWaitModal.readyPlayerInfoGroups[0].transform.parent);

                            var playerInfoGroup = newItem.GetComponent<PlayerInfoGroup>();

                            multiplayerWaitModal.readyPlayerInfoGroups.Add(playerInfoGroup);
                        }
                    }

                    break;
                }
                case MultiplayerKickModal multiplayerKickModal:
                {
                    if (multiplayerKickModal.playerInfoGroups.Count > 0)
                    {
                        while (multiplayerKickModal.playerInfoGroups.Count < Main.Settings.OverridePartySize)
                        {
                            var newItem = Object.Instantiate(
                                multiplayerKickModal.playerInfoGroups[0].gameObject,
                                multiplayerKickModal.playerInfoGroups[0].transform.parent);

                            var playerInfoGroup = newItem.GetComponent<PlayerInfoGroup>();

                            multiplayerKickModal.playerInfoGroups.Add(playerInfoGroup);
                        }
                    }

                    break;
                }
                case MultiplayerVoteModal multiplayerVoteModal:
                {
                    if (multiplayerVoteModal.playerInfoGroups.Count > 0)
                    {
                        while (multiplayerVoteModal.playerInfoGroups.Count < Main.Settings.OverridePartySize)
                        {
                            var newItem = Object.Instantiate(
                                multiplayerVoteModal.playerInfoGroups[0].gameObject,
                                multiplayerVoteModal.playerInfoGroups[0].transform.parent);

                            var playerInfoGroup = newItem.GetComponent<PlayerInfoGroup>();

                            multiplayerVoteModal.playerInfoGroups.Add(playerInfoGroup);
                        }
                    }

                    break;
                }
            }
        }
    }
}
