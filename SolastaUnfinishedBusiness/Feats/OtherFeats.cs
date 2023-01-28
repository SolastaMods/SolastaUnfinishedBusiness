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
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;

namespace SolastaUnfinishedBusiness.Feats;

internal static class OtherFeats
{
    internal const string FeatCantripsAdeptTag = "Adept";
    internal const string FeatSpellSniperTag = "Sniper";
    internal const string FeatEldritchAdept = "FeatEldritchAdept";
    internal const string FeatWarCaster = "FeatWarCaster";
    internal const string MagicAffinityFeatWarCaster = "MagicAffinityFeatWarCaster";

    private const string FeatMagicInitiateTag = "Initiate";

    // private const string FeatMagicInitiate = "FeatMagicInitiate";

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
        var featCantripsAdept = BuildCantripsAdept();
        var featEldritchAdept = BuildEldritchAdept();
        var featHealer = BuildHealer();
        var featInspiringLeader = BuildInspiringLeader();
        var featMetamagic = BuildMetamagic();
        var featMobile = BuildMobile();
        var featMonkInitiate = BuildMonkInitiate();
        var featPickPocket = BuildPickPocket();
        var featPoisonousSkin = BuildPoisonousSkin();
        var featSpellSniper = BuildSpellSniper();
        var featTough = BuildTough();
        var featWarCaster = BuildWarcaster();

        BuildMagicInitiate(feats);

        feats.AddRange(
            featAstralArms,
            featCantripsAdept,
            featEldritchAdept,
            featHealer,
            featInspiringLeader,
            featMetamagic,
            featMobile,
            featMonkInitiate,
            featPickPocket,
            featPoisonousSkin,
            featSpellSniper,
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
            featCantripsAdept,
            featSpellSniper,
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

    private static FeatDefinition BuildCantripsAdept()
    {
        const string NAME = "FeatCantripsAdept";

        return FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPointPoolBuilder
                    .Create($"PointPool{NAME}Cantrip")
                    .SetGuiPresentationNoContent(true)
                    .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 2,
                        SpellListDefinitions.SpellListAllCantrips,
                        FeatCantripsAdeptTag)
                    .AddToDB())
            .SetValidators(ValidatorsFeat.HasCantrips())
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
            .SetValidators(ValidatorsFeat.ValidateMinCharLevel(2))
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
        const string NAME = "FeatMagicInitiate";

        var magicInitiateFeats = new List<FeatDefinition>();
        var castSpells = new List<FeatureDefinitionCastSpell>
        {
            CastSpellBard,
            CastSpellCleric,
            CastSpellDruid,
            CastSpellSorcerer,
            CastSpellWarlock,
            CastSpellWizard
        };

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var castSpell in castSpells)
        {
            var spellList = castSpell.SpellListDefinition;
            var className = spellList.Name.Replace("SpellList", "");
            var classDefinition = GetDefinition<CharacterClassDefinition>(className);
            var classTitle = classDefinition.FormatTitle();
            var featMagicInitiate = FeatDefinitionWithPrerequisitesBuilder
                .Create($"{NAME}{className}")
                .SetGuiPresentation(
                    Gui.Format($"Feat/&{NAME}Title", classTitle),
                    Gui.Format($"Feat/&{NAME}Description", classTitle))
                .SetFeatures(
                    FeatureDefinitionCastSpellBuilder
                        .Create(castSpell, $"CastSpell{NAME}{className}#")
                        .SetGuiPresentation(
                            Gui.Format($"Feature/&CastSpell{NAME}Title", classTitle),
                            Gui.Format($"Feat/&{NAME}Description", classTitle))
                        .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
                        .SetSpellKnowledge(SpellKnowledge.Selection)
                        .SetSpellReadyness(SpellReadyness.AllKnown)
                        .SetSlotsRecharge(RechargeRate.LongRest)
                        .SetSlotsPerLevel(SharedSpellsContext.InitiateCastingSlots)
                        .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
                        .SetKnownSpells(2, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
                        .SetUniqueLevelSlots(false)
                        .SetCustomSubFeatures(new SpellTag(FeatMagicInitiateTag))
                        .AddToDB(),
                    FeatureDefinitionPointPoolBuilder
                        .Create($"PointPool{NAME}{className}Cantrip")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 2, spellList,
                            FeatMagicInitiateTag)
                        .AddToDB(),
                    FeatureDefinitionPointPoolBuilder
                        .Create($"PointPool{NAME}{className}Spell")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Spell, 1, spellList,
                            FeatMagicInitiateTag, 1, 1)
                        .AddToDB())
                .SetFeatFamily(NAME)
                .AddToDB();

            magicInitiateFeats.Add(featMagicInitiate);
        }

        _ = GroupFeats.MakeGroup("FeatGroupMagicInitiate", NAME, magicInitiateFeats);

        feats.AddRange(magicInitiateFeats);
    }

    private static FeatDefinition BuildMetamagic()
    {
        // KEEP FOR BACKWARD COMPATIBILITY until next DLC
        BuildMetamagicBackwardCompatibility();

        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatMetamagicAdept")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                ActionAffinitySorcererMetamagicToggle,
                FeatureDefinitionAttributeModifierBuilder
                    .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(
                        FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                        AttributeDefinitions.SorceryPoints,
                        2)
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolFeatMetamagicAdept")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Metamagic, 2)
                    .AddToDB())
            .SetMustCastSpellsPrerequisite()
            .SetValidators(ValidatorsFeat.ValidateMinCharLevel(3))
            .AddToDB();
    }

    private static void BuildMetamagicBackwardCompatibility()
    {
        var attributeModifierSorcererSorceryPointsBonus3 = FeatureDefinitionAttributeModifierBuilder
            .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus3")
            .SetGuiPresentationNoContent(true)
            .SetModifier(
                FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddProficiencyBonus,
                AttributeDefinitions.SorceryPoints)
            .AddToDB();

        var metaMagicFeats = new List<FeatDefinition>();
        var dbMetamagicOptionDefinition = DatabaseRepository.GetDatabase<MetamagicOptionDefinition>();

        metaMagicFeats.SetRange(dbMetamagicOptionDefinition
            .Select(metamagicOptionDefinition => FeatDefinitionWithPrerequisitesBuilder
                .Create($"FeatAdept{metamagicOptionDefinition.Name}")
                .SetGuiPresentationNoContent(true)
                .SetFeatures(
                    ActionAffinitySorcererMetamagicToggle,
                    attributeModifierSorcererSorceryPointsBonus3,
                    FeatureDefinitionBuilder
                        .Create($"CustomCodeFeatAdept{metamagicOptionDefinition.Name}")
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new CustomCodeFeatMetamagicAdept(metamagicOptionDefinition))
                        .AddToDB())
                .SetAbilityScorePrerequisite(AttributeDefinitions.Charisma, 13)
                .SetMustCastSpellsPrerequisite()
                .AddToDB()));
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

    private static FeatDefinition BuildSpellSniper()
    {
        const string NAME = "FeatSpellSniper";

        var spellSniperSpells = SpellListDefinitions.SpellListAllCantrips.SpellsByLevel
            .SelectMany(x => x.Spells)
            .Where(x => x.EffectDescription.RangeType is RangeType.RangeHit or RangeType.Distance &&
                        x.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Damage) != null)
            .ToArray();

        var spellListDefinition = SpellListDefinitionBuilder
            .Create($"SpellList{NAME}")
            .SetGuiPresentationNoContent(true)
            .ClearSpells()
            .SetSpellsAtLevel(0, spellSniperSpells)
            .FinalizeSpells(true, -1)
            .AddToDB();

        return FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionPointPoolBuilder
                    .Create($"PointPool{NAME}Cantrip")
                    .SetGuiPresentationNoContent(true)
                    .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, spellListDefinition,
                        FeatSpellSniperTag)
                    .AddToDB())
            .SetCustomSubFeatures(new ModifyMagicEffectFeatSpellSniper())
            .SetValidators(ValidatorsFeat.HasCantrips())
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

    internal sealed class SpellTag
    {
        internal SpellTag(string spellTag)
        {
            Name = spellTag;
        }

        internal string Name { get; }
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

    private sealed class ModifyMagicEffectFeatSpellSniper : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effect,
            RulesetCharacter caster)
        {
            if (definition is not SpellDefinition spellDefinition)
            {
                return effect;
            }

            if ((effect.rangeType != RangeType.RangeHit && effect.rangeType != RangeType.Distance) ||
                effect.GetFirstFormOfType(EffectForm.EffectFormType.Damage) == null)
            {
                return effect;
            }

            effect.rangeParameter = spellDefinition.EffectDescription.RangeParameter * 2;
            effect.ignoreCover = true;

            return effect;
        }
    }
}
