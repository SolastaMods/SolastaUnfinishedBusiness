using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaCommunityExpansion.Level20.Features
{
    class PowerClericDivineInterventionImprovementBuilder : BaseDefinitionBuilder<FeatureDefinitionPower>
    {
        const string PowerClericDivineInterventionImprovementClericName = "ZSPowerClericDivineInterventionImprovementCleric";
        const string PowerClericDivineInterventionImprovementClericGuid = "cc4303e4-114e-43aa-a7ee-e197c9f8ef40";

        const string PowerClericDivineInterventionImprovementPaladinName = "ZSPowerClericDivineInterventionImprovementPaladin";
        const string PowerClericDivineInterventionImprovementPaladinGuid = "3a9f52a1-ca5d-4138-a95a-5c4d9748763d";

        const string PowerClericDivineInterventionImprovementWizardName = "ZSPowerClericDivineInterventionImprovementWizard";
        const string PowerClericDivineInterventionImprovementWizardGuid = "78b25422-6497-441e-a285-b4dd97211a32";

        protected PowerClericDivineInterventionImprovementBuilder(FeatureDefinitionPower basePower, string name, string guid) : base(basePower, name, guid)
        {
            Definition.SetHasCastingFailure(false);
            Definition.SetOverriddenPower(basePower);
        }

        private static FeatureDefinitionPower CreateAndAddToDB(FeatureDefinitionPower basePower, string name, string guid)
            => new PowerClericDivineInterventionImprovementBuilder(basePower, name, guid).AddToDB();

        internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementCleric =
            CreateAndAddToDB(PowerClericDivineInterventionCleric, PowerClericDivineInterventionImprovementClericName, PowerClericDivineInterventionImprovementClericGuid);

        internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementPaladin=
            CreateAndAddToDB(PowerClericDivineInterventionPaladin, PowerClericDivineInterventionImprovementPaladinName, PowerClericDivineInterventionImprovementPaladinGuid);

        internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementWizard=
            CreateAndAddToDB(PowerClericDivineInterventionWizard, PowerClericDivineInterventionImprovementWizardName, PowerClericDivineInterventionImprovementWizardGuid);
    }
}
