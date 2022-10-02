using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal delegate (OperationType, bool) IsContextValidHandler(
    BaseDefinition definition,
    IRestrictedContextProvider provider,
    RulesetCharacter character,
    ItemDefinition itemDefinition,
    bool rangedAttack,
    RulesetAttackMode attackMode,
    RulesetEffect rulesetEffect);

internal class RestrictedContextValidator : IRestrictedContextValidator
{
    private readonly IsContextValidHandler validator;

    internal RestrictedContextValidator(IsContextValidHandler validator)
    {
        this.validator = validator;
    }

    internal RestrictedContextValidator(OperationType operation, IsCharacterValidHandler validator)
        : this((_, _, character, _, _, _, _) => (operation, validator(character)))
    {
    }

    public (OperationType, bool) ValidateContext(
        BaseDefinition definition,
        IRestrictedContextProvider provider,
        RulesetCharacter character,
        ItemDefinition itemDefinition,
        bool rangedAttack,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect)
    {
        return validator(definition, provider, character, itemDefinition, rangedAttack, attackMode, rulesetEffect);
    }
}
