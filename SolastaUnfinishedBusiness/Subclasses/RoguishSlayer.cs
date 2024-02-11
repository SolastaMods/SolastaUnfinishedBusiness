using System.Collections;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishSlayer : AbstractSubclass
{
    internal const string Name = "RoguishSlayer";
    internal const string ConditionChainOfExecutionBeneficialName = $"Condition{Name}{ChainOfExecution}Beneficial";
    internal const int ChainOfExecutionLevel = 9;

    private const string Elimination = "Elimination";
    private const string ChainOfExecution = "ChainOfExecution";
    private const string CloakOfShadows = "CloakOfShadows";
    private const string ConditionChainOfExecutionDetrimentalName = $"Condition{Name}{ChainOfExecution}Detrimental";

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

        _ = ConditionDefinitionBuilder
            .Create(ConditionChainOfExecutionBeneficialName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBullsStrength)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1)
            .AddToDB();

        _ = ConditionDefinitionBuilder
            .Create(ConditionChainOfExecutionDetrimentalName)
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBleeding)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddCustomSubFeatures(new OnConditionAddedOrRemovedChainOfExecution())
            .AddToDB();

        var featureChainOfExecution = FeatureDefinitionBuilder
            .Create($"Feature{Name}{ChainOfExecution}")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new OnReducedToZeroHpByMeChainOfExecution())
            .AddToDB();

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

    internal static void InflictConditionChainOfExecution(
        RulesetCharacter rulesetAttacker,
        RulesetCharacter rulesetDefender)
    {
        var isBeneficial = rulesetAttacker == rulesetDefender;

        var conditionName = isBeneficial
            ? ConditionChainOfExecutionBeneficialName
            : ConditionChainOfExecutionDetrimentalName;

        var rulesetCharacter = isBeneficial
            ? rulesetAttacker
            : rulesetDefender;

        if (isBeneficial && rulesetCharacter.HasConditionOfType(conditionName))
        {
            return;
        }

        rulesetCharacter.InflictCondition(
            conditionName,
            DurationType.Round,
            1,
            TurnOccurenceType.EndOfTurn,
            AttributeDefinitions.TagEffect,
            rulesetAttacker.guid,
            rulesetAttacker.CurrentFaction.Name,
            1,
            conditionName,
            0,
            0,
            0);
    }

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

    private sealed class OnConditionAddedOrRemovedChainOfExecution : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            if (target is not { IsDeadOrDyingOrUnconscious: true } ||
                !RulesetEntity.TryGetEntity<RulesetCharacter>(rulesetCondition.sourceGuid, out var rulesetCharacter))
            {
                return;
            }

            InflictConditionChainOfExecution(rulesetCharacter, rulesetCharacter);
        }
    }

    private sealed class OnReducedToZeroHpByMeChainOfExecution : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            InflictConditionChainOfExecution(rulesetCharacter, rulesetCharacter);

            yield break;
        }
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
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            var rulesetDefender = defender.RulesetCharacter;

            if (battleManager is not { IsBattleInProgress: true } ||
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
