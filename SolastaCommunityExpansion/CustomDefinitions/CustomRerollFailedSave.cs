using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public sealed class CustomRerollFailedSave : IUsePowerToRerollFailedSave
{
    private readonly FeatureDefinitionPower power;

    public CustomRerollFailedSave(FeatureDefinitionPower power, [CanBeNull] string reactionName = null)
    {
        this.power = power;
        ReactionName = string.IsNullOrEmpty(reactionName) ? power.Name : reactionName;
    }

    public string ReactionName { get; }

    public FeatureDefinitionPower GetPowerToRerollFailedSave(RulesetCharacter character,
        RuleDefinitions.RollOutcome saveOutcome)
    {
        return power;
    }
}
