using static SolastaModApi.DatabaseHelper.ConditionDefinitions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionConditionAffinitys;

namespace SolastaCommunityExpansion.Features
{
    internal sealed class ConditionDefinitionCalmEmotionImmunitiesBuilder : ConditionDefinitionBuilder
    {
        private const string Name = "CEConditionCalmEmotionImmunities";
        private const string Guid = "300fc35d-bf78-46ba-b096-1733cac4be12";

        private ConditionDefinitionCalmEmotionImmunitiesBuilder(params FeatureDefinition[] conditionFeatures) :
            base(Name, Guid, conditionFeatures, RuleDefinitions.DurationType.Minute, 1, true, ConditionCalmedByCalmEmotionsAlly.GuiPresentation)
        {
        }

        internal static readonly ConditionDefinition ConditionCalmEmotionImmunities = CreateAndAddToDB();

        private static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionDefinitionCalmEmotionImmunitiesBuilder(
                ConditionAffinityFrightenedImmunity, ConditionAffinityCharmImmunity).AddToDB();
        }
    }
}
