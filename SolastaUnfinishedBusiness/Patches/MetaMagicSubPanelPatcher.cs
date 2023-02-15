using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class MetaMagicSubPanelPatcher
{
    //PATCH: allow refreshing custom metamagic options
    [HarmonyPatch(typeof(MetaMagicSubPanel), nameof(MetaMagicSubPanel.SetState))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshActions_Patch
    {
        [UsedImplicitly]
        public static void Prefix(MetaMagicSubPanel __instance)
        {
            GameUiContext.RefreshMetamagicOffering(__instance);
        }
    }
}
