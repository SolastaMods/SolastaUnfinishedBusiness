using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class MultiplayerStatusModalPatcher
{
    [HarmonyPatch(typeof(MultiplayerWaitModal), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow1_Patch
    {
        public static void Prefix([NotNull] MultiplayerWaitModal __instance)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            if (__instance.notReadyPlayerInfoGroups.Count > 0)
            {
                while (__instance.notReadyPlayerInfoGroups.Count < Main.Settings.OverridePartySize)
                {
                    __instance.notReadyPlayerInfoGroups.Add(__instance.notReadyPlayerInfoGroups[0]);
                }
            }

            if (__instance.readyPlayerInfoGroups.Count > 0)
            {
                while (__instance.readyPlayerInfoGroups.Count < Main.Settings.OverridePartySize)
                {
                    __instance.readyPlayerInfoGroups.Add(__instance.readyPlayerInfoGroups[0]);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MultiplayerKickModal), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow2_Patch
    {
        public static void Prefix([NotNull] MultiplayerKickModal __instance)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            if (__instance.playerInfoGroups.Count <= 0)
            {
                return;
            }

            while (__instance.playerInfoGroups.Count < Main.Settings.OverridePartySize)
            {
                __instance.playerInfoGroups.Add(__instance.playerInfoGroups[0]);
            }
        }
    }

    [HarmonyPatch(typeof(MultiplayerVoteModal), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class OnBeginShow3_Patch
    {
        public static void Prefix([NotNull] MultiplayerVoteModal __instance)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            if (__instance.playerInfoGroups.Count <= 0)
            {
                return;
            }

            while (__instance.playerInfoGroups.Count < Main.Settings.OverridePartySize)
            {
                __instance.playerInfoGroups.Add(__instance.playerInfoGroups[0]);
            }
        }
    }
}
