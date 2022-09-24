using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Level20;

internal sealed class FeatureDefinitionIndomitableMightBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionIndomitableMight, FeatureDefinitionIndomitableMightBuilder>
{
    private const string IndomitableMightName = "BarbarianIndomitableMight";
    private const string IndomitableMightGuid = "2a0e9082-c81d-4d02-800a-92f04fbe85dc";

    internal static readonly FeatureDefinition FeatureDefinitionIndomitableMight =
        CreateAndAddToDB(IndomitableMightName, IndomitableMightGuid);

    private FeatureDefinitionIndomitableMightBuilder(string name, string guid) : base(name, guid)
    {
        // Empty
    }

    private static FeatureDefinition CreateAndAddToDB(string name, string guid)
    {
        return new FeatureDefinitionIndomitableMightBuilder(name, guid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }
}

internal sealed class FeatureDefinitionIndomitableMight : FeatureDefinition, IChangeAbilityCheck
{
    public int MinRoll(
        [CanBeNull] RulesetCharacter character,
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
