using System.Collections.Generic;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Features;

public interface IAddExtraAttack
{
    void TryAddExtraAttack(RulesetCharacterHero hero);
}

public class AddBonusUnarmedAttack : IAddExtraAttack
{
    private readonly ActionDefinitions.ActionType actionType;
    private readonly int attacksNumber;
    private readonly bool clearSameType;
    private readonly CharacterValidator[] validators;

    public AddBonusUnarmedAttack(ActionDefinitions.ActionType actionType, int attacksNumber, bool clearSameType, params CharacterValidator[] validators)
    {
        this.actionType = actionType;
        this.attacksNumber = attacksNumber;
        this.clearSameType = clearSameType;
        this.validators = validators;
    }
    
    public AddBonusUnarmedAttack(ActionDefinitions.ActionType actionType, params CharacterValidator[] validators) : this(actionType, 1,false, validators)
    {
    }

    public void TryAddExtraAttack(RulesetCharacterHero hero)
    {
        if (!hero.IsValid(validators))
        {
            return;
        }

        var strikeDefinition = hero.UnarmedStrikeDefinition;

        var attackModifiers = hero.GetField<List<IAttackModificationProvider>>("attackModifiers");

        var attackModes = hero.AttackModes;
        if (clearSameType)
        {
            attackModes.RemoveAll(m => m.ActionType == actionType);
        }

        var attackMode = hero.RefreshAttackModePublic(
            actionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            false,
            true,
            EquipmentDefinitions.SlotTypeOffHand,
            attackModifiers,
            hero.FeaturesOrigin,
            null
        );
        attackMode.AttacksNumber = attacksNumber;
        
        attackModes.Add(attackMode);
    }
}