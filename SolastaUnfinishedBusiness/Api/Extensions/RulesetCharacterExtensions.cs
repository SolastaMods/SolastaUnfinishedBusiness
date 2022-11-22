using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Classes.Inventor;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Api.Extensions;

internal static class RulesetCharacterExtensions
{
#if false
    internal static bool IsWearingLightArmor([NotNull] this RulesetCharacter _)
    {
        return false;
    }

    internal static bool IsWieldingTwoHandedWeapon([NotNull] this RulesetCharacter _)
    {
        return false;
    }
#endif

    internal static bool IsWearingMediumArmor([NotNull] this RulesetCharacter _)
    {
        return false;
    }

    internal static bool IsValid(this RulesetCharacter instance, [NotNull] params IsCharacterValidHandler[] validators)
    {
        return validators.All(v => v(instance));
    }

    /**Checks if power has enough uses and that all validators are OK*/
    internal static bool CanUsePower(
        this RulesetCharacter instance,
        [CanBeNull] FeatureDefinitionPower power,
        bool considerUses = true)
    {
        if (power == null)
        {
            return false;
        }

        if (considerUses && instance.GetRemainingPowerUses(power) <= 0)
        {
            return false;
        }

        return power.GetAllSubFeaturesOfType<IPowerUseValidity>()
            .All(v => v.CanUsePower(instance, power));
    }

    internal static bool CanCastCantrip(
        [NotNull] this RulesetCharacter character,
        SpellDefinition cantrip,
        [CanBeNull] out RulesetSpellRepertoire spellRepertoire)
    {
        spellRepertoire = null;

        foreach (var repertoire in character.spellRepertoires
                     .Where(repertoire => repertoire.KnownCantrips
                         .Any(knownCantrip =>
                             knownCantrip == cantrip ||
                             (knownCantrip.SpellsBundle && knownCantrip.SubspellsList.Contains(cantrip)))))
        {
            spellRepertoire = repertoire;

            return true;
        }

        return false;
    }

#if false
    [NotNull]
    internal static List<RulesetAttackMode> GetAttackModesByActionType([NotNull] this RulesetCharacter instance,
        ActionDefinitions.ActionType actionType)
    {
        return instance.AttackModes
            .Where(a => !a.AfterChargeOnly && a.ActionType == actionType)
            .ToList();
    }
#endif

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

    [CanBeNull]
    internal static RulesetSpellRepertoire GetClassSpellRepertoire(
        this RulesetCharacter instance,
        CharacterClassDefinition classDefinition)
    {
        var name = string.Empty;

        if (classDefinition != null)
        {
            name = classDefinition.Name;
        }

        return instance.GetClassSpellRepertoire(name);
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
        if (instance == null || guid == 0)
        {
            return false;
        }

        var (caster, definition) = EffectHelpers.GetCharacterAndSourceDefinitionByEffectGuid(guid);

        if (caster == null || definition == null)
        {
            return false;
        }

        return caster == instance
               //detecting if this item is from infusion by checking if it has infusion limiter
               && definition.GetAllSubFeaturesOfType<ILimitEffectInstances>().Contains(InventorClass.InfusionLimiter);
    }

    /**@returns character who summoned this creature, or null*/
    internal static GameLocationCharacter GetMySummoner(this RulesetCharacter instance)
    {
        if (instance == null)
        {
            return null;
        }

        if (!instance.TryGetConditionOfCategoryAndType(AttributeDefinitions.TagConjure,
                RuleDefinitions.ConditionConjuredCreature, out var conjured))
        {
            return null;
        }

        return RulesetEntity.TryGetEntity<RulesetCharacter>(conjured.SourceGuid, out var actor)
            ? GameLocationCharacter.GetFromActor(actor)
            : null;
    }

    internal static int GetClassLevel(this RulesetCharacter instance, CharacterClassDefinition classDefinition)
    {
        return instance is not RulesetCharacterHero hero ? 0 : hero.GetClassLevel(classDefinition);
    }

    internal static int GetClassLevel(this RulesetCharacter instance, string className)
    {
        return instance is not RulesetCharacterHero hero ? 0 : hero.GetClassLevel(className);
    }

    internal static bool CanCastAnyInvocationOfActionId(this RulesetCharacter instance,
        ActionDefinitions.Id actionId,
        ActionDefinitions.ActionScope scope,
        bool canCastSpells,
        bool canOnlyUseCantrips)
    {
        if (instance.Invocations.Empty())
        {
            return false;
        }

        foreach (var invocation in instance.Invocations)
        {
            bool isValid;
            var definition = invocation.invocationDefinition;
            if (scope == ActionDefinitions.ActionScope.Battle)
            {
                isValid = definition.GetActionId() == actionId;
            }
            else
            {
                isValid = definition.GetMainActionId() == actionId;
            }

            if (isValid && definition.GrantedSpell != null)
            {
                if (!canCastSpells)
                {
                    isValid = false;
                }
                else if (canOnlyUseCantrips && definition.GrantedSpell.SpellLevel > 0)
                {
                    isValid = false;
                }
            }

            if (isValid && instance.CanCastInvocation(invocation))
            {
                return true;
            }
        }

        return false;
    }
}
