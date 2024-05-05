using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Feats;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static RuleDefinitions.RollContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishUmbralStalker : AbstractSubclass
{
    internal const string Name = "RoguishUmbralStalker";

    internal static readonly ConditionDefinition ConditionShadowDanceAdditionalDice = ConditionDefinitionBuilder
        .Create($"Condition{Name}ShadowDanceAdditionalDice")
        .SetGuiPresentationNoContent(true)
        .SetSilent(Silent.WhenAddedOrRemoved)
        .SetSpecialInterruptions(ConditionInterruption.Attacks)
        .AddToDB();

    public RoguishUmbralStalker()
    {
        // LEVEL 3

        // Deadly Shadows

        var additionalDamageDeadlyShadows = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}DeadlyShadows")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.SourceAndTargetAreNotBrightAndWithin5Ft)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        additionalDamageDeadlyShadows.AddCustomSubFeatures(
            ModifyAdditionalDamageClassLevelRogue.Instance,
            new ClassFeats.ModifyAdditionalDamageCloseQuarters(additionalDamageDeadlyShadows));

        var featureSetDeadlyShadows = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DeadlyShadows")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(FeatureDefinitionSenses.SenseDarkvision, additionalDamageDeadlyShadows)
            .AddToDB();

        // Gloomblade

        var dieRollModifierDieRollModifier = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}GloomBlade")
            .SetGuiPresentationNoContent(true)
            .SetModifiers(AttackDamageValueRoll, 1, 1, 2, "Feature/&GloomBladeAttackReroll")
            .AddToDB();

        var additionalDamageGloomBlade = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}GloomBlade")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("GloomBlade")
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetRequiredProperty(RestrictedContextRequiredProperty.MeleeWeapon)
            .SetImpactParticleReference(Power_HornOfBlasting)
            .AddToDB();

        additionalDamageGloomBlade.AddCustomSubFeatures(
            new ClassFeats.ModifyAdditionalDamageCloseQuarters(additionalDamageGloomBlade));

        var conditionGloomBlade = ConditionDefinitionBuilder
            .Create($"Condition{Name}GloomBlade")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddFeatures(dieRollModifierDieRollModifier, additionalDamageGloomBlade)
            .AddCustomSubFeatures(new AllowRerollDiceOnAllDamageFormsGloomBlade())
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var actionAffinityHailOfBladesToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityGloomBladeToggle")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.GloomBladeToggle)
            .AddCustomSubFeatures(new PhysicalAttackBeforeHitConfirmedOnEnemyGloomBlade(conditionGloomBlade))
            .AddToDB();

        // LEVEL 9

        // Shadow Stride

        var powerShadowStride = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowStride")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerSilhouetteStep, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 24, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .SetParticleEffectParameters(PowerRoguishDarkweaverShadowy)
                    .Build())
            .AddToDB();

        var powerShadowStrideBonus = FeatureDefinitionPowerBuilder
            .Create(powerShadowStride, $"Power{Name}ShadowStrideBonus")
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(powerShadowStride)
                    .AddEffectForms(EffectFormBuilder.ConditionForm(
                        ConditionDefinitions.ConditionDisengaging,
                        ConditionForm.ConditionOperation.Add, true))
                    .Build())
            .AddToDB();

        var powerShadowStrideMain = FeatureDefinitionPowerBuilder
            .Create(powerShadowStrideBonus, $"Power{Name}ShadowStrideMain")
            .SetUsesFixed(ActivationTime.Action)
            .AddToDB();

        powerShadowStride.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => CanUseShadowStride(c, true), ValidatorsCharacter.HasAvailableMoves),
            new CustomBehaviorShadowStride(powerShadowStride, false));

        powerShadowStrideBonus.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => CanUseShadowStride(c), ValidatorsCharacter.HasAvailableBonusDash),
            new CustomBehaviorShadowStride(powerShadowStrideBonus, true));

        powerShadowStrideMain.AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(c => CanUseShadowStride(c)),
            new CustomBehaviorShadowStride(powerShadowStrideMain, true));

        var featureSetShadowStride = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}ShadowStride")
            .SetGuiPresentation($"Power{Name}ShadowStride", Category.Feature)
            .SetFeatureSet(powerShadowStride, powerShadowStrideMain, powerShadowStrideBonus)
            .AddToDB();

        // LEVEL 13

        // Umbral Soul

        var powerUmbralSoul = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}UmbralSoul")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
            .AddToDB();

        powerUmbralSoul.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new OnReducedToZeroHpByEnemyUmbralSoul(powerUmbralSoul));

        var featureSetUmbralSoul = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}UmbralSoul")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(
                powerUmbralSoul,
                FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance,
                FeatureDefinitionDamageAffinitys.DamageAffinityNecroticResistance)
            .AddToDB();

        // LEVEL 17

        // Shadow Dance

        var conditionShadowDance = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionBlinded, $"Condition{Name}ShadowDance")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionChildOfDarkness_DimLight)
            .SetConditionType(ConditionType.Beneficial)
            .SetPossessive()
            .SetFeatures()
            .AddCustomSubFeatures(new CustomBehaviorShadowDance())
            .AddToDB();

        var powerShadowDance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowDance")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerShadowDance", Resources.PowerShadowDance, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionShadowDance))
                    .SetParticleEffectParameters(PowerSorakAssassinShadowMurder)
                    .Build())
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheSilhouette, 256))
            .AddFeaturesAtLevel(3, featureSetDeadlyShadows, actionAffinityHailOfBladesToggle)
            .AddFeaturesAtLevel(9, featureSetShadowStride)
            .AddFeaturesAtLevel(13, featureSetUmbralSoul)
            .AddFeaturesAtLevel(17, powerShadowDance)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static bool SourceAndTargetAreNotBrightAndWithin5Ft(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        AdvantageType advantageType)
    {
        return
            advantageType != AdvantageType.Disadvantage &&
            attacker.RulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Rogue, Name) > 0 &&
            attacker.IsWithinRange(defender, 1) &&
            attacker.LightingState != LocationDefinitions.LightingState.Bright &&
            defender.LightingState != LocationDefinitions.LightingState.Bright;
    }


    private static bool CanUseShadowStride(RulesetCharacter character, bool enableOffBattle = false)
    {
        if (Gui.Battle == null)
        {
            return enableOffBattle;
        }

        var locationCharacter = GameLocationCharacter.GetFromActor(character);

        return locationCharacter != null &&
               locationCharacter.OnceInMyTurnIsValid("ShadowStride") &&
               ValidatorsCharacter.IsNotInBrightLight(character);
    }

    //
    // Gloom Blade
    //

    private sealed class AllowRerollDiceOnAllDamageFormsGloomBlade : IAllowRerollDiceOnAllDamageForms;

    private sealed class PhysicalAttackBeforeHitConfirmedOnEnemyGloomBlade(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionGloomBlade) : IPhysicalAttackBeforeHitConfirmedOnEnemy
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
            if (!ValidatorsWeapon.IsMelee(attackMode) ||
                !CharacterContext.IsSneakAttackValid(actionModifier, attacker, defender))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                conditionGloomBlade.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionGloomBlade.Name,
                0,
                0,
                0);
        }
    }

    //
    // Shadow Stride
    //

    private sealed class CustomBehaviorShadowStride(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerShadowStride,
        bool isDashing) : IFilterTargetingPosition, IMagicEffectInitiatedByMe, IModifyEffectDescription
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var distance = actingCharacter.RemainingTacticalMoves + (isDashing ? actingCharacter.MaxTacticalMoves : 0);

            yield return cursorLocationSelectPosition
                .MyComputeValidPositions(LocationDefinitions.LightingState.Bright, distance);
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;

            if (Gui.Battle == null)
            {
                yield break;
            }

            var sourcePosition = actingCharacter.LocationPosition;
            var targetPosition = action.ActionParams.Positions[0];
            var distance = (int)Math.Round(
                // must use vanilla distance calculation here
                int3.Distance(sourcePosition, targetPosition) -
                (isDashing ? actingCharacter.MaxTacticalMoves : 0));

            actingCharacter.UsedSpecialFeatures.TryAdd("ShadowStride", 1);
            actingCharacter.UsedTacticalMoves += distance;
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerShadowStride;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var locationCharacter = GameLocationCharacter.GetFromActor(character);

            if (locationCharacter != null)
            {
                effectDescription.rangeParameter =
                    locationCharacter.RemainingTacticalMoves + (isDashing ? locationCharacter.MaxTacticalMoves : 0);
            }

            return effectDescription;
        }
    }

    //
    // Umbral Soul
    //

    private sealed class OnReducedToZeroHpByEnemyUmbralSoul(FeatureDefinitionPower powerUmbralSoul)
        : IOnReducedToZeroHpByEnemy
    {
        public IEnumerator HandleReducedToZeroHpByEnemy(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (ServiceRepository.GetService<IGameLocationBattleService>() is not GameLocationBattleManager
                {
                    IsBattleInProgress: true
                } battleManager)
            {
                yield break;
            }

            var rulesetCharacter = defender.RulesetCharacter;

            if (rulesetCharacter.GetRemainingPowerUses(powerUmbralSoul) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerUmbralSoul, rulesetCharacter);
            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                StringParameter = "UmbralSoul",
                ActionModifiers = { new ActionModifier() },
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { defender }
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var hitPoints = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Rogue);

            rulesetCharacter.StabilizeAndGainHitPoints(hitPoints);

            EffectHelpers.StartVisualEffect(
                defender, defender, PowerDefilerMistyFormEscape, EffectHelpers.EffectType.Caster);
            ServiceRepository.GetService<ICommandService>()?
                .ExecuteAction(new CharacterActionParams(defender, ActionDefinitions.Id.StandUp), null, true);
        }
    }

    //
    // Shadow Dance
    //

    private sealed class CustomBehaviorShadowDance : IPhysicalAttackBeforeHitConfirmedOnEnemy, IForceLightingState
    {
        public LocationDefinitions.LightingState GetLightingState(
            GameLocationCharacter gameLocationCharacter,
            LocationDefinitions.LightingState lightingState)
        {
            return LocationDefinitions.LightingState.Darkness;
        }

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
            if (!CharacterContext.IsSneakAttackValid(actionModifier, attacker, defender))
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                ConditionShadowDanceAdditionalDice.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionShadowDanceAdditionalDice.Name,
                0,
                0,
                0);
        }
    }
}
