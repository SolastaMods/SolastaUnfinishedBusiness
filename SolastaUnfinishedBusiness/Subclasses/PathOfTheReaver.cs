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

// ReSharper disable once IdentifierTypo
internal sealed class PathOfTheReaver : AbstractSubclass
{
    private const string Name = "PathOfTheReaver";

    internal PathOfTheReaver()
    {
        // LEVEL 03

        var additionalDamageVoraciousFury = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageConditionRaging, $"AdditionalDamage{Name}VoraciousFury")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("VoraciousFury")
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 2, 1, 4)
            .SetCustomSubFeatures(new RestrictedContextValidator((_, _, character, _, _, _, _) =>
                (OperationType.Set, ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)(character))))
            .AddToDB();

        // LEVEL 06

        var featureSetProfaneVitality = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ProfaneVitality")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityPoisonResistance)
            .AddToDB();

        // LEVEL 10

        var reactionBloodbath = ReactionDefinitionBuilder
            .Create("ReactionBloodbath")
            .SetGuiPresentation(Category.Reaction)
            .SetReact("ReactionBloodbath")
            .AddToDB();

        var powerBloodbath = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Bloodbath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerBloodbath.SetCustomSubFeatures(new TargetReducedToZeroHpBloodbath(powerBloodbath, reactionBloodbath));

        // LEVEL 14

        var featureCorruptedBlood = FeatureDefinitionBuilder
            .Create($"Feature{Name}CorruptedBlood")
            .SetGuiPresentationNoContent(true)
            .SetCustomSubFeatures()
            .AddToDB();

        var conditionCorruptedBlood = ConditionDefinitionBuilder
            .Create($"Condition{Name}CorruptedBlood")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(featureCorruptedBlood)
            .AddToDB();

        var powerCorruptedBlood = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CorruptedBlood")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Permanent)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Permanent)
                .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cube, 3)
                .SetRecurrentEffect(
                    RecurrentEffect.OnActivation | RecurrentEffect.OnEnter | RecurrentEffect.OnTurnStart)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionCorruptedBlood, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        featureCorruptedBlood.SetCustomSubFeatures(
            new AfterAttackEffectCorruptedBlood(conditionCorruptedBlood, powerCorruptedBlood));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheReaver, 256))
            .AddFeaturesAtLevel(3, additionalDamageVoraciousFury)
            .AddFeaturesAtLevel(6, featureSetProfaneVitality)
            .AddFeaturesAtLevel(10, powerBloodbath)
            .AddFeaturesAtLevel(14, powerCorruptedBlood)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private class AfterAttackEffectCorruptedBlood : IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public AfterAttackEffectCorruptedBlood(
            ConditionDefinition conditionDefinition,
            FeatureDefinitionPower featureDefinitionPower)
        {
            _conditionDefinition = conditionDefinition;
            _featureDefinitionPower = featureDefinitionPower;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome is not RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                return;
            }

            var condition = attacker.RulesetCharacter.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition && x.SourceGuid == defender.Guid);

            if (condition == null)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;
            var constitution = rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var totalDamage = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeNecrotic, DieType = DieType.D1, DiceNumber = 0, BonusDamage = totalDamage
            };

            GameConsoleHelper.LogCharacterUsedPower(rulesetDefender, _featureDefinitionPower);

            RulesetActor.InflictDamage(
                totalDamage,
                damageForm,
                DamageTypeNecrotic,
                new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetAttacker },
                rulesetAttacker,
                false,
                attacker.Guid,
                false,
                attackMode.AttackTags,
                new RollInfo(DieType.D1, new List<int>(), totalDamage),
                true,
                out _);
        }
    }

    private class TargetReducedToZeroHpBloodbath : ITargetReducedToZeroHp
    {
        private readonly FeatureDefinitionPower _featureDefinitionPower;
        private readonly ReactionDefinition _reactionDefinition;

        public TargetReducedToZeroHpBloodbath(
            FeatureDefinitionPower featureDefinitionPower,
            ReactionDefinition reactionDefinition)
        {
            _featureDefinitionPower = featureDefinitionPower;
            _reactionDefinition = reactionDefinition;
        }

        public IEnumerator HandleCharacterReducedToZeroHp(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService == null)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.MissingHitPoints == 0 || !rulesetAttacker.HasConditionOfType(ConditionRaging))
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(rulesetAttacker, attackMode))
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree);
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequest(_reactionDefinition.Name, reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var characterLevel = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var constitution = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var constitutionModifier = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
            var totalHealing = constitutionModifier * 2 + characterLevel;

            GameConsoleHelper.LogCharacterUsedPower(rulesetAttacker, _featureDefinitionPower);
            rulesetAttacker.ReceiveHealing(totalHealing, true, rulesetAttacker.Guid);
        }
    }
}
