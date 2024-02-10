using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static RuleDefinitions.RollContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class RoguishUmbralStalker : AbstractSubclass
{
    private const string Name = "RoguishUmbralStalker";

    public RoguishUmbralStalker()
    {
        // LEVEL 3

        // Deadly Shadows

        var additionalDamageDeadlyShadows = FeatureDefinitionAdditionalDamageBuilder
            .Create(AdditionalDamageRogueSneakAttack, $"AdditionalDamage{Name}DeadlyShadows")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(TagsDefinitions.AdditionalDamageSneakAttackTag)
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 2)
            .SetTriggerCondition(ExtraAdditionalDamageTriggerCondition.SourceAndTargetAreNotBright)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddCustomSubFeatures(ModifyAdditionalDamageClassLevelRogue.Instance)
            .AddToDB();

        var featureSetDeadlyShadows = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}DeadlyShadows")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(FeatureDefinitionSenses.SenseDarkvision, additionalDamageDeadlyShadows)
            .AddToDB();

        // Gloom blade

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
            .AddToDB();

        var conditionGloomBlade = ConditionDefinitionBuilder
            .Create($"Condition{Name}GloomBlade")
            .SetGuiPresentationNoContent(true)
            .AddFeatures(dieRollModifierDieRollModifier, additionalDamageGloomBlade)
            .SetSpecialInterruptions(ConditionInterruption.Attacks)
            .AddToDB();

        var actionAffinityHailOfBladesToggle = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle, "ActionAffinityGloomBladeToggle")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.HailOfBladesToggle)
            .AddCustomSubFeatures(new AttackBeforeHitConfirmedOnEnemyGloomBlade(conditionGloomBlade))
            .AddToDB();

        // LEVEL 9

        // Shadow Stride

        var powerShadowStride = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ShadowStride")
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(Name, Resources.PowerSilhouetteStep, 256, 128))
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.TurnStart)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 24, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .SetParticleEffectParameters(FeatureDefinitionPowers.PowerRoguishDarkweaverShadowy)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.IsNotInBrightLight),
                new FilterTargetingPositionShadowStride())
            .AddToDB();

        // LEVEL 13

        // Umbral Soul

        var powerUmbralSoul = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}UmbralSoul")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Reaction, RechargeRate.LongRest)
            .SetReactionContext(ExtraReactionContext.Custom)
            .AddToDB();

        powerUmbralSoul.AddCustomSubFeatures(new OnReducedToZeroHpByEnemyUmbralSoul(powerUmbralSoul));

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

        /*

          you can use your bonus action to empower yourself with a swirling nimbus of shadow energy for one minute.
          While this shadow energy persists, you are obscured by magical darkness which you can see through, and whenever you deal sneak attack damage and roll the maximum number on one of your sneak dice, you reroll that die and add it to the damage of the sneak attack.
          You can use this feature once per long rest.

        */

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.RoguishDuelist, 256))
            .AddFeaturesAtLevel(3, featureSetDeadlyShadows, actionAffinityHailOfBladesToggle)
            .AddFeaturesAtLevel(9, powerShadowStride)
            .AddFeaturesAtLevel(13, featureSetUmbralSoul)
            .AddFeaturesAtLevel(17)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Rogue;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceRogueRoguishArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    internal static bool SourceAndTargetAreNotBright(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        AdvantageType advantageType)
    {
        return
            advantageType != AdvantageType.Disadvantage &&
            attacker.RulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Rogue, Name) > 0 &&
            attacker.LightingState != LocationDefinitions.LightingState.Bright &&
            defender.LightingState != LocationDefinitions.LightingState.Bright;
    }

    //
    // Gloom Blade
    //

    private sealed class AttackBeforeHitConfirmedOnEnemyGloomBlade(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionGloomBlade) : IAttackBeforeHitConfirmedOnEnemy
    {
        public IEnumerator OnAttackBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (!ValidatorsWeapon.IsMelee(attackMode) ||
                !CharacterContext.IsSneakAttackValid(actionModifier, attacker, defender, attackMode))
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

    private sealed class FilterTargetingPositionShadowStride : IFilterTargetingPosition
    {
        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var source = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var range = source.RemainingTacticalMoves;
            var boxInt = new BoxInt(
                source.LocationPosition, new int3(-range, -range, -range), new int3(range, range, range));

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (gameLocationPositioningService.CanPlaceCharacter(
                        source, position, CellHelpers.PlacementMode.Station) &&
                    gameLocationPositioningService.CanCharacterStayAtPosition_Floor(
                        source, position, onlyCheckCellsWithRealGround: true))
                {
                    cursorLocationSelectPosition.validPositionsCache.Add(position);
                }

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
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
            GameLocationCharacter source,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            var gameLocationActionService =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var gameLocationBattleService =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (gameLocationActionService == null || gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var rulesetCharacter = source.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (!rulesetCharacter.CanUsePower(powerUmbralSoul))
            {
                yield break;
            }

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerUmbralSoul, rulesetCharacter);
            var reactionParams = new CharacterActionParams(source, ActionDefinitions.Id.PowerNoCost)
            {
                StringParameter = "UmbralSoul",
                RulesetEffect = implementationManagerService
                    .MyInstantiateEffectPower(rulesetCharacter, usablePower, false),
                UsablePower = usablePower
            };

            var count = gameLocationActionService.PendingReactionRequestGroups.Count;

            gameLocationActionService.ReactToUsePower(reactionParams, "UsePower", source);

            yield return gameLocationBattleService.WaitForReactions(source, gameLocationActionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var hitPoints = rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Rogue);

            rulesetCharacter.StabilizeAndGainHitPoints(hitPoints);

            ServiceRepository.GetService<ICommandService>()?
                .ExecuteAction(new CharacterActionParams(source, ActionDefinitions.Id.StandUp), null, true);
        }
    }
}
