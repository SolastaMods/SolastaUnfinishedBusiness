using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Tooltip;

internal static class UsablePowerBoxPatcher
{
    [HarmonyPatch(typeof(UsablePowerBox), "Bind")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class UsablePowerBox_Bind
    {
        internal static void Postfix(UsablePowerBox __instance)
        {
            var panel = __instance.GetComponentInParent<CharacterControlPanelExploration>();
            if (panel == null)
            {
                return;
            }

            __instance.GuiTooltip.Context = panel.GuiCharacter?.RulesetCharacter;
        }
    }
}