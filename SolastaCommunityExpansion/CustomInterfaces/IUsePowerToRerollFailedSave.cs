namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IUsePowerToRerollFailedSave
{
    string ReactionName { get; }
    FeatureDefinitionPower GetPowerToRerollFailedSave(RulesetCharacter character, RuleDefinitions.RollOutcome saveOutcome);
}
