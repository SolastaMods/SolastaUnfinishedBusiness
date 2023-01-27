using System.Collections.Generic;
using JetBrains.Annotations;
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
        var featCrusherStr = BuildCrusherStr();
        var featCrusherCon = BuildCrusherCon();
        var featDefensiveDuelist = BuildDefensiveDuelist();
        var featPiercerDex = BuildPiercerDex();
        var featPiercerStr = BuildPiercerStr();
        var featPowerAttack = BuildPowerAttack();
        var featRecklessAttack = BuildRecklessAttack();
        var featSavageAttack = BuildSavageAttack();
        var featSlasherStr = BuildSlasherStr();
        var featSlasherDex = BuildSlasherDex();

        feats.AddRange(
            featDefensiveDuelist,
            featCrusherStr,
            featCrusherCon,
            featPiercerDex,
            featPiercerStr,
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featSlasherDex,
            featSlasherStr);

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
            featPowerAttack,
            featRecklessAttack,
            featSavageAttack,
            featGroupCrusher,
            FeatGroupPiercer,
            featGroupSlasher);
    }

    private static FeatDefinition BuildDefensiveDuelist()
    {
        const string NAME = "FeatDefensiveDuelist";

        var conditionDefensiveDuelist = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{NAME}")
                .SetGuiPresentation($"Power{NAME}", Category.Feature)
                .SetModifier(
                    FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus,
                    AttributeDefinitions.ArmorClass)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerDefensiveDuelist = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentationNoContent(true)
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
            .SetGuiPresentation("FeatPowerAttack", Category.Feat,
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
            .SetGuiPresentation(Category.Feat,
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

                attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
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
}
