using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class MultiplayerStatusModalPatcher
{
    [HarmonyPatch(typeof(MultiplayerStatusModal), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow1_Patch
    {
        public static void Prefix([NotNull] MultiplayerStatusModal __instance)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            switch (__instance)
            {
                case MultiplayerWaitModal multiplayerStatusModal:
                {
                    if (multiplayerStatusModal.notReadyPlayerInfoGroups.Count > 0)
                    {
                        while (multiplayerStatusModal.notReadyPlayerInfoGroups.Count < Main.Settings.OverridePartySize)
                        {
                            multiplayerStatusModal.notReadyPlayerInfoGroups.Add(multiplayerStatusModal
                                .notReadyPlayerInfoGroups[0]);
                        }
                    }

                    if (multiplayerStatusModal.readyPlayerInfoGroups.Count > 0)
                    {
                        while (multiplayerStatusModal.readyPlayerInfoGroups.Count < Main.Settings.OverridePartySize)
                        {
                            multiplayerStatusModal.readyPlayerInfoGroups.Add(multiplayerStatusModal
                                .readyPlayerInfoGroups[0]);
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
                            multiplayerKickModal.playerInfoGroups.Add(multiplayerKickModal.playerInfoGroups[0]);
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
                            multiplayerVoteModal.playerInfoGroups.Add(multiplayerVoteModal.playerInfoGroups[0]);
                        }
                    }

                    break;
                }
            }
        }
    }
}
