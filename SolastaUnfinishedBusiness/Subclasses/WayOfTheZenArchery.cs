using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Patches;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class WayOfZenArchery : AbstractSubclass
{
    internal const string Name = "WayOfZenArchery";
    internal const string HailOfArrowsAttack = "HailOfArrowsAttack";
    internal const string HailOfArrowsAttacksTab = "HailOfArrowsAttacksTab";
    internal const int StunningStrikeWithBowAllowedLevel = 6;
    private const ActionDefinitions.Id ZenShotToggle = (ActionDefinitions.Id)ExtraActionId.ZenShotToggle;

    public WayOfZenArchery()
    {
        //
        // LEVEL 03
        //

        // Flurry of Arrows

        var featureFlurryOfArrows = FeatureDefinitionBuilder
            .Create($"Feature{Name}FlurryOfArrows")
            .SetGuiPresentation(Category.Feature)
            .AddCustomSubFeatures(
                new ModifyWeaponAttackModeFlurryOfArrows(),
                new AddExtraRangedAttack(
                    ActionDefinitions.ActionType.Bonus,
                    ValidatorsWeapon.AlwaysValid,
                    ValidatorsCharacter.HasBowWithoutArmor,
                    ValidatorsCharacter.HasAnyOfConditions(ConditionMonkMartialArtsUnarmedStrikeBonus)))
            .AddToDB();

        // One with the Bow

        var proficiencyOneWithTheBow =
            FeatureDefinitionProficiencyBuilder
                .Create($"Proficiency{Name}OneWithTheBow")
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(ProficiencyType.Tool, ToolDefinitions.ArtisanToolType)
                .AddCustomSubFeatures(new CustomLevelUpLogicOneWithTheBow())
                .AddToDB();

        // Zen Shot

        var powerZenShot = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ZenShot")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.KiPoints)
            .DelegatedToAction()
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "ZenShotToggle")
            .SetOrUpdateGuiPresentation(powerZenShot.Name, Category.Feature)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ZenShotToggle)
            .SetActivatedPower(powerZenShot)
            .OverrideClassName("Toggle")
            .AddToDB();

        var actionAffinityZenShotToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityZenShotToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions(ZenShotToggle)
            .AddToDB();

        powerZenShot.AddCustomSubFeatures(new PhysicalAttackBeforeHitConfirmedOnEnemyZenShot(powerZenShot));

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
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Self, 0, TargetType.Cone, 9)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeHailOfArrows())
            .AddToDB();

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

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.WayOfTheZenArchery, 256))
            .AddFeaturesAtLevel(3,
                proficiencyOneWithTheBow, featureFlurryOfArrows, actionAffinityZenShotToggle, powerZenShot)
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
    // Flurry of Arrows
    //

    // set attacks number to 2 to allow a mix of unarmed / bow attacks otherwise game engine will consume bonus action
    // once at least one bonus attack is used this check fails and everything gets back to normal
    // the patch on CharacterActionItemForm.Refresh finishes the trick by hiding the number of attacks
    // when attack tags have a proper hide tag
    private sealed class ModifyWeaponAttackModeFlurryOfArrows : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter rulesetCharacter, RulesetAttackMode attackMode)
        {
            var character = GameLocationCharacter.GetFromActor(rulesetCharacter);

            if (character is not { UsedBonusAttacks: 0 } ||
                attackMode.ActionType != ActionDefinitions.ActionType.Bonus ||
                !rulesetCharacter.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, ConditionFlurryOfBlows) ||
                !ValidatorsWeapon.IsOfWeaponType(WeaponTypeDefinitions.LongbowType, WeaponTypeDefinitions.ShortbowType)(
                    attackMode, null, null))
            {
                return;
            }

            attackMode.AddAttackTagAsNeeded(
                CharacterActionItemFormPatcher.Refresh_Patch.HideAttacksNumberOnActionPanel);
            attackMode.AttacksNumber = 2;
        }
    }

    //
    // One with the Bow
    //

    private sealed class CustomLevelUpLogicOneWithTheBow : ICustomLevelUpLogic
    {
        public void ApplyFeature(RulesetCharacterHero hero, string tag)
        {
            const string PREFIX = "CustomInvocationMonkWeaponSpecialization";

            var heroBuildingData = hero.GetHeroBuildingData();
            var invocationShortbowType = GetDefinition<InvocationDefinition>($"{PREFIX}ShortbowType");
            var invocationLongbowType = GetDefinition<InvocationDefinition>($"{PREFIX}LongbowType");

            heroBuildingData.LevelupTrainedInvocations.Add(tag, [invocationShortbowType, invocationLongbowType]);
        }

        public void RemoveFeature(RulesetCharacterHero hero, string tag)
        {
            // empty
        }
    }

    //
    // Zen Shot
    //

    private sealed class PhysicalAttackBeforeHitConfirmedOnEnemyZenShot(FeatureDefinitionPower powerZenShot)
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
            var usablePower = PowerProvider.Get(powerZenShot, rulesetAttacker);

            if (!ValidatorsCharacter.HasBowWithoutArmor(attacker.RulesetCharacter) ||
                !attacker.OnceInMyTurnIsValid("ZenShot") ||
                !rulesetAttacker.IsToggleEnabled(ZenShotToggle) ||
                rulesetAttacker.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            var firstDamageForm = actualEffectForms.FirstOrDefault(x => x.FormType == EffectForm.EffectFormType.Damage);
            var index = actualEffectForms.IndexOf(firstDamageForm);
            var dieType = rulesetAttacker.GetMonkDieType();
            var wisdom = rulesetAttacker.TryGetAttributeValue(AttributeDefinitions.Wisdom);
            var wisMod = AttributeDefinitions.ComputeAbilityScoreModifier(wisdom);
            var effectDamageForm = EffectFormBuilder.DamageForm(
                firstDamageForm!.DamageForm.DamageType, 1, dieType, wisMod);

            actualEffectForms.Insert(index + 1, effectDamageForm);
            attacker.UsedSpecialFeatures.TryAdd("ZenShot", 0);
            rulesetAttacker.UsePower(usablePower);
        }
    }

    //
    // Unerring Precision
    //

    private sealed class ModifyWeaponAttackModeUnerringPrecision(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinition featureUnerringPrecision) : IModifyWeaponAttackMode
    {
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
                featureUnerringPrecision.Name, featureUnerringPrecision));

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            damage.BonusDamage += bonus;
            damage.DamageBonusTrends.Add(new TrendInfo(bonus, FeatureSourceType.CharacterFeature,
                featureUnerringPrecision.Name, featureUnerringPrecision));
        }
    }

    //
    // Hail of Arrows
    //

    private sealed class PowerOrSpellFinishedByMeHailOfArrows : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var targets = action.ActionParams.TargetCharacters
                .Where(x => CanBowAttack(actingCharacter, x))
                .ToList();

            if (targets.Count == 0)
            {
                yield break;
            }

            var attackModeMain = actingCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackModeMain == null)
            {
                yield break;
            }

            //get copy to be sure we don't break existing mode
            var attackMode = RulesetAttackMode.AttackModesPool.Get();

            attackMode.Copy(attackModeMain);
            attackMode.ActionType = ActionDefinitions.ActionType.NoCost;
            actingCharacter.UsedSpecialFeatures.TryAdd(HailOfArrowsAttack, 0);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var target in targets)
            {
                var attackModifier = new ActionModifier();

                actingCharacter.MyExecuteActionAttack(
                    ActionDefinitions.Id.AttackFree,
                    target,
                    attackMode,
                    attackModifier);
            }
        }

        private static bool CanBowAttack([NotNull] GameLocationCharacter caster, GameLocationCharacter target)
        {
            if (!ValidatorsCharacter.HasBowWithoutArmor(caster.RulesetCharacter))
            {
                return false;
            }

            var battleService = ServiceRepository.GetService<IGameLocationBattleService>();
            var attackModifier = new ActionModifier();
            var evalParams = new BattleDefinitions.AttackEvaluationParams();
            var attackMode = caster.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            evalParams.FillForPhysicalRangeAttack(
                caster, caster.LocationPosition, attackMode, target, target.LocationPosition, attackModifier);

            return battleService.CanAttack(evalParams);
        }
    }
}
