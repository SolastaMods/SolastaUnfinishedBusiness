using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class DefeatModalPatcher
{
    //PATCH: prevents a spider bitmap to display if RemoveBugVisualModels is on
    [HarmonyPatch(typeof(DefeatModal), nameof(DefeatModal.OnBeginShow))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Postfix(DefeatModal __instance)
        {
            if (Main.Settings.RemoveBugVisualModels)
            {
                __instance.backdrop.gameObject.SetActive(false);
            }
        }
    }
}
