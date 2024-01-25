namespace SolastaUnfinishedBusiness.Interfaces;

/**
 * Can influence results of calls to `RulesetImplementationManager.IsValidContextForRestrictedContextProvider`
 * Note: RestrictedProperty on feature that needs modifying should not be `None` or context validation won't happen
 * Note: for now supports only single modification per feature - only first `IValidateContextInsteadOfRestrictedProperty` would apply
 */
public enum OperationType
{
    Ignore,
    Set,
    Or,
    And
}

// can validate FeatureDefinitionAdditionalDamage, FeatureDefinitionAttackModifier, FeatureDefinitionAttributeModifier, FeatureDefinitionCombatAffinity, FeatureDefinitionPower
public interface IValidateContextInsteadOfRestrictedProperty
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
