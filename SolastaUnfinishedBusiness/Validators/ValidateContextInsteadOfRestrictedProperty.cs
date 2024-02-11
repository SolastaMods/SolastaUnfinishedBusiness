using System;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Validators;

internal delegate (OperationType, bool) IsContextValidHandler(
    BaseDefinition definition,
    IRestrictedContextProvider provider,
    RulesetCharacter character,
    ItemDefinition itemDefinition,
    bool rangedAttack,
    RulesetAttackMode attackMode,
    RulesetEffect rulesetEffect);

internal class ValidateContextInsteadOfRestrictedProperty(IsContextValidHandler validator)
    : IValidateContextInsteadOfRestrictedProperty
{
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

#if false
    public static ValidateContextInsteadOfRestrictedProperty And(
        OperationType type, params IValidateContextInsteadOfRestrictedProperty[] validators)
    {
        return new ValidateContextInsteadOfRestrictedProperty(
            (definition, provider, character, itemDefinition, rangedAttack, attackMode, rulesetEffect) =>
            {
                foreach (var validator in validators)
                {
                    // Ignore sub validator operation type
                    var (_, result) = validator.ValidateContext(
                        definition, provider, character, itemDefinition, rangedAttack, attackMode, rulesetEffect);

                    if (!result)
                    {
                        return (type, false);
                    }
                }

                return (type, true);
            });
    }
#endif

    public static ValidateContextInsteadOfRestrictedProperty Or(
        OperationType type, params IValidateContextInsteadOfRestrictedProperty[] validators)
    {
        return new ValidateContextInsteadOfRestrictedProperty(
            (definition, provider, character, itemDefinition, rangedAttack, attackMode, rulesetEffect) =>
            {
                foreach (var validator in validators)
                {
                    // Ignore sub validator operation type
                    var (_, result) = validator.ValidateContext(
                        definition, provider, character, itemDefinition, rangedAttack, attackMode, rulesetEffect);

                    if (result)
                    {
                        return (type, true);
                    }
                }

                return (type, false);
            });
    }
}

public static class RestrictedContextValidatorPatch
{
    public static bool ModifyResult(
        bool def,
        IRestrictedContextProvider provider,
        RulesetCharacter character,
        ItemDefinition itemDefinition,
        bool rangedAttack,
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect)
    {
        if (provider is not BaseDefinition definition)
        {
            return def;
        }

        var validator = definition.GetFirstSubFeatureOfType<IValidateContextInsteadOfRestrictedProperty>();

        if (validator == null)
        {
            return def;
        }

        var (operation, result) = validator.ValidateContext(
            definition, provider, character, itemDefinition, rangedAttack, attackMode, rulesetEffect);

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
                throw new ArgumentOutOfRangeException($"Unknown operation type '{operation}'");
        }

        return def;
    }
}
