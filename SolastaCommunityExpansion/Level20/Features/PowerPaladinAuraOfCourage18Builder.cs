using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class PowerPaladinAuraOfCourage18Builder : FeatureDefinitionPowerBuilder
    {
        private static FeatureDefinitionPower _instance;

        private const string PowerPaladinAuraOfCourage18Name = "ZSPowerPaladinAuraOfCourage18";
        private const string PowerPaladinAuraOfCourage18Guid = "d68c46024ea8432b981506a13ae35ecd";

        public PowerPaladinAuraOfCourage18Builder() : base(PowerPaladinAuraOfCourage, PowerPaladinAuraOfCourage18Name, PowerPaladinAuraOfCourage18Guid)
        {
            var ed = Definition.EffectDescription;

            ed.SetTargetParameter(6);
            ed.SetRangeParameter(0);
            ed.SetRequiresTargetProximity(false);

            Definition.SetOverriddenPower(PowerPaladinAuraOfCourage);
            Definition.GuiPresentation.Description = "Feature/&PowerPaladinAuraOfCourage18Description";
            Definition.GuiPresentation.Title = "Feature/&PowerPaladinAuraOfCourage18Title";
        }

        internal static FeatureDefinitionPower Instance => _instance ??= new PowerPaladinAuraOfCourage18Builder().AddToDB();
    }
}
