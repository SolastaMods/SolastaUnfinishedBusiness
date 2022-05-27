using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.CustomDefinitions;

public class AddExtraUnarmedAttack : IAddExtraAttack
{
    private readonly ActionDefinitions.ActionType actionType;
    private readonly List<string> additionalTags = new();
    private readonly int attacksNumber;
    private readonly bool clearSameType;
    private readonly CharacterValidator[] validators;

    public AddExtraUnarmedAttack(ActionDefinitions.ActionType actionType, int attacksNumber, bool clearSameType,
        params CharacterValidator[] validators)
    {
        this.actionType = actionType;
        this.attacksNumber = attacksNumber;
        this.clearSameType = clearSameType;
        this.validators = validators;
    }

    public AddExtraUnarmedAttack(ActionDefinitions.ActionType actionType, params CharacterValidator[] validators) :
        this(actionType, 1, false, validators)
    {
    }

    public void TryAddExtraAttack(RulesetCharacterHero hero)
    {
        if (!hero.IsValid(validators))
        {
            return;
        }

        var mainHandItem = hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand]
            .EquipedItem;

        var isUnarmedWeapon = mainHandItem != null && WeaponValidators.IsUnarmedWeapon(mainHandItem);
        var strikeDefinition = isUnarmedWeapon
            ? mainHandItem.ItemDefinition
            : hero.UnarmedStrikeDefinition;

        var attackModifiers = hero.GetField<List<IAttackModificationProvider>>("attackModifiers");

        var attackModes = hero.AttackModes;
        if (clearSameType)
        {
            for (var i = attackModes.Count - 1; i > 0; i--)
            {
                var mode = attackModes[i];
                if (mode.ActionType == actionType)
                {
                    RulesetAttackMode.AttackModesPool.Return(mode);
                    attackModes.RemoveAt(i);
                }
            }
        }

        var attackMode = hero.RefreshAttackModePublic(
            actionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            false,
            true,
            EquipmentDefinitions.SlotTypeMainHand,
            attackModifiers,
            hero.FeaturesOrigin,
            isUnarmedWeapon ? mainHandItem : null
        );
        attackMode.AttacksNumber = attacksNumber;
        attackMode.AttackTags.AddRange(additionalTags);

        if (attackModes.Any(m => attackMode.IsComparableForNetwork(m)))
        {
            RulesetAttackMode.AttackModesPool.Return(attackMode);
        }
        else
        {
            attackModes.Add(attackMode);
        }
    }

    public AddExtraUnarmedAttack SetTags(params string[] tags)
    {
        additionalTags.AddRange(tags);
        return this;
    }
}

public class AddExtraThrownAttack : IAddExtraAttack
{
    private readonly ActionDefinitions.ActionType actionType;
    private readonly List<string> additionalTags = new();
    private readonly int attacksNumber;
    private readonly bool clearSameType;
    private readonly CharacterValidator[] validators;

    public AddExtraThrownAttack(ActionDefinitions.ActionType actionType, int attacksNumber, bool clearSameType,
        params CharacterValidator[] validators)
    {
        this.actionType = actionType;
        this.attacksNumber = attacksNumber;
        this.clearSameType = clearSameType;
        this.validators = validators;
    }

    public AddExtraThrownAttack(ActionDefinitions.ActionType actionType, params CharacterValidator[] validators) :
        this(actionType, 1, false, validators)
    {
    }

    public void TryAddExtraAttack(RulesetCharacterHero hero)
    {
        if (!hero.IsValid(validators))
        {
            return;
        }

        var attackModes = hero.AttackModes;
        if (clearSameType)
        {
            for (var i = attackModes.Count - 1; i > 0; i--)
            {
                var mode = attackModes[i];
                if (mode.ActionType == actionType)
                {
                    RulesetAttackMode.AttackModesPool.Return(mode);
                    attackModes.RemoveAt(i);
                }
            }
        }

        AddItemAttack(attackModes, EquipmentDefinitions.SlotTypeMainHand, hero);
        AddItemAttack(attackModes, EquipmentDefinitions.SlotTypeOffHand, hero);
    }

    private void AddItemAttack(List<RulesetAttackMode> attackModes, string slot, RulesetCharacterHero hero)
    {
        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;
        if (item == null || !WeaponValidators.IsThrownWeapon(item))
        {
            return;
        }

        var strikeDefinition = item.ItemDefinition;

        var attackMode = hero.RefreshAttackModePublic(
            actionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            false,
            true,
            slot,
            hero.GetField<List<IAttackModificationProvider>>("attackModifiers"),
            hero.FeaturesOrigin,
            item
        );
        attackMode.Reach = false;
        attackMode.Ranged = true;
        attackMode.Thrown = true;
        attackMode.AttacksNumber = attacksNumber;
        attackMode.AttackTags.AddRange(additionalTags);
        attackMode.AttackTags.Remove(TagsDefinitions.WeaponTagMelee);

        if (attackModes.Any(m => attackMode.IsComparableForNetwork(m)))
        {
            RulesetAttackMode.AttackModesPool.Return(attackMode);
        }
        else
        {
            attackModes.Add(attackMode);
        }
    }

    public AddExtraThrownAttack SetTags(params string[] tags)
    {
        additionalTags.AddRange(tags);
        return this;
    }
}

public class AddBonusShieldAttack : IAddExtraAttack
{
    public void TryAddExtraAttack(RulesetCharacterHero hero)
    {
        var inventorySlotsByName = hero.CharacterInventory.InventorySlotsByName;
        var offHandItem = inventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand]
            .EquipedItem;

        if (!ShieldStrikeContext.IsShield(offHandItem))
        {
            return;
        }

        var attackModes = hero.AttackModes;
        var attackModifiers = hero.GetField<List<IAttackModificationProvider>>("attackModifiers");

        var attackMode = hero.RefreshAttackModePublic(
            ActionDefinitions.ActionType.Bonus,
            offHandItem.ItemDefinition,
            ShieldStrikeContext.ShieldWeaponDescription,
            false,
            hero.CanAddAbilityBonusToOffhand(),
            EquipmentDefinitions.SlotTypeOffHand,
            attackModifiers,
            hero.FeaturesOrigin,
            offHandItem
        );

        var features = new List<FeatureDefinition>();

        var bonus = 0;
        offHandItem.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(features);
        foreach (var modifier in features.OfType<FeatureDefinitionAttributeModifier>())
        {
            if (modifier.ModifiedAttribute != AttributeDefinitions.ArmorClass)
            {
                continue;
            }

            if (modifier.ModifierType != FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive)
            {
                continue;
            }

            bonus += modifier.ModifierValue;
        }

        if (bonus != 0)
        {
            var damage = attackMode.EffectDescription?.FindFirstDamageForm();
            var trendInfo = new TrendInfo(bonus, FeatureSourceType.Equipment, offHandItem.Name, null);

            attackMode.ToHitBonus += bonus;
            attackMode.ToHitBonusTrends.Add(trendInfo);

            if (damage != null)
            {
                damage.BonusDamage += bonus;
                damage.DamageBonusTrends.Add(trendInfo);
            }
        }

        if (attackModes.Any(m => attackMode.IsComparableForNetwork(m)))
        {
            RulesetAttackMode.AttackModesPool.Return(attackMode);
        }
        else
        {
            attackModes.Add(attackMode);
        }
    }
}