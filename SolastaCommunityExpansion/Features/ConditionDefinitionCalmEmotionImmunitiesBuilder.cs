using SolastaModApi.Extensions;
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

        private static GuiPresentation GetCalmEmotionsFrightenedImmunityGuiPresentation()
        {
            var gp = new GuiPresentation(ConditionAffinityProtectedFromEvilFrightenedImmunity.GuiPresentation);
            gp.SetSpriteReference(ConditionCalmedByCalmEmotionsAlly.GuiPresentation.SpriteReference);
            return gp;
        }

        private static GuiPresentation GetCalmEmotionsCharmImmunityGuiPresentation()
        {
            var gp = new GuiPresentation(ConditionAffinityProtectedFromEvilCharmImmunity.GuiPresentation);
            gp.SetSpriteReference(ConditionCalmedByCalmEmotionsAlly.GuiPresentation.SpriteReference);
            return gp;
        }

        private static readonly FeatureDefinitionConditionAffinity ConditionAffinityCalmEmotionsFrightenedImmunity
            = FeatureDefinitionConditionAffinityBuilder.CreateAndAddToDB(ConditionAffinityProtectedFromEvilFrightenedImmunity,
                "CEConditionAffinityCalmEmotionsFrightenedImmunity",
                "b6243764-bcb5-4a0a-a61c-650f0b9cf59f",
                GetCalmEmotionsFrightenedImmunityGuiPresentation());

        private readonly static FeatureDefinitionConditionAffinity ConditionAffinityCalmEmotionsCharmImmunity
            = FeatureDefinitionConditionAffinityBuilder.CreateAndAddToDB(ConditionAffinityProtectedFromEvilCharmImmunity,
                "CEConditionAffinityCalmEmotionsCharmImmunity",
                "da89cda3-5562-41a5-ab37-bc0dd1111a2b",
                GetCalmEmotionsCharmImmunityGuiPresentation());

        internal static readonly ConditionDefinition ConditionCalmEmotionImmunities = CreateAndAddToDB();

        private static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionDefinitionCalmEmotionImmunitiesBuilder(
                ConditionAffinityCalmEmotionsFrightenedImmunity, ConditionAffinityCalmEmotionsCharmImmunity).AddToDB();
        }
    }
}
