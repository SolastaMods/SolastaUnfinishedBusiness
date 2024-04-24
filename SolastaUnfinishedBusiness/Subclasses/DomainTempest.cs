using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
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

        var powerWrathOfTheStorm = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}WrathOfTheStorm")
            .SetGuiPresentation($"FeatureSet{NAME}WrathOfTheStorm", Category.Feature)
            .SetUsesAbilityBonus(ActivationTime.NoCost, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .Build())
            .AddToDB();

        powerWrathOfTheStorm.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden, new CustomBehaviorWrathOfTheStorm(powerWrathOfTheStorm));

        var powerWrathOfTheStormLightning = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}WrathOfTheStormLightning")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerWrathOfTheStorm)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeLightning, 2, DieType.D8)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerWrathOfTheStormThunder = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}WrathOfTheStormThunder")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerWrathOfTheStorm)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeThunder, 2, DieType.D8)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(
            powerWrathOfTheStorm, false, powerWrathOfTheStormLightning, powerWrathOfTheStormThunder);

        var featureSetWrathOfTheStorm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}WrathOfTheStorm")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerWrathOfTheStorm, powerWrathOfTheStormLightning, powerWrathOfTheStormThunder)
            .AddToDB();

        //
        // Level 2 - Destructive Wrath
        //

        var powerDestructiveWrath = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}DestructiveWrath")
            .SetGuiPresentation(Category.Feature, hidden: true)
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

    private static string GetAdditionalDamageType(
        // ReSharper disable once SuggestBaseTypeForParameter
        GameLocationCharacter attacker,
        DamageForm additionalDamageForm,
        // ReSharper disable once SuggestBaseTypeForParameter
        FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage)

    {
        if (additionalDamageForm.DiceNumber <= 0 && additionalDamageForm.BonusDamage <= 0)
        {
            return string.Empty;
        }

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (featureDefinitionAdditionalDamage.AdditionalDamageType)
        {
            case AdditionalDamageType.Specific:
                return featureDefinitionAdditionalDamage.SpecificDamageType;

            case AdditionalDamageType.AncestryDamageType:
                attacker.RulesetCharacter.EnumerateFeaturesToBrowse<FeatureDefinitionAncestry>(
                    FeatureDefinitionAncestry.FeaturesToBrowse);

                foreach (var definitionAncestry in FeatureDefinitionAncestry.FeaturesToBrowse
                             .Select(definition => definition as FeatureDefinitionAncestry)
                             .Where(definitionAncestry =>
                                 definitionAncestry &&
                                 definitionAncestry.Type ==
                                 featureDefinitionAdditionalDamage.AncestryTypeForDamageType &&
                                 !string.IsNullOrEmpty(definitionAncestry.DamageType)))
                {
                    return definitionAncestry.DamageType;
                }

                break;
        }

        return string.Empty;
    }

    private sealed class CustomBehaviorWrathOfTheStorm(FeatureDefinitionPower powerWrathOfTheStorm)
        : IPhysicalAttackBeforeHitConfirmedOnMe, IMagicEffectBeforeHitConfirmedOnMe
    {
        public IEnumerator OnMagicEffectBeforeHitConfirmedOnMe(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier actionModifier,
            RulesetEffect rulesetEffect,
            List<EffectForm> actualEffectForms,
            bool firstTarget,
            bool criticalHit)
        {
            yield return HandleReaction(battleManager, attacker, defender);
        }

        public IEnumerator OnPhysicalAttackBeforeHitConfirmedOnMe(
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
            yield return HandleReaction(battleManager, attacker, defender);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender)
        {
            var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (!actionManager)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerWrathOfTheStorm, rulesetDefender);

            if (!defender.CanReact() ||
                !defender.IsWithinRange(attacker, 1) ||
                rulesetDefender.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.PowerNoCost)
            {
                ActionModifiers = { new ActionModifier() },
                StringParameter = "WrathOfTheStorm",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                UsablePower = usablePower,
                TargetCharacters = { attacker }
            };

            var count = actionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestSpendBundlePower(actionParams);

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);
        }
    }

    private sealed class CustomBehaviorDestructiveWrath(FeatureDefinitionPower powerDestructiveWrath)
        : IForceMaxDamageTypeDependent, IModifyAdditionalDamage, IActionFinishedByMe,
            IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy
    {
        private bool _isValid;

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (!_isValid || action is not (CharacterActionAttack or CharacterActionMagicEffect))
            {
                yield break;
            }

            _isValid = false;

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerDestructiveWrath, rulesetAttacker);

            rulesetAttacker.UsePower(usablePower);
            rulesetAttacker.LogCharacterUsedPower(powerDestructiveWrath);
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
            Validate(attacker.RulesetCharacter, actualEffectForms);

            yield break;
        }

        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm additionalDamageForm)
        {
            var damageType = GetAdditionalDamageType(attacker, additionalDamageForm, featureDefinitionAdditionalDamage);

            _isValid = damageType is DamageTypeLightning or DamageTypeThunder;
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
            Validate(attacker.RulesetCharacter, actualEffectForms);

            yield break;
        }

        private void Validate(
            RulesetCharacter rulesetAttacker,
            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
            List<EffectForm> actualEffectForms)
        {
            var usablePower = PowerProvider.Get(powerDestructiveWrath, rulesetAttacker);

            _isValid =
                rulesetAttacker.GetRemainingUsesOfPower(usablePower) > 0 &&
                rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle) &&
                actualEffectForms.Any(x =>
                    x.FormType == EffectForm.EffectFormType.Damage &&
                    x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder);
        }
    }

    private sealed class CustomBehaviorThunderousStrike
        : IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy, IModifyAdditionalDamage
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
            MaybeAddPushForm(attacker, actualEffectForms);

            yield break;
        }

        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm additionalDamageForm)
        {
            var damageType = GetAdditionalDamageType(attacker, additionalDamageForm, featureDefinitionAdditionalDamage);

            if (damageType is DamageTypeLightning or DamageTypeThunder)
            {
                actualEffectForms.Add(PushForm);
            }
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
            MaybeAddPushForm(attacker, actualEffectForms);

            yield break;
        }

        private static void MaybeAddPushForm(
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker,
            // ReSharper disable once SuggestBaseTypeForParameter
            List<EffectForm> actualEffectForms)
        {
            if (actualEffectForms
                    .Any(x => x.FormType == EffectForm.EffectFormType.Damage &&
                              x.DamageForm.DamageType is DamageTypeLightning or DamageTypeThunder) &&
                attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle))
            {
                actualEffectForms.Add(PushForm);
            }
        }
    }
}
