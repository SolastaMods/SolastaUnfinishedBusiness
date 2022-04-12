using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using static SolastaCommunityExpansion.Builders.DefinitionBuilder;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Classes.Warlock.Features
{
    internal static partial class WarlockFeatures
    {
        public readonly static FeatureDefinitionPower WarlockEldritchMasterPower = FeatureDefinitionPowerBuilder
            .Create(PowerWizardArcaneRecovery, "ClassWarlockEldritchMaster", CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetActivationTime(RuleDefinitions.ActivationTime.Minute1)
            .AddToDB();
    }
}
