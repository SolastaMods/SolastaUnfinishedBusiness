using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Validators;

/**
 * Sets attacks number to 0 for attack modes that fail validation for currently active additional action feature
 */
internal interface IValidateAdditionalActionAttack
{
    bool ValidateAttackMode(RulesetCharacter character, RulesetAttackMode mode);
}

internal class ValidateAdditionalActionAttack : IValidateAdditionalActionAttack
{
    internal static readonly IValidateAdditionalActionAttack MeleeOnly =
        new ValidateAdditionalActionAttack(ValidatorsWeapon.IsMelee);

    internal static readonly IValidateAdditionalActionAttack TwoHandedRanged =
        new ValidateAdditionalActionAttack(ValidatorsWeapon.IsTwoHandedRanged);

    private readonly IsWeaponValidHandler _validator;

    private ValidateAdditionalActionAttack(IsWeaponValidHandler validator)
    {
        _validator = validator;
    }

    public bool ValidateAttackMode(RulesetCharacter character, RulesetAttackMode mode)
    {
        return _validator == null || _validator(mode, null, character);
    }

    internal static void ValidateAttackModes(RulesetCharacter character)
    {
        //TODO: Implement for wild shapes? Currently only RulesetCharacterHero calls this
        var locCharacter = GameLocationCharacter.GetFromActor(character);

        if (locCharacter == null)
        {
            return;
        }

        ValidateActionType(locCharacter, ActionDefinitions.ActionType.Main);
        ValidateActionType(locCharacter, ActionDefinitions.ActionType.Bonus);
        //TODO: should we implement for Reaction and NoCost?
    }

    private static void ValidateActionType(GameLocationCharacter character, ActionDefinitions.ActionType type)
    {
        var feature = character.GetCurrentAdditionalActionFeature(type);

        if (!feature)
        {
            return;
        }

        var validator = feature.GetFirstSubFeatureOfType<IValidateAdditionalActionAttack>();

        if (validator == null)
        {
            return;
        }

        character.RulesetCharacter?.AttackModes
            .ForEach(m =>
            {
                if (m.ActionType == type && !validator.ValidateAttackMode(character.RulesetCharacter, m))
                {
                    m.attacksNumber = 0;
                }
            });
    }
}
