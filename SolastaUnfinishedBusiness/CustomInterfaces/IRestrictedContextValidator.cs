namespace SolastaUnfinishedBusiness.CustomInterfaces;

//TODO: maybe move this enum to different place?
public enum OperationType
{
    Ignore,
    Set,
    Or,
    And
}

/**
 * Can influence results of calls to `RulesetImplementationManager.IsValidContextForRestrictedContextProvider`
 * Note: RestrictedProperty on feature that needs modifying should not be `None` or context validation won't happen
 * Note: for now supports only single modification per feature - only first `IRestrictedContextValidator` would apply
 */
public interface IRestrictedContextValidator
{
    public (OperationType, bool) ValidateContext(
        BaseDefinition definition,
        IRestrictedContextProvider provider,
        RulesetCharacter character,
        ItemDefinition itemDefinition,
        bool rangedAttack,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect);
}
