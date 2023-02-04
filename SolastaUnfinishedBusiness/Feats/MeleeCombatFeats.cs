using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static RuleDefinitions.RollContext;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class MeleeCombatFeats
{
    private const string Slasher = "Slasher";
    private const string Piercer = "Piercer";
    private const string Crusher = "Crusher";

    private static readonly FeatureDefinitionPower PowerFeatCrusherHit = FeatureDefinitionPowerBuilder
        .Create("PowerFeatCrusherHit")
        .SetGuiPresentationNoContent(true)
        .SetUsesFixed(ActivationTime.OnAttackHitMelee, RechargeRate.TurnStart)
        .SetCustomSubFeatures(
            new RestrictReactionAttackMode((mode, _, _) => ValidatorsWeapon.IsBludgeoningMeleeOrUnarmed(mode)))
        .SetShowCasting(false)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Enemy, RangeType.Self, 1, TargetType.IndividualsUnique)
            .SetDurationData(DurationType.Instantaneous)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                .Build())
            .Build())
        .AddToDB();

    private static readonly FeatureDefinition FeatureFeatCrusher = FeatureDefinitionAdditionalDamageBuilder
        .Create("FeatureFeatCrusher")
        .SetGuiPresentationNoContent(true)
        .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
        .SetDamageDice(DieType.D1, 0)
        .SetNotificationTag(Crusher)
        .SetCustomSubFeatures(
            new AfterAttackEffectFeatCrusher(
                ConditionDefinitionBuilder
                    .Create("ConditionFeatCrusherCriticalHit")
                    .SetGuiPresentation("FeatCrusherStr", Category.Feat)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetPossessive()
                    .SetFeatures(
                        FeatureDefinitionCombatAffinityBuilder
                            .Create("CombatAffinityFeatCrusher")
                            .SetGuiPresentation("ConditionFeatCrusherCriticalHit", Category.Condition)
                            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
                            .AddToDB())
                    .AddToDB(),
                DamageTypeBludgeoning))
        .AddToDB();

    private static readonly FeatureDefinition FeatureFeatPiercer = FeatureDefinitionBuilder
        .Create("FeatureFeatPiercer")
        .SetGuiPresentationNoContent(true)
        .SetCustomSubFeatures(
            new BeforeAttackEffectFeatPiercer(ConditionDefinitionBuilder
                    .Create("ConditionFeatPiercerNonMagic")
                    .SetGuiPresentation(Category.Condition)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetPossessive()
                    .SetSpecialInterruptions(ConditionInterruption.Attacked)
                    .SetFeatures(
                        FeatureDefinitionDieRollModifierBuilder
                            .Create("DieRollModifierFeatPiercerNonMagic")
                            .SetGuiPresentation("ConditionFeatPiercerNonMagic", Category.Condition)
                            .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatPiercerReroll")
                            .AddToDB())
                    .AddToDB(),
                DamageTypePiercing),
            new CustomAdditionalDamageFeatPiercer(
                FeatureDefinitionAdditionalDamageBuilder
                    .Create("AdditionalDamageFeatPiercer")
                    .SetGuiPresentation(Category.Feature)
                    .SetNotificationTag(Piercer)
                    .SetDamageValueDetermination(AdditionalDamageValueDetermination.SameAsBaseWeaponDie)
                    .SetIgnoreCriticalDoubleDice(true)
                    .AddToDB(),
                DamageTypePiercing))
        .AddToDB();

    private static readonly FeatureDefinition FeatureFeatSlasher = FeatureDefinitionBuilder
        .Create("FeatureFeatSlasher")
        .SetGuiPresentationNoContent(true)
        .SetCustomSubFeatures(
            new AfterAttackEffectFeatSlasher(
                ConditionDefinitionBuilder
                    .Create("ConditionFeatSlasherHit")
                    .SetGuiPresentation(Category.Condition)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetPossessive()
                    .SetFeatures(
                        FeatureDefinitionMovementAffinityBuilder
                            .Create("MovementAffinityFeatSlasher")
                            .SetGuiPresentation("ConditionFeatSlasherHit", Category.Condition)
                            .SetBaseSpeedAdditiveModifier(-2)
                            .AddToDB())
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create("ConditionFeatSlasherCriticalHit")
                    .SetGuiPresentation(Category.Condition)
                    .SetConditionType(ConditionType.Detrimental)
                    .SetSpecialDuration(DurationType.Round, 1)
                    .SetPossessive()
                    .SetFeatures(
                        FeatureDefinitionCombatAffinityBuilder
                            .Create("CombatAffinityFeatSlasher")
                            .SetGuiPresentation("ConditionFeatSlasherCriticalHit", Category.Condition)
                            .SetMyAttackAdvantage(AdvantageType.Disadvantage)
                            .AddToDB())
                    .AddToDB(),
                DamageTypeSlashing))
        .AddToDB();

    internal static FeatDefinition FeatGroupPiercer { get; private set; }

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featBladeMastery = BuildBladeMastery();
        var featCrusherStr = BuildCrusherStr();
        var featCrusherCon = BuildCrusherCon();
        var featDefensiveDuelist = BuildDefensiveDuelist();
        var featFellHanded = BuildFellHanded();
        var featPiercerDex = BuildPiercerDex();
        var featPiercerStr = BuildPiercerStr();
        var featPowerAttack = BuildPowerAttack();
        var featRecklessAttack = BuildRecklessAttack();
        var featSavageAttack = BuildSavageAttack();
        var featSlasherStr = BuildSlasherStr();
        var featSlasherDex = BuildSlasherDex();
        var featSpearMastery = BuildSpearMastery();

        feats.AddRange(
            featBladeMastery,
            featCrusherStr,
            featCrusherCon,
            featDefensiveDuelist,
            featFellHanded,
            featPiercerDex,
            featPiercerStr,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSlasherDex,
            featSlasherStr,
            featSpearMastery);

        var featGroupCrusher = GroupFeats.MakeGroup("FeatGroupCrusher", Crusher,
            featCrusherStr,
            featCrusherCon);

        FeatGroupPiercer = GroupFeats.MakeGroup("FeatGroupPiercer", Piercer,
            featPiercerDex,
            featPiercerStr);

        var featGroupSlasher = GroupFeats.MakeGroup("FeatGroupSlasher", Slasher,
            featSlasherDex,
            featSlasherStr);

        GroupFeats.MakeGroup("FeatGroupMeleeCombat", null,
            FeatDefinitions.CloakAndDagger,
            FeatDefinitions.DauntingPush,
            FeatDefinitions.DistractingGambit,
            FeatDefinitions.TripAttack,
            featBladeMastery,
            featFellHanded,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSpearMastery,
            featGroupCrusher,
            FeatGroupPiercer,
            featGroupSlasher);
    }

    private static FeatDefinition BuildBladeMastery()
    {
        const string NAME = "FeatBladeMastery";

        var weaponTypes = new[] { ShortswordType, LongswordType, ScimitarType, RapierType, GreatswordType };

        var conditionBladeMastery = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{NAME}")
                .SetGuiPresentationNoContent(true)
                .SetModifier(
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    AttributeDefinitions.ArmorClass,
                    1)
                .AddToDB())
            .AddToDB();

        var powerBladeMastery = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            conditionBladeMastery,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true)
                        .Build())
                    .Build())
            .SetCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.MainHandHasWeaponType(weaponTypes)))
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerBladeMastery)
            .SetCustomSubFeatures(
                new OnComputeAttackModifierFeatBladeMastery(weaponTypes),
                new ModifyAttackModeForWeaponTypeFilter($"Feature/&ModifyAttackMode{NAME}Title", weaponTypes))
            .AddToDB();
    }

    private static FeatDefinition BuildDefensiveDuelist()
    {
        const string NAME = "FeatDefensiveDuelist";

        var conditionDefensiveDuelist = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{NAME}")
                .SetGuiPresentationNoContent(true)
                .SetModifier(
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus,
                    AttributeDefinitions.ArmorClass)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerDefensiveDuelist = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Feat)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            conditionDefensiveDuelist,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true)
                        .Build())
                    .Build())
            .SetCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.MainHandIsFinesseWeapon))
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerDefensiveDuelist)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildFellHanded()
    {
        const string NAME = "FeatFellHanded";

        var weaponTypes = new[] { BattleaxeType, GreataxeType, HandaxeType, MaulType, WarhammerType };

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetCustomSubFeatures(
                new AfterAttackEffectFeatFellHanded(weaponTypes),
                new ModifyAttackModeForWeaponTypeFilter(
                    $"Feature/&ModifyAttackMode{NAME}Title", weaponTypes))
            .AddToDB();
    }

    private static FeatDefinition BuildPowerAttack()
    {
        var concentrationProvider = new StopPowerConcentrationProvider("PowerAttack",
            "Tooltip/&PowerAttackConcentration",
            Sprites.GetSprite("PowerAttackConcentrationIcon", Resources.PowerAttackConcentrationIcon, 64, 64));

        var conditionPowerAttackTrigger = ConditionDefinitionBuilder
            .Create("ConditionPowerAttackTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("TriggerFeaturePowerAttack")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(concentrationProvider)
                .AddToDB())
            .AddToDB();

        var conditionPowerAttack = ConditionDefinitionBuilder
            .Create("ConditionPowerAttack")
            .SetGuiPresentation("FeatPowerAttack", Category.Feat, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("ModifyAttackModeForWeaponFeatPowerAttack")
                    .SetGuiPresentation("FeatPowerAttack", Category.Feat)
                    .SetCustomSubFeatures(new ModifyAttackModeForWeaponFeatPowerAttack())
                    .AddToDB())
            .AddToDB();

        var powerAttack = FeatureDefinitionPowerBuilder
            .Create("PowerAttack")
            .SetGuiPresentation("Feat/&FeatPowerAttackTitle",
                Gui.Format("Feat/&FeatPowerAttackDescription", Main.Settings.DeadEyeAndPowerAttackBaseValue.ToString()),
                Sprites.GetSprite("PowerAttackIcon", Resources.PowerAttackIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttackTrigger, ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerAttack);

        var powerTurnOffPowerAttack = FeatureDefinitionPowerBuilder
            .Create("PowerTurnOffPowerAttack")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetDurationData(DurationType.Round, 1)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttackTrigger, ConditionForm.ConditionOperation.Remove)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(conditionPowerAttack, ConditionForm.ConditionOperation.Remove)
                        .Build())
                .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffPowerAttack);
        concentrationProvider.StopPower = powerTurnOffPowerAttack;

        return FeatDefinitionBuilder
            .Create("FeatPowerAttack")
            .SetGuiPresentation("Feat/&FeatPowerAttackTitle",
                Gui.Format("Feat/&FeatPowerAttackDescription", Main.Settings.DeadEyeAndPowerAttackBaseValue.ToString()))
            .SetFeatures(
                powerAttack,
                powerTurnOffPowerAttack
            )
            .AddToDB();
    }

    private static FeatDefinition BuildRecklessAttack()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatRecklessAttack")
            .SetGuiPresentation("RecklessAttack", Category.Action)
            .SetFeatures(ActionAffinityBarbarianRecklessAttack)
            .SetValidators(ValidatorsFeat.ValidateNotClass(CharacterClassDefinitions.Barbarian))
            .AddToDB();
    }

    private static FeatDefinition BuildSavageAttack()
    {
        return FeatDefinitionBuilder
            .Create("FeatSavageAttack")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageAttackNonMagic")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackReroll")
                    .AddToDB(),
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageAttackMagic")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(MagicDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackReroll")
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildPiercerDex()
    {
        return FeatDefinitionBuilder
            .Create("FeatPiercerDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Misaye,
                FeatureFeatPiercer)
            .SetFeatFamily(Piercer)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildPiercerStr()
    {
        return FeatDefinitionBuilder
            .Create("FeatPiercerStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Einar,
                FeatureFeatPiercer)
            .SetFeatFamily(Piercer)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildCrusherStr()
    {
        return FeatDefinitionBuilder
            .Create("FeatCrusherStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Einar,
                PowerFeatCrusherHit,
                FeatureFeatCrusher)
            .SetFeatFamily(Crusher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildCrusherCon()
    {
        return FeatDefinitionBuilder
            .Create("FeatCrusherCon")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Arun,
                PowerFeatCrusherHit,
                FeatureFeatCrusher)
            .SetFeatFamily(Crusher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildSlasherDex()
    {
        return FeatDefinitionBuilder
            .Create("FeatSlasherDex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Misaye,
                FeatureFeatSlasher)
            .SetFeatFamily(Slasher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildSlasherStr()
    {
        return FeatDefinitionBuilder
            .Create("FeatSlasherStr")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Einar,
                FeatureFeatSlasher)
            .SetFeatFamily(Slasher)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Strength, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildSpearMastery()
    {
        const string NAME = "FeatSpearMastery";

        var weaponTypes = new[] { SpearType };

        var conditionFeatSpearMasteryReach = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Reach")
            .SetGuiPresentation($"Power{NAME}Reach", Category.Feature)
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        var powerFeatSpearMasteryReach = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Reach")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite($"Power{NAME}Reach", Resources.SpearMasteryReach, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            conditionFeatSpearMasteryReach,
                            ConditionForm.ConditionOperation.Add,
                            true,
                            true)
                        .Build())
                    .Build())
            .AddToDB();

        var conditionFeatSpearMasteryCharge = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Charge")
            .SetGuiPresentation($"Power{NAME}Charge", Category.Feature)
            .AddToDB();

        var powerFeatSpearMasteryCharge = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}Charge")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite($"Power{NAME}Charge", Resources.SpearMasteryCharge, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetEffectForms(EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            conditionFeatSpearMasteryCharge,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                    .Build())
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerFeatSpearMasteryReach,
                powerFeatSpearMasteryCharge)
            .SetCustomSubFeatures(
                new AfterAttackEffectFeatSpearMastery(conditionFeatSpearMasteryCharge, weaponTypes),
                new ModifyAttackModeForWeaponFeatSpearMastery(
                    $"Feature/&ModifyAttackMode{NAME}Title", conditionFeatSpearMasteryReach, weaponTypes))
            .AddToDB();
    }

    //
    // HELPERS
    //

    private sealed class ModifyAttackModeForWeaponFeatPowerAttack : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsMelee(attackMode) && !ValidatorsWeapon.IsUnarmedWeapon(character, attackMode))
            {
                return;
            }

            SrdAndHouseRulesContext.ModifyAttackModeAndDamage(character, "Feat/&FeatPowerAttackTitle", attackMode);
        }
    }

    private sealed class BeforeAttackEffectFeatPiercer : IBeforeAttackEffect
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly string _damageType;

        internal BeforeAttackEffectFeatPiercer(ConditionDefinition conditionDefinition, string damageType)
        {
            _conditionDefinition = conditionDefinition;
            _damageType = damageType;
        }

        public void BeforeOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null || damage.DamageType != _damageType)
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                attacker.RulesetCharacter.Guid,
                _conditionDefinition,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    private sealed class CustomAdditionalDamageFeatPiercer : CustomAdditionalDamage
    {
        private readonly string _damageType;

        public CustomAdditionalDamageFeatPiercer(IAdditionalDamageProvider provider, string damageType) : base(provider)
        {
            _damageType = damageType;
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

            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            return criticalHit && damage != null && damage.DamageType == _damageType;
        }
    }

    private sealed class AfterAttackEffectFeatCrusher : IAfterAttackEffect
    {
        private readonly ConditionDefinition _criticalConditionDefinition;
        private readonly string _damageType;

        internal AfterAttackEffectFeatCrusher(
            ConditionDefinition criticalConditionDefinition,
            string damageType)
        {
            _criticalConditionDefinition = criticalConditionDefinition;
            _damageType = damageType;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null || damage.DamageType != _damageType)
            {
                return;
            }

            if (outcome is not RollOutcome.CriticalSuccess)
            {
                return;
            }

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                defender.RulesetCharacter.Guid,
                _criticalConditionDefinition,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            defender.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    private sealed class AfterAttackEffectFeatSlasher : IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly ConditionDefinition _criticalConditionDefinition;
        private readonly string _damageType;

        internal AfterAttackEffectFeatSlasher(
            ConditionDefinition conditionDefinition,
            ConditionDefinition criticalConditionDefinition,
            string damageType)
        {
            _conditionDefinition = conditionDefinition;
            _criticalConditionDefinition = criticalConditionDefinition;
            _damageType = damageType;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null || damage.DamageType != _damageType)
            {
                return;
            }

            RulesetCondition rulesetCondition;

            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                rulesetCondition = RulesetCondition.CreateActiveCondition(
                    attacker.RulesetCharacter.Guid,
                    _conditionDefinition,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    attacker.RulesetCharacter.Guid,
                    attacker.RulesetCharacter.CurrentFaction.Name);

                defender.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
            }

            if (outcome is not RollOutcome.CriticalSuccess)
            {
                return;
            }

            rulesetCondition = RulesetCondition.CreateActiveCondition(
                defender.RulesetCharacter.Guid,
                _criticalConditionDefinition,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            defender.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    private sealed class AfterAttackEffectFeatFellHanded : IAfterAttackEffect
    {
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public AfterAttackEffectFeatFellHanded(params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            switch (attackModifier.AttackAdvantageTrend)
            {
                case >= 0 when outcome is RollOutcome.Success or RollOutcome.CriticalSuccess:
                    var lowerRoll = Math.Min(Global.FirstAttackRoll, Global.SecondAttackRoll);
                    var defenderAc = defender.RulesetCharacter.GetAttribute(AttributeDefinitions.ArmorClass)
                        .CurrentValue;

                    if (lowerRoll + attackMode.ToHitBonus >= defenderAc)
                    {
                        var console = Gui.Game.GameConsole;
                        var entry = new GameConsoleEntry("Feedback/&FeatFellHanded", console.consoleTableDefinition);

                        console.AddCharacterEntry(defender.RulesetActor, entry);
                        console.AddEntry(entry);

                        var rulesetCondition = RulesetCondition.CreateActiveCondition(
                            defender.RulesetCharacter.Guid,
                            ConditionDefinitions.ConditionProne,
                            DurationType.Round,
                            1,
                            TurnOccurenceType.StartOfTurn,
                            attacker.RulesetCharacter.Guid,
                            attacker.RulesetCharacter.CurrentFaction.Name);

                        defender.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                            rulesetCondition);
                    }

                    break;
                case < 0 when outcome is RollOutcome.Failure or RollOutcome.CriticalFailure:
                    var strength = attacker.RulesetCharacter.GetAttribute(AttributeDefinitions.Strength)
                        .CurrentValue;
                    var strengthMod = AttributeDefinitions.ComputeAbilityScoreModifier(strength);

                    if (strengthMod > 0)
                    {
                        defender.RulesetCharacter.SustainDamage(strengthMod, DamageTypeBludgeoning, false,
                            attacker.Guid, null, out _);
                    }

                    break;
            }
        }
    }

    private sealed class ModifyAttackModeForWeaponTypeFilter : IModifyAttackModeForWeapon
    {
        private readonly string _sourceName;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public ModifyAttackModeForWeaponTypeFilter(string sourceName,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _sourceName = sourceName;
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (attackMode.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            attackMode.ToHitBonus += 1;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(1, FeatureSourceType.CharacterFeature, _sourceName, null));
        }
    }

    private sealed class OnComputeAttackModifierFeatBladeMastery : IOnComputeAttackModifier
    {
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public OnComputeAttackModifierFeatBladeMastery(params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void ComputeAttackModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            RulesetAttackMode attackMode,
            ref ActionModifier attackModifier)
        {
            if (attackMode == null || defender == null ||
                attackMode.actionType != ActionDefinitions.ActionType.Reaction)
            {
                return;
            }

            if (!ValidatorsWeapon.IsWeaponType(myself.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand),
                    _weaponTypeDefinition))
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(new TrendInfo(1,
                FeatureSourceType.CharacterFeature, "Feature/&ModifyAttackModeFeatBladeMasteryTitle",
                null));
        }
    }

    private sealed class AfterAttackEffectFeatSpearMastery : IAfterAttackEffect
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public AfterAttackEffectFeatSpearMastery(
            ConditionDefinition conditionDefinition,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _conditionDefinition = conditionDefinition;
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            var damageForm = attackMode.EffectDescription.FindFirstDamageForm();

            if (damageForm == null)
            {
                return;
            }

            /* When you use a spear, its damage die changes from a d6
            to a d8, and from a d8 to a d10 when wielded with two
            hands. */

            damageForm.dieType = damageForm.DieType switch
            {
                DieType.D6 => DieType.D8,
                DieType.D8 => DieType.D10,
                _ => damageForm.dieType
            };

            /* You can set your spear to receive a charge. As a bonus
            action, choose a creature you can see that is at least 20
            feet away from you. If that creatures moves within your
            spear’s reach on its next turn, you can make a melee
            attack against it with your spear as a reaction. If the attack
            hits, the target takes an extra 1d8 piercing damage, or an
            extra 1d10 piercing damage if you wield the spear with
            two hands. */

            if (attackMode.actionType != ActionDefinitions.ActionType.Reaction ||
                defender.RulesetCharacter.AllConditions.All(x => x.ConditionDefinition != _conditionDefinition))
            {
                return;
            }

            damageForm.diceNumber += 1;
        }
    }

    private sealed class ModifyAttackModeForWeaponFeatSpearMastery : IModifyAttackModeForWeapon
    {
        private readonly ConditionDefinition _conditionDefinition;
        private readonly string _sourceName;
        private readonly List<WeaponTypeDefinition> _weaponTypeDefinition = new();

        public ModifyAttackModeForWeaponFeatSpearMastery(
            string sourceName,
            ConditionDefinition conditionDefinition,
            params WeaponTypeDefinition[] weaponTypeDefinition)
        {
            _sourceName = sourceName;
            _conditionDefinition = conditionDefinition;
            _weaponTypeDefinition.AddRange(weaponTypeDefinition);
        }

        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (attackMode.sourceDefinition is not ItemDefinition { IsWeapon: true } sourceDefinition ||
                !_weaponTypeDefinition.Contains(sourceDefinition.WeaponDescription.WeaponTypeDefinition))
            {
                return;
            }

            /* You gain a +1 bonus to attack rolls you make with a spear */

            attackMode.ToHitBonus += 1;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(1, FeatureSourceType.CharacterFeature, _sourceName, null));

            /* As a bonus action on your turn, you can increase your
            reach with a spear by 5 feet for the rest of your turn */

            if (character.AllConditions.All(x => x.ConditionDefinition != _conditionDefinition))
            {
                return;
            }

            attackMode.reach = true;
            attackMode.reachRange = 2;
        }
    }
}
