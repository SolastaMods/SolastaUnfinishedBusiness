using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal class ConditionSourceCanUsePowerToImproveFailedSaveRoll
{
    protected ConditionSourceCanUsePowerToImproveFailedSaveRoll(FeatureDefinitionPower power, string reactionName)
    {
        Power = power;
        ReactionName = reactionName;
    }

    internal FeatureDefinitionPower Power { get; }
    internal string ReactionName { get; }

    [UsedImplicitly]
    internal virtual bool ShouldTrigger(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        RuleDefinitions.RollOutcome saveOutcome,
        int saveOutcomeDelta)
    {
        return true;
    }

    [UsedImplicitly]
    internal virtual bool TryModifyRoll(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        CharacterActionParams reactionParams,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        ref RuleDefinitions.RollOutcome saveOutcome,
        ref int saveOutcomeDelta)
    {
        return true;
    }

    [UsedImplicitly]
    internal virtual string FormatReactionDescription(
        CharacterAction action,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter helper,
        ActionModifier saveModifier,
        bool hasHitVisual,
        bool hasBorrowedLuck,
        RuleDefinitions.RollOutcome saveOutcome,
        int saveOutcomeDelta)
    {
        return null;
    }
}
