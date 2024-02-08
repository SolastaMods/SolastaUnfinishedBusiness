namespace SolastaUnfinishedBusiness.CustomUI;

public abstract class PowerTooltipModifier
{
    public void ModifyPowerTooltip(ITooltip tooltip, TooltipFeaturePowerParameters parameters)
    {
        if (tooltip.DataProvider is not GuiPowerDefinition guiPowerDefinition)
        {
            return;
        }

        Apply(parameters, guiPowerDefinition, tooltip.Context as RulesetCharacter);
    }

    protected abstract void Apply(
        TooltipFeaturePowerParameters tooltip,
        GuiPowerDefinition guiPowerDefinition,
        // ReSharper disable once UnusedParameter.Global
        RulesetCharacter character);
}
