using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionSenses;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal class RangerFeralSensesBuilder : BaseDefinitionBuilder<FeatureDefinitionSense>
    {
        private const string RangerFeralSensesName = "ZSRangerFeralSenses";
        private const string RangerFeralSensesGuid = "0e3207505ac04a499477ca1185287117";

        protected RangerFeralSensesBuilder(string name, string guid) : base(SenseSeeInvisible12, name, guid)
        {
            Definition.SetSenseRange(6);
            Definition.GuiPresentation.Title = "Feature/&RangerFeralSensesTitle";
            Definition.GuiPresentation.Description = "Feature/&RangerFeralSensesDescription";
        }

        private static FeatureDefinitionSense CreateAndAddToDB(string name, string guid)
        {
            return new RangerFeralSensesBuilder(name, guid).AddToDB();
        }

        internal static readonly FeatureDefinitionSense RangerFeralSenses =
            CreateAndAddToDB(RangerFeralSensesName, RangerFeralSensesGuid);
    }
}
