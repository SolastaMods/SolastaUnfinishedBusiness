using System.Collections.Generic;
using System.Linq;
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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalActions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class OtherFeats
{
    private const string SavageAttackerFeat = "FeatSavageAttacker";
    private const string PolearmExpertFeat = "FeatPolearmExpert";
    private const string RangedExpertFeat = "FeatRangedExpert";
    private const string RecklessAttackFeat = "FeatRecklessAttack";

    internal const string MagicAffinityWarcaster = "MagicAffinityFeatWarCaster";

    internal const string SentinelFeat = "FeatSentinel";
    internal const string WarcasterFeat = "FeatWarCaster";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featDeadEye = BuildDeadEye();
        var featDualFlurry = BuildDualFlurry();
        var featDualWeaponDefense = BuildDualWeaponDefense();
        var featMarksman = BuildMarksman();
        var featRangedExpert = BuildRangedExpert();
        var featTough = BuildTough();
        var featWarCaster = BuildWarcaster();

        feats.AddRange(BuildMetamagic());

        feats.Add(BuildPolearmExpert());
        feats.Add(BuildPowerAttack());
        feats.Add(BuildRecklessAttack());
        feats.Add(BuildSavageAttacker());
        feats.Add(BuildSentinel());
        feats.Add(BuildTorchbearer());

        feats.Add(featDeadEye);
        feats.Add(featDualFlurry);
        feats.Add(featDualWeaponDefense);
        feats.Add(featMarksman);
        feats.Add(featRangedExpert);
        feats.Add(featTough);
        feats.Add(featWarCaster);

        GroupFeats.MakeGroup("FeatGroupBodyResilience", null,
            FeatDefinitions.BadlandsMarauder,
            FeatDefinitions.Enduring_Body,
            FeatDefinitions.FocusedSleeper,
            FeatDefinitions.HardToKill,
            FeatDefinitions.Hauler,
            FeatDefinitions.Robust,
            featTough);

        GroupFeats.MakeGroup("FeatGroupRangedCombat", null,
            FeatDefinitions.TakeAim,
            FeatDefinitions.UncannyAccuracy,
            CraftyFeats.FeatCraftyFletcher,
            featDeadEye,
            featMarksman,
            featRangedExpert);

        GroupFeats.MakeGroup("FeatGroupSpellCombat", null,
            FeatDefinitions.FlawlessConcentration,
            FeatDefinitions.PowerfulCantrip,
            featWarCaster);

        GroupFeats.MakeGroup("FeatGroupTwoWeaponCombat", null,
            FeatDefinitions.Ambidextrous,
            FeatDefinitions.TwinBlade,
            featDualFlurry,
            featDualWeaponDefense);
    }

    private static FeatDefinition BuildDeadEye()
    {
        var conditionDeadeye = ConditionDefinitionBuilder
            .Create("ConditionDeadeye")
            .SetGuiPresentation("FeatDeadeye", Category.Feat)
            .SetDuration(DurationType.Round, 1)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("ModifyAttackModeForWeaponFeatDeadeye")
                    .SetGuiPresentation("FeatDeadeye", Category.Feat)
                    .SetCustomSubFeatures(new ModifyDeadeyeAttackPower())
                    .AddToDB())
            .AddToDB();

        var concentrationProvider = new StopPowerConcentrationProvider(
            "Deadeye",
            "Tooltip/&DeadeyeConcentration",
            Sprites.GetSprite("DeadeyeConcentrationIcon", Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionDeadeyeTrigger = ConditionDefinitionBuilder
            .Create("ConditionDeadeyeTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetDuration(DurationType.Permanent)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("TriggerFeatureDeadeye")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        var powerDeadeye = FeatureDefinitionPowerBuilder
            .Create("PowerDeadeye")
            .SetGuiPresentation("FeatDeadeye", Category.Feat,
                Sprites.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeyeTrigger, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerDeadeye);

        var powerTurnOffDeadeye = FeatureDefinitionPowerBuilder
            .Create("PowerTurnOffDeadeye")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeyeTrigger, ConditionForm.ConditionOperation.Remove)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffDeadeye);
        concentrationProvider.StopPower = powerTurnOffDeadeye;

        return FeatDefinitionBuilder
            .Create("FeatDeadeye")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerDeadeye,
                powerTurnOffDeadeye,
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityDeadeyeIgnoreDefender")
                    .SetGuiPresentation("FeatDeadeye", Category.Feat)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildDualFlurry()
    {
        var conditionDualFlurryApply = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryApply")
            .SetGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 0, false)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .AddToDB();

        var conditionDualFlurryGrant = ConditionDefinitionBuilder
            .Create("ConditionDualFlurryGrant")
            .SetGuiPresentation(Category.Condition)
            .SetDuration(DurationType.Round, 0, false)
            .SetTurnOccurence(TurnOccurenceType.EndOfTurn)
            .SetPossessive()
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetConditionType(ConditionType.Beneficial)
            .SetFeatures(
                FeatureDefinitionAdditionalActionBuilder
                    .Create(AdditionalActionSurgedMain, "AdditionalActionDualFlurry")
                    .SetGuiPresentationNoContent(true)
                    .SetActionType(ActionDefinitions.ActionType.Bonus)
                    .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                    .AddToDB())
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatDualFlurry")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("OnAttackDamageEffectFeatDualFlurry")
                    .SetGuiPresentation("FeatDualFlurry", Category.Feat)
                    .SetCustomSubFeatures(
                        new OnAttackHitEffectFeatDualFlurry(conditionDualFlurryGrant, conditionDualFlurryApply))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildDualWeaponDefense()
    {
        return FeatDefinitionBuilder
            .Create("FeatDualWeaponDefense")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierSwiftBladeBladeDance)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static IEnumerable<FeatDefinition> BuildMetamagic()
    {
        // Metamagic
        var attributeModifierSorcererSorceryPointsBonus2 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                AttributeDefinitions.SorceryPoints, 2)
            .AddToDB();

        var metaMagicFeats = new List<FeatDefinition>();
        var dbMetamagicOptionDefinition = DatabaseRepository.GetDatabase<MetamagicOptionDefinition>();

        metaMagicFeats.SetRange(dbMetamagicOptionDefinition
            .Select(metamagicOptionDefinition => FeatDefinitionBuilder
                .Create($"FeatAdept{metamagicOptionDefinition.Name}")
                .SetGuiPresentation(
                    Gui.Format("Feat/&FeatAdeptMetamagicTitle", metamagicOptionDefinition.FormatTitle()),
                    Gui.Format("Feat/&FeatAdeptMetamagicDescription", metamagicOptionDefinition.FormatTitle()))
                .SetFeatures(
                    AttributeModifierCreed_Of_Solasta,
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus2,
                    FeatureDefinitionBuilder
                        .Create($"CustomCodeFeatAdept{metamagicOptionDefinition.Name}")
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new CustomCodeFeatMetamagicAdept(metamagicOptionDefinition))
                        .AddToDB())
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .AddToDB()));

        GroupFeats.MakeGroup("FeatGroupMetamagic", null, metaMagicFeats);

        return metaMagicFeats;
    }

    private static FeatDefinition BuildMarksman()
    {
        return FeatDefinitionBuilder
            .Create("FeatMarksman")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(ActionAffinityMarksmanReactionShot)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildPolearmExpert()
    {
        return FeatDefinitionBuilder
            .Create(PolearmExpertFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureFeatPolearm")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new CanMakeAoOOnReachEntered(ValidatorsCharacter.HasPolearm),
                    new AddPolearmFollowupAttack())
                .AddToDB())
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
            .SetDuration(DurationType.Permanent)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("TriggerFeaturePowerAttack")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(concentrationProvider)
                .AddToDB())
            .AddToDB();

        var conditionPowerAttack = ConditionDefinitionBuilder
            .Create("ConditionPowerAttack")
            .SetGuiPresentation("PowerAttack", Category.Feature, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetAllowMultipleInstances(false)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("ModifyAttackModeForWeaponFeatPowerAttack")
                    .SetGuiPresentation("PowerAttack", Category.Feature)
                    .SetCustomSubFeatures(new ModifyPowerAttackPower())
                    .AddToDB())
            .SetDuration(DurationType.Round, 1)
            .AddToDB();

        var powerAttack = FeatureDefinitionPowerBuilder
            .Create("PowerAttack")
            .SetGuiPresentation("FeatPowerAttack", Category.Feat,
                Sprites.GetSprite("PowerAttackIcon", Resources.PowerAttackIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
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
                .SetTargetingData(Side.Ally, RangeType.Self, 1, TargetType.Self)
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
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerAttack,
                powerTurnOffPowerAttack
            )
            .AddToDB();
    }

    private static FeatDefinition BuildRangedExpert()
    {
        return FeatDefinitionBuilder
            .Create(RangedExpertFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureFeatRangedExpert")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new RangedAttackInMeleeDisadvantageRemover(),
                    new AddExtraRangedAttack(IsOneHandedRanged, ActionDefinitions.ActionType.Bonus,
                        ValidatorsCharacter.HasAttacked))
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRecklessAttack()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create(RecklessAttackFeat)
            .SetGuiPresentation("RecklessAttack", Category.Action)
            .SetFeatures(ActionAffinityBarbarianRecklessAttack)
            .SetValidators(ValidatorsFeat.ValidateNotClass(CharacterClassDefinitions.Barbarian))
            .AddToDB();
    }

    private static FeatDefinition BuildSavageAttacker()
    {
        return FeatDefinitionBuilder
            .Create(SavageAttackerFeat)
            .SetFeatures(
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageAttacker")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(AttackDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackerReroll")
                    .AddToDB(),
                FeatureDefinitionDieRollModifierBuilder
                    .Create("DieRollModifierFeatSavageMagicAttacker")
                    .SetGuiPresentationNoContent(true)
                    .SetModifiers(MagicDamageValueRoll, 1, 1, 1, "Feat/&FeatSavageAttackerReroll")
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatDefinition BuildSentinel()
    {
        return FeatDefinitionBuilder
            .Create(SentinelFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("OnAttackHitEffectFeatSentinel")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    AttacksOfOpportunity.CanIgnoreDisengage,
                    AttacksOfOpportunity.SentinelFeatMarker,
                    new OnAttackHitEffectFeatSentinel(CustomConditions.StopMovement))
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildTorchbearer()
    {
        return FeatDefinitionBuilder
            .Create("FeatTorchbearer")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionPowerBuilder
                .Create("PowerTorchbearer")
                .SetGuiPresentation(Category.Feature)
                .SetUsesFixed(ActivationTime.BonusAction)
                .SetEffectDescription(EffectDescriptionBuilder
                    .Create(SpellDefinitions.Fireball.EffectDescription)
                    .SetCanBePlacedOnCharacter(false)
                    .SetCreatedByCharacter()
                    .SetDurationData(DurationType.Round, 3)
                    .SetSpeed(SpeedType.Instant, 11f)
                    .SetTargetingData(Side.Enemy, RangeType.Touch, 30, TargetType.Individuals)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitions.ConditionOnFire1D4,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .SetSavingThrowData(
                        false,
                        AttributeDefinitions.Dexterity,
                        false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                        AttributeDefinitions.Dexterity,
                        15)
                    .Build())
                .SetShowCasting(false)
                .SetCustomSubFeatures(new ValidatorsPowerUse(ValidatorsCharacter.OffHandHasLightSource))
                .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildTough()
    {
        return FeatDefinitionBuilder
            .Create("FeatTough")
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatTough")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                        AttributeDefinitions.HitPointBonusPerLevel, 2)
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatDefinition BuildWarcaster()
    {
        var warcaster = FeatDefinitionBuilder
            .Create(WarcasterFeat)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionMagicAffinityBuilder
                    .Create(MagicAffinityWarcaster)
                    .SetGuiPresentation(WarcasterFeat, Category.Feat)
                    .SetCastingModifiers(0, SpellParamsModifierType.FlatValue, 0,
                        SpellParamsModifierType.None)
                    .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
                    .SetHandsFullCastingModifiers(true, true, true)
                    .AddToDB())
            .SetMustCastSpellsPrerequisite()
            .AddToDB();

        return warcaster;
    }

    //
    // HELPERS
    //

    private static bool IsOneHandedRanged(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return ValidatorsWeapon.IsRanged(weapon) && ValidatorsWeapon.IsOneHanded(weapon);
    }

    private sealed class CustomCodeFeatMetamagicAdept : IFeatureDefinitionCustomCode
    {
        public CustomCodeFeatMetamagicAdept(MetamagicOptionDefinition metamagicOption)
        {
            MetamagicOption = metamagicOption;
        }

        private MetamagicOptionDefinition MetamagicOption { get; }

        public void ApplyFeature([NotNull] RulesetCharacterHero hero, string tag)
        {
            if (hero.MetamagicFeatures.ContainsKey(MetamagicOption))
            {
                return;
            }

            hero.TrainMetaMagicOptions(new List<MetamagicOptionDefinition> { MetamagicOption });
        }
    }

    private sealed class ModifyPowerAttackPower : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (!ValidatorsWeapon.IsMelee(attackMode))
            {
                return;
            }

            var proficiency = character.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            const int TO_HIT = -3;
            var toDamage = 3 + proficiency;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT,
                FeatureSourceType.Power, "PowerAttack", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new TrendInfo(toDamage,
                FeatureSourceType.Power, "PowerAttack", null));
        }
    }

    private sealed class ModifyDeadeyeAttackPower : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            if (attackMode is not { Reach: false, Ranged: true })
            {
                return;
            }

            var proficiency = character.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
            var toHit = -proficiency;
            var toDamage = 2 * proficiency;

            attackMode.ToHitBonus += toHit;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(toHit,
                FeatureSourceType.Power, "Deadeye", null));

            damage.BonusDamage += toDamage;
            damage.DamageBonusTrends.Add(new TrendInfo(toDamage,
                FeatureSourceType.Power, "Deadeye", null));
        }
    }

    private sealed class OnAttackHitEffectFeatDualFlurry : IOnAttackHitEffect
    {
        private readonly ConditionDefinition _conditionDualFlurryApply;
        private readonly ConditionDefinition _conditionDualFlurryGrant;

        internal OnAttackHitEffectFeatDualFlurry(
            ConditionDefinition conditionDualFlurryGrant,
            ConditionDefinition conditionDualFlurryApply)
        {
            _conditionDualFlurryGrant = conditionDualFlurryGrant;
            _conditionDualFlurryApply = conditionDualFlurryApply;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (attackMode == null)
            {
                return;
            }

            var condition = attacker.RulesetCharacter.HasConditionOfType(_conditionDualFlurryApply.Name)
                ? _conditionDualFlurryGrant
                : _conditionDualFlurryApply;

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                attacker.RulesetCharacter.Guid,
                condition,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    private sealed class OnAttackHitEffectFeatSentinel : IOnAttackHitEffect
    {
        private readonly ConditionDefinition _conditionSentinelStopMovement;

        internal OnAttackHitEffectFeatSentinel(ConditionDefinition conditionSentinelStopMovement)
        {
            _conditionSentinelStopMovement = conditionSentinelStopMovement;
        }

        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (outcome != RollOutcome.Success && outcome != RollOutcome.CriticalSuccess)
            {
                return;
            }

            if (attackMode is not { ActionType: ActionDefinitions.ActionType.Reaction })
            {
                return;
            }

            if (attackMode.AttackTags.Contains(AttacksOfOpportunity.NotAoOTag))
            {
                return;
            }

            var character = defender.RulesetCharacter;

            character.AddConditionOfCategory(AttributeDefinitions.TagCombat,
                RulesetCondition.CreateActiveCondition(character.Guid,
                    _conditionSentinelStopMovement,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.StartOfTurn,
                    attacker.Guid,
                    string.Empty
                ));
        }
    }
}
