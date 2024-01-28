using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishSlayer : AbstractSubclass
{
    private const string Name = "RoguishSlayer";
    private const string Elimination = "Elimination";
    private const string ChainOfExecution = "ChainOfExecution";
    private const string CloakOfShadows = "CloakOfShadows";

    public RoguishSlayer()
    {
        //
        // Elimination
        //

        var featureElimination = FeatureDefinitionBuilder
            .Create($"Feature{Name}{Elimination}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureElimination.AddCustomSubFeatures(new CustomBehaviorElimination(featureElimination));

        //
        // Chain of Execution
        //

        var conditionChainOfExecutionBeneficial = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ChainOfExecution}Beneficial")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBullsStrength)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1)
            .AddToDB();

        var conditionChainOfExecutionDetrimental = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ChainOfExecution}Detrimental")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBleeding)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        conditionChainOfExecutionDetrimental.AddCustomSubFeatures(
            new OnConditionAddedOrRemovedChainOfExecution(
                conditionChainOfExecutionBeneficial,
                conditionChainOfExecutionDetrimental));

        var rogueHolder = new RogueHolder();

        var additionalDamageChainOfExecution = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{ChainOfExecution}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionChainOfExecutionDetrimental
                })
            .SetImpactParticleReference(AdditionalDamageHalfOrcSavageAttacks.impactParticleReference)
            .AddCustomSubFeatures(rogueHolder)
            .AddToDB();

        // add the additional chain of execution dice based off sneak attack ones
        additionalDamageChainOfExecution.DiceByRankTable.ForEach(x =>
        {
            switch (x.Rank)
            {
                case >= 17:
                    x.diceNumber += 5;
                    break;
                case >= 13:
                    x.diceNumber += 4;
                    break;
                case >= 9:
                    x.diceNumber += 3;
                    break;
            }
        });

        var additionalDamageChainOfExecutionSneakAttack = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{ChainOfExecution}SneakAttack")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionChainOfExecutionDetrimental
                })
            .AddCustomSubFeatures(rogueHolder)
            .AddToDB();

        var featureChainOfExecution = FeatureDefinitionBuilder
            .Create($"Feature{Name}{ChainOfExecution}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureChainOfExecution.AddCustomSubFeatures(
            new CustomBehaviorChainOfExecution(conditionChainOfExecutionBeneficial),
            new CustomAdditionalDamageSneakAttack(additionalDamageChainOfExecutionSneakAttack),
            new CustomAdditionalDamageChainOfExecution(
                additionalDamageChainOfExecution,
                featureChainOfExecution,
                conditionChainOfExecutionBeneficial));

        //
        // Cloak of Shadows
        //

        var powerCloakOfShadows = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{CloakOfShadows}")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.Invisibility)
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.ShortRest, AttributeDefinitions.Dexterity)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(SpellDefinitions.Invisibility.EffectDescription)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetParticleEffectParameters(PowerSorakAssassinShadowMurder)
                    .Build())
            .AddToDB();

        //
        // Fatal Strike
        //

        var featureFatalStrike = FeatureDefinitionBuilder
            .Create($"Feature{Name}FatalStrike")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureFatalStrike.AddCustomSubFeatures(new PhysicalAttackInitiatedByMeFatalStrike(featureFatalStrike));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishSlayer, 256))
            .AddFeaturesAtLevel(3, featureElimination)
            .AddFeaturesAtLevel(9, featureChainOfExecution)
            .AddFeaturesAtLevel(13, powerCloakOfShadows)
            .AddFeaturesAtLevel(17, featureFatalStrike)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Elimination
    //

    private sealed class CustomBehaviorElimination(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinition)
        : IModifyAttackActionModifier, IModifyAttackCriticalThreshold
    {
        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            //
            // Allow advantage if first round and higher initiative order vs defender
            //

            var battle = Gui.Battle;

            // always grant advantage on battle round zero
            if (battle == null)
            {
                attackModifier.AttackAdvantageTrends.Add(
                    new TrendInfo(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition));

                return;
            }

            if (battle.CurrentRound > 1)
            {
                return;
            }

            // battle round one from here
            var gameLocationAttacker = GameLocationCharacter.GetFromActor(myself);
            var gameLocationDefender = GameLocationCharacter.GetFromActor(defender);

            if (gameLocationAttacker == null || gameLocationDefender == null)
            {
                return;
            }

            var attackerAttackOrder = battle.initiativeSortedContenders.IndexOf(gameLocationAttacker);
            var defenderAttackOrder = battle.initiativeSortedContenders.IndexOf(gameLocationDefender);

            if (defenderAttackOrder >= 0 && attackerAttackOrder > defenderAttackOrder)
            {
                return;
            }

            attackModifier.AttackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, featureDefinition.Name, featureDefinition));
        }

        public int GetCriticalThreshold(
            int current, RulesetCharacter me, RulesetCharacter target, BaseDefinition attackMethod)
        {
            return target.HasConditionOfType(ConditionSurprised) ? 1 : current;
        }
    }

    //
    // Chain of Execution
    //

    private sealed class CustomAdditionalDamageChainOfExecution(
        IAdditionalDamageProvider provider,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureDefinitionTrigger,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionDefinition)
        : CustomAdditionalDamage(provider)
    {
        private readonly IAdditionalDamageProvider _featureDefinitionAdditionalDamage = provider;

        internal override bool IsValid(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            var rulesetAttacker = attacker.RulesetCharacter;
            var isConsciousCharacterOfSideNextToCharacter = battleManager.IsConsciousCharacterOfSideNextToCharacter(
                defender, attacker.Side, attacker);

            if ((attackMode == null && (rulesetEffect == null || _featureDefinitionAdditionalDamage.RequiredProperty !=
                    RestrictedContextRequiredProperty.SpellWithAttackRoll)) ||
                (advantageType != AdvantageType.Advantage && (advantageType == AdvantageType.Disadvantage ||
                                                              !isConsciousCharacterOfSideNextToCharacter)))
            {
                return false;
            }

            if (!rulesetAttacker.HasAnyConditionOfType($"Condition{Name}{ChainOfExecution}Beneficial"))
            {
                return false;
            }

            rulesetAttacker.LogCharacterUsedFeature(featureDefinitionTrigger);

            if (rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionDefinition.Name, out var activeCondition))
            {
                rulesetAttacker.RemoveCondition(activeCondition);
            }

            return true;
        }
    }

    private sealed class CustomAdditionalDamageSneakAttack(IAdditionalDamageProvider provider)
        : CustomAdditionalDamage(provider)
    {
        private readonly IAdditionalDamageProvider _featureDefinitionAdditionalDamage = provider;

        internal override bool IsValid(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget,
            out CharacterActionParams reactionParams)
        {
            reactionParams = null;

            var rulesetAttacker = attacker.RulesetCharacter;

            return (attackMode != null || (rulesetEffect != null &&
                                           _featureDefinitionAdditionalDamage.RequiredProperty ==
                                           RestrictedContextRequiredProperty.SpellWithAttackRoll)) &&
                   (advantageType == AdvantageType.Advantage || (advantageType != AdvantageType.Disadvantage &&
                                                                 battleManager
                                                                     .IsConsciousCharacterOfSideNextToCharacter(
                                                                         defender, attacker.Side, attacker))) &&
                   !rulesetAttacker.HasAnyConditionOfType($"Condition{Name}{ChainOfExecution}Beneficial");
        }
    }

    private sealed class OnConditionAddedOrRemovedChainOfExecution(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionChainOfExecutionBeneficial,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionChainOfExecutionDetrimental)
        : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // SHOULD ONLY TRIGGER ON DEATH
            if (target is not { IsDeadOrDyingOrUnconscious: true })
            {
                return;
            }

            if (rulesetCondition.ConditionDefinition != conditionChainOfExecutionDetrimental ||
                !RulesetEntity.TryGetEntity<RulesetCharacter>(rulesetCondition.sourceGuid, out var rulesetCharacter))
            {
                return;
            }

            ApplyConditionChainOfExecutionGranted(rulesetCharacter);
        }

        private void ApplyConditionChainOfExecutionGranted(RulesetCharacter rulesetCharacter)
        {
            if (rulesetCharacter.HasConditionOfType(conditionChainOfExecutionBeneficial.Name))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                conditionChainOfExecutionBeneficial.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionChainOfExecutionBeneficial.Name,
                0,
                0,
                0);
        }
    }

    private sealed class CustomBehaviorChainOfExecution(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionChainOfExecutionBeneficial)
        : IOnReducedToZeroHpByMe, ICustomLevelUpLogic
    {
        // remove original sneak attack as we've added a conditional one
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            foreach (var featureDefinitions in hero.ActiveFeatures.Values)
            {
                featureDefinitions.RemoveAll(x => x == AdditionalDamageRogueSneakAttack);
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }

        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            ApplyConditionChainOfExecutionGranted(attacker.RulesetCharacter);

            yield break;
        }

        private void ApplyConditionChainOfExecutionGranted(RulesetCharacter rulesetCharacter)
        {
            if (rulesetCharacter.HasConditionOfType(conditionChainOfExecutionBeneficial.Name))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                conditionChainOfExecutionBeneficial.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                conditionChainOfExecutionBeneficial.Name,
                0,
                0,
                0);
        }
    }

    private sealed class RogueHolder : IClassHoldingFeature
    {
        // allows Chain of Execution damage to scale with rogue level
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;
    }

    //
    // Fatal Strike
    //

    private sealed class PhysicalAttackInitiatedByMeFatalStrike(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureFatalStrike)
        : IPhysicalAttackInitiatedByMe
    {
        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (__instance is not { IsBattleInProgress: true } ||
                rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetDefender.HasAnyConditionOfType(ConditionSurprised))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var modifierTrend = rulesetDefender.actionModifier.savingThrowModifierTrends;
            var advantageTrends = rulesetDefender.actionModifier.savingThrowAdvantageTrends;
            var attackerDexterityModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Dexterity));
            var attackerProficiencyBonus =
                rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var defenderConstitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(
                rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution));

            rulesetDefender.RollSavingThrow(0, AttributeDefinitions.Constitution, featureFatalStrike, modifierTrend,
                advantageTrends, defenderConstitutionModifier, 8 + attackerProficiencyBonus + attackerDexterityModifier,
                false,
                out var savingOutcome,
                out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            attackModifier.attackerDamageMultiplier *= 2;
        }
    }
}
