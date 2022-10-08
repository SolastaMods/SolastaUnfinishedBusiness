using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Classes.Inventor;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class RulesetCharacterExtensions
{
#if false
    internal static bool IsWearingLightArmor([NotNull] this RulesetCharacter _)
    {
        return false;
    }
#endif

    internal static bool IsWearingMediumArmor([NotNull] this RulesetCharacter _)
    {
        return false;
    }

#if false
    internal static bool IsWieldingTwoHandedWeapon([NotNull] this RulesetCharacter _)
    {
        return false;
    }
#endif

    internal static bool IsValid(this RulesetCharacter instance, [NotNull] params IsCharacterValidHandler[] validators)
    {
        return validators.All(v => v(instance));
    }

    /**Checks if power has enough uses and that all validators are OK*/
    internal static bool CanUsePower(this RulesetCharacter instance, [CanBeNull] FeatureDefinitionPower power)
    {
        if (power == null)
        {
            return false;
        }

        if (instance.GetRemainingPowerUses(power) <= 0)
        {
            return false;
        }

        return (power.GetAllSubFeaturesOfType<IPowerUseValidity>() ?? new List<IPowerUseValidity>())
            .All(v => v.CanUsePower(instance));
    }

    internal static bool CanCastCantrip(
        [NotNull] this RulesetCharacter character,
        SpellDefinition cantrip,
        [CanBeNull] out RulesetSpellRepertoire spellRepertoire)
    {
        spellRepertoire = null;
        foreach (var repertoire in from repertoire in character.spellRepertoires
                 from knownCantrip in repertoire.KnownCantrips
                 where knownCantrip == cantrip
                       || (knownCantrip.SpellsBundle && knownCantrip.SubspellsList.Contains(cantrip))
                 select repertoire)
        {
            spellRepertoire = repertoire;
            return true;
        }

        return false;
    }

    [NotNull]
    internal static List<RulesetAttackMode> GetAttackModesByActionType([NotNull] this RulesetCharacter instance,
        ActionDefinitions.ActionType actionType)
    {
        return instance.AttackModes
            .Where(a => !a.AfterChargeOnly && a.ActionType == actionType)
            .ToList();
    }

    internal static bool CanAddAbilityBonusToOffhand(this RulesetCharacter instance)
    {
        return instance.GetSubFeaturesByType<IAttackModificationProvider>()
            .Any(p => p.CanAddAbilityBonusToSecondary);
    }

    [CanBeNull]
    internal static RulesetItem GetItemInSlot([CanBeNull] this RulesetCharacter instance, string slot)
    {
        var inventorySlot = instance?.CharacterInventory?.InventorySlotsByName?[slot];

        return inventorySlot?.EquipedItem;
    }

    [CanBeNull]
    internal static RulesetSpellRepertoire GetClassSpellRepertoire(this RulesetCharacter instance, string className)
    {
        if (string.IsNullOrEmpty(className))
        {
            return instance.GetClassSpellRepertoire();
        }

        return instance.SpellRepertoires.FirstOrDefault(r =>
            r.SpellCastingClass != null && r.SpellCastingClass.Name == className);
    }

    /**@returns true if item holds an infusion created by this character*/
    internal static bool HoldsMyInfusion(this RulesetCharacter instance, RulesetItem item)
    {
        if (item == null)
        {
            return false;
        }

        return instance.IsMyInfusion(item.SourceSummoningEffectGuid)
               || item.dynamicItemProperties.Any(property => instance.IsMyInfusion(property.SourceEffectGuid));
    }

    /**@returns true if effect with this guid is an infusion created by this character*/
    private static bool IsMyInfusion(this RulesetCharacter instance, ulong guid)
    {
        if (instance == null || guid == 0) { return false; }

        var (caster, definition) = EffectHelpers.GetCharacterAndSourceDefinitionByEffectGuid(guid);

        if (caster == null || definition == null) { return false; }

        return caster == instance
               //detecting if this item is from infusion by checking if it has infusion limiter
               && definition.GetAllSubFeaturesOfType<ILimitedEffectInstances>()
                   .Contains(InventorClass.InfusionLimiter);
    }
}
