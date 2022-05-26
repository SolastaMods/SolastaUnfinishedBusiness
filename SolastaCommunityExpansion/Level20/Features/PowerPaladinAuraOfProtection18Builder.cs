using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class PowerPaladinAuraOfProtection18Builder : FeatureDefinitionPowerBuilder
    {
        private static FeatureDefinitionPower _instance;

        private const string PowerPaladinAuraOfProtection18Name = "ZSPowerPaladinAuraOfProtection18";
        private const string PowerPaladinAuraOfProtection18Guid = "1574c379dfb74cfeb3488209bd3b6d33";

        public PowerPaladinAuraOfProtection18Builder() : base(PowerPaladinAuraOfProtection, PowerPaladinAuraOfProtection18Name, PowerPaladinAuraOfProtection18Guid)
        {
            var ed = Definition.EffectDescription;

            ed.SetTargetParameter(6);
            ed.SetRangeParameter(0);
            ed.SetRequiresTargetProximity(false);

            Definition.SetOverriddenPower(PowerPaladinAuraOfProtection);
            Definition.GuiPresentation.Description = "Feature/&PowerPaladinAuraOfProtection18Description";
            Definition.GuiPresentation.Title = "Feature/&PowerPaladinAuraOfProtection18Title";
        }

        internal static FeatureDefinitionPower Instance => _instance ??= new PowerPaladinAuraOfProtection18Builder().AddToDB();
    }
}
