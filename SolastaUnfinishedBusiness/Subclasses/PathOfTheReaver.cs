using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
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

        // Voracious Fury

        var featureVoraciousFury = FeatureDefinitionBuilder
            .Create($"Feature{Name}VoraciousFury")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureVoraciousFury.AddCustomSubFeatures(new PhysicalAttackFinishedByMeVoraciousFury(featureVoraciousFury));

        // Draconic Resilience

        var attributeModifierDraconicResilience = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}ProfaneVitality")
            .SetGuiPresentation(Category.Feature)
            .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.HitPointBonusPerLevel, 1)
            .AddCustomSubFeatures(new CustomLevelUpLogicDraconicResilience())
            .AddToDB();

        // LEVEL 06

        // Profane Vitality

        var featureSetProfaneVitality = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ProfaneVitality")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(DamageAffinityNecroticResistance, DamageAffinityPoisonResistance)
            .AddToDB();

        // LEVEL 10

        // Bloodbath

        var powerBloodbath = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Bloodbath")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest)
            .SetShowCasting(false)
            .AddToDB();

        powerBloodbath.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden, new OnReducedToZeroHpByMeBloodbath(powerBloodbath));

        // LEVEL 14

        // Corrupted Blood

        var powerCorruptedBlood = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}CorruptedBlood")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetExplicitAbilityScore(AttributeDefinitions.Constitution)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .SetDamageForm(DamageTypeNecrotic)
                            .Build())
                    .SetImpactEffectParameters(PowerSorcererChildRiftOffering)
                    .Build())
            .AddToDB();

        powerCorruptedBlood.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden, new PhysicalAttackFinishedOnMeCorruptedBlood(powerCorruptedBlood));

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheReaver, 256))
            .AddFeaturesAtLevel(3, featureVoraciousFury, attributeModifierDraconicResilience)
            .AddFeaturesAtLevel(6, featureSetProfaneVitality)
            .AddFeaturesAtLevel(10, powerBloodbath)
            .AddFeaturesAtLevel(14, powerCorruptedBlood)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Barbarian;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private static void ReceiveHealing(GameLocationCharacter character, int totalHealing)
    {
        EffectHelpers.StartVisualEffect(character, character, Heal, EffectHelpers.EffectType.Effect);
        character.RulesetCharacter.ReceiveHealing(totalHealing, true, character.Guid);
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

    private sealed class PhysicalAttackFinishedByMeVoraciousFury(FeatureDefinition featureVoraciousFury)
        : IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var damageForm =
                actualEffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);

            if (damageForm == null ||
                !IsVoraciousFuryValidContext(rulesetAttacker, attackMode) ||
                !attacker.OnceInMyTurnIsValid(featureVoraciousFury.Name))
            {
                yield break;
            }

            attacker.UsedSpecialFeatures.TryAdd(featureVoraciousFury.Name, 1);
            rulesetAttacker.LogCharacterUsedFeature(featureVoraciousFury);

            var multiplier =
                1 +
                (criticalHit ? 1 : 0) +
                (rulesetAttacker.MissingHitPoints > rulesetAttacker.CurrentHitPoints ? 1 : 0);
            var pb = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var totalDamageOrHealing = pb * multiplier;
            var index = actualEffectForms.IndexOf(damageForm);
            var effectForm =
                EffectFormBuilder.DamageForm(DamageTypeNecrotic, 0, DieType.D1, totalDamageOrHealing);

            actualEffectForms.Insert(index + 1, effectForm);
            ReceiveHealing(attacker, totalDamageOrHealing);
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
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerBloodBath, rulesetAttacker);

            if (rulesetAttacker.MissingHitPoints == 0 ||
                !rulesetAttacker.HasConditionOfTypeOrSubType(ConditionRaging) ||
                rulesetAttacker.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmed(attackMode))
            {
                yield break;
            }

            var classLevel = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var totalHealing = 2 * classLevel;

            yield return attacker.MyReactToSpendPower(
                usablePower,
                attacker,
                "Bloodbath",
                "SpendPowerBloodbathDescription".Formatted(Category.Reaction, totalHealing.ToString()),
                ReactionValidated);

            yield break;

            void ReactionValidated()
            {
                ReceiveHealing(attacker, totalHealing);
            }
        }
    }

    //
    // Corrupted Blood
    //

    private class PhysicalAttackFinishedOnMeCorruptedBlood(FeatureDefinitionPower powerCorruptedBlood)
        : IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetCharacter;

            if (rollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                rulesetAttacker is not { IsDeadOrDyingOrUnconscious: false } ||
                rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerCorruptedBlood, rulesetDefender);

            defender.MyExecuteActionPowerNoCost(usablePower, attacker);
        }
    }
}
