using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomBuilders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
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
    internal const string FeatEldritchAdept = "FeatEldritchAdept";
    internal const string FeatWarCaster = "FeatWarCaster";
    internal const string MagicAffinityFeatWarCaster = "MagicAffinityFeatWarCaster";
    internal const string FeatMagicInitiateTag = "Initiate";
    internal const string FeatSpellSniperTag = "Sniper";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featAstralArms = BuildAstralArms();
        var featEldritchAdept = BuildEldritchAdept();
        var featHealer = BuildHealer();
        var featInfusionAdept = BuildInfusionsAdept();
        var featInspiringLeader = BuildInspiringLeader();
        var featMetamagicAdept = BuildMetamagicAdept();
        var featMobile = BuildMobile();
        var featMonkInitiate = BuildMonkInitiate();
        var featPickPocket = BuildPickPocket();
        var featPoisonousSkin = BuildPoisonousSkin();
        var featTacticianAdept = BuildTacticianAdept();
        var featTough = BuildTough();
        var featWarCaster = BuildWarcaster();

        var spellSniperGroup = BuildSpellSniper(feats);
        var elementalAdeptGroup = BuildElementalAdept(feats);
        var elementalMasterGroup = BuildElementalMaster(feats);

        BuildMagicInitiate(feats);

        feats.AddRange(
            featAstralArms,
            featEldritchAdept,
            featHealer,
            featInfusionAdept,
            featInspiringLeader,
            featMetamagicAdept,
            featMobile,
            featMonkInitiate,
            featPickPocket,
            featPoisonousSkin,
            featTacticianAdept,
            featTough,
            featWarCaster);

        GroupFeats.FeatGroupUnarmoredCombat.AddFeats(
            featAstralArms,
            featMonkInitiate,
            featPoisonousSkin);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featHealer,
            featInspiringLeader);

        GroupFeats.FeatGroupAgilityCombat.AddFeats(
            featMobile);

        GroupFeats.FeatGroupSpellCombat.AddFeats(
            elementalAdeptGroup,
            elementalMasterGroup,
            featWarCaster,
            spellSniperGroup);

        GroupFeats.FeatGroupGeneralAdept.AddFeats(
            featEldritchAdept,
            featInfusionAdept,
            featMetamagicAdept,
            featTacticianAdept);

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
            FeatDefinitions.ArcaneAppraiser,
            FeatDefinitions.Manipulator,
            featHealer,
            featPickPocket);
    }

    #region Eldritch Adept

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
            .SetValidators(ValidatorsFeat.IsLevel2)
            .AddToDB();
    }

    #endregion

    #region Tactician Adept

    private static FeatDefinition BuildTacticianAdept()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatTacticianAdept")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                GambitsBuilders.GambitPool,
                GambitsBuilders.Learn2Gambit,
                MartialTactician.BuildGambitPoolIncrease(2, "FeatTacticianAdept"))
            .SetValidators(ValidatorsFeat.IsLevel4)
            .AddToDB();
    }

    #endregion

    #region Infusions Adept

    private static FeatDefinition BuildInfusionsAdept()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatInfusionsAdept")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(InventorClass.InfusionPool, InventorClass.BuildLearn(2, "FeatInfusionsAdept"))
            .SetValidators(ValidatorsFeat.IsLevel2)
            .AddToDB();
    }

    #endregion

    #region Healer

    private static FeatDefinition BuildHealer()
    {
        var spriteMedKit = Sprites.GetSprite("PowerMedKit", Resources.PowerMedKit, 256, 128);
        var powerFeatHealerMedKit = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerMedKit")
            .SetGuiPresentation(Category.Feature, spriteMedKit)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.ShortRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
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

        var spriteResuscitate = Sprites.GetSprite("PowerResuscitate", Resources.PowerResuscitate, 256, 128);
        var powerFeatHealerResuscitate = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerResuscitate")
            .SetGuiPresentation(Category.Feature, spriteResuscitate)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetTargetingData(Side.Ally, RangeType.Touch, 1, TargetType.IndividualsUnique)
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

        var spriteStabilize = Sprites.GetSprite("PowerStabilize", Resources.PowerStabilize, 256, 128);
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

    #endregion

    #region Inspiring Leader

    private static FeatDefinition BuildInspiringLeader()
    {
        var powerFeatInspiringLeader = FeatureDefinitionPowerBuilder
            .Create("PowerFeatInspiringLeader")
            .SetGuiPresentation("FeatInspiringLeader", Category.Feat,
                Sprites.GetSprite("PowerInspiringLeader", Resources.PowerInspiringLeader, 256, 128))
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

    #endregion

    #region Magic Initiate

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
            var featMagicInitiate = FeatDefinitionBuilder
                .Create($"{NAME}{className}")
                .SetGuiPresentation(
                    Gui.Format($"Feat/&{NAME}Title", classTitle),
                    Gui.Format($"Feat/&{NAME}Description", classTitle))
                .SetFeatures(
                    FeatureDefinitionCastSpellBuilder
                        .Create(castSpell, $"CastSpell{NAME}{className}")
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
                        .SetReplacedSpells(1, 0)
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

        GroupFeats.MakeGroup("FeatGroupMagicInitiate", NAME, magicInitiateFeats);

        feats.AddRange(magicInitiateFeats);
    }

    #endregion

    #region Monk Initiate

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

    #endregion

    #region Pick Pocket

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

    #endregion

    #region Tough

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

    #endregion

    #region War Caster

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

    #endregion

    #region Metamagic Adept

    private static FeatDefinition BuildMetamagicAdept()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatMetamagicAdept")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                ActionAffinitySorcererMetamagicToggle,
                FeatureDefinitionAttributeModifierBuilder
                    .Create(AttributeModifierSorcererSorceryPointsBase, "AttributeModifierSorcererSorceryPointsBonus2")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(
                        FeatureDefinitionAttributeModifier.AttributeModifierOperation.AddHalfProficiencyBonus,
                        AttributeDefinitions.SorceryPoints)
                    .AddToDB(),
                FeatureDefinitionPointPoolBuilder
                    .Create("PointPoolFeatMetamagicAdept")
                    .SetGuiPresentationNoContent(true)
                    .SetPool(HeroDefinitions.PointsPoolType.Metamagic, 2)
                    .AddToDB())
            .SetMustCastSpellsPrerequisite()
            .SetValidators(ValidatorsFeat.IsLevel2)
            .AddToDB();
    }

    #endregion

    #region Common Helpers

    internal sealed class SpellTag
    {
        internal SpellTag(string spellTag)
        {
            Name = spellTag;
        }

        internal string Name { get; }
    }

    #endregion

    #region Astral Arms

    private static FeatDefinition BuildAstralArms()
    {
        return FeatDefinitionBuilder
            .Create("FeatAstralArms")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike)
            .SetCustomSubFeatures(
                new CanMakeAoOOnReachEntered
                {
                    AllowRange = false,
                    WeaponValidator = (mode, _, character) =>
                        ModifyWeaponAttackModeFeatAstralArms.ValidWeapon(character, mode)
                },
                new ModifyWeaponAttackModeFeatAstralArms())
            .AddToDB();
    }

    private sealed class ModifyWeaponAttackModeFeatAstralArms : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            if (!ValidWeapon(character, attackMode))
            {
                return;
            }

            attackMode.reach = true;
            attackMode.reachRange = 2;
        }

        public static bool ValidWeapon(RulesetCharacter character, RulesetAttackMode attackMode)
        {
            return ValidatorsWeapon.IsUnarmed(character, attackMode) && !attackMode.ranged;
        }
    }

    #endregion

    #region Elemental Adept

    private static FeatDefinition BuildElementalAdept(List<FeatDefinition> feats)
    {
        const string NAME = "FeatElementalAdept";

        var elementalAdeptFeats = new List<FeatDefinition>();

        var damageTypes = new[]
        {
            DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypePoison, DamageTypeThunder
        };

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var damageType in damageTypes)
        {
            var damageTitle = Gui.Localize($"Rules/&{damageType}Title");
            var guiPresentation = new GuiPresentationBuilder(
                    Gui.Format($"Feat/&{NAME}Title", damageTitle),
                    Gui.Format($"Feat/&{NAME}Description", damageTitle))
                .Build();

            var feat = FeatDefinitionBuilder
                .Create($"{NAME}{damageType}")
                .SetGuiPresentation(guiPresentation)
                .SetFeatures(FeatureDefinitionDieRollModifierDamageTypeDependentBuilder
                    .Create($"DieRollModifierDamageTypeDependent{NAME}{damageType}")
                    .SetGuiPresentation(guiPresentation)
                    .SetModifiers(RollContext.MagicDamageValueRoll, 1, 1, 1,
                        "Feature/&DieRollModifierFeatElementalAdeptReroll", damageType)
                    .SetCustomSubFeatures(new IgnoreDamageResistanceElementalAdept(damageType))
                    .AddToDB())
                .SetMustCastSpellsPrerequisite()
                .SetFeatFamily("ElementalAdept")
                .AddToDB();

            elementalAdeptFeats.Add(feat);
        }

        var elementalAdeptGroup =
            GroupFeats.MakeGroup("FeatGroupElementalAdept", "ElementalAdept", elementalAdeptFeats);

        feats.AddRange(elementalAdeptFeats);

        return elementalAdeptGroup;
    }

    private sealed class IgnoreDamageResistanceElementalAdept : IIgnoreDamageAffinity
    {
        private readonly List<string> _damageTypes = new();

        public IgnoreDamageResistanceElementalAdept(params string[] damageTypes)
        {
            _damageTypes.AddRange(damageTypes);
        }

        public bool CanIgnoreDamageAffinity(IDamageAffinityProvider provider, RulesetActor actor)
        {
            return provider.DamageAffinityType == DamageAffinityType.Resistance &&
                   _damageTypes.Contains(provider.DamageType);
        }
    }

    #endregion

    #region Elemental Master

    private static FeatDefinition BuildElementalMaster(List<FeatDefinition> feats)
    {
        const string NAME = "FeatElementalMaster";

        var elementalAdeptFeats = new List<FeatDefinition>();

        var damageTypes = new[]
        {
            DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypePoison, DamageTypeThunder
        };

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var damageType in damageTypes)
        {
            var damageTitle = Gui.Localize($"Rules/&{damageType}Title");
            var guiPresentation = new GuiPresentationBuilder(
                    Gui.Format($"Feat/&{NAME}Title", damageTitle),
                    Gui.Format($"Feat/&{NAME}Description", damageTitle))
                .Build();

            var feat = FeatDefinitionBuilder
                .Create($"{NAME}{damageType}")
                .SetGuiPresentation(guiPresentation)
                .SetFeatures(
                    FeatureDefinitionDieRollModifierDamageTypeDependentBuilder
                        .Create($"DieRollModifierDamageTypeDependent{NAME}{damageType}")
                        .SetGuiPresentation(guiPresentation)
                        .SetModifiers(RollContext.AttackRoll, 1, 1, 1,
                            "Feature/&DieRollModifierFeatElementalMasterReroll", damageType)
                        .SetCustomSubFeatures(new IgnoreDamageResistanceElementalMaster(damageType))
                        .AddToDB(),
                    FeatureDefinitionDamageAffinityBuilder
                        .Create($"DamageAffinity{NAME}{damageType}")
                        .SetGuiPresentation(guiPresentation)
                        .SetDamageAffinityType(DamageAffinityType.Resistance)
                        .SetDamageType(damageType)
                        .AddToDB())
                .SetMustCastSpellsPrerequisite()
                .SetFeatFamily("ElementalMaster")
                .SetKnownFeatsPrerequisite($"FeatElementalAdept{damageType}")
                .AddToDB();

            elementalAdeptFeats.Add(feat);
        }

        var elementalAdeptGroup =
            GroupFeats.MakeGroup("FeatGroupElementalMaster", "ElementalMaster", elementalAdeptFeats);

        feats.AddRange(elementalAdeptFeats);

        return elementalAdeptGroup;
    }

    private sealed class IgnoreDamageResistanceElementalMaster : IIgnoreDamageAffinity
    {
        private readonly List<string> _damageTypes = new();

        public IgnoreDamageResistanceElementalMaster(params string[] damageTypes)
        {
            _damageTypes.AddRange(damageTypes);
        }

        public bool CanIgnoreDamageAffinity(
            IDamageAffinityProvider provider, RulesetActor rulesetActor)
        {
            return provider.DamageAffinityType == DamageAffinityType.Immunity &&
                   _damageTypes.Contains(provider.DamageType);
        }
    }

    #endregion

    #region Mobile

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
                        new AooImmunityFeatMobile(),
                        new ActionFinishedFeatMobileDash(
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


    private sealed class ActionFinishedFeatMobileDash : IActionFinished
    {
        private readonly ConditionDefinition _conditionDefinition;

        public ActionFinishedFeatMobileDash(ConditionDefinition conditionDefinition)
        {
            _conditionDefinition = conditionDefinition;
        }

        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not (CharacterActionDash or
                CharacterActionFlurryOfBlows or
                CharacterActionFlurryOfBlowsSwiftSteps or
                CharacterActionFlurryOfBlowsUnendingStrikes))
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            rulesetAttacker.InflictCondition(
                _conditionDefinition.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagCombat,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                null,
                0,
                0,
                0);
        }
    }

    private sealed class AooImmunityFeatMobile : IImmuneToAooOfRecentAttackedTarget
    {
    }

    #endregion

    #region Poisonous Skin

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
                .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                .SetConditionForm(ConditionDefinitions.ConditionPoisoned, ConditionForm.ConditionOperation.Add)
                .Build())
            .SetDurationData(DurationType.Minute, 1)
            .SetRecurrentEffect(RecurrentEffect.OnTurnStart | RecurrentEffect.OnActivation)
            .Build())
        .AddToDB();

    private static FeatDefinition BuildPoisonousSkin()
    {
        return FeatDefinitionBuilder
            .Create("FeatPoisonousSkin")
            .SetGuiPresentation(Category.Feat)
            .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
            .SetCustomSubFeatures(new CustomBehaviorFeatPoisonousSkin())
            .AddToDB();
    }

    private class CustomBehaviorFeatPoisonousSkin :
        IAttackEffectAfterDamage, ICustomConditionFeature, IActionFinished
    {
        // handle Shove scenario
        public IEnumerator OnActionFinished(CharacterAction action)
        {
            if (action is not CharacterActionShove)
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            foreach (var target in action.actionParams.TargetCharacters)
            {
                ApplyPower(rulesetAttacker, target);
            }
        }

        // handle standard attack scenario
        public void OnAttackEffectAfterDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!ValidatorsWeapon.IsUnarmed(rulesetAttacker, attackMode) || attackMode.ranged ||
                outcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                return;
            }

            ApplyPower(attacker.RulesetCharacter, defender);
        }

        // handle Grappled scenario
        public void ApplyFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var conditionDefinition = rulesetCondition.ConditionDefinition;

            if (!conditionDefinition.Name.Contains("Grappled"))
            {
                return;
            }

            if (!RulesetEntity.TryGetEntity<RulesetCharacter>(rulesetCondition.SourceGuid, out var grappler))
            {
                return;
            }

            var grapplerLocationCharacter = GameLocationCharacter.GetFromActor(grappler);

            ApplyPower(target, grapplerLocationCharacter);
        }

        public void RemoveFeature(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }

        private static RulesetEffectPower GetUsablePower(RulesetCharacter rulesetCharacter)
        {
            var constitution = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Constitution);
            var proficiencyBonus = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var usablePower = new RulesetUsablePower(PowerFeatPoisonousSkin, null, null)
            {
                saveDC = ComputeAbilityScoreBasedDC(constitution, proficiencyBonus)
            };

            return new RulesetEffectPower(rulesetCharacter, usablePower);
        }

        private static void ApplyPower(RulesetCharacter attacker, GameLocationCharacter defender)
        {
            var hasEffect = defender.AffectingGlobalEffects.Any(x =>
                x is RulesetEffectPower rulesetEffectPower &&
                rulesetEffectPower.PowerDefinition != PowerFeatPoisonousSkin);

            if (hasEffect)
            {
                return;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (rulesetDefender == null || rulesetDefender.IsDeadOrDying)
            {
                return;
            }

            var effectPower = GetUsablePower(attacker);

            effectPower.ApplyEffectOnCharacter(rulesetDefender, true, defender.LocationPosition);
        }
    }

    #endregion

    #region Spell Sniper

    private static FeatDefinition BuildSpellSniper([NotNull] List<FeatDefinition> feats)
    {
        const string NAME = "FeatSpellSniper";

        var spellSniperFeats = new List<FeatDefinition>();
        var castSpells = new List<FeatureDefinitionCastSpell>
        {
            // CastSpellBard, // Bard doesn't have any cantrips in Solasta that are RangeHit
            // CastSpellCleric, // Cleric doesn't have any cantrips in Solasta that are RangeHit
            CastSpellDruid,
            CastSpellSorcerer,
            CastSpellWarlock,
            CastSpellWizard
        };

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var castSpell in castSpells)
        {
            var spellSniperSpells = castSpell.SpellListDefinition.SpellsByLevel
                .SelectMany(x => x.Spells)
                .Where(x => x.SpellLevel == 0 && x.EffectDescription.RangeType is RangeType.RangeHit &&
                            x.EffectDescription.HasDamageForm())
                .ToArray();

            if (spellSniperSpells.Length == 0)
            {
                continue;
            }

            var className = castSpell.SpellListDefinition.Name.Replace("SpellList", "");
            var classDefinition = GetDefinition<CharacterClassDefinition>(className);
            var classTitle = classDefinition.FormatTitle();

            var spellList = SpellListDefinitionBuilder
                .Create($"SpellList{NAME}{className}")
                .SetGuiPresentationNoContent(true)
                .ClearSpells()
                .SetSpellsAtLevel(0, spellSniperSpells)
                .FinalizeSpells()
                .AddToDB();

            var featSpellSniper = FeatDefinitionBuilder
                .Create($"{NAME}{className}")
                .SetGuiPresentation(
                    Gui.Format($"Feat/&{NAME}Title", classTitle),
                    Gui.Format($"Feat/&{NAME}Description", classTitle))
                .SetFeatures(
                    FeatureDefinitionCombatAffinityBuilder
                        .Create($"CombatAffinity{NAME}{className}")
                        .SetGuiPresentationNoContent(true)
                        .SetCustomSubFeatures(new RestrictedContextValidator((_, _, _, _, _, mode, _) =>
                            (OperationType.Set,
                                mode.sourceDefinition is SpellDefinition &&
                                mode.EffectDescription.RangeType == RangeType.RangeHit)))
                        .SetIgnoreCover()
                        .AddToDB(),
                    FeatureDefinitionCastSpellBuilder
                        .Create(castSpell, $"CastSpell{NAME}{className}")
                        .SetGuiPresentation(
                            Gui.Format($"Feature/&CastSpell{NAME}Title", classTitle),
                            Gui.Format($"Feat/&{NAME}Description", classTitle))
                        .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Race)
                        .SetSpellKnowledge(SpellKnowledge.Selection)
                        .SetSpellReadyness(SpellReadyness.AllKnown)
                        .SetSlotsRecharge(RechargeRate.LongRest)
                        .SetSlotsPerLevel(SharedSpellsContext.RaceEmptyCastingSlots)
                        .SetKnownCantrips(1, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
                        .SetKnownSpells(0, FeatureDefinitionCastSpellBuilder.CasterProgression.Flat)
                        .SetReplacedSpells(1, 0)
                        .SetUniqueLevelSlots(false)
                        .SetCustomSubFeatures(new SpellTag(FeatSpellSniperTag))
                        .SetSpellList(spellList)
                        .AddToDB(),
                    FeatureDefinitionPointPoolBuilder
                        .Create($"PointPool{NAME}{className}Cantrip")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, spellList,
                            FeatSpellSniperTag)
                        .AddToDB())
                .SetFeatFamily(NAME)
                .SetCustomSubFeatures(new ModifyMagicEffectFeatSpellSniper())
                .AddToDB();

            spellSniperFeats.Add(featSpellSniper);
        }

        var spellSniperGroup = GroupFeats.MakeGroup("FeatGroupSpellSniper", NAME, spellSniperFeats);

        feats.AddRange(spellSniperFeats);

        spellSniperGroup.mustCastSpellsPrerequisite = true;

        return spellSniperGroup;
    }

    private sealed class ModifyMagicEffectFeatSpellSniper : IModifyMagicEffect
    {
        public EffectDescription ModifyEffect(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            if (definition is not SpellDefinition spellDefinition)
            {
                return effectDescription;
            }

            if (effectDescription.rangeType != RangeType.RangeHit || !effectDescription.HasDamageForm())
            {
                return effectDescription;
            }

            effectDescription.rangeParameter = spellDefinition.EffectDescription.RangeParameter * 2;

            return effectDescription;
        }
    }

    #endregion
}
