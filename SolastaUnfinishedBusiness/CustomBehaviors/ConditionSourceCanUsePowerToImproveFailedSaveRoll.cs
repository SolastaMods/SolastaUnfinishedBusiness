namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class ConditionSourceCanUsePowerToImproveFailedSaveRoll
{
    public ConditionSourceCanUsePowerToImproveFailedSaveRoll(FeatureDefinitionPower power, string reactionName)
    {
        Power = power;
        ReactionName = reactionName;
    }

    internal FeatureDefinitionPower Power { get; }
    internal string ReactionName { get; }

    internal virtual bool ShouldTrigger(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        RuleDefinitions.RollOutcome saveOutcome,
        int saveOutcomeDelta)
    {
        return true;
    }

    internal virtual bool TryModifyRoll(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        ref RuleDefinitions.RollOutcome saveOutcome,
        ref int saveOutcomeDelta)
    {
        return true;
    }

    internal virtual string FormatReactionDescription(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RulesetCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        RuleDefinitions.RollOutcome saveOutcome,
        int saveOutcomeDelta)
    {
        return null;
    }
}
