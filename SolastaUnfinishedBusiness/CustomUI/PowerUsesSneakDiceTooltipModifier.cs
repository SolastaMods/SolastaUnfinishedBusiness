namespace SolastaUnfinishedBusiness.CustomUI;

public class PowerUsesSneakDiceTooltipModifier : PowerTooltipModifier
{
    private PowerUsesSneakDiceTooltipModifier()
    {
    }

    public static PowerTooltipModifier Instance { get; } = new PowerUsesSneakDiceTooltipModifier();

    protected override void Apply(TooltipFeaturePowerParameters tooltip, GuiPowerDefinition guiPowerDefinition,
        RulesetCharacter character)
    {
        tooltip.usesLabel.Text = Gui.Format("{0} {1}",
            guiPowerDefinition.PowerDefinition.CostPerUse.ToString(),
            "Tooltip/&PowerCostSneakAttackDice");
    }
}
