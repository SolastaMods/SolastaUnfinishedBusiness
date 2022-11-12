namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal delegate void ProcessConditionInterruptionHandler(RulesetActor actor,
    RuleDefinitions.ConditionInterruption interruption, int amount);
