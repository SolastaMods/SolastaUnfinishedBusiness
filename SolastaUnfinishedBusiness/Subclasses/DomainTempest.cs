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

        PowerWrathOfTheStorm = FeatureDefinitionPowerBuilder
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

        PowerWrathOfTheStorm.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            ReactionResourceWrathOfTheStorm.Instance,
            new CustomBehaviorWrathOfTheStorm(PowerWrathOfTheStorm));

        var powerWrathOfTheStormLightning = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}WrathOfTheStormLightning")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, PowerWrathOfTheStorm)
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
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (5, 1), (11, 2), (17, 3))
                            .Build())
                    .SetImpactEffectParameters(LightningBolt)
                    .Build())
            // required as in a feature set
            .AddCustomSubFeatures(ClassHolder.Cleric, ModifyPowerVisibility.Hidden)
            .AddToDB();

        var powerWrathOfTheStormThunder = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{NAME}WrathOfTheStormThunder")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, PowerWrathOfTheStorm)
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
                    .SetImpactEffectParameters(Shatter)
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(
            PowerWrathOfTheStorm, false, powerWrathOfTheStormLightning, powerWrathOfTheStormThunder);

        var featureSetWrathOfTheStorm = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{NAME}WrathOfTheStorm")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(PowerWrathOfTheStorm, powerWrathOfTheStormLightning, powerWrathOfTheStormThunder)
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
            .SetOrUpdateGuiPresentation(powerDestructiveWrath.Name, Category.Feature)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.DestructiveWrathToggle)
            .SetActivatedPower(powerDestructiveWrath)
            .OverrideClassName("Toggle")
            .AddToDB();

        var actionAffinityDestructiveWrathToggle = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityDestructiveWrathToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle)
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
            .SetSpecificDamageType(DamageTypeThunder)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 8, 6)
            .SetFrequencyLimit(FeatureLimitedUsage.OnceInMyTurn)
            .SetAttackModeOnly()
            .SetImpactParticleReference(Shatter)
            //.AddCustomSubFeatures(ClassHolder.Cleric)
            .AddToDB();

        // LEVEL 17 - Stormborn

        var sprite = Sprites.GetSprite("PowerStormborn", Resources.PowerStormborn, 256, 128);

        var powerStormbornSprout = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}StormbornSprout")
            .SetGuiPresentation(Category.Feature, sprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
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
            .SetGuiPresentation(Category.Feature, sprite)
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
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(NAME, Resources.DomainTempest, 256))
            .AddFeaturesAtLevel(1,
                autoPreparedSpellsDomainNature,
                featureSetBonusProficiency,
                featureSetWrathOfTheStorm)
            .AddFeaturesAtLevel(2, featureSetDestructiveWrath)
            .AddFeaturesAtLevel(6, actionAffinityThunderousStrike)
            .AddFeaturesAtLevel(8, additionalDamageDivineStrike)
            .AddFeaturesAtLevel(10, PowerClericDivineInterventionPaladin)
            .AddFeaturesAtLevel(17, featureSetStormborn)
            .AddFeaturesAtLevel(20, Level20SubclassesContext.PowerClericDivineInterventionImprovementPaladin)
            .AddToDB();
    }

    internal static FeatureDefinitionPower PowerWrathOfTheStorm { get; private set; }

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
        : IMagicEffectFinishedOnMe, IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnMagicEffectFinishedOnMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            List<GameLocationCharacter> targets)
        {
            var rulesetEffect = action.ActionParams.RulesetEffect;

            if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield break;
            }

            yield return HandleReaction(action, defender);
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            yield return HandleReaction(action, defender);
        }

        private IEnumerator HandleReaction(CharacterAction action, GameLocationCharacter defender)
        {
            var attacker = action.ActingCharacter;
            var rulesetDefender = defender.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerWrathOfTheStorm, rulesetDefender);

            if (action.AttackRoll == 0 ||
                action.AttackRollOutcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess) ||
                !defender.CanReact() ||
                !defender.IsWithinRange(attacker, 1) ||
                !defender.CanPerceiveTarget(attacker) ||
                rulesetDefender.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            yield return defender.MyReactToSpendPowerBundle(
                usablePower,
                [attacker],
                attacker,
                "WrathOfTheStorm",
                reactionValidated: ReactionValidated);

            yield break;

            void ReactionValidated(ReactionRequestSpendBundlePower reactionRequest)
            {
                defender.SpendActionType(ActionDefinitions.ActionType.Reaction);
            }
        }
    }

    private sealed class CustomBehaviorDestructiveWrath(FeatureDefinitionPower powerDestructiveWrath)
        : IForceMaxDamageTypeDependent, IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var actingCharacter = action.ActingCharacter;
            var hasTag = actingCharacter.GetSpecialFeatureUses(powerDestructiveWrath.Name) == 1;

            actingCharacter.SetSpecialFeatureUses(powerDestructiveWrath.Name, 0);

            if (action is not (CharacterActionAttack or CharacterActionMagicEffect or CharacterActionSpendPower) ||
                !hasTag)
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter.GetEffectControllerOrSelf();
            var usablePower = PowerProvider.Get(powerDestructiveWrath, rulesetAttacker);

            rulesetAttacker.LogCharacterUsedPower(powerDestructiveWrath);
            rulesetAttacker.UsePower(usablePower);
        }

        public bool IsValid(RulesetActor rulesetActor, DamageForm damageForm)
        {
            if (rulesetActor is not RulesetCharacter rulesetCharacter)
            {
                return false;
            }

            var rulesetAttacker = rulesetCharacter.GetEffectControllerOrSelf();
            var attacker = GameLocationCharacter.GetFromActor(rulesetActor);
            var usablePower = PowerProvider.Get(powerDestructiveWrath, rulesetAttacker);
            var isValid =
                rulesetAttacker!.GetRemainingUsesOfPower(usablePower) > 0 &&
                rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.DestructiveWrathToggle) &&
                damageForm.DamageType is DamageTypeLightning or DamageTypeThunder;

            if (attacker.GetSpecialFeatureUses(powerDestructiveWrath.Name) == 1)
            {
                return isValid;
            }

            attacker.SetSpecialFeatureUses(powerDestructiveWrath.Name, isValid ? 1 : 0);

            return isValid;
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
            MaybeAddPushForm(attacker, defender.RulesetCharacter, actualEffectForms);

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
            var rulesetDefender = defender.RulesetCharacter;

            if (attacker.IsMyTurn() &&
                damageType is DamageTypeThunder &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.DragonSize &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.Gargantuan &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.Huge &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.SpiderQueenSize &&
                attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle))
            {
                actualEffectForms.TryAdd(PushForm);
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
            MaybeAddPushForm(attacker, defender.RulesetCharacter, actualEffectForms);

            yield break;
        }

        private static void MaybeAddPushForm(
            // ReSharper disable once SuggestBaseTypeForParameter
            GameLocationCharacter attacker,
            [CanBeNull] RulesetCharacter rulesetDefender,
            // ReSharper disable once SuggestBaseTypeForParameter
            List<EffectForm> actualEffectForms)
        {
            if (rulesetDefender == null) { return; }

            if (attacker.IsMyTurn() &&
                actualEffectForms
                    .Any(x =>
                        x.FormType == EffectForm.EffectFormType.Damage &&
                        x.DamageForm.DamageType is DamageTypeThunder) &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.DragonSize &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.Gargantuan &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.Huge &&
                rulesetDefender.SizeDefinition != CharacterSizeDefinitions.SpiderQueenSize &&
                attacker.RulesetCharacter.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.ThunderousStrikeToggle))
            {
                actualEffectForms.TryAdd(PushForm);
            }
        }
    }
}
