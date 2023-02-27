using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class RoguishSlayer : AbstractSubclass
{
    private const string Name = "RoguishSlayer";
    private const string ChainOfExecution = "ChainOfExecution";
    private const string CloakOfShadows = "CloakOfShadows";
    private const string Elimination = "Elimination";

    internal RoguishSlayer()
    {
        //
        // Elimination
        //
        var featureElimination = FeatureDefinitionBuilder
            .Create($"Feature{Name}{Elimination}")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureElimination.SetCustomSubFeatures(new CustomBehaviorElimination(featureElimination));

        //
        // Chain of Execution
        //

        var additionalDamageChainOfExecutionGranted = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{ChainOfExecution}Granted")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(ChainOfExecution)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 4)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFirstTargetOnly(true)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetCustomSubFeatures(new RogueHolder())
            .AddToDB();

        var conditionChainOfExecutionGranted = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ChainOfExecution}Granted")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBleeding)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Minute, 1)
            .SetFeatures(additionalDamageChainOfExecutionGranted)
            .AddToDB();

        var conditionChainOfExecution = ConditionDefinitionBuilder
            .Create($"Condition{Name}{ChainOfExecution}")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBleeding)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var customSubFeature =
            new CustomBehaviorChainOfExecution(conditionChainOfExecution, conditionChainOfExecutionGranted);

        conditionChainOfExecution.SetCustomSubFeatures(customSubFeature);

        var additionalDamageChainOfExecution = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{ChainOfExecution}")
            .SetGuiPresentation(Category.Feature)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.None)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFirstTargetOnly(true)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    hasSavingThrow = false,
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionDefinition = conditionChainOfExecution
                })
            .SetCustomSubFeatures(customSubFeature)
            .AddToDB();

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

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, RangerShadowTamer)
            .AddFeaturesAtLevel(3, featureElimination)
            .AddFeaturesAtLevel(9, additionalDamageChainOfExecution)
            .AddFeaturesAtLevel(13, powerCloakOfShadows)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    internal override DeityDefinition DeityDefinition { get; }

    private sealed class CustomBehaviorElimination : IOnComputeAttackModifier, IChangeDieRoll
    {
        private readonly FeatureDefinition _featureDefinition;

        public CustomBehaviorElimination(FeatureDefinition featureDefinition)
        {
            _featureDefinition = featureDefinition;
        }

        public void ChangeDieRoll(
            DieType dieType,
            AdvantageType advantageType,
            ref int firstRoll,
            ref int secondRoll,
            float rollAlterationScore,
            RulesetActor actor,
            RollContext rollContext,
            ref int result)
        {
            if (rollContext != RollContext.AttackRoll)
            {
                return;
            }

            var defender = Gui.Battle?.DefenderContender?.RulesetCharacter;

            if (defender == null)
            {
                return;
            }

            if (defender.AllConditions.All(x => x.ConditionDefinition != ConditionDefinitions.ConditionSurprised))
            {
                return;
            }

            result += 20 - firstRoll;
            firstRoll = 20;
        }

        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            var battle = Gui.Battle;

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

            var gameLocationAttacker = GameLocationCharacter.GetFromActor(myself);
            var gameLocationDefender = GameLocationCharacter.GetFromActor(defender);
            var attackerAttackOrder = battle.initiativeSortedContenders.IndexOf(gameLocationAttacker);
            var defenderAttackOrder = battle.initiativeSortedContenders.IndexOf(gameLocationDefender);

            if (defenderAttackOrder >= 0 && attackerAttackOrder > defenderAttackOrder)
            {
                return;
            }

            attackModifier.AttackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.CharacterFeature, _featureDefinition.Name, _featureDefinition));
        }
    }

    private sealed class CustomBehaviorChainOfExecution : INotifyConditionRemoval, ITargetReducedToZeroHp
    {
        private readonly ConditionDefinition _conditionChainOfExecution;
        private readonly ConditionDefinition _conditionChainOfExecutionGranted;

        public CustomBehaviorChainOfExecution(
            ConditionDefinition conditionChainOfExecution,
            ConditionDefinition conditionChainOfExecutionGranted)
        {
            _conditionChainOfExecution = conditionChainOfExecution;
            _conditionChainOfExecutionGranted = conditionChainOfExecutionGranted;
        }

        public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
        {
            // Empty
        }

        public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
        {
            if (rulesetCondition.ConditionDefinition != _conditionChainOfExecution ||
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
            if (rulesetCharacter.HasConditionOfCategoryAndType(AttributeDefinitions.TagCombat,
                    _conditionChainOfExecutionGranted.Name))
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                rulesetCharacter.Guid,
                _conditionChainOfExecutionGranted,
                DurationType.Round,
                2,
                TurnOccurenceType.EndOfTurn,
                rulesetCharacter.Guid,
                rulesetCharacter.CurrentFaction.Name);

            rulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }
    
    private sealed class RogueHolder : IClassHoldingFeature
    {
        // allows Chain of Execution damage to scale with rogue level
        public CharacterClassDefinition Class => CharacterClassDefinitions.Rogue;
    }
}
