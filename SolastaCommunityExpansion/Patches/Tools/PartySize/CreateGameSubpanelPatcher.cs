using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.Tools.PartySize;

[UsedImplicitly]
public static class CreateGameSubpanelPatcher
{
    [HarmonyPatch(typeof(CreateGameSubpanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class UpdateRelevance_Patch
    {
        public static void Prefix([NotNull] CreateGameSubpanel __instance)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            var value = Math.Max(DungeonMakerContext.GamePartySize, Main.Settings.OverridePartySize);

            __instance.maxPlayersSlider.maxValue = value;
        }
    }
}
