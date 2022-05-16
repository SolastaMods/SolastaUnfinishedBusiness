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
    private readonly CharacterValidator[] validators;

    public AddBonusUnarmedAttack(params CharacterValidator[] validators)
    {
        this.validators = validators;
    }

    public void TryAddExtraAttack(RulesetCharacterHero hero)
    {
        if (!hero.IsValid(validators))
        {
            return;
        }

        var strikeDefinition = hero.UnarmedStrikeDefinition;

        var attackModifiers = hero.GetField<List<IAttackModificationProvider>>("attackModifiers");

        hero.AttackModes.Add(hero.RefreshAttackModePublic(
            ActionDefinitions.ActionType.Bonus,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            false,
            true,
            EquipmentDefinitions.SlotTypeOffHand,
            attackModifiers,
            hero.FeaturesOrigin,
            null
        ));
    }
}