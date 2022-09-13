using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
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
        var effectDescription = Definition.EffectDescription;

        effectDescription.SetTargetParameter(6);
        effectDescription.SetRangeParameter(0);
        effectDescription.SetRequiresTargetProximity(false);

        Definition.overriddenPower = PowerPaladinAuraOfProtection;
    }

    internal static FeatureDefinitionPower Instance =>
        _instance ??= new PowerPaladinAuraOfProtection18Builder()
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
}
