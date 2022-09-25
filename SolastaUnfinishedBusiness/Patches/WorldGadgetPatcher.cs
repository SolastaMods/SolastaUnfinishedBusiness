using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class WorldGadgetPatcher
{
    [HarmonyPatch(typeof(WorldGadget), "SetHighlightVisibility")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class SetHighlightVisibility_Patch
    {
        internal static void Prefix(WorldGadget __instance, ref bool visible)
        {
            //PATCH: disables item highlights not in party field of view (AltOnlyHighlightItemsInPartyFieldOfView)
            if (!Main.Settings.AltOnlyHighlightItemsInPartyFieldOfView || !visible || !__instance.IsUserGadget)
            {
                return;
            }

            GameUiContext.SetHighlightVisibilityExtended(__instance, ref visible);
        }
    }
}
