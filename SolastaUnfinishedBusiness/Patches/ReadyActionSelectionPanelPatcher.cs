using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class ReadyActionSelectionPanelPatcher
{
    [HarmonyPatch(typeof(ReadyActionSelectionPanel), nameof(ReadyActionSelectionPanel.Bind))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class Bind_Patch
    {
        [UsedImplicitly]
        public static void Prefix(ReadyActionSelectionPanel __instance)
        {
            //PATCH: adds toggle button to ready action panel for 'force preferred cantrip' feature
            CustomReactionsContext.SetupForcePreferredToggle(__instance.preferredCantripSelectionGroup);
        }

        [UsedImplicitly]
        public static void Postfix(ReadyActionSelectionPanel __instance)
        {
            //PATCH: allows to ready non-standard ranged attacks (like Armorer's Lightning Launcher)
            if (!__instance.rangedAttackButton.interactable)
            {
                __instance.rangedAttackButton.interactable =
                    __instance.Character.GetFirstRangedModeThatCanBeReadied() != null;
            }
        }
    }
}
