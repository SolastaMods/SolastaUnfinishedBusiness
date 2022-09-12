using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class PowerClericDivineInterventionImprovementBuilder : FeatureDefinitionPowerBuilder
{
    private const string PowerClericDivineInterventionImprovementClericName =
        "PowerClericDivineInterventionImprovementCleric";

    private const string PowerClericDivineInterventionImprovementClericGuid =
        "cc4303e4-114e-43aa-a7ee-e197c9f8ef40";

    private const string PowerClericDivineInterventionImprovementPaladinName =
        "PowerClericDivineInterventionImprovementPaladin";

    private const string PowerClericDivineInterventionImprovementPaladinGuid =
        "3a9f52a1-ca5d-4138-a95a-5c4d9748763d";

    private const string PowerClericDivineInterventionImprovementWizardName =
        "PowerClericDivineInterventionImprovementWizard";

    private const string PowerClericDivineInterventionImprovementWizardGuid =
        "78b25422-6497-441e-a285-b4dd97211a32";

    internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementCleric =
        CreateAndAddToDB(PowerClericDivineInterventionCleric, PowerClericDivineInterventionImprovementClericName,
            PowerClericDivineInterventionImprovementClericGuid);

    internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementPaladin =
        CreateAndAddToDB(PowerClericDivineInterventionPaladin, PowerClericDivineInterventionImprovementPaladinName,
            PowerClericDivineInterventionImprovementPaladinGuid);

    internal static readonly FeatureDefinitionPower PowerClericDivineInterventionImprovementWizard =
        CreateAndAddToDB(PowerClericDivineInterventionWizard, PowerClericDivineInterventionImprovementWizardName,
            PowerClericDivineInterventionImprovementWizardGuid);

    private PowerClericDivineInterventionImprovementBuilder(FeatureDefinitionPower basePower, string name,
        string guid) : base(basePower, name, guid)
    {
        Definition.hasCastingFailure = false;
        Definition.overriddenPower = basePower;
    }

    private static FeatureDefinitionPower CreateAndAddToDB(FeatureDefinitionPower basePower, string name,
        string guid)
    {
        return new PowerClericDivineInterventionImprovementBuilder(basePower, name, guid).AddToDB();
    }
}
