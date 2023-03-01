using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Classes.Inventor;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static ActionDefinitions;
using static RuleDefinitions;

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

    internal static RulesetItem GetMainWeapon(this RulesetCharacter hero)
    {
        return hero.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand);
    }

    internal static RulesetItem GetOffhandWeapon(this RulesetCharacter hero)
    {
        return hero.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand);
    }

    // ReSharper disable once UnusedParameter.Global
    internal static bool IsWearingMediumArmor([NotNull] this RulesetCharacter _)
    {
        return false;
    }

    internal static bool IsValid(this RulesetCharacter instance, [NotNull] params IsCharacterValidHandler[] validators)
    {
        return validators.All(v => v(instance));
    }

    internal static bool IsValid(this RulesetCharacter instance,
        [NotNull] IEnumerable<IsCharacterValidHandler> validators)
    {
        return validators.All(v => v(instance));
    }

    internal static bool HasPower(
        this RulesetCharacter instance,
        [CanBeNull] FeatureDefinitionPower power)
    {
        return instance.GetPowerFromDefinition(power) != null && instance.HasAnyFeature(power);
    }

    internal static bool CanSeeAndUseAtLeastOnePower(this RulesetCharacter character, ActionType type, bool battle)
    {
        var usablePowers = character.UsablePowers;
        var overridenPowers = new List<FeatureDefinitionPower>();

        foreach (var power in usablePowers.Where(x => x.PowerDefinition.OverriddenPower != null))
        {
            overridenPowers.TryAdd(power.PowerDefinition.OverriddenPower);
        }

        foreach (var usablePower in usablePowers)
        {
            var power = usablePower.PowerDefinition;
            if (power.DelegatedToAction)
            {
                continue;
            }

            if (overridenPowers.Contains(power))
            {
                continue;
            }

            var activationTime = power.ActivationTime;

            if (activationTime is not (ActivationTime.Action
                or ActivationTime.BonusAction
                or ActivationTime.NoCost
                or ActivationTime.Reaction
                or ActivationTime.Minute1
                or ActivationTime.Minute10
                or ActivationTime.Hours1
                or ActivationTime.Hours24
                or ActivationTime.Rest
                or ActivationTime.Permanent
                or ActivationTime.PermanentUnlessIncapacitated))
            {
                continue;
            }

            if (battle)
            {
                if (!CastingTimeToActionDefinition.ContainsKey(activationTime))
                {
                    continue;
                }

                var activation = CastingTimeToActionDefinition[activationTime];

                if (activation != type)
                {
                    continue;
                }
            }

            if (PowerVisibilityModifier.IsPowerHidden(character, power, type))
            {
                continue;
            }

            if (power.GuiPresentation.Hidden)
            {
                continue;
            }

            if (!character.CanUsePower(power, true, true))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    /**Checks if power has enough uses and that all validators are OK*/
    internal static bool CanUsePower(this RulesetCharacter instance,
        [CanBeNull] FeatureDefinitionPower power,
        bool considerUses = true,
        bool considerHaving = false)
    {
        if (power == null)
        {
            return false;
        }

        if (considerHaving && !instance.HasPower(power))
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

    internal static bool CanCastCantrip(this RulesetCharacter character,
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

        var classDefinition = DatabaseHelper.GetDefinition<CharacterClassDefinition>(className);

        return instance.GetClassSpellRepertoire(classDefinition);
    }

    [CanBeNull]
    internal static RulesetSpellRepertoire GetClassSpellRepertoire(this RulesetCharacter instance,
        CharacterClassDefinition classDefinition)
    {
        var className = classDefinition == null ? string.Empty : classDefinition.name;

        if (string.IsNullOrEmpty(className) || instance is not RulesetCharacterHero hero)
        {
            return instance.GetClassSpellRepertoire();
        }

        CharacterSubclassDefinition subclassDefinition = null;

        if (classDefinition != null)
        {
            hero.ClassesAndSubclasses.TryGetValue(classDefinition, out subclassDefinition);
        }

        return instance.SpellRepertoires.FirstOrDefault(r =>
            (r.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Class &&
             r.SpellCastingClass == classDefinition) ||
            (r.SpellCastingFeature.SpellCastingOrigin == FeatureDefinitionCastSpell.CastingOrigin.Subclass &&
             r.SpellCastingSubclass == subclassDefinition));
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
                ConditionConjuredCreature, out var conjured))
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
        Id actionId,
        ActionScope scope,
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

            if (definition.HasSubFeatureOfType<Hidden>())
            {
                continue;
            }

            if (scope == ActionScope.Battle)
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

            if (isValid)
            {
                return true;
            }
        }

        return false;
    }

    internal static bool KnowsAnyInvocationOfActionId(this RulesetCharacter instance,
        Id actionId,
        ActionScope scope)
    {
        if (instance.Invocations.Empty())
        {
            return false;
        }

        foreach (var invocation in instance.Invocations)
        {
            bool isValid;
            var definition = invocation.invocationDefinition;

            if (scope == ActionScope.Battle)
            {
                isValid = definition.GetActionId() == actionId;
            }
            else
            {
                isValid = definition.GetMainActionId() == actionId;
            }

            if (isValid)
            {
                return true;
            }
        }

        return false;
    }

    internal static void ShowDieRoll(
        this RulesetCharacter character,
        DieType dieType,
        int roll1,
        int roll2 = 0,
        string title = "",
        bool displayOutcome = false,
        RollOutcome outcome = RollOutcome.Neutral,
        bool displayModifier = false,
        int modifier = 0,
        AdvantageType advantage = AdvantageType.None
    )
    {
        if (Gui.GameLocation.FiniteStateMachine.CurrentState is LocationState_NarrativeSequence or LocationState_Map)
        {
            return;
        }

        var labelScreen = Gui.GuiService.GetScreen<GameLocationLabelScreen>();

        if (labelScreen == null)
        {
            return;
        }

        var worldChar = labelScreen.characterLabelsMap.Keys
            .FirstOrDefault(x => x.gameCharacter.RulesetCharacter == character);

        if (worldChar == null)
        {
            return;
        }

        var roll = advantage switch
        {
            AdvantageType.Advantage => Math.Max(roll1, roll2),
            AdvantageType.Disadvantage => Math.Min(roll1, roll2),
            _ => roll1
        };

        var label = labelScreen.characterLabelsMap[worldChar];

        var info = new DieRollModule.RollInfo(
            title,
            dieType,
            DieRollModule.RollType.Attack,
            roll,
            advantage,
            roll1,
            modifier,
            roll2,
            outcome,
            displayOutcome: displayOutcome,
            side: character.Side,
            displayModifier: displayModifier) { rollImmediatly = false };

        label.dieRollModule.RollDie(info);
    }

    internal static bool IsToggleEnabled(this RulesetCharacter rulesetCharacter, Id actionId)
    {
        var toggleName = actionId.ToString();

        return !rulesetCharacter.ToggledPowersOn.Contains(toggleName);
    }

    internal static void DisableToggle(this RulesetCharacter rulesetCharacter, Id actionId)
    {
        var toggleName = actionId.ToString();

        if (!rulesetCharacter.ToggledPowersOn.Contains(toggleName))
        {
            rulesetCharacter.ToggledPowersOn.Add(toggleName);
        }
    }

    internal static void EnableToggle(this RulesetCharacter rulesetCharacter, Id actionId)
    {
        var toggleName = actionId.ToString();

        rulesetCharacter.ToggledPowersOn.Remove(toggleName);
    }

    internal static RulesetAttackMode TryRefreshAttackMode(
        this RulesetCharacter character,
        ActionType actionType,
        ItemDefinition itemDefinition,
        WeaponDescription weaponDescription,
        bool freeOffHand,
        bool canAddAbilityDamageBonus,
        string slotName,
        List<IAttackModificationProvider> attackModifiers,
        Dictionary<FeatureDefinition, FeatureOrigin> featuresOrigin,
        RulesetItem weapon = null)
    {
        return character switch
        {
            RulesetCharacterHero hero => hero.RefreshAttackMode(
                actionType,
                itemDefinition,
                weaponDescription,
                freeOffHand,
                canAddAbilityDamageBonus,
                slotName,
                attackModifiers,
                featuresOrigin,
                weapon),
            RulesetCharacterMonster monster => monster.RefreshAttackMode(
                actionType,
                itemDefinition,
                weaponDescription,
                canAddAbilityDamageBonus,
                attackModifiers,
                featuresOrigin),
            _ => null
        };
    }

    internal static bool CanMagicEffectPreventHit(
        this RulesetCharacter character,
        IMagicEffect effect,
        int totalAttack)
    {
        return character.CanAttackOutcomeFromAlterationMagicalEffectFail(
            effect.EffectDescription.EffectForms,
            totalAttack);
    }
}
