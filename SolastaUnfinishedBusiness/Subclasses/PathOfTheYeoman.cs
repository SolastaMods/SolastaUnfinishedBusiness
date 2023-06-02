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

internal sealed class PathOfTheYeoman : AbstractSubclass
{
    internal const string Name = "PathOfTheYeoman";

    internal PathOfTheYeoman()
    {
        var isLongbow = ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.LongbowType);

        // LEVEL 03

        // Fletcher

        var proficiencyFletcher = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}Fletcher")
            .SetGuiPresentation(Category.Feature)
            // don't use ToolDefinitions.ArtisanToolType as that constant has an incorrect name
            .SetProficiencies(ProficiencyType.ToolOrExpertise, "ArtisanToolSmithToolsType")
            .AddToDB();

        // Strong Bow

        var additionalDamageStrongBow = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}StrongBow")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("Rage")
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel)
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.RageDamage)
            .SetRequiredProperty(RestrictedContextRequiredProperty.None)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.NotWearingHeavyArmor)
            .AddToDB();

        var featureStrongBow = FeatureDefinitionBuilder
            .Create($"Feature{Name}StrongBow")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new CanUseAttribute(AttributeDefinitions.Strength, isLongbow),
                new CustomAdditionalDamageStrongBow(additionalDamageStrongBow))
            .AddToDB();

        // LEVEL 06

        // Staggering Blow

        var actionAffinityStaggeringBlow = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}StaggeringBlow")
            .SetGuiPresentation(Category.Feature)
            .SetAllowedActionTypes()
            .SetAuthorizedActions(ActionDefinitions.Id.ShoveBonus)
            .SetCustomSubFeatures(
                new ValidatorsDefinitionApplication(
                    ValidatorsCharacter.HasLongbow,
                    ValidatorsCharacter.HasAnyOfConditions(ConditionRaging)))
            .AddToDB();

        // Keen Eye

        var additionalDamageKeenEye = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}KeenEye")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("BrutalCritical")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.BrutalCriticalDice)
            .SetIgnoreCriticalDoubleDice(true)
            .AddToDB();

        var featureKeenEye = FeatureDefinitionBuilder
            .Create($"Feature{Name}KeenEye")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new CustomAdditionalDamageKeenEye(additionalDamageKeenEye))
            .AddToDB();

        // LEVEL 10

        // Bulwark

        var movementAffinityBulwark = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}Bulwark")
            .SetGuiPresentation(Category.Feature)
            .SetBaseSpeedMultiplicativeModifier(0)
            .AddToDB();

        var featureBulwark = FeatureDefinitionBuilder
            .Create($"Feature{Name}Bulwark")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new RangedAttackInMeleeDisadvantageRemover(isLongbow),
                new CanMakeAoOOnReachEntered { WeaponValidator = isLongbow, AllowRange = true })
            .AddToDB();

        var combatAffinityBulwark = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}Bulwark")
            .SetGuiPresentation(Category.Feature)
            .SetSituationalContext(SituationalContext.AttackerAwayFromTarget)
            .SetAttackOnMeAdvantage(AdvantageType.Disadvantage)
            .AddToDB();

        var conditionBulwark = ConditionDefinitionBuilder
            .Create($"Condition{Name}Bulwark")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionEncumbered)
            .SetPossessive()
            .AddFeatures(
                movementAffinityBulwark,
                featureBulwark,
                combatAffinityBulwark)
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();

        var powerBulwark = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Bulwark")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite("Bulwark", Resources.PowerBulwark, 256, 128))
            .SetUsesAbilityBonus(ActivationTime.BonusAction, RechargeRate.LongRest, AttributeDefinitions.Constitution)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerTraditionCourtMageSpellShield)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBulwark, ConditionForm.ConditionOperation.Add, true)
                            .Build())
                    .Build())
            .AddToDB();

        var powerBulwarkTurnOff = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}BulwarkTurnOff")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionBulwark, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddToDB();

        movementAffinityBulwark.SetCustomSubFeatures(new StopPowerConcentrationProvider(
            "Bulwark",
            "Tooltip/&BulwarkConcentration",
            Sprites.GetSprite("DeadeyeConcentrationIcon", Resources.DeadeyeConcentrationIcon, 64, 64))
        {
            StopPower = powerBulwarkTurnOff
        });

        Global.PowersThatIgnoreInterruptions.Add(powerBulwark);
        Global.PowersThatIgnoreInterruptions.Add(powerBulwarkTurnOff);

        // LEVEL 14

        // Mighty Shot

        var powerMightyShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}MightyShot")
            .SetGuiPresentation($"Feature{Name}MightyShot", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.Cube, 7)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, true,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency, AttributeDefinitions.Strength, 8)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .SetDamageForm(DamageTypeThunder)
                            .Build())
                    .Build())
            .AddToDB();

        var featureMightyShot = FeatureDefinitionBuilder
            .Create($"Feature{Name}MightyShot")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(
                new UpgradeWeaponDice((_, damage) => (damage.diceNumber, DieType.D12, DieType.D12), isLongbow),
                new PhysicalAttackFinishedMightyShot(powerMightyShot))
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.PathOfTheYeoman, 256))
            .AddFeaturesAtLevel(3, proficiencyFletcher, featureStrongBow)
            .AddFeaturesAtLevel(6, actionAffinityStaggeringBlow, featureKeenEye)
            .AddFeaturesAtLevel(10, powerBulwark)
            .AddFeaturesAtLevel(14, featureMightyShot, powerMightyShot)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceBarbarianPrimalPath;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Strong Bow
    //

    private sealed class CustomAdditionalDamageStrongBow : CustomAdditionalDamage
    {
        public CustomAdditionalDamageStrongBow(IAdditionalDamageProvider provider) : base(provider)
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

            var rulesetAttacker = attacker.RulesetCharacter;

            return rulesetAttacker.HasConditionOfType(ConditionRaging) &&
                   ValidatorsCharacter.HasLongbow(rulesetAttacker);
        }
    }

    //
    // Keen Eye
    //

    private sealed class CustomAdditionalDamageKeenEye : CustomAdditionalDamage
    {
        public CustomAdditionalDamageKeenEye(IAdditionalDamageProvider provider) : base(provider)
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

            var rulesetAttacker = attacker.RulesetCharacter;

            return criticalHit && ValidatorsCharacter.HasLongbow(rulesetAttacker);
        }
    }

    //
    // Mighty Shot
    //

    private sealed class PhysicalAttackFinishedMightyShot : IPhysicalAttackFinished
    {
        private readonly FeatureDefinitionPower _powerMightyShot;

        public PhysicalAttackFinishedMightyShot(FeatureDefinitionPower powerMightyShot)
        {
            _powerMightyShot = powerMightyShot;
        }

        public IEnumerator OnAttackFinished(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            if (attackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.HasConditionOfType(ConditionRaging) ||
                !ValidatorsCharacter.HasLongbow(rulesetAttacker))
            {
                yield break;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var levels = rulesetAttacker.GetClassLevel(CharacterClassDefinitions.Barbarian);
            var rageBonus = levels switch
            {
                >= 16 => 4,
                >= 9 => 3,
                _ => 2
            };
            var usablePower = UsablePowersProvider.Get(_powerMightyShot, rulesetAttacker);
            var effectPower = new RulesetEffectPower(rulesetAttacker, usablePower);
            var damageForm = effectPower.EffectDescription.FindFirstDamageForm();

            damageForm.bonusDamage = rageBonus +
                                     AttributeDefinitions.ComputeAbilityScoreModifier(
                                         rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Strength));

            GameConsoleHelper.LogCharacterUsedPower(rulesetAttacker, _powerMightyShot);

            foreach (var target in gameLocationBattleService.Battle.AllContenders
                         .Where(x =>
                             x.Side != attacker.Side &&
                             x != defender &&
                             gameLocationBattleService.IsWithinXCells(defender, x, 3))
                         .ToList())
            {
                // EffectHelpers.StartVisualEffect(attacker, defender,
                //     FeatureDefinitionPowers.PowerDomainElementalLightningBlade, EffectHelpers.EffectType.Effect);
                EffectHelpers.StartVisualEffect(attacker, defender, SpellDefinitions.CallLightning,
                    EffectHelpers.EffectType.Effect);
                effectPower.ApplyEffectOnCharacter(target.RulesetCharacter, true, target.LocationPosition);
            }
        }
    }
}
