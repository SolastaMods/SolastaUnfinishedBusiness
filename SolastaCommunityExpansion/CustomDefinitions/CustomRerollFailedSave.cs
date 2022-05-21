using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions;

public class CustomRerollFailedSave : IUsePowerToRerollFailedSave
{
    private readonly FeatureDefinitionPower power;

    public CustomRerollFailedSave(FeatureDefinitionPower power, string reactionName = null)
    {
        this.power = power;
        ReactionName = string.IsNullOrEmpty(reactionName) ? power.Name : reactionName;
    }

    public string ReactionName { get; }

    public FeatureDefinitionPower GetPowerToRerollFailedSave(RulesetCharacter character, RuleDefinitions.RollOutcome saveOutcome)
    {
        return power;
    }
}
