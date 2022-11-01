using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

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
            var value = Math.Max(ToolsContext.GamePartySize, Main.Settings.OverridePartySize);

            __instance.maxPlayersSlider.maxValue = value;
        }
    }
}
