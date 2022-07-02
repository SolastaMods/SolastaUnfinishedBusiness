using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Api.Extensions;

internal static class RulesetCharacterExension
{
    public static bool IsValid(this RulesetCharacter instance, [NotNull] params CharacterValidator[] validators)
    {
        return validators.All(v => v(instance));
    }

    public static bool IsValid(this RulesetCharacter instance, [CanBeNull] IEnumerable<CharacterValidator> validators)
    {
        return validators == null || validators.All(v => v(instance));
    }

    /**Checks if power has enough uses and that all validators are OK*/
    public static bool CanUsePower(this RulesetCharacter instance, [CanBeNull] FeatureDefinitionPower power)
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

    public static bool CanCastCantrip(
        [NotNull] this RulesetCharacter character,
        SpellDefinition cantrip,
        [CanBeNull] out RulesetSpellRepertoire spellRepertoire)
    {
        spellRepertoire = null;
        foreach (var reperoire in from reperoire in character.spellRepertoires
                 from knownCantrip in reperoire.KnownCantrips
                 where knownCantrip == cantrip
                       || (knownCantrip.SpellsBundle && knownCantrip.SubspellsList.Contains(cantrip))
                 select reperoire)
        {
            spellRepertoire = reperoire;
            return true;
        }

        return false;
    }

    [NotNull]
    public static List<RulesetAttackMode> GetAttackModesByActionType([NotNull] this RulesetCharacter instance,
        ActionDefinitions.ActionType actionType)
    {
        return instance.AttackModes
            .Where(a => !a.AfterChargeOnly && a.ActionType == actionType)
            .ToList();
    }

    public static bool CanAddAbilityBonusToOffhand(this RulesetCharacter instance)
    {
        return instance.GetSubFeaturesByType<IAttackModificationProvider>()
            .Any(p => p.CanAddAbilityBonusToSecondary);
    }

    [CanBeNull]
    public static RulesetItem GetItemInSlot([CanBeNull] this RulesetCharacter instance, string slot)
    {
        if (instance == null
            || instance.CharacterInventory == null
            || instance.CharacterInventory.InventorySlotsByName == null
           )
        {
            return null;
        }

        var inventorySlot = instance.CharacterInventory.InventorySlotsByName[slot];

        return inventorySlot?.EquipedItem;
    }
}
