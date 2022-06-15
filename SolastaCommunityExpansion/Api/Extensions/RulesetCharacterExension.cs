using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Api.Extensions;

internal static class RulesetCharacterExension
{
    public static bool IsValid(this RulesetCharacter instance, params CharacterValidator[] validators)
    {
        return validators.All(v => v(instance));
    }

    public static bool IsValid(this RulesetCharacter instance, IEnumerable<CharacterValidator> validators)
    {
        return validators == null || validators.All(v => v(instance));
    }

    /**Checks if power has enough uses and that all validators are OK*/
    public static bool CanUsePower(this RulesetCharacter instance, FeatureDefinitionPower power)
    {
        if (power == null)
        {
            return false;
        }

        if (instance.GetRemainingPowerUses(power) <= 0)
        {
            return false;
        }

        return power.GetAllSubFeaturesOfType<IPowerUseValidity>()
            .All(v => v.CanUsePower(instance));
    }

    public static bool CanCastCantrip(
        this RulesetCharacter character,
        SpellDefinition cantrip,
        out RulesetSpellRepertoire spellRepertoire)
    {
        spellRepertoire = null;
        foreach (var reperoire in character.spellRepertoires)
        {
            foreach (var knownCantrip in reperoire.KnownCantrips)
            {
                if (knownCantrip == cantrip
                    || (knownCantrip.SpellsBundle && knownCantrip.SubspellsList.Contains(cantrip)))
                {
                    spellRepertoire = reperoire;
                    return true;
                }
            }
        }

        return false;
    }

    public static List<RulesetAttackMode> GetAttackModesByActionType(this RulesetCharacter instance,
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

    public static RulesetItem GetItemInSlot(this RulesetCharacter instance, string slot)
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
