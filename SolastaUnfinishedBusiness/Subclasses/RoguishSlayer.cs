using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishSlayer : AbstractSubclass
{
    private const string Name = "RoguishSlayer";
    private const string Elimination = "Elimination";
    private const string ChainOfExecution = "ChainOfExecution";
    private const string CloakOfShadows = "CloakOfShadows";

    internal RoguishSlayer()
    {
        //
        // Elimination
        //

        var attributeModifierElimination = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{Elimination}")
            .SetGuiPresentationNoContent(true)
            .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.ForceAnyway,
                AttributeDefinitions.CriticalThreshold, 1)
            .AddToDB();

        var conditionElimination = ConditionDefinitionBuilder
            .Create($"Condition{Name}{Elimination}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetFeatures(attributeModifierElimination)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var featureElimination = FeatureDefinitionBuilder
            .Create($"Feature{Name}{Elimination}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureElimination.SetCustomSubFeatures(
            new CustomBehaviorElimination(featureElimination, conditionElimination));

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

        var customBehaviorChainOfExecution = new CustomBehaviorChainOfExecution(
            conditionChainOfExecutionBeneficial,
            conditionChainOfExecutionDetrimental);

        conditionChainOfExecutionDetrimental.SetCustomSubFeatures(customBehaviorChainOfExecution);

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
            .SetCustomSubFeatures(rogueHolder)
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
            .SetCustomSubFeatures(rogueHolder)
            .AddToDB();

        var featureChainOfExecution = FeatureDefinitionBuilder
            .Create($"Feature{Name}{ChainOfExecution}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureChainOfExecution.SetCustomSubFeatures(
            customBehaviorChainOfExecution,
            new CustomAdditionalDamageSneakAttack(
                additionalDamageChainOfExecutionSneakAttack),
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
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.ShortRest)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create(SpellDefinitions.Invisibility.EffectDescription)
                .SetDurationData(DurationType.Minute, 2)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .Build())
            .AddToDB();

        //
        // Fatal Strike
        //

        var featureFatalStrike = FeatureDefinitionBuilder
            .Create($"Feature{Name}FatalStrike")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new AfterDamageFatalStrike())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass,
                Sprites.GetSprite("RoguishSlayer", Resources.RoguishSlayer, 256))
            .AddFeaturesAtLevel(3, featureElimination)
            .AddFeaturesAtLevel(9, featureChainOfExecution)
            .AddFeaturesAtLevel(13, powerCloakOfShadows)
            .AddFeaturesAtLevel(17, featureFatalStrike)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Elimination
    //

    private sealed class CustomBehaviorElimination : IPhysicalAttackInitiated, IAttackComputeModifier
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinition _featureDefinition;

        public CustomBehaviorElimination(FeatureDefinition featureDefinition, ConditionDefinition conditionDefinition)
        {
            _featureDefinition = featureDefinition;
            _conditionDefinition = conditionDefinition;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
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
                    new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));

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
                new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
        }

        public IEnumerator OnAttackInitiated(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            //
            // allow critical hit if defender is surprised
            //

            var rulesetDefender = defender.RulesetCharacter;
            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetDefender == null || rulesetAttacker == null)
            {
                yield break;
            }

            if (rulesetDefender.HasAnyConditionOfType(ConditionSurprised))
            {
                rulesetAttacker.InflictCondition(
                    _conditionDefinition.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagCombat,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    null,
                    0,
                    0,
                    0);
            }
        }
    }

    //
    // Chain of Execution
    //

    private sealed class CustomAdditionalDamageChainOfExecution : CustomAdditionalDamage
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly IAdditionalDamageProvider _featureDefinitionAdditionalDamage;
        private readonly FeatureDefinition _featureDefinitionTrigger;

        public CustomAdditionalDamageChainOfExecution(
            IAdditionalDamageProvider provider,
            FeatureDefinition featureDefinitionTrigger,
            ConditionDefinition conditionDefinition) : base(provider)
        {
            _featureDefinitionAdditionalDamage = provider;
            _featureDefinitionTrigger = featureDefinitionTrigger;
            _conditionDefinition = conditionDefinition;
        }

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

            GameConsoleHelper.LogCharacterUsedFeature(rulesetAttacker, _featureDefinitionTrigger);

            var rulesetCondition =
                rulesetAttacker.AllConditions.FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition);

            if (rulesetCondition != null)
            {
                rulesetAttacker.RemoveConditionOfCategory(
                    AttributeDefinitions.TagCombat, rulesetCondition, true, true, true);
            }

            return true;
        }
    }

    private sealed class CustomAdditionalDamageSneakAttack : CustomAdditionalDamage
    {
        private readonly IAdditionalDamageProvider _featureDefinitionAdditionalDamage;

        public CustomAdditionalDamageSneakAttack(IAdditionalDamageProvider provider) : base(provider)
        {
            _featureDefinitionAdditionalDamage = provider;
        }

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

    private sealed class CustomBehaviorChainOfExecution :
        INotifyConditionRemoval, ITargetReducedToZeroHp, IFeatureDefinitionCustomCode
    {
        private readonly ConditionDefinition _conditionChainOfExecutionBeneficial;
        private readonly ConditionDefinition _conditionChainOfExecutionDetrimental;

        public CustomBehaviorChainOfExecution(
            ConditionDefinition conditionChainOfExecutionBeneficial,
            ConditionDefinition conditionChainOfExecutionDetrimental)
        {
            _conditionChainOfExecutionBeneficial = conditionChainOfExecutionBeneficial;
            _conditionChainOfExecutionDetrimental = conditionChainOfExecutionDetrimental;
        }

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

        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            // Empty
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.ConditionDefinition != _conditionChainOfExecutionDetrimental ||
                !RulesetEntity.TryGetEntity<RulesetCharacter>(rulesetCondition.sourceGuid, out var rulesetCharacter))
            {
                return;
            }

            ApplyConditionChainOfExecutionGranted(rulesetCharacter);
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
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
            if (rulesetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagCombat, _conditionChainOfExecutionBeneficial.Name))
            {
                return;
            }

            rulesetCharacter.InflictCondition(
                _conditionChainOfExecutionBeneficial.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                null,
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

    private sealed class AfterDamageFatalStrike : IAttackEffectBeforeDamage
    {
        public void OnAttackEffectBeforeDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var battle = Gui.Battle;
            var rulesetDefender = defender.RulesetCharacter;

            if (battle == null ||
                rulesetDefender == null ||
                rulesetDefender.IsDeadOrDying ||
                !rulesetDefender.HasAnyConditionOfType(ConditionSurprised))
            {
                return;
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

            rulesetDefender.RollSavingThrow(0, AttributeDefinitions.Constitution, null, modifierTrend,
                advantageTrends, defenderConstitutionModifier, 8 + attackerProficiencyBonus + attackerDexterityModifier,
                false,
                out var savingOutcome,
                out _);

            if (savingOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                return;
            }

            attackModifier.attackerDamageMultiplier *= 2;
        }
    }
}
