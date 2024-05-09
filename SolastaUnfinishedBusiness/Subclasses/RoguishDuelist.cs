using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishDuelist : AbstractSubclass
{
    internal const string Name = "RoguishDuelist";
    internal const string ConditionReflexiveParryName = $"Condition{Name}ReflexiveParry";
    private const string SureFooted = "SureFooted";

    public RoguishDuelist()
    {
        // LEVEL 03

        // Daring Duel

        var conditionDaringDuel = ConditionDefinitionBuilder
            .Create($"Condition{Name}DaringDuel")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var additionalDamageDaringDuel = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DaringDuel")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.TargetIsDuelingWithYou)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionDaringDuel)
            .AddToDB();

        additionalDamageDaringDuel.AddCustomSubFeatures(
            ModifyAdditionalDamageClassLevelRogue.Instance,
            new ClassFeats.ModifyAdditionalDamageCloseQuarters(additionalDamageDaringDuel));

        // Sure Footed

        var attributeModifierSureFooted = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, 2)
            .SetSituationalContext(ExtraSituationalContext.WearingNoArmorOrLightArmorWithoutShield)
            .AddToDB();

        var featureSetSureFooted = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{SureFooted}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(FeatureDefinitionCombatAffinitys.CombatAffinityEagerForBattle, attributeModifierSureFooted)
            .AddToDB();

        // LEVEL 09

        // Swirling Dance

        var actionAffinitySwirlingDance = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}SwirlingDance")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions(ActionDefinitions.Id.SwirlingDance)
            .AddToDB();

        // LEVEL 13

        // Reflexive Parry

        var featureReflexiveParry = FeatureDefinitionBuilder
            .Create($"Feature{Name}ReflexiveParry")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureReflexiveParry.AddCustomSubFeatures(new CustomBehaviorReflexiveParry(featureReflexiveParry));

        // LEVEL 17

        // Master Duelist

        var featureMasterDuelist = FeatureDefinitionBuilder
            .Create($"Feature{Name}MasterDuelist")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(new PhysicalAttackFinishedByMeMasterDuelist(conditionDaringDuel))
            .AddToDB();

        var featureSetMasterDuelist = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}MasterDuelist")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(featureMasterDuelist)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, additionalDamageDaringDuel, featureSetSureFooted)
            .AddFeaturesAtLevel(9, actionAffinitySwirlingDance)
            .AddFeaturesAtLevel(13, featureReflexiveParry)
            .AddFeaturesAtLevel(17, featureSetMasterDuelist)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static bool TargetIsDuelingWithRoguishDuelist(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        AdvantageType advantageType)
    {
        return
            advantageType != AdvantageType.Disadvantage &&
            attacker.RulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Rogue, Name) > 0 &&
            attacker.IsWithinRange(defender, 1) &&
            Gui.Battle.AllContenders
                .Where(x => x != attacker && x != defender)
                .All(x => !attacker.IsWithinRange(x, 1));
    }

    //
    // Reflexive Parry
    //

    private sealed class CustomBehaviorReflexiveParry(
        FeatureDefinition featureReflexiveParry) : IAttackBeforeHitPossibleOnMeOrAlly
    {
        public IEnumerator OnAttackBeforeHitPossibleOnMeOrAlly(
            GameLocationBattleManager battleManager,
            [UsedImplicitly] GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect,
            int attackRoll)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if ( helper != defender ||
                 rulesetEffect != null ||
                 !ValidatorsWeapon.IsMelee(attackMode) ||
                 !defender.OncePerTurnIsValid(featureReflexiveParry.Name) ||
                 rulesetDefender.HasAnyConditionOfTypeOrSubType(
                     ConditionDefinitions.ConditionIncapacitated.Name,
                     ConditionDefinitions.ConditionShocked.Name,
                     ConditionDefinitions.ConditionSlowed.Name))
            {
                yield break;
            }

            defender.UsedSpecialFeatures.TryAdd(featureReflexiveParry.Name, 0);
            
            actionModifier.DefenderDamageMultiplier *= 0.5f;
            rulesetDefender.DamageHalved(rulesetDefender, featureReflexiveParry);
        }
    }

    //
    // Master Duelist
    //

    private sealed class PhysicalAttackFinishedByMeMasterDuelist(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDaringDuel) : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetDefender.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDaringDuel.Name, out var activeCondition))
            {
                yield break;
            }

            rulesetDefender.RemoveCondition(activeCondition);

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var actionParams = action.ActionParams.Clone();
            var attackModeMain = attacker.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            actionParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.AttackFree];
            actionParams.AttackMode = attackModeMain;

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);
        }
    }
}
