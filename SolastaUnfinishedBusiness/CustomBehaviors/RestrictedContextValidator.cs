using System;
using SolastaUnfinishedBusiness.Api.GameExtensions;
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

    public RestrictedContextValidator(IsContextValidHandler validator)
    {
        this.validator = validator;
    }

    internal RestrictedContextValidator(OperationType operation, IsCharacterValidHandler validator)
        : this((_, _, character, _, _, _, _) => (operation, validator(character)))
    {
        // Empty
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

        var validator = definition.GetFirstSubFeatureOfType<IRestrictedContextValidator>();

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
                throw new ArgumentOutOfRangeException($"Unknown operationtype '{operation}'");
        }

        return def;
    }
}
