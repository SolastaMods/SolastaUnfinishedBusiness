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
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

// ReSharper disable once IdentifierTypo
internal sealed class PathOfTheReaver : AbstractSubclass
{
    private const string Name = "PathOfTheReaver";

    internal PathOfTheReaver()
    {
        // LEVEL 03

        var featureVoraciousFury = FeatureDefinitionBuilder
            .Create($"Feature{Name}VoraciousFury")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new AttackEffectAfterDamageVoraciousFury(),
                new CustomAdditionalDamageVoraciousFury(
                    FeatureDefinitionAdditionalDamageBuilder
                        .Create($"AdditionalDamage{Name}VoraciousFury")
                        .SetGuiPresentation($"Feature{Name}VoraciousFury", Category.Feature)
                        .SetNotificationTag("VoraciousFury")
                        .SetDamageDice(DieType.D1, 2)
                        .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 2, 1, 4)
                        .SetSpecificDamageType(DamageTypeNecrotic)
                        .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
                        .SetImpactParticleReference(ConditionDefinitions
                            .ConditionTraditionSurvivalUnbreakableBody.conditionStartParticleReference)
                        .SetCustomSubFeatures(new BarbarianHolder())
                        .AddToDB()))
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

        var powerBloodbath = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Bloodbath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.ShortRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerBloodbath.SetCustomSubFeatures(new TargetReducedToZeroHpBloodbath(powerBloodbath));

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
            new AttackEffectAfterDamageCorruptedBlood(conditionCorruptedBlood, powerCorruptedBlood));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheReaver, 256))
            .AddFeaturesAtLevel(3, featureVoraciousFury)
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

    private static bool IsVoraciousFuryValidContext(RulesetCharacter rulesetCharacter, RulesetAttackMode attackMode)
    {
        var isValid = (ValidatorsWeapon.IsMelee(attackMode) ||
                       ValidatorsWeapon.IsUnarmed(rulesetCharacter, attackMode)) &&
                      ValidatorsCharacter.DoesNotHaveHeavyArmor(rulesetCharacter) &&
                      ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)(rulesetCharacter);

        return isValid;
    }

    //
    // Corrupted Blood
    //

    private class AttackEffectAfterDamageCorruptedBlood : IAttackEffectAfterDamage
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public AttackEffectAfterDamageCorruptedBlood(
            ConditionDefinition conditionDefinition,
            FeatureDefinitionPower featureDefinitionPower)
        {
            _conditionDefinition = conditionDefinition;
            _featureDefinitionPower = featureDefinitionPower;
        }

        public void OnAttackEffectAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetAttacker == null ||
                rulesetDefender == null ||
                rulesetDefender.IsDeadOrDying)
            {
                return;
            }

            var condition = rulesetAttacker.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition == _conditionDefinition && x.SourceGuid == defender.Guid);

            if (condition == null)
            {
                return;
            }

            var constitution = rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var totalDamage = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
            var damageForm = new DamageForm
            {
                DamageType = DamageTypeNecrotic, DieType = DieType.D1, DiceNumber = 0, BonusDamage = totalDamage
            };

            EffectHelpers.StartVisualEffect(attacker, defender, SpellDefinitions.VampiricTouch,
                EffectHelpers.EffectType.Effect);
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

    //
    // Bloodbath
    //

    private class TargetReducedToZeroHpBloodbath : ITargetReducedToZeroHp
    {
        private readonly FeatureDefinitionPower _featureDefinitionPower;

        public TargetReducedToZeroHpBloodbath(FeatureDefinitionPower featureDefinitionPower)
        {
            _featureDefinitionPower = featureDefinitionPower;
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

            if (rulesetAttacker.GetRemainingPowerCharges(_featureDefinitionPower) <= 0)
            {
                yield break;
            }

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var totalHealing = 2 * classLevel;
            var reactionParams =
                new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter =
                        Gui.Format("Reaction/&CustomReactionBloodbathDescription", totalHealing.ToString())
                };
            var previousReactionCount = gameLocationActionService.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("Bloodbath", reactionParams);

            gameLocationActionService.AddInterruptRequest(reactionRequest);

            yield return gameLocationBattleService.WaitForReactions(
                attacker, gameLocationActionService, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UpdateUsageForPower(_featureDefinitionPower, _featureDefinitionPower.CostPerUse);
            GameConsoleHelper.LogCharacterUsedPower(rulesetAttacker, _featureDefinitionPower);
            EffectHelpers.StartVisualEffect(attacker, attacker, SpellDefinitions.Heal, EffectHelpers.EffectType.Effect);
            rulesetAttacker.ReceiveHealing(totalHealing, true, rulesetAttacker.Guid);
        }
    }

    //
    // Voracious Fury
    //

    private sealed class CustomAdditionalDamageVoraciousFury : CustomAdditionalDamage
    {
        public CustomAdditionalDamageVoraciousFury(IAdditionalDamageProvider provider) : base(provider)
        {
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

            return IsVoraciousFuryValidContext(attacker.RulesetCharacter, attackMode);
        }
    }

    private sealed class AttackEffectAfterDamageVoraciousFury : IAttackEffectAfterDamage
    {
        private const string SpecialFeatureName = $"AdditionalHealing{Name}VoraciousFury";

        public void OnAttackEffectAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                return;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!IsVoraciousFuryValidContext(rulesetAttacker, attackMode) ||
                attacker.UsedSpecialFeatures.ContainsKey(SpecialFeatureName))
            {
                return;
            }

            attacker.UsedSpecialFeatures.TryAdd(SpecialFeatureName, 1);

            var proficiencyBonus = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var multiplier = outcome is RollOutcome.Success ? 1 : 2;

            rulesetAttacker.ReceiveHealing(multiplier * proficiencyBonus, true, rulesetAttacker.Guid);
        }
    }

    private sealed class BarbarianHolder : IClassHoldingFeature
    {
        // allows Illuminating Strike damage to scale with barbarian level
        public CharacterClassDefinition Class => CharacterClassDefinitions.Barbarian;
    }
}
