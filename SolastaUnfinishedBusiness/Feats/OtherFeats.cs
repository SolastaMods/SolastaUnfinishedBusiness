using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellListDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class OtherFeats
{
    internal const string FeatEldritchAdept = "FeatEldritchAdept";
    internal const string FeatWarCaster = "FeatWarCaster";
    internal const string MagicAffinityFeatWarCaster = "MagicAffinityFeatWarCaster";

    private const string MagicInitiate = "MagicInitiate";

    private static readonly FeatureDefinitionPower PowerFeatPoisonousSkin = FeatureDefinitionPowerBuilder
        .Create("PowerFeatPoisonousSkin")
        .SetGuiPresentation(Category.Feature)
        .SetEffectDescription(EffectDescriptionBuilder
            .Create()
            .SetSavingThrowData(false,
                AttributeDefinitions.Constitution, false, EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                AttributeDefinitions.Constitution)
            .SetEffectForms(EffectFormBuilder
                .Create()
                .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.StartOfTurn)
                .SetConditionForm(ConditionDefinitions.ConditionPoisoned, ConditionForm.ConditionOperation.Add)
                .CanSaveToCancel(TurnOccurenceType.EndOfTurn)
                .Build())
            .SetDurationData(DurationType.Minute, 1)
            .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnActivation)
            .Build())
        .AddToDB();

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featAstralArms = BuildAstralArms();
        var featEldritchAdept = BuildEldritchAdept();
        var featHealer = BuildHealer();
        var featInspiringLeader = BuildInspiringLeader();
        var featMobile = BuildMobile();
        var featMonkInitiate = BuildMonkInitiate();
        var featPickPocket = BuildPickPocket();
        var featPoisonousSkin = BuildPoisonousSkin();
        var featTough = BuildTough();
        var featWarCaster = BuildWarcaster();

        BuildMagicInitiate(feats);

        feats.AddRange(
            featAstralArms,
            featEldritchAdept,
            featHealer,
            featInspiringLeader,
            featMobile,
            featMonkInitiate,
            featPickPocket,
            featPoisonousSkin,
            featTough,
            featWarCaster);

        GroupFeats.MakeGroup("FeatGroupBodyResilience", null,
            FeatDefinitions.BadlandsMarauder,
            FeatDefinitions.BlessingOfTheElements,
            FeatDefinitions.Enduring_Body,
            FeatDefinitions.FocusedSleeper,
            FeatDefinitions.HardToKill,
            FeatDefinitions.Hauler,
            FeatDefinitions.Robust,
            featTough);

        GroupFeats.MakeGroup("FeatGroupSkills", null,
            FeatDefinitions.Manipulator,
            featHealer,
            featPickPocket);

        var group = GroupFeats.MakeGroup("FeatGroupSpellCombat", null,
            FeatDefinitions.FlawlessConcentration,
            FeatDefinitions.PowerfulCantrip,
            featWarCaster);

        group.mustCastSpellsPrerequisite = true;
    }

    private static FeatDefinition BuildAstralArms()
    {
        return FeatDefinitionBuilder
            .Create("FeatAstralArms")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                FeatureDefinitionBuilder
                    .Create("ModifyAttackModeForWeaponFeatAstralArms")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(new ModifyAttackModeForWeaponFeatAstralArms())
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildEldritchAdept()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create(FeatEldritchAdept)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPointPoolBuilder
                    .Create($"PointPool{FeatEldritchAdept}")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Invocation, 1)
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildHealer()
    {
        var spriteMedKit = Sprites.GetSprite("PowerMedKit", Resources.PowerMedKit, 128);
        var powerFeatHealerMedKit = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerMedKit")
            .SetGuiPresentation(Category.Feature, spriteMedKit)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals)
                .SetDurationData(DurationType.Instantaneous)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetHealingForm(
                        HealingComputation.Dice,
                        4,
                        DieType.D6,
                        1,
                        false,
                        HealingCap.MaximumHitPoints)
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                    .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        var spriteResuscitate = Sprites.GetSprite("PowerResuscitate", Resources.PowerResuscitate, 128);
        var powerFeatHealerResuscitate = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerResuscitate")
            .SetGuiPresentation(Category.Feature, spriteResuscitate)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.Individuals)
                .SetTargetFiltering(
                    TargetFilteringMethod.CharacterOnly,
                    TargetFilteringTag.No,
                    5,
                    DieType.D8)
                .SetDurationData(DurationType.Permanent)
                .SetRequiredCondition(ConditionDefinitions.ConditionDead)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetReviveForm(12, ReviveHitPoints.One)
                    .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        var spriteStabilize = Sprites.GetSprite("PowerStabilize", Resources.PowerStabilize, 128);
        var powerFeatHealerStabilize = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerStabilize")
            .SetGuiPresentation(Category.Feature, spriteStabilize)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(SpellDefinitions.SpareTheDying.EffectDescription)
            .AddToDB();

        var proficiencyFeatHealerMedicine = FeatureDefinitionProficiencyBuilder
            .Create("ProficiencyFeatHealerMedicine")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Medecine)
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatHealer")
            .SetGuiPresentation(Category.Feat, PowerFunctionGoodberryHealingOther)
            .SetFeatures(
                powerFeatHealerMedKit,
                powerFeatHealerResuscitate,
                powerFeatHealerStabilize,
                proficiencyFeatHealerMedicine)
            .AddToDB();
    }

    private static FeatDefinition BuildInspiringLeader()
    {
        var powerFeatInspiringLeader = FeatureDefinitionPowerBuilder
            .Create("PowerFeatInspiringLeader")
            .SetGuiPresentation("FeatInspiringLeader", Category.Feat, PowerOathOfTirmarGoldenSpeech)
            .SetUsesFixed(ActivationTime.Minute10, RechargeRate.ShortRest)
            .SetExplicitAbilityScore(AttributeDefinitions.Charisma)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                .SetDurationData(DurationType.Permanent)
                .SetEffectForms(EffectFormBuilder.Create()
                    .SetTempHpForm()
                    .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                    .SetBonusMode(AddBonusMode.AbilityBonus)
                    .Build())
                .SetEffectAdvancement(EffectIncrementMethod.None)
                .SetParticleEffectParameters(SpellDefinitions.MagicWeapon)
                .Build())
            .AddToDB();

        return FeatDefinitionBuilder
            .Create("FeatInspiringLeader")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerFeatInspiringLeader)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
            .AddToDB();
    }

    private static void BuildMagicInitiate([NotNull] List<FeatDefinition> feats)
    {
        var magicInitiateFeats = new List<FeatDefinition>();
        var spellLists = new List<SpellListDefinition>
        {
            SpellListBard,
            SpellListCleric,
            SpellListDruid,
            SpellListSorcerer,
            SpellListWarlock,
            SpellListWizard
        };

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var spellList in spellLists)
        {
            var className = spellList.Name.Replace("SpellList", "");
            var classDefinition = GetDefinition<CharacterClassDefinition>(className);
            var featMagicInitiate = FeatDefinitionWithPrerequisitesBuilder
                .Create($"Feat{MagicInitiate}{className}")
                .SetGuiPresentation(
                    Gui.Format($"Feat/&Feat{MagicInitiate}Title", classDefinition.FormatTitle()),
                    Gui.Format($"Feat/&Feat{MagicInitiate}Description", classDefinition.FormatTitle()))
                .SetFeatures(
                    FeatureDefinitionPointPoolBuilder
                        .Create($"PointPoolFeat{MagicInitiate}{className}Cantrip")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 2, spellList,
                            MagicInitiate)
                        .AddToDB(),
                    FeatureDefinitionPointPoolBuilder
                        .Create($"PointPoolFeat{MagicInitiate}{className}Spell")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Spell, 1, spellList,
                            MagicInitiate, 1, 1)
                        .AddToDB())
                .SetMustCastSpellsPrerequisite()
                .AddToDB();

            magicInitiateFeats.Add(featMagicInitiate);
        }

        var group = GroupFeats.MakeGroup("FeatGroupMagicInitiate", MagicInitiate, magicInitiateFeats);

        group.mustCastSpellsPrerequisite = true;
        feats.AddRange(magicInitiateFeats);
    }

    private static FeatDefinition BuildMobile()
    {
        return FeatDefinitionBuilder
            .Create("FeatMobile")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("OnAfterActionFeatMobileDash")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(
                        new AooImmunityMobile(),
                        new OnAfterActionFeatMobileDash(
                            ConditionDefinitionBuilder
                                .Create(ConditionDefinitions.ConditionFreedomOfMovement, "ConditionFeatMobileAfterDash")
                                .SetOrUpdateGuiPresentation(Category.Condition)
                                .SetPossessive()
                                .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
                                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                                .SetFeatures(FeatureDefinitionMovementAffinitys.MovementAffinityFreedomOfMovement)
                                .AddToDB()))
                    .AddToDB(),
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityFeatMobile")
                    .SetGuiPresentationNoContent(true)
                    .SetBaseSpeedAdditiveModifier(2)
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildMonkInitiate()
    {
        return FeatDefinitionBuilder
            .Create("FeatMonkInitiate")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                PowerMonkPatientDefense,
                FeatureSetMonkStepOfTheWind,
                FeatureSetMonkFlurryOfBlows,
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierMonkKiPointsAddProficiencyBonus")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus,
                        AttributeDefinitions.KiPoints)
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Wisdom, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildPickPocket()
    {
        var abilityCheckAffinityFeatPickPocket = FeatureDefinitionAbilityCheckAffinityBuilder
            .Create(FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityFeatLockbreaker,
                "AbilityCheckAffinityFeatPickPocket")
            .SetGuiPresentation("FeatPickPocket", Category.Feat)
            .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand))
            .AddToDB();

        var proficiencyFeatPickPocket = FeatureDefinitionProficiencyBuilder
            .Create(FeatureDefinitionProficiencys.ProficiencyFeatLockbreaker,
                "ProficiencyFeatPickPocket")
            .SetGuiPresentation("FeatPickPocket", Category.Feat)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.SleightOfHand)
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(FeatDefinitions.Lockbreaker, "FeatPickPocket")
            .SetFeatures(abilityCheckAffinityFeatPickPocket, proficiencyFeatPickPocket)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    private static FeatDefinition BuildPoisonousSkin()
    {
        return FeatDefinitionBuilder
            .Create("FeatPoisonousSkin")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("OnAttackHitEffectFeatPoisonousSkin")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(
                        new OnAttackHitEffectFeatPoisonousSkin(),
                        new CustomConditionFeatureFeatPoisonousSkin())
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
            .AddToDB();
    }

    private static FeatDefinition BuildTough()
    {
        return FeatDefinitionBuilder
            .Create("FeatTough")
            .SetFeatures(FeatureDefinitionAttributeModifierBuilder
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
        return FeatDefinitionBuilder
            .Create(FeatWarCaster)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionMagicAffinityBuilder
                .Create(MagicAffinityFeatWarCaster)
                .SetGuiPresentation(FeatWarCaster, Category.Feat)
                .SetCastingModifiers(0, SpellParamsModifierType.FlatValue, 0,
                    SpellParamsModifierType.None)
                .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
                .SetHandsFullCastingModifiers(true, true, true)
                .AddToDB())
            .SetMustCastSpellsPrerequisite()
            .AddToDB();
    }


    private static RulesetEffectPower GetUsablePower(RulesetCharacter rulesetCharacter)
    {
        var constitution = rulesetCharacter.GetAttribute(AttributeDefinitions.Constitution).CurrentValue;
        var proficiencyBonus = rulesetCharacter.GetAttribute(AttributeDefinitions.ProficiencyBonus).CurrentValue;
        var usablePower = new RulesetUsablePower(PowerFeatPoisonousSkin, null, null)
        {
            saveDC = ComputeAbilityScoreBasedDC(constitution, proficiencyBonus)
        };

        return new RulesetEffectPower(rulesetCharacter, usablePower);
    }

    //
    // HELPERS
    //

    private sealed class OnAfterActionFeatMobileDash : IOnAfterActionFeature
    {
        private readonly ConditionDefinition _conditionDefinition;

        public OnAfterActionFeatMobileDash(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public void OnAfterAction(CharacterAction action)
        {
            if (action is not CharacterActionDash or CharacterActionFlurryOfBlowsSwiftSteps
                or CharacterActionFlurryOfBlows or CharacterActionFlurryOfBlowsSwiftSteps
                or CharacterActionFlurryOfBlowsUnendingStrikes)
            {
                return;
            }

            var attacker = action.ActingCharacter;

            var rulesetCondition = RulesetCondition.CreateActiveCondition(
                attacker.RulesetCharacter.Guid,
                _conditionDefinition,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                attacker.RulesetCharacter.Guid,
                attacker.RulesetCharacter.CurrentFaction.Name);

            attacker.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagCombat, rulesetCondition);
        }
    }

    private sealed class OnAttackHitEffectFeatPoisonousSkin : IAfterAttackEffect
    {
        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!ValidatorsWeapon.IsUnarmedWeapon(rulesetAttacker, attackMode) || attackMode.ranged ||
                outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            var hasEffect = defender.AffectingGlobalEffects.Any(x =>
                x is RulesetEffectPower rulesetEffectPower &&
                rulesetEffectPower.PowerDefinition != PowerFeatPoisonousSkin);

            if (hasEffect)
            {
                return;
            }

            var effectPower = GetUsablePower(rulesetAttacker);

            effectPower.ApplyEffectOnCharacter(defender.RulesetCharacter, true, defender.LocationPosition);
        }
    }

    private class CustomConditionFeatureFeatPoisonousSkin : ICustomConditionFeature
    {
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var conditionDefinition = rulesetCondition.ConditionDefinition;

            if (!conditionDefinition.Name.Contains("Grappled"))
            {
                return;
            }

            if (!RulesetEntity.TryGetEntity<RulesetCharacter>(rulesetCondition.SourceGuid, out var rulesetAttacker))
            {
                return;
            }

            var effectPower = GetUsablePower(rulesetAttacker);
            var targetLocationCharacter = GameLocationCharacter.GetFromActor(target);

            effectPower.ApplyEffectOnCharacter(target, true, targetLocationCharacter.LocationPosition);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    private sealed class ModifyAttackModeForWeaponFeatAstralArms : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsUnarmedWeapon(character, attackMode) || attackMode.ranged)
            {
                return;
            }

            attackMode.reach = true;
            attackMode.reachRange = 2;
        }
    }

    private sealed class AooImmunityMobile : IImmuneToAooOfRecentAttackedTarget
    {
    }
}
