using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomUI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ActiveCharacterPanelPatcher
{
    [HarmonyPatch(typeof(ActiveCharacterPanel), "Refresh")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Refresh_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ActiveCharacterPanel __instance)
        {
            //PATCH: support for custom point pools and concentration powers on portrait
            IconsOnPortrait.CharacterPanelRefresh(__instance);
        }
    }
}
