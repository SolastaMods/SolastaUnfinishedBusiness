using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.BehaviorsSpecific;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

internal enum AttackModeOrder
{
    Start,
    End
}

internal abstract class AddExtraAttackBase(
    ActionDefinitions.ActionType actionType,
    params IsCharacterValidHandler[] validators) : IAddExtraAttack
{
    // private readonly List<string> additionalTags = new();
    protected readonly ActionDefinitions.ActionType ActionType = actionType;

    public void TryAddExtraAttack(RulesetCharacter character)
    {
        if (!character.IsValid(validators))
        {
            return;
        }

        var attackModes = character.AttackModes;

        var newAttacks = GetAttackModes(character);

        if (newAttacks == null || newAttacks.Count == 0)
        {
            return;
        }

        foreach (var attackMode in newAttacks)
        {
            var same = attackModes.FirstOrDefault(m => ModesEqual(attackMode, m));

            if (same != null)
            {
                //If same attack mode exists, ensure it has max amount of attacks
                same.attacksNumber = Math.Max(attackMode.attacksNumber, same.attacksNumber);
                //and dispose of newly created one
                RulesetAttackMode.AttackModesPool.Return(attackMode);
            }
            else
            {
                var order = GetOrder(character);

                switch (order)
                {
                    case AttackModeOrder.Start:
                        attackModes.Insert(0, attackMode);
                        break;
                    case AttackModeOrder.End:
                        attackModes.Add(attackMode);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(order.ToString());
                }
            }
        }
    }

    public virtual int Priority()
    {
        return 0;
    }

    protected abstract List<RulesetAttackMode> GetAttackModes(RulesetCharacter character);

    protected virtual AttackModeOrder GetOrder(RulesetCharacter character)
    {
        return AttackModeOrder.End;
    }

    //Copied from RulesetAttackMode.IsComparableForNetwork, but not checking for attack number
    private static bool ModesEqual([NotNull] RulesetAttackMode a, RulesetAttackMode b)
    {
        //added all these locals for debug log
        var actionType = a.actionType == b.actionType;
        var sourceDefinition = a.sourceDefinition == b.sourceDefinition;
        var sourceObject = a.sourceObject == b.sourceObject;
        var slotName = a.slotName == b.slotName;
        var ranged = a.ranged == b.ranged;
        var thrown = a.thrown == b.thrown;
        var reach = a.reach == b.reach;
        var reachRange = a.reachRange == b.reachRange;
        var closeRange = a.closeRange == b.closeRange;
        var maxRange = a.maxRange == b.maxRange;
        var toHitBonus = a.toHitBonus == b.toHitBonus;
        //var attacksNumber = a.attacksNumber == b.attacksNumber;
        var useVersatileDamage = a.useVersatileDamage == b.useVersatileDamage;
        var freeOffHand = a.freeOffHand == b.freeOffHand;
        var automaticHit = a.automaticHit == b.automaticHit;
        var afterChargeOnly = a.afterChargeOnly == b.afterChargeOnly;

        return actionType
               && sourceDefinition
               && sourceObject
               && slotName
               && ranged
               && thrown
               && reach
               && reachRange
               && closeRange
               && maxRange
               && toHitBonus
               // && attacksNumber
               && useVersatileDamage
               && freeOffHand
               && automaticHit
               && afterChargeOnly;
    }
}

internal sealed class AddExtraUnarmedAttack : AddExtraAttackBase
{
    internal AddExtraUnarmedAttack(
        ActionDefinitions.ActionType actionType,
        params IsCharacterValidHandler[] validators) : base(actionType, validators)
    {
        // Empty
    }

    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
    {
        var hero = character as RulesetCharacterHero;
        var originalHero = character.GetOriginalHero();
        var monster = character as RulesetCharacterMonster;

        if (hero == null && monster == null)
        {
            return null;
        }

        var mainHandItem = hero.GetMainWeapon();
        var isUnarmedWeapon = mainHandItem != null && ValidatorsWeapon.IsUnarmed(mainHandItem.ItemDefinition, null);
        var strikeDefinition = isUnarmedWeapon
            ? mainHandItem.ItemDefinition
            : originalHero != null
                ? originalHero.UnarmedStrikeDefinition
                : DatabaseHelper.ItemDefinitions.UnarmedStrikeBase;

        var attackModifiers = hero?.attackModifiers ?? monster?.attackModifiers;

        var attackMode = character.TryRefreshAttackMode(
            ActionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            ValidatorsCharacter.IsFreeOffhandVanilla(hero),
            true,
            EquipmentDefinitions.SlotTypeMainHand,
            attackModifiers,
            character.FeaturesOrigin,
            isUnarmedWeapon ? mainHandItem : null
        );

        return [attackMode];
    }
}

internal sealed class AddExtraMainHandAttack : AddExtraAttackBase
{
    internal AddExtraMainHandAttack(
        ActionDefinitions.ActionType actionType,
        params IsCharacterValidHandler[] validators) : base(actionType, validators)
    {
        // Empty
    }

    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return null;
        }

        var mainHandItem = hero.GetMainWeapon();

        // don't use ?? on Unity Objects as it bypasses the lifetime check on the underlying object
        var strikeDefinition = mainHandItem?.ItemDefinition;

#pragma warning disable IDE0270
        if (strikeDefinition == null)
#pragma warning restore IDE0270
        {
            strikeDefinition = hero.UnarmedStrikeDefinition;
        }

        var attackModifiers = hero.attackModifiers;

        var attackMode = hero.RefreshAttackMode(
            ActionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            ValidatorsCharacter.IsFreeOffhand(hero),
            true,
            EquipmentDefinitions.SlotTypeMainHand,
            attackModifiers,
            hero.FeaturesOrigin,
            mainHandItem
        );

        return [attackMode];
    }
}

internal sealed class AddExtraRangedAttack : AddExtraAttackBase
{
    private readonly IsWeaponValidHandler _weaponValidator;

    internal AddExtraRangedAttack(
        ActionDefinitions.ActionType actionType,
        IsWeaponValidHandler weaponValidator,
        params IsCharacterValidHandler[] validators) : base(actionType, validators)
    {
        _weaponValidator = weaponValidator;
    }

    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return null;
        }

        var result = new List<RulesetAttackMode>();

        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);
        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);

        return result;
    }

    private void AddItemAttack(
        // ReSharper disable once SuggestBaseTypeForParameter
        List<RulesetAttackMode> attackModes,
        [NotNull] string slot,
        [NotNull] RulesetCharacterHero hero)
    {
        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;

        if (item == null || !_weaponValidator.Invoke(null, item, hero))
        {
            return;
        }

        var strikeDefinition = item.ItemDefinition;
        var attackMode = hero.RefreshAttackMode(
            ActionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            ValidatorsCharacter.IsFreeOffhand(hero),
            true,
            slot,
            hero.attackModifiers,
            hero.FeaturesOrigin,
            item
        );

        attackMode.Reach = false;
        attackMode.Ranged = true;
        attackMode.Thrown = ValidatorsWeapon.HasAnyWeaponTag(item, TagsDefinitions.WeaponTagThrown);
        attackMode.AttackTags.Remove(TagsDefinitions.WeaponTagMelee);

        attackModes.Add(attackMode);
    }
}

internal sealed class AddPolearmFollowUpAttack : AddExtraAttackBase
{
    private readonly WeaponTypeDefinition _weaponTypeDefinition;

    internal AddPolearmFollowUpAttack(WeaponTypeDefinition weaponTypeDefinition) : base(
        ActionDefinitions.ActionType.Bonus,
        ValidatorsCharacter.HasUsedWeaponType(weaponTypeDefinition),
        ValidatorsCharacter.HasMainHandWeaponType(weaponTypeDefinition))
    {
        _weaponTypeDefinition = weaponTypeDefinition;
    }

    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return null;
        }

        var result = new List<RulesetAttackMode>();

        AddItemAttack(result, EquipmentDefinitions.SlotTypeMainHand, hero);

        return result;
    }

    private void AddItemAttack(
        // ReSharper disable once SuggestBaseTypeForParameter
        List<RulesetAttackMode> attackModes,
        [NotNull] string slot,
        [NotNull] RulesetCharacterHero hero)
    {
        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;

        if (item == null || !ValidatorsWeapon.IsWeaponType(item, _weaponTypeDefinition))
        {
            return;
        }

        var strikeDefinition = item.ItemDefinition;
        var attackMode = hero.RefreshAttackMode(
            ActionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            ValidatorsCharacter.IsFreeOffhand(hero),
            true,
            slot,
            hero.attackModifiers,
            hero.FeaturesOrigin,
            item
        );

        attackMode.Reach = true;
        attackMode.Ranged = false;
        attackMode.Thrown = false;

        // this is required to correctly interact with Spear Mastery dice upgrade
        attackMode.AttackTags.Add("Polearm");

        var damage = DamageForm.GetCopy(attackMode.EffectDescription.FindFirstDamageForm());

        damage.DieType = DieType.D4;
        damage.VersatileDieType = DieType.D4;
        damage.versatile = false;
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

internal sealed class AddBonusShieldAttack : AddExtraAttackBase
{
    internal AddBonusShieldAttack() : base(ActionDefinitions.ActionType.Bonus)
    {
        // Empty
    }

    [CanBeNull]
    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return null;
        }

        var offHandItem = hero.GetOffhandWeapon();

        if (offHandItem == null || !ValidatorsWeapon.IsShield(offHandItem))
        {
            return null;
        }

        var attackModifiers = hero.attackModifiers;
        var attackMode = hero.RefreshAttackMode(
            ActionDefinitions.ActionType.Bonus,
            offHandItem.ItemDefinition,
            ShieldStrike.ShieldWeaponDescription,
            ValidatorsCharacter.IsFreeOffhand(hero),
            hero.CanAddAbilityBonusToOffhand(),
            EquipmentDefinitions.SlotTypeOffHand,
            attackModifiers,
            hero.FeaturesOrigin,
            offHandItem
        );

        var attackModes = new List<RulesetAttackMode> { attackMode };
        var features = new List<FeatureDefinition>();

        offHandItem.EnumerateFeaturesToBrowse<FeatureDefinitionAttributeModifier>(features);

        var bonus = features
            .OfType<FeatureDefinitionAttributeModifier>()
            .Where(x =>
                x.ModifiedAttribute == AttributeDefinitions.ArmorClass &&
                x.ModifierOperation == AttributeModifierOperation.Additive)
            .Sum(x => x.ModifierValue);

        if (offHandItem.ItemDefinition.Magical || bonus > 0)
        {
            attackMode.AddAttackTagAsNeeded(TagsDefinitions.MagicalWeapon);
        }

        if (bonus == 0)
        {
            return attackModes;
        }

        var damage = attackMode.EffectDescription?.FindFirstDamageForm();
        var trendInfo = new TrendInfo(bonus, FeatureSourceType.Equipment,
            offHandItem.ItemDefinition.GuiPresentation.Title, null);

        attackMode.ToHitBonus += bonus;
        attackMode.ToHitBonusTrends.Add(trendInfo);

        if (damage == null)
        {
            return attackModes;
        }

        damage.BonusDamage += bonus;
        damage.DamageBonusTrends.Add(trendInfo);

        return attackModes;
    }
}

internal sealed class AddBonusTorchAttack : AddExtraAttackBase
{
    private readonly FeatureDefinitionPower _torchPower;

    internal AddBonusTorchAttack(FeatureDefinitionPower torchPower) : base(ActionDefinitions.ActionType.Bonus)
    {
        _torchPower = torchPower;
    }

    protected override List<RulesetAttackMode> GetAttackModes([NotNull] RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return null;
        }

        var result = new List<RulesetAttackMode>();

        AddItemAttack(result, EquipmentDefinitions.SlotTypeOffHand, hero);

        return result;
    }

    private void AddItemAttack(
        // ReSharper disable once SuggestBaseTypeForParameter
        List<RulesetAttackMode> attackModes,
        [NotNull] string slot,
        [NotNull] RulesetCharacterHero hero)
    {
        var item = hero.CharacterInventory.InventorySlotsByName[slot].EquipedItem;

        if (item == null || !ValidatorsCharacter.HasLightSourceOffHand(hero))
        {
            return;
        }

        var strikeDefinition = item.ItemDefinition;
        var attackMode = hero.RefreshAttackMode(
            ActionType,
            strikeDefinition,
            strikeDefinition.WeaponDescription,
            ValidatorsCharacter.IsFreeOffhand(hero),
            true,
            slot,
            hero.attackModifiers,
            hero.FeaturesOrigin,
            item
        );

        attackMode.Reach = false;
        attackMode.Ranged = false;
        attackMode.Thrown = false;
        attackMode.AutomaticHit = true;
        attackMode.EffectDescription.Clear();
        attackMode.EffectDescription.Copy(_torchPower.EffectDescription);

        var proficiencyBonus = hero.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
        var dexterity = hero.TryGetAttributeValue(AttributeDefinitions.Dexterity);

        attackMode.EffectDescription.fixedSavingThrowDifficultyClass =
            8 + proficiencyBonus + AttributeDefinitions.ComputeAbilityScoreModifier(dexterity);

        attackModes.Add(attackMode);
    }
}

internal sealed class AddExtraFlurryOfArrowsAttack()
    : AddExtraAttackBase(ActionDefinitions.ActionType.Bonus, ValidatorsCharacter.HasBowWithoutArmor)
{
    protected override AttackModeOrder GetOrder(RulesetCharacter character)
    {
        return AttackModeOrder.Start;
    }

    protected override List<RulesetAttackMode> GetAttackModes(RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero || !ValidatorsCharacter.HasBowWithoutArmor(hero))
        {
            return null;
        }

        var mainHandItem = hero.GetMainWeapon();
        var attackModifiers = hero.attackModifiers;
        var attackMode = hero.RefreshAttackMode(
            ActionType,
            mainHandItem!.ItemDefinition,
            mainHandItem.ItemDefinition.WeaponDescription,
            false,
            true,
            EquipmentDefinitions.SlotTypeMainHand,
            attackModifiers,
            hero.FeaturesOrigin,
            mainHandItem);

        attackMode.attacksNumber = 1;

        return [attackMode];
    }
}
