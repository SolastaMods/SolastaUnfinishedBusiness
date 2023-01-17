using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ViewPatcher
{
    [HarmonyPatch(typeof(View), nameof(View.HandleInputs))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class HandleInputs_Patch
    {
        [UsedImplicitly]
        public static bool Prefix()
        {
            //PATCH: prevents game from receiving input if Mod UI is open
            return !UnityModManagerUIPatcher.ModManagerUI.IsOpen;
        }
    }
}
