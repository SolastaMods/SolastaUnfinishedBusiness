using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.GameUi.Tooltip;

[HarmonyPatch(typeof(TooltipFeaturePowerParameters), "Bind")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class TooltipFeaturePowerParameters_Bind
{
    internal static void Postfix(TooltipFeaturePowerParameters __instance, ITooltip tooltip)
    {
        if (tooltip.DataProvider is not GuiPowerDefinition guiPowerDefinition)
        {
            return;
        }

        if (tooltip.Context is not RulesetCharacter character)
        {
            return;
        }

        var power = guiPowerDefinition.PowerDefinition;
        var usesLabel = __instance.usesLabel;
        usesLabel.Text = FormatUses(power, character, usesLabel.Text);
    }

    private static string FormatUses(FeatureDefinitionPower power, RulesetCharacter character, string def)
    {
        if (power.UsesDetermination != RuleDefinitions.UsesDetermination.Fixed)
        {
            return def;
        }

        if (power.RechargeRate == RuleDefinitions.RechargeRate.AtWill)
        {
            return def;
        }

        if (power.CostPerUse == 0)
        {
            return def;
        }

        var usablePower = UsablePowersProvider.Get(power, character);
        var maxUses = CustomFeaturesContext.GetMaxUsesForPool(usablePower, character);
        var remainingUses = character.GetRemainingUsesOfPower(usablePower);
        return $"{remainingUses}/{maxUses}";
    }
}
