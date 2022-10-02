using System;
using SolastaUnfinishedBusiness.Api.Extensions;

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
 * Note: for now supports only single modification per feature - only first `IRestictedContextValidator` would apply
 */
internal interface IRestrictedContextValidator
{
    public (OperationType, bool) ValidateContext(BaseDefinition definition, IRestrictedContextProvider provider,
        RulesetCharacter character, ItemDefinition itemDefinition, bool rangedAttack, RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect);
}

//TODO: try to find better place for this code
internal static class RestrictedContextValidatorPatch
{
    internal static bool ModifyResult(bool def, IRestrictedContextProvider provider, RulesetCharacter character,
        ItemDefinition itemDefinition, bool rangedAttack, RulesetAttackMode attackMode, RulesetEffect rulesetEffect)
    {
        if (provider is not BaseDefinition definition)
        {
            return def;
        }

        var validator = definition.GetFirstSubFeatureOfType<IRestrictedContextValidator>();
        if (validator == null)
        {
            return def;
        }

        var (operation, result) = validator.ValidateContext(definition, provider, character, itemDefinition,
            rangedAttack, attackMode, rulesetEffect);

        switch (operation)
        {
            case OperationType.Ignore:
                break;
            case OperationType.Set:
                def = result;
                break;
            case OperationType.Or:
                def = def || result;
                break;
            case OperationType.And:
                def = def && result;
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unknown operationtype '{operation}'");
        }

        return def;
    }
}
