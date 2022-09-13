using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class PowerPaladinAuraOfCourage18Builder : FeatureDefinitionPowerBuilder
{
    private const string PowerPaladinAuraOfCourage18Name = "PowerPaladinAuraOfCourage18";
    private const string PowerPaladinAuraOfCourage18Guid = "d68c46024ea8432b981506a13ae35ecd";
    private static FeatureDefinitionPower _instance;

    private PowerPaladinAuraOfCourage18Builder() : base(PowerPaladinAuraOfCourage, PowerPaladinAuraOfCourage18Name,
        PowerPaladinAuraOfCourage18Guid)
    {
        var effectDescription = Definition.EffectDescription;

        effectDescription.SetTargetParameter(6);
        effectDescription.SetRangeParameter(0);
        effectDescription.SetRequiresTargetProximity(false);

        Definition.overriddenPower = PowerPaladinAuraOfCourage;
    }

    internal static FeatureDefinitionPower Instance =>
        _instance ??= new PowerPaladinAuraOfCourage18Builder()
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
}
