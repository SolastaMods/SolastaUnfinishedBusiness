using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class
    SenseRangerFeralSensesBuilder : FeatureDefinitionBuilder<FeatureDefinitionSense, SenseRangerFeralSensesBuilder>
{
    private const string SenseRangerFeralSensesName = "SenseRangerFeralSenses";
    private const string SenseRangerFeralSensesGuid = "0e3207505ac04a499477ca1185287117";

    internal static readonly FeatureDefinitionSense SenseRangerFeralSenses =
        CreateAndAddToDB(SenseRangerFeralSensesName, SenseRangerFeralSensesGuid);

    private SenseRangerFeralSensesBuilder(string name, string guid) : base(SenseSeeInvisible12, name, guid)
    {
        Definition.senseRange = 6;
    }

    private static FeatureDefinitionSense CreateAndAddToDB(string name, string guid)
    {
        return new SenseRangerFeralSensesBuilder(name, guid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }
}
