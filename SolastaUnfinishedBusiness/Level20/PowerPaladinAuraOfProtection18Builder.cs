using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class PowerPaladinAuraOfProtection18Builder : FeatureDefinitionPowerBuilder
{
    private const string PowerPaladinAuraOfProtection18Name = "PowerPaladinAuraOfProtection18";
    private const string PowerPaladinAuraOfProtection18Guid = "1574c379dfb74cfeb3488209bd3b6d33";
    private static FeatureDefinitionPower _instance;

    private PowerPaladinAuraOfProtection18Builder() : base(PowerPaladinAuraOfProtection,
        PowerPaladinAuraOfProtection18Name, PowerPaladinAuraOfProtection18Guid)
    {
        var ed = Definition.EffectDescription;

        ed.SetTargetParameter(6);
        ed.SetRangeParameter(0);
        ed.SetRequiresTargetProximity(false);

        Definition.overriddenPower = PowerPaladinAuraOfProtection;
        Definition.GuiPresentation.Description = "Feature/&PowerPaladinAuraOfProtection18Description";
        Definition.GuiPresentation.Title = "Feature/&PowerPaladinAuraOfProtection18Title";
    }

    internal static FeatureDefinitionPower Instance =>
        _instance ??= new PowerPaladinAuraOfProtection18Builder().AddToDB();
}
