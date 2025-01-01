using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Subclasses;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Validators;

public delegate bool IsCharacterValidHandler(RulesetCharacter character);

internal static class ValidatorsCharacter
{
    internal static readonly IsCharacterValidHandler HasTacticalMovesAvailable = character =>
    {
        var glc = GameLocationCharacter.GetFromActor(character);

        return Gui.Battle == null || glc is { RemainingTacticalMoves: > 0 };
    };

    internal static readonly IsCharacterValidHandler HasBonusAttackAvailable = character =>
    {
        if (Gui.Battle == null)
        {
            return true;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

        if (gameLocationCharacter == null ||
            gameLocationCharacter.GetActionStatus(Id.AttackOff, ActionScope.Battle) != ActionStatus.Available)
        {
            return false;
        }

        var maxAttacksNumber = character.AttackModes
            .FirstOrDefault(attackMode => attackMode.ActionType == ActionType.Bonus)?.AttacksNumber ?? 0;

        return maxAttacksNumber - gameLocationCharacter.UsedBonusAttacks > 0;
    };

    internal static readonly IsCharacterValidHandler HasMainAttackAvailable = character =>
    {
        if (Gui.Battle == null)
        {
            return true;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

        if (gameLocationCharacter == null ||
            gameLocationCharacter.GetActionStatus(Id.AttackMain, ActionScope.Battle) != ActionStatus.Available)
        {
            return false;
        }

        var maxAttacksNumber = character.AttackModes
            .FirstOrDefault(attackMode => attackMode.ActionType == ActionType.Main)?.AttacksNumber ?? 0;

        return maxAttacksNumber - gameLocationCharacter.UsedMainAttacks > 0;
    };

    internal static readonly IsCharacterValidHandler HasAvailableMoves = character =>
    {
        var locationCharacter = GameLocationCharacter.GetFromActor(character);

        return locationCharacter is { RemainingTacticalMoves: > 0 };
    };

    internal static readonly IsCharacterValidHandler HasAvailableBonusDash = character =>
    {
        var locationCharacter = GameLocationCharacter.GetFromActor(character);

        return locationCharacter != null &&
               locationCharacter.GetActionStatus(Id.DashBonus, ActionScope.Battle) == ActionStatus.Available;
    };

    internal static readonly IsCharacterValidHandler HasAvailableBonusAction = character =>
    {
        var locationCharacter = GameLocationCharacter.GetFromActor(character);

        return locationCharacter != null &&
               locationCharacter.GetActionTypeStatus(ActionType.Bonus) == ActionStatus.Available;
    };

    internal static readonly IsCharacterValidHandler HasUnavailableBonusAction = character =>
    {
        var locationCharacter = GameLocationCharacter.GetFromActor(character);

        return locationCharacter != null &&
               locationCharacter.GetActionTypeStatus(ActionType.Bonus) == ActionStatus.Unavailable;
    };

    internal static readonly IsCharacterValidHandler HasAttacked = character => character.ExecutedAttacks > 0;

    internal static readonly IsCharacterValidHandler HasNoArmor = character => !character.IsWearingArmor();

    internal static readonly IsCharacterValidHandler HasArmor = character => character.IsWearingArmor();

    internal static readonly IsCharacterValidHandler HasNoShield = character => !character.IsWearingShield();

    internal static readonly IsCharacterValidHandler HasShield = character => character.IsWearingShield();

    internal static readonly IsCharacterValidHandler HasLightArmor = character =>
        HasArmorCategory(character, EquipmentDefinitions.LightArmorCategory);

    internal static readonly IsCharacterValidHandler HasHeavyArmor = character =>
        HasArmorCategory(character, EquipmentDefinitions.HeavyArmorCategory);

    internal static readonly IsCharacterValidHandler DoesNotHaveHeavyArmor = character =>
        !HasArmorCategory(character, EquipmentDefinitions.HeavyArmorCategory);

    internal static readonly IsCharacterValidHandler HasLightSourceOffHand = character =>
        character is RulesetCharacterHero && character.GetOffhandWeapon()?.ItemDefinition.IsLightSourceItem == true;

    internal static readonly IsCharacterValidHandler HasFreeHand = character =>
    {
        var mainHand = character.GetMainWeapon();
        var offHand = character.GetOffhandWeapon();

        return ValidatorsWeapon.IsUnarmed(offHand) ||
               ValidatorsWeapon.IsUnarmed(mainHand) ||
               character is RulesetCharacterMonster;
    };

    internal static readonly IsCharacterValidHandler HasFreeHandBoth = character =>
    {
        var mainHand = character.GetMainWeapon();
        var offHand = character.GetOffhandWeapon();

        return (ValidatorsWeapon.IsUnarmed(offHand) &&
                ValidatorsWeapon.IsUnarmed(mainHand)) ||
               character is RulesetCharacterMonster;
    };

    internal static readonly IsCharacterValidHandler HasFreeHandWithoutTwoHandedInMain = character =>
        HasFreeHand(character) &&
        (character.GetMainWeapon()?.ItemDefinition is not { } itemDefinition ||
         !ValidatorsWeapon.HasAnyWeaponTag(
             itemDefinition, TagsDefinitions.WeaponTagTwoHanded));

    internal static readonly IsCharacterValidHandler HasFreeHandConsiderGrapple = character =>
    {
        var freeHands = 0;
        var mainHand = character.GetMainWeapon();
        var offHand = character.GetOffhandWeapon();
        var hasGrappleSource = GrappleContext.HasGrappleSource(character);

        if (ValidatorsWeapon.IsUnarmed(mainHand))
        {
            freeHands++;
        }

        if (ValidatorsWeapon.IsUnarmed(offHand))
        {
            freeHands++;
        }

        if (hasGrappleSource)
        {
            freeHands--;
        }

        return freeHands > 0 || character is RulesetCharacterMonster;
    };

    internal static readonly IsCharacterValidHandler HasTwoHandedQuarterstaff = character =>
        ValidatorsWeapon.IsWeaponType(character.GetMainWeapon(), QuarterstaffType) && IsFreeOffhandVanilla(character);

    internal static readonly IsCharacterValidHandler HasLongbow = character =>
        ValidatorsWeapon.IsWeaponType(character.GetMainWeapon(), LongbowType);

    internal static readonly IsCharacterValidHandler HasMeleeWeaponInMainHandOrUnarmed = character =>
        ValidatorsWeapon.IsUnarmed(character.GetMainWeapon()) || HasMeleeWeaponInMainHand(character);

    internal static readonly IsCharacterValidHandler HasMeleeWeaponInMainHand = character =>
    {
        var weapon = character.GetMainWeapon();

        return ValidatorsWeapon.IsMelee(null, weapon, character);
    };

    private static readonly IsCharacterValidHandler HasMeleeWeaponInOffHand = character =>
    {
        var weapon = character.GetOffhandWeapon();

        return ValidatorsWeapon.IsMelee(null, weapon, character);
    };

    internal static readonly IsCharacterValidHandler HasMeleeWeaponInMainAndOffhand = character =>
        HasMeleeWeaponInMainHand(character) && HasMeleeWeaponInOffHand(character);

#if false
    internal static readonly IsCharacterValidHandler HasMeleeWeaponInMainOrOffhand = character =>
        HasMeleeWeaponInMainHand(character) || HasMeleeWeaponInOffHand(character);
#endif

    internal static readonly IsCharacterValidHandler HasMeleeWeaponInMainHandAndFreeOffhand = character =>
        HasFreeHandWithoutTwoHandedInMain(character) &&
        HasMeleeWeaponInMainHand(character);

    internal static readonly IsCharacterValidHandler HasMonkWeaponInMainHandAndFreeOffhand = character =>
        HasFreeHand(character) &&
        character.IsMonkWeaponOrUnarmed(character.GetMainWeapon()?.ItemDefinition);
    
    internal static readonly IsCharacterValidHandler IsNotInBrightLight = character =>
        HasAnyOfLightingStates(
            LocationDefinitions.LightingState.Darkness,
            LocationDefinitions.LightingState.Unlit,
            LocationDefinitions.LightingState.Dim)(character);

    internal static readonly IsCharacterValidHandler IsUnlitOrDarkness = character =>
        HasAnyOfLightingStates(
            LocationDefinitions.LightingState.Darkness,
            LocationDefinitions.LightingState.Unlit)(character);

    internal static bool HasBowWithoutArmor(RulesetCharacter character)
    {
        return HasNoArmor(character) &&
               ValidatorsWeapon.IsWeaponType(character.GetMainWeapon(), ShortbowType, LongbowType);
    }

    internal static IsCharacterValidHandler HasAvailablePowerUsage(FeatureDefinitionPower power)
    {
        // must use GetRemainingPowerUses
        return character => character.GetRemainingPowerUses(power) > 0;
    }

    internal static IsCharacterValidHandler HasNotAvailablePowerUsage(FeatureDefinitionPower power)
    {
        // must use GetRemainingPowerUses
        return character => character.GetRemainingPowerUses(power) == 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsCharacterValidHandler HasAnyOfConditions(params string[] conditions)
    {
        return character => conditions.Any(character.HasConditionOfTypeOrSubType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsCharacterValidHandler HasNoneOfConditions(params string[] conditions)
    {
        return character => !conditions.Any(character.HasConditionOfTypeOrSubType);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IsCharacterValidHandler HasAnyOfLightingStates(
        params LocationDefinitions.LightingState[] lightingStates)
    {
        return character =>
        {
            var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

            return gameLocationCharacter != null && lightingStates.Contains(gameLocationCharacter.LightingState);
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsCharacterValidHandler HasMainHandWeaponType(params WeaponTypeDefinition[] weaponTypeDefinition)
    {
        return character => ValidatorsWeapon.IsWeaponType(character.GetMainWeapon(), weaponTypeDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsCharacterValidHandler HasOffhandWeaponType(params WeaponTypeDefinition[] weaponTypeDefinition)
    {
        return character => ValidatorsWeapon.IsWeaponType(character.GetOffhandWeapon(), weaponTypeDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsCharacterValidHandler HasWeaponType(params WeaponTypeDefinition[] weaponTypeDefinition)
    {
        return character =>
            ValidatorsWeapon.IsWeaponType(character.GetMainWeapon(), weaponTypeDefinition) ||
            ValidatorsWeapon.IsWeaponType(character.GetOffhandWeapon(), weaponTypeDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static IsCharacterValidHandler HasUsedWeaponType(WeaponTypeDefinition weaponTypeDefinition)
    {
        return character =>
        {
            var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

            return gameLocationCharacter is { HasAttackedSinceLastTurn: true } &&
                   !gameLocationCharacter.OncePerTurnIsValid(weaponTypeDefinition.Name);
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void RegisterWeaponTypeUsed(
        GameLocationCharacter gameLocationCharacter,
        RulesetAttackMode attackMode)
    {
        if (attackMode?.ActionType == ActionType.Reaction)
        {
            return;
        }

        if (attackMode?.SourceDefinition is not ItemDefinition itemDefinition)
        {
            return;
        }

        var type = itemDefinition.IsWeapon
            ? itemDefinition.WeaponDescription.WeaponType
            : itemDefinition.ArmorDescription.ArmorType;

        gameLocationCharacter.UsedSpecialFeatures.TryAdd(type, 0);
        gameLocationCharacter.UsedSpecialFeatures[type]++;
    }

    //
    // BOOL VALIDATORS
    //

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMonkWeaponOrUnarmed(this RulesetActor character, WeaponDescription weaponDescription)
    {
        var monkWeaponSpecializations = character.GetSubFeaturesByType<WayOfBlade.WeaponSpecialization>();

        return weaponDescription == null ||
               weaponDescription.IsMonkWeaponOrUnarmed() ||
               WayOfZenArchery.IsZenArcheryWeapon(character, weaponDescription) ||
               (Main.Settings.EnableMonkKatanaSpecialization &&
                weaponDescription.WeaponTypeDefinition == CustomWeaponsContext.KatanaWeaponType) ||
               monkWeaponSpecializations.Exists(x => x.WeaponType == weaponDescription.WeaponTypeDefinition);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsMonkWeaponOrUnarmed(this RulesetCharacter character, ItemDefinition itemDefinition)
    {
        if (!itemDefinition)
        {
            return false;
        }

        return itemDefinition.IsWeapon && character.IsMonkWeaponOrUnarmed(itemDefinition.WeaponDescription);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsFreeOffhandVanilla(RulesetCharacter character)
    {
        var offHand = character.GetOffhandWeapon();

        // does character have free offhand in TA's terms as used in RefreshAttackModes for Monk bonus unarmed attack?
        return offHand == null || !offHand.ItemDefinition.IsWeapon;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool HasConditionWithSubFeatureOfType<T>(this RulesetCharacter character) where T : class
    {
        return character.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Any(rulesetCondition => rulesetCondition.ConditionDefinition.HasSubFeatureOfType<T>());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool HasArmorCategory(RulesetCharacter character, string category)
    {
        // required for wildshape scenarios
        if (character is not RulesetCharacterHero)
        {
            return false;
        }

        var equipedItem =
            character.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

        if (equipedItem == null || !equipedItem.ItemDefinition.IsArmor)
        {
            return false;
        }

        var armorDescription = equipedItem.ItemDefinition.ArmorDescription;
        var element = DatabaseHelper.GetDefinition<ArmorTypeDefinition>(armorDescription.ArmorType);

        return DatabaseHelper.GetDefinition<ArmorCategoryDefinition>(element.ArmorCategory).IsPhysicalArmor &&
               element.ArmorCategory == category;
    }
}
