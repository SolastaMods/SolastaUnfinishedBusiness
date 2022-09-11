using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches.Tools.DefaultParty;

//PATCH: EnableTogglesToOverwriteDefaultTestParty
[HarmonyPatch(typeof(MultiplayerSetupPanel), "OnBeginShow")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class MultiplayerSetupPanel_OnBeginShow
{
    internal static void Prefix()
    {
        Global.IsSettingUpMultiplayer = true;
    }
}
