using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.GameUi.Tooltip;

[HarmonyPatch(typeof(UsablePowerBox), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class UsablePowerBox_Bind
{
    internal static void Postfix(UsablePowerBox __instance)
    {
        CharacterControlPanel panel = __instance.GetComponentInParent<CharacterControlPanelExploration>();
        if (panel == null)
        {
            panel = __instance.GetComponentInParent<CharacterControlPanelBattle>();
            if (panel == null)
            {
                return;
            }
        }

        __instance.GuiTooltip.Context = panel.GuiCharacter?.RulesetCharacter;
    }
}
