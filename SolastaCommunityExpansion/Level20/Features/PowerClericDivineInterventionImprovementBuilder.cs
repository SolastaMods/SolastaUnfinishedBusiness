using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class PowerClericDivineInterventionImprovementBuilder : FeatureDefinitionPowerBuilder
    {
        private const string PowerClericDivineInterventionImprovementClericName = "ZSPowerClericDivineInterventionImprovementCleric";
        private const string PowerClericDivineInterventionImprovementClericGuid = "cc4303e4-114e-43aa-a7ee-e197c9f8ef40";

        private const string PowerClericDivineInterventionImprovementPaladinName = "ZSPowerClericDivineInterventionImprovementPaladin";
        private const string PowerClericDivineInterventionImprovementPaladinGuid = "3a9f52a1-ca5d-4138-a95a-5c4d9748763d";

        private const string PowerClericDivineInterventionImprovementWizardName = "ZSPowerClericDivineInterventionImprovementWizard";
        private const string PowerClericDivineInterventionImprovementWizardGuid = "78b25422-6497-441e-a285-b4dd97211a32";

        private PowerClericDivineInterventionImprovementBuilder(FeatureDefinitionPower basePower, string name, string guid) : base(basePower, name, guid)
        {
            Definition.SetHasCastingFailure(false);
            Definition.SetOverriddenPower(basePower);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(FeatureDefinitionPower basePower, string name, string guid)
        {
            return new PowerClericDivineInterventionImprovementBuilder(basePower, name, guid).AddToDB();
        }

        internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementCleric =
            CreateAndAddToDB(PowerClericDivineInterventionCleric, PowerClericDivineInterventionImprovementClericName, PowerClericDivineInterventionImprovementClericGuid);

        internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementPaladin =
            CreateAndAddToDB(PowerClericDivineInterventionPaladin, PowerClericDivineInterventionImprovementPaladinName, PowerClericDivineInterventionImprovementPaladinGuid);

        internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementWizard =
            CreateAndAddToDB(PowerClericDivineInterventionWizard, PowerClericDivineInterventionImprovementWizardName, PowerClericDivineInterventionImprovementWizardGuid);
    }
}
