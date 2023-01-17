using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CreateGameSubpanelPatcher
{
    [HarmonyPatch(typeof(CreateGameSubpanel), nameof(CreateGameSubpanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix([NotNull] CreateGameSubpanel __instance)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            __instance.maxPlayersSlider.maxValue = Main.Settings.OverridePartySize;
        }
    }
}
