using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class RestAfterPanelPatcher
{
    [HarmonyPatch(typeof(RestAfterPanel), "OnBeginShow")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class OnBeginShow_Patch
    {
        [UsedImplicitly]
        public static void Postfix([NotNull] RestSubPanel __instance)
        {
            //PATCH: Allow More Real State On Rest Panel
            if (!Main.Settings.AllowMoreRealStateOnRestPanel)
            {
                return;
            }

            // this is recovered features which we hide in Rest After Panel
            __instance.restModules[1].gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(__instance.RestModulesTable);
        }
    }
}
