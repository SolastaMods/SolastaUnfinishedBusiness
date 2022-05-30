using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Level20.Features;

internal sealed class FeatureDefinitionIndomitableMightBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionIndomitableMight, FeatureDefinitionIndomitableMightBuilder>
{
    private const string IndomitableMightName = "ZSBarbarianIndomitableMight";
    private const string IndomitableMightGuid = "2a0e9082-c81d-4d02-800a-92f04fbe85dc";

    internal static readonly FeatureDefinition FeatureDefinitionIndomitableMight =
        CreateAndAddToDB(IndomitableMightName, IndomitableMightGuid);

    private FeatureDefinitionIndomitableMightBuilder(string name, string guid) : base(name, guid)
    {
        var guiPresentationBuilder = new GuiPresentationBuilder(
            "Feature/&BarbarianIndomitableMightTitle",
            "Feature/&BarbarianIndomitableMightDescription");

        Definition.SetGuiPresentation(guiPresentationBuilder.Build());
    }

    private static FeatureDefinition CreateAndAddToDB(string name, string guid)
    {
        return new FeatureDefinitionIndomitableMightBuilder(name, guid).AddToDB();
    }
}

internal sealed class FeatureDefinitionIndomitableMight : FeatureDefinition, IChangeAbilityCheck
{
    public int MinRoll(
        RulesetCharacter character,
        int baseBonus,
        int rollModifier,
        string abilityScoreName,
        string proficiencyName,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        List<RuleDefinitions.TrendInfo> modifierTrends)
    {
        if (character == null || abilityScoreName != AttributeDefinitions.Strength)
        {
            return 1;
        }

        return character.GetAttribute(AttributeDefinitions.Strength).CurrentValue;
    }
}
