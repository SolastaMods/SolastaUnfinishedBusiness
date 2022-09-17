using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public delegate (OperationType, bool) IsContextValidHandler(BaseDefinition definition,
    IRestrictedContextProvider provider,
    RulesetCharacter character, ItemDefinition itemDefinition, bool rangedAttack, RulesetAttackMode attackMode,
    RulesetEffect rulesetEffect);

public class RestrictedContextValidator : IRestrictedContextValidator
{
    private readonly IsContextValidHandler validator;

    public RestrictedContextValidator(IsContextValidHandler validator)
    {
        this.validator = validator;
    }

    public RestrictedContextValidator(OperationType operation, IsCharacterValidHandler validator)
        : this((_, _, character, _, _, _, _) => (operation, validator(character)))
    {
    }

    public (OperationType, bool) ValidateContext(BaseDefinition definition, IRestrictedContextProvider provider,
        RulesetCharacter character, ItemDefinition itemDefinition, bool rangedAttack, RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect)
    {
        return validator(definition, provider, character, itemDefinition, rangedAttack, attackMode, rulesetEffect);
    }
}