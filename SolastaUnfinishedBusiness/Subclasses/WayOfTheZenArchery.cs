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

        // kept name for backward compatibility
        var featureFlurryOfArrows = FeatureDefinitionProficiencyBuilder
            .Create($"Feature{Name}FlurryOfArrows")
            .SetGuiPresentation(Category.Feature)
            .SetProficiencies(ProficiencyType.Weapon, WeaponTypeDefinitions.LongbowType.Name)
            .AddCustomSubFeatures(
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
            .AddCustomSubFeatures(
                new AddTagToWeaponWeaponAttack(
                    TagsDefinitions.MagicalWeapon,
                    ValidatorsWeapon.AlwaysValid,
                    ValidatorsCharacter.HasBowWithoutArmor))
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
            .SetUsesFixed(ActivationTime.Action, RechargeRate.KiPoints, 4, 4)
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

    internal static bool IsZenArcheryWeapon(RulesetActor rulesetActor, WeaponDescription weaponDescription)
    {
        return
            weaponDescription != null &&
            rulesetActor is RulesetCharacter rulesetCharacter &&
            (weaponDescription.WeaponTypeDefinition == WeaponTypeDefinitions.ShortbowType ||
             weaponDescription.WeaponTypeDefinition == WeaponTypeDefinitions.LongbowType) &&
            rulesetCharacter.GetSubclassLevel(CharacterClassDefinitions.Monk, Name) >= 3;
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

            effectDamageForm.DamageForm.IgnoreCriticalDoubleDice = true;

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
        public void ModifyWeaponAttackMode(RulesetCharacter character, RulesetAttackMode attackMode, RulesetItem weapon,
            bool canAddAbilityDamageBonus)
        {
            if (!ValidatorsCharacter.HasBowWithoutArmor(character))
            {
                return;
            }

            const int BONUS = 2;

            attackMode.ToHitBonus += BONUS;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(BONUS, FeatureSourceType.CharacterFeature,
                featureUnerringPrecision.Name, featureUnerringPrecision));

            var damage = attackMode.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            damage.BonusDamage += BONUS;
            damage.DamageBonusTrends.Add(new TrendInfo(BONUS, FeatureSourceType.CharacterFeature,
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
                .ToArray();

            if (targets.Length == 0)
            {
                yield break;
            }

            var attackModeMain = actingCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            //get copy to be sure we don't break existing mode
            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            actingCharacter.SetSpecialFeatureUses(HailOfArrowsAttack, 0);

            attackModeCopy.Copy(attackModeMain);
            attackModeCopy.ActionType = ActionDefinitions.ActionType.NoCost;

            foreach (var target in targets)
            {
                actingCharacter.MyExecuteActionAttack(
                    ActionDefinitions.Id.AttackFree,
                    target,
                    attackModeCopy,
                    new ActionModifier());
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
