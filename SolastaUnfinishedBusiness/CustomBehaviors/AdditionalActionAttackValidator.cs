using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomValidators;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

/**
 * Sets attacks number to 0 for attack modes that fail validation for currently active additional action feature
 */
internal interface IAdditionalActionAttackValidator
{
    bool ValidateAttackMode(RulesetCharacter character, RulesetAttackMode mode);
}

internal class AdditionalActionAttackValidator : IAdditionalActionAttackValidator
{
    private readonly IsWeaponValidHandler validator;

    internal static readonly IAdditionalActionAttackValidator MeleeOnly =
        new AdditionalActionAttackValidator(ValidatorsWeapon.IsMelee);

    internal AdditionalActionAttackValidator(IsWeaponValidHandler validator)
    {
        this.validator = validator;
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
        if (feature == null)
        {
            return;
        }

        var validator = feature.GetFirstSubFeatureOfType<IAdditionalActionAttackValidator>();
        if (validator == null)
        {
            return;
        }

        character.RulesetCharacter?.AttackModes
            .ForEach(m => {
                if (m.ActionType == type && !validator.ValidateAttackMode(character.RulesetCharacter, m))
                {
                    m.attacksNumber = 0;
                }
            });
    }

    public bool ValidateAttackMode(RulesetCharacter character, RulesetAttackMode mode)
    {
        return validator == null || validator(mode, null, character);
    }
}
