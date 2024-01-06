using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Properties;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfZenArchery : AbstractSubclass
{
    internal const string Name = "WayOfZenArchery";
    internal const string HailOfArrows = "HailOfArrows";
    internal const int StunningStrikeWithBowAllowedLevel = 6;

    public WayOfZenArchery()
    {
        //
        // LEVEL 03
        //

        // Way of The Bow

        var proficiencyOneWithTheBow =
            FeatureDefinitionProficiencyBuilder
                .Create($"Proficiency{Name}OneWithTheBow")
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(ProficiencyType.Tool, ToolDefinitions.ArtisanToolType)
                .AddCustomSubFeatures(new CustomLevelUpLogicOneWithTheBow())
                .AddToDB();

        // Flurry of Arrows

        var conditionFlurryOfArrows = ConditionDefinitionBuilder
            .Create($"Condition{Name}FlurryOfArrows")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddCustomSubFeatures(new AddExtraFlurryOfArrowsAttack())
            .AddToDB();

        var featureFlurryOfArrows = FeatureDefinitionBuilder
            .Create($"Feature{Name}FlurryOfArrows")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new ActionFinishedByMeFlurryOfArrows(conditionFlurryOfArrows))
            .AddToDB();

        //
        // LEVEL 06
        //

        // Ki-Empowered Arrows

        var featureKiEmpoweredArrows = FeatureDefinitionBuilder
            .Create($"Feature{Name}KiEmpoweredArrows")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(new AddTagToWeaponWeaponAttack(
                TagsDefinitions.MagicalWeapon, ValidatorsWeapon.AlwaysValid, ValidatorsCharacter.HasBowWithoutArmor))
            .AddToDB();

        //
        // LEVEL 11
        //

        // Unerring Precision

        var featureUnerringPrecision = FeatureDefinitionBuilder
            .Create($"Feature{Name}UnerringPrecision")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureUnerringPrecision.AddCustomSubFeatures(
            new ModifyWeaponAttackModeUnerringPrecision(featureUnerringPrecision));

        //
        // LEVEL 17
        //

        // Hail of Arrows

        var actionAffinityHailOfArrows = FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}HailOfArrows")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.HailOfArrows)
            .AddCustomSubFeatures(new ValidateDefinitionApplication(ValidatorsCharacter.HasBowWithoutArmor))
            .AddToDB();

        var powerHailOfArrows = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}HailOfArrows")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 3, 3)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cone, 9)
                    .Build())
            .AddCustomSubFeatures(new AttackAfterMagicEffectHailOfArrows())
            .AddToDB();

        powerHailOfArrows.EffectDescription.EffectParticleParameters.casterParticleReference = new AssetReference();

        var actionHailOfArrows = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.Volley, "ActionHailOfArrows")
            .SetOrUpdateGuiPresentation($"Power{Name}HailOfArrows", Category.Feature)
            .SetActionId(ExtraActionId.HailOfArrows)
            .SetActionType(ActionDefinitions.ActionType.Main)
            .SetActivatedPower(powerHailOfArrows)
            .SetStealthBreakerBehavior(ActionDefinitions.StealthBreakerBehavior.RollIfTargets)
            .OverrideClassName("UsePower")
            .AddToDB();

        actionHailOfArrows.particlePrefab = new AssetReference();

        //
        // PROGRESSION
        //

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheDistantHand, 256))
            .AddFeaturesAtLevel(3, proficiencyOneWithTheBow, featureFlurryOfArrows)
            .AddFeaturesAtLevel(6, featureKiEmpoweredArrows)
            .AddFeaturesAtLevel(11, featureUnerringPrecision)
            .AddFeaturesAtLevel(17, actionAffinityHailOfArrows, powerHailOfArrows)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Monk;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceMonkMonasticTraditions;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // One with the Bow
    //

    private sealed class CustomLevelUpLogicOneWithTheBow : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            hero.TrainedInvocations.TryAdd(GetDefinition<InvocationDefinition>(
                "CustomInvocationMonkWeaponSpecializationShortbowType"));
            hero.TrainedInvocations.TryAdd(GetDefinition<InvocationDefinition>(
                "CustomInvocationMonkWeaponSpecializationLongbowType"));
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    //
    // Flurry of Arrows
    //

    private sealed class ActionFinishedByMeFlurryOfArrows : IActionFinishedByMe
    {
        private readonly ConditionDefinition _condition;

        public ActionFinishedByMeFlurryOfArrows(ConditionDefinition condition)
        {
            _condition = condition;
        }

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionFlurryOfBlows)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                _condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                _condition.Name,
                0,
                0,
                0);
        }
    }

    //
    // Unerring Precision
    //

    private sealed class ModifyWeaponAttackModeUnerringPrecision : IModifyWeaponAttackMode
    {
        private readonly FeatureDefinition _featureUnerringPrecision;

        public ModifyWeaponAttackModeUnerringPrecision(FeatureDefinition featureUnerringPrecision)
        {
            _featureUnerringPrecision = featureUnerringPrecision;
        }

        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsCharacter.HasBowWithoutArmor(character))
            {
                return;
            }

            var proficiencyBonus = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var bonus = proficiencyBonus / 2;

            attackMode.ToHitBonus += bonus;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(bonus, FeatureSourceType.CharacterFeature,
                _featureUnerringPrecision.Name, _featureUnerringPrecision));

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(new TrendInfo(bonus, FeatureSourceType.CharacterFeature,
                _featureUnerringPrecision.Name, _featureUnerringPrecision));
        }
    }

    //
    // Hail of Arrows
    //

    private sealed class AttackAfterMagicEffectHailOfArrows : IAttackAfterMagicEffect
    {
        public IAttackAfterMagicEffect.CanAttackHandler CanAttack { get; } =
            CanBowAttack;

        public IAttackAfterMagicEffect.GetAttackAfterUseHandler PerformAttackAfterUse { get; } =
            DefaultAttackHandler;

        public IAttackAfterMagicEffect.CanUseHandler CanBeUsedToAttack { get; } =
            DefaultCanUseHandler;

        private static bool CanBowAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            if (!ValidatorsCharacter.HasBowWithoutArmor(caster.RulesetCharacter))
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService is not { IsBattleInProgress: true })
            {
                return false;
            }

            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            evalParams.FillForPhysicalRangeAttack(
                caster, caster.LocationPosition, attackMode, target, target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }

        [CanBeNull]
        private static IEnumerable<CharacterActionParams> DefaultAttackHandler(
            [CanBeNull] CharacterActionMagicEffect effect)
        {
            var actionParams = effect?.ActionParams;

            if (actionParams == null)
            {
                return null;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (battleService is not { IsBattleInProgress: true })
            {
                return null;
            }

            var caster = actionParams.ActingCharacter;
            var targets = actionParams.TargetCharacters;

            if (targets.Count == 0)
            {
                return null;
            }

            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                return null;
            }

            //get copy to be sure we don't break existing mode
            var rulesetAttackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            rulesetAttackModeCopy.Copy(attackMode);

            attackMode = rulesetAttackModeCopy;

            //set action type to be same as the one used for the magic effect
            attackMode.ActionType = effect.ActionType;
            attackMode.AttackTags.Add(HailOfArrows);

            var attackModifier = new ActionModifier();

            return targets
                .Where(t => CanBowAttack(caster, t))
                .Select(target =>
                    new CharacterActionParams(caster, ActionDefinitions.Id.AttackFree)
                    {
                        AttackMode = attackMode, TargetCharacters = { target }, ActionModifiers = { attackModifier }
                    });
        }

        private static bool DefaultCanUseHandler(
            [NotNull] CursorLocationSelectTarget targeting,
            GameLocationCharacter caster,
            GameLocationCharacter target, [NotNull] out string failure)
        {
            failure = string.Empty;

            return true;
        }
    }
}
