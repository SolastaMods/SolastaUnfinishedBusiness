using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;
using static RuleDefinitions;

namespace SolastaCommunityExpansion.CustomDefinitions;

public abstract class AddExtraAttackBase : IAddExtraAttack
{
    protected readonly ActionDefinitions.ActionType actionType;
    private readonly List<string> additionalTags = new();
    private readonly bool clearSameType;
    private readonly CharacterValidator[] validators;

    protected AddExtraAttackBase(ActionDefinitions.ActionType actionType, bool clearSameType,
        params CharacterValidator[] validators)
    {
        this.actionType = actionType;
        this.clearSameType = clearSameType;
        this.validators = validators;
    }

    protected AddExtraAttackBase(ActionDefinitions.ActionType actionType, params CharacterValidator[] validators) :
        this(actionType, false, validators)
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

        var newAttacks = GetAttackModes(hero);
        if (newAttacks == null || newAttacks.Empty())
        {
            return;
        }

        foreach (var attackMode in newAttacks)
        {
            foreach (var tag in additionalTags)
            {
                attackMode.AddAttackTagAsNeeded(tag);
            }

            if (attackModes.Any(m => ModesEqual(attackMode, m)))
            {
                RulesetAttackMode.AttackModesPool.Return(attackMode);
            }
            else
            {
                attackModes.Add(attackMode);
            }
        }
    }

    public AddExtraAttackBase SetTags(params string[] tags)
    {
        additionalTags.AddRange(tags);
        return this;
    }

    protected abstract List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero);

    private static bool ModesEqual([NotNull] RulesetAttackMode a, RulesetAttackMode b)
    {
        return a.IsComparableForNetwork(b);
    }
}

public sealed class AddExtraUnarmedAttack : AddExtraAttackBase
{
    public AddExtraUnarmedAttack(ActionDefinitions.ActionType actionType, bool clearSameType,
        params CharacterValidator[] validators) : base(actionType, clearSameType, validators)
    {
    }

    public AddExtraUnarmedAttack(ActionDefinitions.ActionType actionType, params CharacterValidator[] validators) :
        base(actionType, validators)
    {
    }

    [NotNull]
    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacterHero hero)
    {
        var mainHandItem = hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand]
            .EquipedItem;

        var isUnarmedWeapon = mainHandItem != null && WeaponValidators.IsUnarmedWeapon(mainHandItem);
        var strikeDefinition = isUnarmedWeapon
            ? mainHandItem.ItemDefinition
            : hero.UnarmedStrikeDefinition;

        var attackModifiers = hero.attackModifiers;


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

        return new List<RulesetAttackMode> {attackMode};
    }
}

public sealed class AddExtraMainHandAttack : AddExtraAttackBase
{
    public AddExtraMainHandAttack(ActionDefinitions.ActionType actionType, bool clearSameType,
        params CharacterValidator[] validators) : base(actionType, clearSameType, validators)
    {
    }

    public AddExtraMainHandAttack(ActionDefinitions.ActionType actionType, params CharacterValidator[] validators) :
        base(actionType, validators)
    {
    }

    protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero)
    {
        var mainHandItem = hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeMainHand]
            .EquipedItem;

        var strikeDefinition = mainHandItem.ItemDefinition;

        var attackModifiers = hero.attackModifiers;

        var attackMode = hero.RefreshAttackModePublic(
            actionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            false,
            true,
            EquipmentDefinitions.SlotTypeMainHand,
            attackModifiers,
            hero.FeaturesOrigin,
            mainHandItem
        );

        return new List<RulesetAttackMode> {attackMode};
    }
}

public sealed class AddExtraRangedAttack : AddExtraAttackBase
{
    private readonly IsWeaponValidHandler weaponValidator;

    public AddExtraRangedAttack(IsWeaponValidHandler weaponValidator, ActionDefinitions.ActionType actionType,
        params CharacterValidator[] validators) :
        base(
            actionType, validators)
    {
        this.weaponValidator = weaponValidator;
    }

    protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero)
    {
        var result = new List<RulesetAttackMode>();
        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);
        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);
        return result;
    }

    private void AddItemAttack(List<RulesetAttackMode> attackModes, string slot, RulesetCharacterHero hero)
    {
        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;
        if (item == null || !weaponValidator.Invoke(null, item, hero))
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
            hero.attackModifiers,
            hero.FeaturesOrigin,
            item
        );
        attackMode.Reach = false;
        attackMode.Ranged = true;
        attackMode.Thrown = WeaponValidators.IsThrownWeapon(item);
        attackMode.AttackTags.Remove(TagsDefinitions.WeaponTagMelee);

        attackModes.Add(attackMode);
    }
}

public sealed class AddPolearmFollowupAttack : AddExtraAttackBase
{
    public AddPolearmFollowupAttack() : base(ActionDefinitions.ActionType.Bonus, false,
        CharacterValidators.HasAttacked, CharacterValidators.HasPolearm)
    {
    }

    [NotNull]
    protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero)
    {
        var result = new List<RulesetAttackMode>();
        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);
        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);
        return result;
    }

    private void AddItemAttack(List<RulesetAttackMode> attackModes, [NotNull] string slot, RulesetCharacterHero hero)
    {
        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;
        if (item == null || !WeaponValidators.IsPolearm(item))
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
            hero.attackModifiers,
            hero.FeaturesOrigin,
            item
        );
        attackMode.Reach = true;
        attackMode.Ranged = false;
        attackMode.Thrown = false;

        var damage = DamageForm.GetCopy(attackMode.EffectDescription.FindFirstDamageForm());
        damage.DieType = DieType.D4;
        damage.DiceNumber = 1;
        damage.DamageType = DamageTypeBludgeoning;

        var effectForm = EffectForm.Get();
        effectForm.FormType = EffectForm.EffectFormType.Damage;
        effectForm.DamageForm = damage;
        attackMode.EffectDescription.Clear();
        attackMode.EffectDescription.EffectForms.Add(effectForm);

        attackModes.Add(attackMode);
    }
}

public class AddBonusShieldAttack : AddExtraAttackBase
{
    public AddBonusShieldAttack() : base(ActionDefinitions.ActionType.Bonus, false)
    {
    }

    protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacterHero hero)
    {
        var inventorySlotsByName = hero.CharacterInventory.InventorySlotsByName;
        var offHandItem = inventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand]
            .EquipedItem;

        if (!ShieldStrikeContext.IsShield(offHandItem))
        {
            return null;
        }

        var attackModifiers = hero.attackModifiers;

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

        return new List<RulesetAttackMode> {attackMode};
    }
}
