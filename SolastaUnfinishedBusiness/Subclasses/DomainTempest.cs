using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Builders.Features.AutoPreparedSpellsGroupBuilder;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class DomainTempest : AbstractSubclass
{
    public DomainTempest()
    {
        const string NAME = "DomainTempest";

        var divinePowerPrefix = Gui.Localize("Feature/&ClericChannelDivinityTitle") + ": ";

        var autoPreparedSpellsDomainNature = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{NAME}")
            .SetGuiPresentation("ExpandedSpells", Category.Feature)
            .SetAutoTag("Domain")
            .SetPreparedSpellGroups(
                BuildSpellGroup(1, FogCloud, Thunderwave),
                BuildSpellGroup(3, Shatter, SpellsContext.BindingIce),
                BuildSpellGroup(5, CallLightning, SleetStorm),
                BuildSpellGroup(7, IceStorm, SpellsContext.BlessingOfRime),
                BuildSpellGroup(9, InsectPlague, SpellsContext.DivineWrath))
            .SetSpellcastingClass(CharacterClassDefinitions.Cleric)
            .AddToDB();

        // LEVEL 01 - Wrath of The Storm

        var proficiencyHeavyArmor = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{NAME}HeavyArmor")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.HeavyArmorCategory)
            .AddToDB();

        var proficiencyMartialWeapons = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{NAME}MartialWeapons")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
            .AddToDB();

        var featureSetBonusProficiency = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}BonusProficiency")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(proficiencyHeavyArmor, proficiencyMartialWeapons)
            .AddToDB();

        var featureSetWrathOfTheStorm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}WrathOfTheStorm")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet()
            .AddToDB();

        //
        // Level 2 - Destructive Wrath
        //

        var powerDestructiveWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DestructiveWrath")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("DestructiveWrath", Resources.PowerCharmAnimalsAndPlants, 256, 128), hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ChannelDivinity)
            .DelegatedToAction()
            .AddToDB();

        powerDestructiveWrath.AddCustomSubFeatures(new CustomBehaviorDestructiveWrath(powerDestructiveWrath));

        _ = ActionDefinitionBuilder
                .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "DestructiveWrathToggle")
                .SetOrUpdateGuiPresentation(Category.Action)
                .RequiresAuthorization()
                .SetActionId(ExtraActionId.DestructiveWrathToggle)
                .SetActivatedPower(powerDestructiveWrath)
                .AddToDB();
        
        var actionAffinityDestructiveWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityDestructiveWrathToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle)
            .AddCustomSubFeatures(
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerDestructiveWrath)))
            .AddToDB();

        var featureSetDestructiveWrath = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}DestructiveWrath")
            .SetGuiPresentation(
                divinePowerPrefix + powerDestructiveWrath.FormatTitle(),
                powerDestructiveWrath.FormatDescription())
            .AddFeatureSet(actionAffinityDestructiveWrathToggle, powerDestructiveWrath)
            .AddToDB();

        //
        // LEVEL 6 - Thunderous Strike
        //

        var actionAffinityThunderousStrike = FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityThunderousStrikeToggle")
            .SetGuiPresentation(Category.Feature)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle)
            .AddCustomSubFeatures(new CustomBehaviorThunderousStrike())
            .AddToDB();

        //
        // LEVEL 08 - Divine Strike
        //

        var additionalDamageDivineStrike = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}DivineStrike")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("DivineStrike")
            .SetDamageDice(DieType.D8, 1)
            .SetSpecificDamageType(DamageTypeLightning)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .SetImpactParticleReference(LightningBolt)
            .AddToDB();

        // LEVEL 17 - Stormborn

        var powerStormbornSprout = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}StormbornSprout")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerStormbornSprout", Resources.PowerAngelicFormSprout, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 8)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFlyingAdaptive,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetParticleEffectParameters(PowerDomainElementalHeraldOfTheElementsThunder)
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasNoneOfConditions(ConditionFlyingAdaptive)))
            .AddToDB();

        var powerStormbornDismiss = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}StormbornDismiss")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite("PowerStormbornDismiss", Resources.PowerAngelicFormDismiss, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionFlyingAdaptive,
                                ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(ConditionFlyingAdaptive)))
            .AddToDB();

        var featureSetStormborn = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}Stormborn")
            .SetGuiPresentation($"Power{NAME}StormbornSprout", Category.Feature)
            .AddFeatureSet(powerStormbornSprout, powerStormbornDismiss)
            .AddToDB();

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Subclass, CharacterSubclassDefinitions.MartialSpellblade)
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainNature,
                featureSetBonusProficiency,
                featureSetWrathOfTheStorm)
            .AddFeaturesAtLevel(2, featureSetDestructiveWrath)
            .AddFeaturesAtLevel(6, actionAffinityThunderousStrike)
            .AddFeaturesAtLevel(8, additionalDamageDivineStrike)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(17, featureSetStormborn)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Cleric;

    internal override CharacterSubclassDefinition Subclass { get; }

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override FeatureDefinitionSubclassChoice SubclassChoice { get; }

    internal override DeityDefinition DeityDefinition => DeityDefinitions.Einar;

    private sealed class CustomBehaviorDestructiveWrath(FeatureDefinitionPower powerDestructiveWrath)
        : IForceMaxDamageTypeDependent, IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy, IActionFinishedByMe
    {
        private bool _isValid;

        public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
        {
            _isValid = false;

            yield break;
        }

        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            return _isValid;
        }

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDestructiveWrath, rulesetAttacker);

            _isValid = actualEffectForms.Any(x =>
                           x.FormType == EffectForm.EffectFormType.Damage &&
                           x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                       rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle) &&
                       rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0;

            if (!_isValid)
            {
                yield break;
            }

            rulesetAttacker.UsePower(usablePower);
            rulesetAttacker.LogCharacterUsedPower(powerDestructiveWrath);
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
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDestructiveWrath, rulesetAttacker);

            _isValid = actualEffectForms.Any(x =>
                           x.FormType == EffectForm.EffectFormType.Damage &&
                           x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                       rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle) &&
                       rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0;

            if (!_isValid)
            {
                yield break;
            }

            rulesetAttacker.UsePower(usablePower);
            rulesetAttacker.LogCharacterUsedPower(powerDestructiveWrath);
        }
    }

    private sealed class CustomBehaviorThunderousStrike
        : IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        private static readonly EffectForm PushForm = EffectFormBuilder
            .Create()
            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 2)
            .Build();

        public IEnumerator OnMagicEffectBeforeHitConfirmedOnEnemy(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            if (actualEffectForms
                    .Any(x => x.FormType == EffectForm.EffectFormType.Damage &&
                              x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                defender.RulesetCharacter.SizeDefinition.Name is "Tiny" or "Small" or "Medium" or "Large" &&
                attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle))
            {
                actualEffectForms.Add(PushForm);
            }

            yield break;
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
            if (actualEffectForms
                    .Any(x => x.FormType == EffectForm.EffectFormType.Damage &&
                              x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                defender.RulesetCharacter.SizeDefinition.Name is "Tiny" or "Small" or "Medium" or "Large" &&
                attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle))
            {
                actualEffectForms.Add(PushForm);
            }

            {
                actualEffectForms.Add(PushForm);
            }

            yield break;
        }
    }
}
