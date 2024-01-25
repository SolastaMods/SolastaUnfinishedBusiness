using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class MultiplayerSetupPanelPatcher
{
    //PATCH: set flag that prevents hero auto assignment under MP (DEFAULT_PARTY)
    [HarmonyPatch(typeof(MultiplayerSetupPanel), nameof(MultiplayerSetupPanel.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Prefix()
        {
            Global.IsSettingUpMultiplayer = true;
        }
    }
}
