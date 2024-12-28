using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static ActionDefinitions;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

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

    internal static RulesetCharacter GetEffectControllerOrSelf(this RulesetCharacter rulesetCharacter)
    {
        if (rulesetCharacter is not RulesetCharacterEffectProxy effectProxy)
        {
            return rulesetCharacter;
        }

        var controllerCharacter = EffectHelpers.GetCharacterByGuid(effectProxy.ControllerGuid);

        return controllerCharacter ?? rulesetCharacter;
    }

    internal static int GetSubclassLevel(
        this RulesetCharacter character, CharacterClassDefinition klass, string subclass)
    {
        var hero = character.GetOriginalHero();

        if (hero == null
            || !hero.ClassesAndSubclasses.TryGetValue(klass, out var characterSubclassDefinition)
            || characterSubclassDefinition.Name != subclass)
        {
            return 0;
        }

        return hero.GetClassLevel(klass);
    }

    internal static DieType GetMonkDieType(this RulesetCharacter character)
    {
        var monkLevel = character.GetClassLevel(DatabaseHelper.CharacterClassDefinitions.Monk);
        var dieType = DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierMonkMartialArtsImprovedDamage
            .DieTypeByRankTable
            .Find(x => x.Rank == monkLevel)?.DieType ?? DieType.D1;

        return dieType;
    }

    internal static RulesetItem GetMainWeapon(this RulesetCharacter hero)
    {
        return hero.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand);
    }

    internal static RulesetItem GetOffhandWeapon(this RulesetCharacter hero)
    {
        return hero.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand);
    }

    internal static bool IsWearingMediumArmor([NotNull] this RulesetCharacter character)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return false;
        }

        var equipedItem = hero.characterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeTorso].EquipedItem;

        if (equipedItem == null || !equipedItem.ItemDefinition.IsArmor)
        {
            return false;
        }

        var armorDescription = equipedItem.ItemDefinition.ArmorDescription;
        var element = DatabaseRepository.GetDatabase<ArmorTypeDefinition>().GetElement(armorDescription.ArmorType);

        return DatabaseRepository.GetDatabase<ArmorCategoryDefinition>()
                   .GetElement(element.ArmorCategory).IsPhysicalArmor
               && element.ArmorCategory == EquipmentDefinitions.MediumArmorCategory;
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

        foreach (var power in usablePowers.Where(x => x.PowerDefinition.OverriddenPower))
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

            if (ModifyPowerVisibility.IsPowerHidden(character, power, type))
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
        if (!power)
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

        return power.GetAllSubFeaturesOfType<IValidatePowerUse>()
            .All(v => v.CanUsePower(instance, power));
    }

    internal static bool CanCastCantrip(this RulesetCharacter character,
        SpellDefinition cantrip,
        [CanBeNull] out RulesetSpellRepertoire spellRepertoire)
    {
        spellRepertoire = null;

        foreach (var repertoire in character.spellRepertoires.Where(repertoire => repertoire.KnownCantrips.Any(Matches)
                     || repertoire.ExtraSpellsByTag.SelectMany(x => x.Value).Any(Matches)))
        {
            spellRepertoire = repertoire;

            return true;
        }

        return false;

        bool Matches(SpellDefinition knownCantrip)
        {
            return knownCantrip == cantrip ||
                   (knownCantrip.SpellsBundle && knownCantrip.SubspellsList.Contains(cantrip));
        }
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

    internal static bool CanAddAbilityBonusToOffhand(this RulesetCharacter instance)
    {
        return instance.GetSubFeaturesByType<IAttackModificationProvider>()
            .Any(p => p.CanAddAbilityBonusToSecondary);
    }
#endif

    [CanBeNull]
    internal static RulesetItem GetItemInSlot([CanBeNull] this RulesetCharacter instance, string slot)
    {
        var inventorySlot = instance?.CharacterInventory?.InventorySlotsByName?[slot];

        return inventorySlot?.EquipedItem;
    }

    [CanBeNull]
    internal static RulesetSpellRepertoire GetClassSpellRepertoire(
        this RulesetCharacter instance,
        string className)
    {
        if (string.IsNullOrEmpty(className))
        {
            return instance.GetClassSpellRepertoire();
        }

        var classDefinition = DatabaseHelper.GetDefinition<CharacterClassDefinition>(className);

        return instance.GetClassSpellRepertoire(classDefinition);
    }

    [CanBeNull]
    internal static RulesetSpellRepertoire GetClassSpellRepertoire(
        this RulesetCharacter instance,
        CharacterClassDefinition classDefinition)
    {
        var className = !classDefinition ? string.Empty : classDefinition.name;
        var gameLocationCharacter = instance.GetMySummoner();
        var rulesetCharacter = gameLocationCharacter?.RulesetCharacter ?? instance;

        if (string.IsNullOrEmpty(className) || rulesetCharacter is not RulesetCharacterHero hero)
        {
            return rulesetCharacter.GetClassSpellRepertoire();
        }

        CharacterSubclassDefinition subclassDefinition = null;

        if (classDefinition)
        {
            hero.ClassesAndSubclasses.TryGetValue(classDefinition, out subclassDefinition);
        }

        return rulesetCharacter.SpellRepertoires.FirstOrDefault(r =>
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

        if (caster == null || !definition)
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
        var hero = instance.GetOriginalHero();

        return hero?.GetClassLevel(classDefinition) ?? 0;
    }

    internal static int GetClassLevel(this RulesetCharacter instance, string className)
    {
        var hero = instance.GetOriginalHero();

        return hero?.GetClassLevel(className) ?? 0;
    }

    internal static bool HasActiveInvocation(this RulesetCharacter self, InvocationDefinition invocation)
    {
        return self?.Invocations.Any(i => i.InvocationDefinition == invocation && i.Active) == true;
    }

    internal static bool KnowsAnyInvocationOfActionId(this RulesetCharacter instance,
        Id actionId,
        ActionScope scope)
    {
        if (instance.Invocations.Count == 0)
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
        AdvantageType advantage = AdvantageType.None)
    {
        if (Gui.GameLocation.FiniteStateMachine.CurrentState is LocationState_NarrativeSequence or LocationState_Map)
        {
            return;
        }

        var labelScreen = Gui.GuiService.GetScreen<GameLocationLabelScreen>();

        if (!labelScreen)
        {
            return;
        }

        var worldChar = labelScreen.characterLabelsMap.Keys
            .FirstOrDefault(x => x.gameCharacter.RulesetCharacter == character);

        if (!worldChar)
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

        return rulesetCharacter.ToggledPowersOn.Contains(toggleName);
    }

    internal static void DisableToggle(this RulesetCharacter rulesetCharacter, Id actionId)
    {
        var toggleName = actionId.ToString();

        rulesetCharacter.ToggledPowersOn.Remove(toggleName);
    }

    internal static void EnableToggle(this RulesetCharacter rulesetCharacter, Id actionId)
    {
        var toggleName = actionId.ToString();

        rulesetCharacter.ToggledPowersOn.Add(toggleName);
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

    internal static bool IsMyFavoriteEnemy(this RulesetCharacter me, RulesetCharacter enemy)
    {
        if (me == null || enemy == null)
        {
            return false;
        }

        return DatabaseHelper.FeatureDefinitionFeatureSets.AdditionalDamageRangerFavoredEnemyChoice.FeatureSet
            .OfType<FeatureDefinitionAdditionalDamage>()
            .Intersect(me.GetFeaturesByType<FeatureDefinitionAdditionalDamage>())
            .Any(x => x.RequiredCharacterFamily.Name == enemy.CharacterFamily);
    }
#if false
    internal static void ShowLabel(this RulesetCharacter character, string text, string color = Gui.ColorBrokenWhite)
    {
        if (character == null)
        {
            return;
        }

        if (!ServiceRepository.GetService<IWorldLocationEntityFactoryService>()
                .TryFindWorldCharacter(character, out var worldCharacter))
        {
            return;
        }

        var labels = Gui.GuiService.GetScreen<GameLocationLabelScreen>();
        if (!labels.characterLabelsMap.TryGetValue(worldCharacter, out var label))
        {
            return;
        }

        label.EnqueueCaption(new CharacterLabel.CaptionInfo { caption = text, colorString = color });
    }
#endif
    [CanBeNull]
    internal static RulesetCharacterHero GetOriginalHero(this RulesetCharacter character)
    {
        return character as RulesetCharacterHero ?? character.OriginalFormCharacter as RulesetCharacterHero;
    }

    internal static bool HasTemporaryConditionOfType(this RulesetCharacter character, string conditionName)
    {
        return character.ConditionsByCategory
            .SelectMany(x => x.Value)
            .Any(condition => condition.ConditionDefinition.IsSubtypeOf(conditionName) &&
                              condition.DurationType != DurationType.Permanent);
    }
}
