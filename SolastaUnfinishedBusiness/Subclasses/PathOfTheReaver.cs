using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
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
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

// ReSharper disable once IdentifierTypo
[UsedImplicitly]
public sealed class PathOfTheReaver : AbstractSubclass
{
    private const string Name = "PathOfTheReaver";

    public PathOfTheReaver()
    {
        // LEVEL 03

        var featureVoraciousFury = FeatureDefinitionBuilder
            .Create($"Feature{Name}VoraciousFury")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // kept name for backward compatibility
        var attributeModifierDraconicResilience = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ProfaneVitality")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddCustomSubFeatures(new CustomLevelUpLogicDraconicResilience())
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

        // LEVEL 14

        var featureCorruptedBlood = FeatureDefinitionBuilder
            .Create($"Feature{Name}CorruptedBlood")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // CONNECT ALL THEM TOGETHER NOW

        featureVoraciousFury.AddCustomSubFeatures(
            new PhysicalAttackFinishedByMeVoraciousFury(featureVoraciousFury, powerBloodbath));
        powerBloodbath.AddCustomSubFeatures(
            new OnReducedToZeroHpByMeBloodbath(powerBloodbath));
        featureCorruptedBlood.AddCustomSubFeatures(
            new PhysicalAttackFinishedOnMeCorruptedBlood(featureCorruptedBlood, powerBloodbath));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheReaver, 256))
            .AddFeaturesAtLevel(3, featureVoraciousFury, attributeModifierDraconicResilience)
            .AddFeaturesAtLevel(6, featureSetProfaneVitality)
            .AddFeaturesAtLevel(10, powerBloodbath)
            .AddFeaturesAtLevel(14, featureCorruptedBlood)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Common Helpers
    //

    private static void InflictDamage(
        RulesetEntity rulesetAttacker,
        RulesetActor rulesetDefender,
        int totalDamage,
        List<string> attackTags)
    {
        var damageForm = new DamageForm
        {
            DamageType = DamageTypeNecrotic, DieType = DieType.D1, DiceNumber = 0, BonusDamage = totalDamage
        };

        RulesetActor.InflictDamage(
            totalDamage,
            damageForm,
            DamageTypeNecrotic,
            new RulesetImplementationDefinitions.ApplyFormsParams { targetCharacter = rulesetDefender },
            rulesetDefender,
            false,
            rulesetAttacker.Guid,
            false,
            attackTags,
            new RollInfo(DieType.D1, [], totalDamage),
            false,
            out _);
    }

    private static void ReceiveHealing(GameLocationCharacter gameLocationCharacter, int totalHealing)
    {
        EffectHelpers.StartVisualEffect(
            gameLocationCharacter, gameLocationCharacter, Heal, EffectHelpers.EffectType.Effect);
        gameLocationCharacter.RulesetCharacter.ReceiveHealing(totalHealing, true, gameLocationCharacter.Guid);
    }

    private static IEnumerator HandleEnemyDeath(
        GameLocationCharacter attacker,
        RulesetAttackMode attackMode,
        FeatureDefinitionPower featureDefinitionPower)
    {
        var gameLocationActionService =
            ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
        var gameLocationBattleService =
            ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
        {
            yield break;
        }

        var rulesetAttacker = attacker.RulesetCharacter;

        if (rulesetAttacker.MissingHitPoints == 0 || !rulesetAttacker.HasConditionOfType(ConditionRaging))
        {
            yield break;
        }

        if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode))
        {
            yield break;
        }

        if (rulesetAttacker.GetRemainingPowerCharges(featureDefinitionPower) <= 0)
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

        rulesetAttacker.UpdateUsageForPower(featureDefinitionPower, featureDefinitionPower.CostPerUse);
        rulesetAttacker.LogCharacterUsedPower(featureDefinitionPower);
        ReceiveHealing(attacker, totalHealing);
    }

    //
    // Draconic Resilience
    //

    private sealed class CustomLevelUpLogicDraconicResilience : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            if (hero.TryGetAttribute(AttributeDefinitions.HitPoints, out var attribute))
            {
                attribute.maxValue += 3;
            }
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    //
    // Voracious Fury
    //

    private sealed class PhysicalAttackFinishedByMeVoraciousFury(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureVoraciousFury,
        FeatureDefinitionPower powerBloodBath)
        : IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome outcome,
            int damageAmount)
        {
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!IsVoraciousFuryValidContext(rulesetAttacker, attackMode))
            {
                yield break;
            }

            if (!attacker.OnceInMyTurnIsValid(featureVoraciousFury.Name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(featureVoraciousFury.Name, 1);

            var multiplier = 1;

            if (outcome is RollOutcome.CriticalSuccess)
            {
                multiplier += 1;
            }

            if (rulesetAttacker.MissingHitPoints > rulesetAttacker.CurrentHitPoints)
            {
                multiplier += 1;
            }

            var proficiencyBonus = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var totalDamageOrHealing = proficiencyBonus * multiplier;

            ReceiveHealing(attacker, totalDamageOrHealing);

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            rulesetAttacker.LogCharacterUsedFeature(featureVoraciousFury);
            EffectHelpers.StartVisualEffect(attacker, defender, VampiricTouch, EffectHelpers.EffectType.Effect);
            InflictDamage(rulesetAttacker, rulesetDefender, totalDamageOrHealing, attackMode.AttackTags);

            if (rulesetDefender.IsDeadOrDying)
            {
                yield return HandleEnemyDeath(attacker, attackMode, powerBloodBath);
            }
        }

        private static bool IsVoraciousFuryValidContext(RulesetCharacter rulesetCharacter, RulesetAttackMode attackMode)
        {
            var isValid =
                attackMode?.thrown == false &&
                (ValidatorsWeapon.IsMelee(attackMode) || ValidatorsWeapon.IsUnarmed(attackMode)) &&
                ValidatorsCharacter.DoesNotHaveHeavyArmor(rulesetCharacter) &&
                ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)(rulesetCharacter);

            return isValid;
        }
    }

    //
    // Bloodbath
    //

    private class OnReducedToZeroHpByMeBloodbath(FeatureDefinitionPower powerBloodBath) : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            yield return HandleEnemyDeath(attacker, attackMode, powerBloodBath);
        }
    }

    //
    // Corrupted Blood
    //

    private class PhysicalAttackFinishedOnMeCorruptedBlood(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureCorruptedBlood,
        FeatureDefinitionPower powerBloodBath)
        : IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome outcome,
            int damageAmount)
        {
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var constitution = rulesetDefender.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var totalDamage = AttributeDefinitions.ComputeAbilityScoreModifier(constitution);
            var defenderAttackTags =
                defender.FindActionAttackMode(ActionDefinitions.Id.AttackMain)?.AttackTags ?? [];

            rulesetDefender.LogCharacterUsedFeature(featureCorruptedBlood);
            EffectHelpers.StartVisualEffect(attacker, defender, PowerSorcererChildRiftOffering);
            InflictDamage(rulesetDefender, rulesetAttacker, totalDamage, defenderAttackTags);

            if (rulesetAttacker.IsDeadOrDying)
            {
                yield return HandleEnemyDeath(defender, attackMode, powerBloodBath);
            }
        }
    }
}
