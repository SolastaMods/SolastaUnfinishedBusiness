using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.FightingStyles;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionActionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAttributeModifiers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionCastSpells;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class OtherFeats
{
    private const string MagicAffinityFeatWarCaster = "MagicAffinityFeatWarCaster";
    internal const string FeatEldritchAdept = "FeatEldritchAdept";
    internal const string FeatWarCaster = "FeatWarCaster";
    internal const string FeatMagicInitiateTag = "Initiate";
    internal const string FeatSpellSniperTag = "Sniper";

    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        // kept for backward compatibility
        _ = BuildAstralArms();

        var featAcrobat = BuildAcrobat();
        var featArcaneArcherAdept = BuildArcaneArcherAdept();
        var featDungeonDelver = BuildDungeonDelver();
        var featEldritchAdept = BuildEldritchAdept();
        var featFightingInitiate = BuildFightingInitiate();
        var featFrostAdaptation = BuildFrostAdaptation();
        var featGiftOfTheChromaticDragon = BuildGiftOfTheChromaticDragon();
        var featHealer = BuildHealer();
        var featInfusionAdept = BuildInfusionsAdept();
        var featInspiringLeader = BuildInspiringLeader();
        var featLucky = BuildFeatLucky();
        var featMagicInitiate = BuildMagicInitiate();
        var featMartialAdept = BuildTacticianAdept();
        var featMenacing = BuildMenacing();
        var featMetamagicAdept = BuildMetamagicAdept();
        var featMobile = BuildMobile();
        var featMonkInitiate = BuildMonkInitiate();
        var featPickPocket = BuildPickPocket();
        var featTough = BuildTough();
        var featVersatilityAdept = EldritchVersatilityBuilders.FeatEldritchVersatilityAdept;
        var featWarCaster = BuildWarcaster();

        var athleteGroup = BuildAthlete(feats);
        var balefulScionGroup = BuildBalefulScion(feats);
        var chefGroup = BuildChef(feats);
        var spellSniperGroup = BuildSpellSniper(feats);
        var elementalAdeptGroup = BuildElementalAdept(feats);
        var elementalMasterGroup = BuildElementalMaster(feats);
        var weaponMasterGroup = BuildWeaponMaster(feats);

        // building this way to keep backward compatibility
        _ = BuildFeatFromFightingStyle(MonkShieldExpert.ShieldExpertName);

        var featMerciless = BuildFeatFromFightingStyle(Merciless.MercilessName);
        var featPolearmExpert = BuildFeatFromFightingStyle(PolearmExpert.PolearmExpertName);
        var featRopeIpUp = BuildFeatFromFightingStyle(RopeItUp.RopeItUpName);
        var featSentinel = BuildFeatFromFightingStyle(Sentinel.SentinelName);
        var featShieldExpert = BuildFeatFromFightingStyle(ShieldExpert.ShieldExpertName);

        feats.AddRange(
            featAcrobat,
            FeatAlert,
            featArcaneArcherAdept,
            featDungeonDelver,
            featEldritchAdept,
            featFrostAdaptation,
            featGiftOfTheChromaticDragon,
            featHealer,
            featInfusionAdept,
            featInspiringLeader,
            featLucky,
            FeatMageSlayer,
            featMagicInitiate,
            featMartialAdept,
            featMenacing,
            featMerciless,
            featMetamagicAdept,
            featMobile,
            featMonkInitiate,
            featPickPocket,
            FeatPoisonousSkin,
            featPolearmExpert,
            featRopeIpUp,
            featSentinel,
            featShieldExpert,
            FeatStealthy,
            featTough,
            featVersatilityAdept,
            featWarCaster);

        GroupFeats.FeatGroupBodyResilience.AddFeats(
            athleteGroup,
            featDungeonDelver,
            featLucky,
            featTough,
            featFrostAdaptation);

        GroupFeats.FeatGroupAgilityCombat.AddFeats(
            featAcrobat,
            FeatAlert,
            featMobile);

        GroupFeats.FeatGroupDefenseCombat.AddFeats(
            featShieldExpert);

        GroupFeats.FeatGroupMeleeCombat.AddFeats(
            balefulScionGroup,
            featPolearmExpert,
            featShieldExpert);

        GroupFeats.FeatGroupTwoHandedCombat.AddFeats(
            featPolearmExpert);

        GroupFeats.FeatGroupSpellCombat.AddFeats(
            balefulScionGroup,
            elementalAdeptGroup,
            elementalMasterGroup,
            featWarCaster,
            spellSniperGroup);

        GroupFeats.FeatGroupSupportCombat.AddFeats(
            featGiftOfTheChromaticDragon,
            chefGroup,
            featHealer,
            featInspiringLeader,
            featLucky,
            FeatMageSlayer,
            featMenacing,
            featMerciless,
            featRopeIpUp,
            featSentinel,
            weaponMasterGroup);

        GroupFeats.FeatGroupUnarmoredCombat.AddFeats(
            FeatPoisonousSkin);

        GroupFeats.FeatGroupSkills.AddFeats(
            athleteGroup,
            featAcrobat,
            featHealer,
            featMenacing,
            featPickPocket,
            FeatStealthy);

        GroupFeats.MakeGroup("FeatGroupGeneralAdept", null,
            featArcaneArcherAdept,
            featEldritchAdept,
            featFightingInitiate,
            featInfusionAdept,
            featMagicInitiate,
            featMartialAdept,
            featMetamagicAdept,
            featMonkInitiate,
            featVersatilityAdept);
    }

    #region Arcane Archer Adept

    private static FeatDefinitionWithPrerequisites BuildArcaneArcherAdept()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatArcaneArcherAdept")
            .SetGuiPresentation(Category.Feat, hidden: true)
            .SetFeatures(
                MartialArcaneArcher.PowerArcaneShot,
                MartialArcaneArcher.InvocationPoolArcaneShotChoice2,
                MartialArcaneArcher.ModifyPowerArcaneShotAdditionalUse1,
                MartialArcaneArcher.ActionAffinityArcaneArcherToggle)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .AddToDB();
    }

    #endregion

    #region Astral Arms

    private static FeatDefinition BuildAstralArms()
    {
        return FeatDefinitionBuilder
            .Create("FeatAstralArms")
            .SetGuiPresentation(Category.Feat, hidden: true)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike)
            .AddCustomSubFeatures(
                new CanMakeAoOOnReachEntered { AllowRange = false, WeaponValidator = ValidWeapon },
                new IncreaseWeaponReach(1, ValidWeapon))
            .AddToDB();

        static bool ValidWeapon(RulesetAttackMode attackMode, RulesetItem item, RulesetCharacter character)
        {
            return ValidatorsWeapon.IsUnarmed(attackMode);
        }
    }

    #endregion

    #region Eldritch Adept

    private static FeatDefinitionWithPrerequisites BuildEldritchAdept()
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

    #region Frost Adaptation

    private static FeatDefinition BuildFrostAdaptation()
    {
        // chilled and frozen immunities are handled by srd house rules now
        return FeatDefinitionBuilder
            .Create("FeatFrostAdaptation")
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatFrostAdaptation")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(AttributeModifierOperation.Additive,
                        AttributeDefinitions.Constitution, 1)
                    .AddToDB(),
                DamageAffinityColdResistance)
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    #endregion

    #region Infusions Adept

    private static FeatDefinitionWithPrerequisites BuildInfusionsAdept()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatInfusionsAdept")
            .SetGuiPresentation(Category.Feat, hidden: true)
            .SetFeatures(
                InventorClass.InfusionPool,
                InventorClass.BuildLearn(2, "FeatInfusionsAdept"),
                InventorClass.BuildInfusionPoolIncrease())
            .SetValidators(ValidatorsFeat.IsLevel2)
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
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 6)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm()
                            .SetLevelAdvancement(EffectForm.LevelApplianceType.AddBonus, LevelSourceType.CharacterLevel)
                            .SetBonusMode(AddBonusMode.AbilityBonus)
                            .Build())
                    .SetParticleEffectParameters(MagicWeapon)
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

    private static FeatDefinition BuildMagicInitiate()
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
                        .AddCustomSubFeatures(new FeatHelpers.SpellTag(FeatMagicInitiateTag))
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

        return GroupFeats.MakeGroup("FeatGroupMagicInitiate", NAME, magicInitiateFeats);
    }

    #endregion

    #region Metamagic Adept

    private static FeatDefinitionWithPrerequisites BuildMetamagicAdept()
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
                        AttributeModifierOperation.AddHalfProficiencyBonus,
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
                    .SetModifier(AttributeModifierOperation.AddProficiencyBonus,
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
            .BuildAndSetAffinityGroups(
                CharacterAbilityCheckAffinity.Advantage,
                abilityProficiencyPairs: (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand))
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

    #region Tactician Adept

    private static FeatDefinitionWithPrerequisites BuildTacticianAdept()
    {
        return FeatDefinitionWithPrerequisitesBuilder
            .Create("FeatTacticianAdept")
            .SetGuiPresentation(Category.Feat, hidden: true)
            .SetFeatures(
                GambitsBuilders.GambitPool,
                GambitsBuilders.Learn2Gambit,
                MartialTactician.BuildGambitPoolIncrease(1, "FeatTacticianAdept"))
            .SetValidators(ValidatorsFeat.IsLevel4)
            .AddToDB();
    }

    #endregion

    #region Tough

    private static FeatDefinition BuildTough()
    {
        return FeatDefinitionBuilder
            .Create("FeatTough")
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create("AttributeModifierFeatTough")
                    .SetGuiPresentationNoContent(true)
                    .SetModifier(AttributeModifierOperation.Additive,
                        AttributeDefinitions.HitPointBonusPerLevel, 2)
                    .AddToDB())
            .SetGuiPresentation(Category.Feat)
            .AddToDB();
    }

    #endregion

    #region Weapon Master

    private static FeatDefinition BuildWeaponMaster(List<FeatDefinition> feats)
    {
        const string Name = "FeatWeaponMaster";

        var simpleOrMartialWeapons = DatabaseRepository.GetDatabase<WeaponTypeDefinition>()
            .Where(x =>
                x != WeaponTypeDefinitions.UnarmedStrikeType &&
                x != CustomWeaponsContext.ThunderGauntletType &&
                x != CustomWeaponsContext.LightningLauncherType);

        foreach (var weaponTypeDefinition in simpleOrMartialWeapons)
        {
            var weaponTypeName = weaponTypeDefinition.Name;
            var featureMonkWeaponSpecialization = FeatureDefinitionProficiencyBuilder
                .Create($"Proficiency{Name}{weaponTypeName}")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(ProficiencyType.Weapon, weaponTypeName)
                .AddToDB();

            _ = CustomInvocationDefinitionBuilder
                .Create($"CustomInvocation{Name}{weaponTypeName}")
                .SetGuiPresentation(
                    weaponTypeDefinition.GuiPresentation.Title,
                    weaponTypeDefinition.GuiPresentation.Description,
                    CustomWeaponsContext.GetStandardWeaponOfType(weaponTypeDefinition.Name))
                .SetPoolType(InvocationPoolTypeCustom.Pools.WeaponMasterChoice)
                .SetGrantedFeature(featureMonkWeaponSpecialization)
                .AddCustomSubFeatures(ModifyInvocationVisibility.Marker)
                .AddToDB();
        }

        var invocationPool = CustomInvocationPoolDefinitionBuilder
            .Create($"InvocationPool{Name}")
            .SetGuiPresentationNoContent(true)
            .Setup(InvocationPoolTypeCustom.Pools.WeaponMasterChoice, 4)
            .AddToDB();

        var weaponMasterStr = FeatDefinitionBuilder
            .Create($"{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar, invocationPool)
            .SetFeatFamily(Name)
            .AddToDB();

        var weaponMasterDex = FeatDefinitionBuilder
            .Create($"{Name}Dex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, invocationPool)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(weaponMasterStr, weaponMasterDex);

        return GroupFeats.MakeGroup(
            "FeatGroupWeaponMaster", Name, weaponMasterStr, weaponMasterDex);
    }

    #endregion

    #region Stealthy

    private const string FeatStealthyName = "FeatStealthy";

    private static readonly FeatDefinition FeatStealthy = FeatDefinitionBuilder
        .Create(FeatStealthyName)
        .SetGuiPresentation(Category.Feat)
        .SetFeatures(
            AttributeModifierCreed_Of_Misaye,
            FeatureDefinitionProficiencyBuilder
                .Create($"Proficiency{FeatStealthyName}")
                .SetGuiPresentationNoContent(true)
                .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Stealth)
                .AddToDB())
        .AddToDB();

    internal static readonly Dictionary<GameLocationCharacter, HashSet<int3>> FeatStealthPositionsCache = [];

    internal static void NotifyFeatStealth(CharacterActionMoveStepBase action)
    {
        if (Gui.Battle == null)
        {
            return;
        }

        var actingCharacter = action.ActingCharacter;
        var rulesetCharacter = actingCharacter.RulesetCharacter;
        var rulesetHero = rulesetCharacter.GetOriginalHero();

        if (rulesetHero == null ||
            !rulesetHero.TrainedFeats.Contains(FeatStealthy))
        {
            return;
        }

        FeatStealthPositionsCache.TryAdd(actingCharacter, []);
        FeatStealthPositionsCache[actingCharacter] = [];

        for (var i = 0; i < action.MovePath.Count - 1; i++)
        {
            var position = action.MovePath[i].position;

            FeatStealthPositionsCache[actingCharacter].Add(position);
        }
    }

    #endregion

    #region Menacing

    private static FeatDefinitionWithPrerequisites BuildMenacing()
    {
        const string NAME = "FeatMenacing";

        var proficiencySkill = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Intimidation)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{NAME}Mark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetUsesFixed(ActivationTime.NoCost)
            .SetGuiPresentation(Category.Feature, Sprites.GetSprite(NAME, Resources.PowerMenacing, 128))
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetTargetingData(Side.Enemy, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetCasterEffectParameters(PowerBerserkerIntimidatingPresence)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new CustomBehaviorMenacing(condition));

        var feat = FeatDefinitionWithPrerequisitesBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Solasta, power, proficiencySkill)
            .AddToDB();

        return feat;
    }

    private sealed class CustomBehaviorMenacing(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMark) : IPowerOrSpellFinishedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var isValid = !target.RulesetActor.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, conditionMark.Name);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustNotHaveMenacingMark");

                return false;
            }

            isValid = target.RulesetCharacter is { CharacterFamily: "Humanoid" };

            if (isValid)
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeHumanoid");

            return false;
        }

        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var attacker = action.ActingCharacter;
            var defender = action.ActionParams.TargetCharacters[0];
            var rulesetAttacker = attacker.RulesetCharacter;
            var rulesetDefender = defender.RulesetActor;

            attacker.BurnOneMainAttack();

            if (!ResolveContest(attacker, defender))
            {
                rulesetDefender.InflictCondition(
                    conditionMark.Name,
                    DurationType.UntilAnyRest,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetAttacker.guid,
                    rulesetAttacker.CurrentFaction.Name,
                    1,
                    conditionMark.Name,
                    0,
                    0,
                    0);

                yield break;
            }

            rulesetDefender.InflictCondition(
                ConditionFrightened,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                ConditionFrightened,
                0,
                0,
                0);
        }

        private static bool ResolveContest(GameLocationCharacter actor, GameLocationCharacter opponent)
        {
            var actionModifierActor = new ActionModifier();
            var actionModifierOpponent = new ActionModifier();

            var abilityCheckBonusActor = actor.RulesetCharacter.ComputeBaseAbilityCheckBonus(
                AttributeDefinitions.Charisma,
                actionModifierActor.AbilityCheckModifierTrends, SkillDefinitions.Intimidation);

            var abilityCheckBonusOpponent = opponent.RulesetCharacter.ComputeBaseAbilityCheckBonus(
                AttributeDefinitions.Wisdom,
                actionModifierOpponent.AbilityCheckModifierTrends, SkillDefinitions.Insight);

            actor.ComputeAbilityCheckActionModifier(
                AttributeDefinitions.Charisma, SkillDefinitions.Intimidation, actionModifierActor);

            opponent.ComputeAbilityCheckActionModifier(
                AttributeDefinitions.Wisdom, SkillDefinitions.Insight, actionModifierOpponent);

            foreach (var key in actor.RulesetCharacter.GetFeaturesByType<IActionPerformanceProvider>())
            {
                foreach (var executionModifier in key.ActionExecutionModifiers)
                {
                    if (executionModifier.actionId != ActionDefinitions.Id.PowerNoCost ||
                        !actor.RulesetCharacter.IsMatchingEquipementCondition(executionModifier.equipmentContext) ||
                        executionModifier.advantageType == AdvantageType.None)
                    {
                        continue;
                    }

                    var num = executionModifier.advantageType == AdvantageType.Advantage ? 1 : -1;
                    var featureOrigin = actor.RulesetCharacter.FeaturesOrigin[(key as FeatureDefinition)!];

                    actionModifierActor.AbilityCheckAdvantageTrends.Add(
                        new TrendInfo(num, featureOrigin.sourceType, featureOrigin.sourceName, featureOrigin.source));
                }
            }

            actor.RulesetCharacter.ResolveContestCheck(
                abilityCheckBonusActor,
                actionModifierActor.AbilityCheckModifier,
                AttributeDefinitions.Charisma,
                SkillDefinitions.Intimidation,
                actionModifierActor.AbilityCheckAdvantageTrends,
                actionModifierActor.AbilityCheckModifierTrends,
                abilityCheckBonusOpponent,
                actionModifierOpponent.AbilityCheckModifier,
                AttributeDefinitions.Wisdom,
                SkillDefinitions.Insight,
                actionModifierOpponent.AbilityCheckAdvantageTrends,
                actionModifierOpponent.AbilityCheckModifierTrends,
                opponent.RulesetCharacter,
                out var outcome);

            return outcome is RollOutcome.Success or RollOutcome.CriticalSuccess;
        }
    }

    #endregion

    #region Baleful Scion

    private static FeatDefinition BuildBalefulScion(List<FeatDefinition> feats)
    {
        const string NAME = "FeatBalefulScion";

        var powerBalefulScion = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .DelegatedToAction()
            .AddToDB();

        var additionalDamageBalefulScion = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("BalefulScion")
            .SetDamageDice(DieType.D6, 1)
            .SetSpecificDamageType(DamageTypeNecrotic)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .SetImpactParticleReference(PowerWightLordRetaliate)
            .AddToDB();

        var conditionBalefulScion = ConditionDefinitionBuilder
            .Create($"Condition{NAME}")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(additionalDamageBalefulScion)
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create(DatabaseHelper.ActionDefinitions.MetamagicToggle, "BalefulScionToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.BalefulScionToggle)
            .SetActivatedPower(powerBalefulScion)
            .AddToDB();

        var actionAffinityBalefulScion = FeatureDefinitionActionAffinityBuilder
            .Create(ActionAffinitySorcererMetamagicToggle,
                "ActionAffinityBalefulScionToggle")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((ActionDefinitions.Id)ExtraActionId.BalefulScionToggle)
            .AddCustomSubFeatures(
                new CustomBehaviorBalefulScion(powerBalefulScion, conditionBalefulScion, additionalDamageBalefulScion),
                new ValidateDefinitionApplication(ValidatorsCharacter.HasAvailablePowerUsage(powerBalefulScion)))
            .AddToDB();

        var featStr = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Einar,
                actionAffinityBalefulScion, additionalDamageBalefulScion, powerBalefulScion)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .SetFeatFamily(NAME)
            .AddToDB();

        var featDex = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}Dex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Misaye,
                actionAffinityBalefulScion, additionalDamageBalefulScion, powerBalefulScion)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .SetFeatFamily(NAME)
            .AddToDB();

        var featCon = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Arun,
                actionAffinityBalefulScion, additionalDamageBalefulScion, powerBalefulScion)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .SetFeatFamily(NAME)
            .AddToDB();

        var featInt = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}Int")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Pakri,
                actionAffinityBalefulScion, additionalDamageBalefulScion, powerBalefulScion)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .SetFeatFamily(NAME)
            .AddToDB();

        var featWis = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}Wis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Maraike,
                actionAffinityBalefulScion, additionalDamageBalefulScion, powerBalefulScion)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .SetFeatFamily(NAME)
            .AddToDB();

        var featCha = FeatDefinitionWithPrerequisitesBuilder
            .Create($"{NAME}Cha")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                AttributeModifierCreed_Of_Solasta,
                actionAffinityBalefulScion, additionalDamageBalefulScion, powerBalefulScion)
            .SetValidators(ValidatorsFeat.IsLevel4)
            .SetFeatFamily(NAME)
            .AddToDB();

        feats.AddRange(featStr, featDex, featCon, featInt, featWis, featCha);

        return GroupFeats.MakeGroupWithPreRequisite(
            "FeatGroupBalefulScion", NAME, ValidatorsFeat.IsLevel4,
            featStr, featDex, featCon, featInt, featWis, featCha);
    }

    private class CustomBehaviorBalefulScion(
        FeatureDefinitionPower powerBalefulScion,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionBalefulScion,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionAdditionalDamage additionalDamageBalefulScion)
        : IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy, IModifyAdditionalDamage,
            IActionFinishedByMe
    {
        private bool _isCritical;

        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not (CharacterActionAttack or CharacterActionMagicEffect))
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            if (!rulesetCharacter.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionBalefulScion.Name, out var activeCondition))
            {
                yield break;
            }

            rulesetCharacter.RemoveCondition(activeCondition);

            var roll = RollDie(DieType.D6, AdvantageType.None, out _, out _);
            var healAmount =
                (roll * (_isCritical ? 2 : 1)) +
                rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            rulesetCharacter.ReceiveHealing(healAmount, true, rulesetCharacter.Guid);
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
            _isCritical = criticalHit;

            if (!rulesetEffect.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            yield return HandleBalefulScion(attacker, defender);
        }

        public void ModifyAdditionalDamage(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            List<EffectForm> actualEffectForms,
            ref DamageForm damageForm)
        {
            if (featureDefinitionAdditionalDamage != additionalDamageBalefulScion)
            {
                return;
            }

            damageForm.BonusDamage =
                attacker.RulesetCharacter.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
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
            _isCritical = criticalHit;

            if (!attackMode.EffectDescription.HasFormOfType(EffectForm.EffectFormType.Damage))
            {
                yield break;
            }

            yield return HandleBalefulScion(attacker, defender);
        }

        private IEnumerator HandleBalefulScion(GameLocationCharacter attacker, GameLocationCharacter defender)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!attacker.IsWithinRange(defender, 12) ||
                !attacker.OncePerTurnIsValid(additionalDamageBalefulScion.Name) ||
                !rulesetAttacker.IsToggleEnabled((ActionDefinitions.Id)ExtraActionId.BalefulScionToggle) ||
                rulesetAttacker.GetRemainingPowerUses(powerBalefulScion) == 0)
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(powerBalefulScion, rulesetAttacker);

            usablePower.Consume();
            rulesetAttacker.InflictCondition(
                conditionBalefulScion.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionBalefulScion.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Dungeon Delver

    private static FeatDefinition BuildDungeonDelver()
    {
        const string Name = "FeatDungeonDelver";

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAbilityCheckAffinityBuilder
                    .Create($"AbilityCheckAffinity{Name}")
                    .SetGuiPresentation(Name, Category.Feat, Gui.NoLocalization)
                    .BuildAndSetAffinityGroups(
                        CharacterAbilityCheckAffinity.Advantage, DieType.D1, 0,
                        AbilityCheckGroupOperation.AddDie,
                        (AttributeDefinitions.Wisdom, SkillDefinitions.Perception,
                            AbilityCheckContext.GadgetInteraction),
                        (AttributeDefinitions.Intelligence, SkillDefinitions.Investigation,
                            AbilityCheckContext.GadgetInteraction))
                    .AddCustomSubFeatures(
                        new CustomBehaviorDungeonDelver(
                            ConditionDefinitionBuilder
                                .Create($"Condition{Name}")
                                .SetGuiPresentation(Name, Category.Feat, Gui.NoLocalization)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetFeatures(
                                    DamageAffinityAcidResistance,
                                    DamageAffinityBludgeoningResistance,
                                    DamageAffinityColdResistance,
                                    DamageAffinityFireResistance,
                                    DamageAffinityForceDamageResistance,
                                    DamageAffinityLightningResistance,
                                    DamageAffinityNecroticResistance,
                                    DamageAffinityPiercingResistance,
                                    DamageAffinityPoisonResistance,
                                    DamageAffinityPsychicResistance,
                                    DamageAffinityRadiantResistance,
                                    DamageAffinitySlashingResistance,
                                    DamageAffinityThunderResistance)
                                .SetSpecialInterruptions(ExtraConditionInterruption.AfterWasAttacked)
                                .AddToDB()))
                    .AddToDB())
            .AddFeatures(
                DatabaseRepository
                    .GetDatabase<TerrainTypeDefinition>()
                    .Select(terrainType =>
                        FeatureDefinitionTerrainTypeAffinityBuilder
                            .Create($"TerrainTypeAffinity{Name}{terrainType.Name}")
                            .SetGuiPresentation(Name, Category.Feat, Gui.NoLocalization)
                            .IgnoreTravelPacePerceptionMalus(terrainType.Name)
                            .AddToDB())
                    .Cast<FeatureDefinition>()
                    .ToArray())
            .AddToDB();
    }

    private sealed class CustomBehaviorDungeonDelver(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionResistance) : IRollSavingThrowInitiated
    {
        public void OnSavingThrowInitiated(
            RulesetCharacter caster,
            RulesetCharacter defender,
            ref int saveBonus,
            ref string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            ref int rollModifier,
            ref int saveDC,
            ref bool hasHitVisual,
            RollOutcome outcome,
            int outcomeDelta,
            List<EffectForm> effectForms)
        {
            if (caster is RulesetCharacterHero or RulesetCharacterMonster)
            {
                return;
            }

            advantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.Condition, conditionResistance.Name, conditionResistance));

            defender.InflictCondition(
                conditionResistance.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                defender.guid,
                defender.CurrentFaction.Name,
                1,
                conditionResistance.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Acrobat

    private static FeatDefinition BuildAcrobat()
    {
        const string Name = "FeatAcrobat";

        var movementAffinity = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetImmunities(difficultTerrainImmunity: true)
            .AddToDB();

        var condition = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Name, Category.Feat, ConditionDefinitions.ConditionPassWithoutTrace)
            .SetFeatures(movementAffinity)
            .AddToDB();

        condition.GuiPresentation.Description = Gui.NoLocalization;

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Category.Feature,
                Sprites.GetSprite(Name, Resources.PowerEleganceDisengage, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddCustomSubFeatures(new PowerOrSpellFinishedByMeAcrobat(condition))
            .AddToDB();

        var skill = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Acrobatics)
            .AddToDB();

        return FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, power, skill)
            .AddToDB();
    }

    private sealed class PowerOrSpellFinishedByMeAcrobat(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionAcrobat) : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var actionModifier = new ActionModifier();

            rulesetCharacter.ComputeBaseAbilityCheckBonus(
                AttributeDefinitions.Dexterity, actionModifier.AbilityCheckModifierTrends, SkillDefinitions.Acrobatics);

            actingCharacter.ComputeAbilityCheckActionModifier(
                AttributeDefinitions.Dexterity, SkillDefinitions.Acrobatics, actionModifier);

            actingCharacter.RollAbilityCheck(
                AttributeDefinitions.Dexterity, SkillDefinitions.Acrobatics, 15,
                AdvantageType.None, actionModifier, false, -1, out var outcome, out _, true);

            if (outcome is not (RollOutcome.Success or RollOutcome.CriticalSuccess))
            {
                var battleService = ServiceRepository.GetService<IGameLocationBattleService>();

                yield return battleService.HandleFailedAbilityCheck(action, actingCharacter, actionModifier);
            }

            if (outcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                rulesetCharacter.InflictCondition(
                    conditionAcrobat.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    rulesetCharacter.guid,
                    rulesetCharacter.CurrentFaction.Name,
                    1,
                    conditionAcrobat.Name,
                    0,
                    0,
                    0);
            }
        }
    }

    #endregion

    #region Athlete

    internal static FeatDefinition FeatAthleteStr { get; private set; }
    internal static FeatDefinition FeatAthleteDex { get; private set; }

    private static FeatDefinition BuildAthlete(List<FeatDefinition> feats)
    {
        const string Name = "Athlete";

        var movementAffinity = FeatureDefinitionMovementAffinityBuilder
            .Create($"MovementAffinity{Name}")
            .SetGuiPresentationNoContent(true)
            .SetClimbing(true)
            .AddToDB();

        var skill = FeatureDefinitionProficiencyBuilder
            .Create($"Proficiency{Name}")
            .SetGuiPresentationNoContent(true)
            .SetProficiencies(ProficiencyType.SkillOrExpertise, SkillDefinitions.Athletics)
            .AddToDB();


        FeatAthleteStr = FeatDefinitionBuilder
            .Create($"Feat{Name}Str")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Einar, movementAffinity, skill)
            .SetFeatFamily(Name)
            .AddToDB();

        FeatAthleteDex = FeatDefinitionBuilder
            .Create($"Feat{Name}Dex")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Misaye, movementAffinity, skill)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(FeatAthleteStr, FeatAthleteDex);

        return GroupFeats.MakeGroup(
            "FeatGroupAthlete", Name, FeatAthleteStr, FeatAthleteDex);
    }

    #endregion

    #region Alert

    private const string FeatAlertName = "FeatAlert";

    internal static readonly FeatDefinition FeatAlert = FeatDefinitionBuilder
        .Create(FeatAlertName)
        .SetGuiPresentation(Category.Feat)
        .AddFeatures(
            FeatureDefinitionAttributeModifierBuilder
                .Create($"AttributeModifier{FeatAlertName}Initiative")
                .SetGuiPresentationNoContent(true)
                .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.Initiative, 5)
                .AddToDB(),
            FeatureDefinitionConditionAffinityBuilder
                .Create($"ConditionAffinity{FeatAlertName}Surprised")
                .SetGuiPresentationNoContent(true)
                .SetConditionAffinityType(ConditionAffinityType.Immunity)
                .SetConditionType(ConditionDefinitions.ConditionSurprised)
                .AddToDB())
        .AddToDB();

    #endregion

    #region Chef

    private static FeatDefinition BuildChef(List<FeatDefinition> feats)
    {
        const string Name = "FeatChef";

        var powerTreat = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Treat")
            .SetGuiPresentation($"Item{Name}Treat", Category.Item, PowerFunctionGoodberryHealing)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.None)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilAnyRest)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(5)
                            .Build())
                    .SetEffectEffectParameters(Goodberry)
                    .Build())
            .AddToDB();

        var itemTreat = ItemDefinitionBuilder
            .Create(ItemDefinitions.Berry_Ration, $"Item{Name}Treat")
            .SetOrUpdateGuiPresentation(Category.Item)
            .SetUsableDeviceDescription(powerTreat)
            .AddToDB();

        var powerCookTreat = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CookTreat")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealingOther)
            .SetUsesFixed(ActivationTime.Hours1, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.UntilAnyRest, 8)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonItemForm(itemTreat, 1)
                            .Build())
                    .SetEffectEffectParameters(Goodberry)
                    .Build())
            .AddToDB();

        powerCookTreat.AddCustomSubFeatures(new ModifyEffectDescriptionCookTreat(powerCookTreat));

        var powerCookMeal = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}CookMeal")
            .SetGuiPresentation(Category.Feature, PowerFunctionGoodberryHealingOther)
            .SetUsesFixed(ActivationTime.Hours1, RechargeRate.ShortRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique, 6)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(HealingComputation.Dice, 0, DieType.D8, 1, false,
                                HealingCap.MaximumHitPoints)
                            .Build())
                    .SetEffectEffectParameters(Goodberry)
                    .Build())
            .AddToDB();

        var featCon = FeatDefinitionBuilder
            .Create($"{Name}Con")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Arun, powerCookTreat, powerCookMeal)
            .SetFeatFamily(Name)
            .AddToDB();

        var featWis = FeatDefinitionBuilder
            .Create($"{Name}Wis")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(AttributeModifierCreed_Of_Maraike, powerCookTreat, powerCookMeal)
            .SetFeatFamily(Name)
            .AddToDB();

        feats.AddRange(featWis, featCon);

        return GroupFeats.MakeGroup("FeatGroupChef", Name, featCon, featWis);
    }

    private sealed class ModifyEffectDescriptionCookTreat(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerCookTreat) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == powerCookTreat;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.EffectForms[0].SummonForm.number =
                character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

            return effectDescription;
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
                .SetFeatures(
                    FeatureDefinitionDieRollModifierBuilder
                        .Create($"DieRollModifierDamageTypeDependent{NAME}{damageType}")
                        .SetGuiPresentation(guiPresentation)
                        .SetModifiers(RollContext.AttackDamageValueRoll, 1, 1, 1,
                            "Feature/&DieRollModifierFeatElementalAdeptReroll")
                        .AddCustomSubFeatures(new ModifyDamageResistanceElementalAdept(damageType))
                        .AddToDB(),
                    FeatureDefinitionDieRollModifierBuilder
                        .Create($"DieRollModifierDamageTypeDependent{NAME}{damageType}Magic")
                        .SetGuiPresentation(guiPresentation)
                        .SetModifiers(RollContext.MagicDamageValueRoll, 1, 1, 1,
                            "Feature/&DieRollModifierFeatElementalAdeptReroll")
                        .AddCustomSubFeatures(new ModifyDamageResistanceElementalAdept(damageType))
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

    private sealed class ModifyDamageResistanceElementalAdept : IModifyDamageAffinity, IValidateDieRollModifier
    {
        private readonly List<string> _damageTypes = [];

        public ModifyDamageResistanceElementalAdept(params string[] damageTypes)
        {
            _damageTypes.AddRange(damageTypes);
        }

        public void ModifyDamageAffinity(RulesetActor attacker, RulesetActor defender, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider { DamageAffinityType: DamageAffinityType.Resistance } y &&
                _damageTypes.Contains(y.DamageType));
        }

        public bool CanModifyRoll(RulesetCharacter character, List<FeatureDefinition> features,
            List<string> damageTypes)
        {
            return _damageTypes.Intersect(damageTypes).Any();
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
                    FeatureDefinitionDieRollModifierBuilder
                        .Create($"DieRollModifierDamageTypeDependent{NAME}{damageType}")
                        .SetGuiPresentation(guiPresentation)
                        .SetModifiers(RollContext.AttackRoll, 1, 1, 1,
                            "Feature/&DieRollModifierFeatElementalMasterReroll")
                        .AddCustomSubFeatures(new ModifyDamageResistanceElementalMaster(damageType))
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

    private sealed class ModifyDamageResistanceElementalMaster : IModifyDamageAffinity, IValidateDieRollModifier
    {
        private readonly List<string> _damageTypes = [];

        public ModifyDamageResistanceElementalMaster(params string[] damageTypes)
        {
            _damageTypes.AddRange(damageTypes);
        }

        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider { DamageAffinityType: DamageAffinityType.Immunity } y &&
                _damageTypes.Contains(y.DamageType));
        }

        public bool CanModifyRoll(RulesetCharacter character, List<FeatureDefinition> features,
            List<string> damageTypes)
        {
            return _damageTypes.Intersect(damageTypes).Any();
        }
    }

    #endregion

    #region Fighting Initiate

    private static FeatDefinitionWithPrerequisites BuildFeatFromFightingStyle(string fightingStyleName)
    {
        var db = DatabaseRepository.GetDatabase<FightingStyleDefinition>();
        var feat = BuildFightingStyleFeat(db.GetElement(fightingStyleName));

        feat.Validators.Clear();
        feat.familyTag = string.Empty;
        feat.hasFamilyTag = false;

        return feat;
    }

    private static FeatDefinition BuildFightingInitiate()
    {
        var fightingStyleFeats = DatabaseRepository
            .GetDatabase<FightingStyleDefinition>()
            .Where(x =>
                x.ContentPack == CeContentPackContext.CeContentPack &&
                !FightingStyleContext.DemotedFightingStyles.Contains(x.Name))
            .Select(BuildFightingStyleFeat)
            .OfType<FeatDefinition>()
            .ToArray();

        var vanillaFightingStyleFeats = DatabaseRepository
            .GetDatabase<FightingStyleDefinition>()
            .Where(x => x.ContentPack != CeContentPackContext.CeContentPack)
            .Select(BuildFightingStyleFeat)
            .OfType<FeatDefinition>()
            .ToArray();

        GroupFeats.FeatGroupFightingStyle.AddFeats(fightingStyleFeats);
        GroupFeats.FeatGroupFightingStyle.AddFeats(vanillaFightingStyleFeats);

        return GroupFeats.FeatGroupFightingStyle;
    }

    private static FeatDefinitionWithPrerequisites BuildFightingStyleFeat(FightingStyleDefinition fightingStyle)
    {
        // we need a brand new one to avoid issues with FS getting hidden
        var guiPresentation = new GuiPresentation(fightingStyle.GuiPresentation);
        var feat = FeatDefinitionWithPrerequisitesBuilder
            .Create($"Feat{fightingStyle.Name}")
            .SetGuiPresentation(guiPresentation)
            .SetFeatures(
                FeatureDefinitionProficiencyBuilder
                    .Create($"ProficiencyFeat{fightingStyle.Name}")
                    .SetProficiencies(ProficiencyType.FightingStyle, fightingStyle.Name)
                    .SetGuiPresentation(guiPresentation)
                    .AddToDB())
            .SetFeatFamily(GroupFeats.FightingStyle)
            .SetValidators(ValidatorsFeat.ValidateNotFightingStyle(fightingStyle))
            .AddToDB();

        // supports custom pools [only superior technique now]
        feat.Features.AddRange(fightingStyle.Features.OfType<FeatureDefinitionCustomInvocationPool>());

        if (!Main.Settings.FightingStyleEnabled.Contains(fightingStyle.Name))
        {
            guiPresentation.hidden = true;
        }

        return feat;
    }

    #endregion

    #region Gift of the Chromatic Dragon

    private static FeatDefinition BuildGiftOfTheChromaticDragon()
    {
        const string Name = "GiftOfTheChromaticDragon";

        (string, IMagicEffect)[] damagesAndEffects =
        [
            (DamageTypeAcid, AcidSplash),
            (DamageTypeCold, ConeOfCold),
            (DamageTypeFire, FireBolt),
            (DamageTypeLightning, LightningBolt),
            (DamageTypePoison, PoisonSpray)
        ];

        var dbDamageAffinities = DatabaseRepository.GetDatabase<FeatureDefinitionDamageAffinity>();

        // Chromatic Infusion

        var powersChromaticInfusion = new List<FeatureDefinitionPower>();
        var powerChromaticInfusion = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ChromaticInfusion")
            .SetGuiPresentation(Category.Feature, PowerDomainElementalLightningBlade)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetDurationData(DurationType.Minute, 1)
                    .Build())
            .AddToDB();

        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var (damageType, magicEffect) in damagesAndEffects)
        {
            var damageTitle = Gui.Localize($"Rules/&{damageType}Title");
            var title = "PowerGiftOfTheChromaticDragonDamageTitle".Formatted(Category.Feature, damageTitle);
            var description = "PowerGiftOfTheChromaticDragonDamageDescription".Formatted(Category.Feature, damageTitle);

            var power = FeatureDefinitionPowerSharedPoolBuilder
                .Create($"Power{Name}{damageType}")
                .SetGuiPresentation(title, description)
                .SetSharedPool(ActivationTime.BonusAction, powerChromaticInfusion)
                .SetEffectDescription(EffectDescriptionBuilder.Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.Item,
                        itemSelectionType: ActionDefinitions.ItemSelectionType.Weapon)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetItemPropertyForm(
                                ItemPropertyUsage.Unlimited, 0,
                                new FeatureUnlockByLevel(
                                    FeatureDefinitionAdditionalDamageBuilder
                                        .Create($"AttackModifier{Name}{damageType}")
                                        .SetGuiPresentation(title, description,
                                            ConditionDefinitions.ConditionBrandingSmite)
                                        .SetNotificationTag($"ChromaticInfusion{damageType}")
                                        .SetDamageDice(DieType.D4, 1)
                                        .SetSpecificDamageType(damageType)
                                        .SetImpactParticleReference(magicEffect)
                                        .AddToDB(),
                                    0))
                            .Build())
                    .Build())
                .AddToDB();

            power.GuiPresentation.hidden = true;
            powersChromaticInfusion.Add(power);

            // use same loop to create Reactive Resistance conditions
            var damageTypeAb = damageType.Replace("Damage", string.Empty);

            var condition = ConditionDefinitionBuilder
                .Create($"Condition{Name}{damageType}")
                .SetGuiPresentation($"Power{Name}ReactiveResistance", Category.Feature,
                    ConditionDefinitions.ConditionProtectedInsideMagicCircle, hidden: true)
                .SetPossessive()
                .SetFeatures(dbDamageAffinities.GetElement($"DamageAffinity{damageTypeAb}Resistance"))
                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                .AddToDB();

            condition.GuiPresentation.description = Gui.NoLocalization;
        }

        PowerBundle.RegisterPowerBundle(powerChromaticInfusion, false, powersChromaticInfusion);

        // Reactive Resistance

        var powerReactiveResistance = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ReactiveResistance")
            .SetGuiPresentation(Category.Feature, hidden: true)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetCasterEffectParameters(PowerDispelEvilBreakEnchantment)
                    .Build())
            .AddToDB();

        powerReactiveResistance.AddCustomSubFeatures(new CustomBehaviorReactiveResistance(powerReactiveResistance));

        return FeatDefinitionBuilder
            .Create($"Feat{Name}")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(powerChromaticInfusion, powerReactiveResistance)
            .AddFeatures(powersChromaticInfusion.OfType<FeatureDefinition>().ToArray())
            .AddToDB();
    }

    private sealed class CustomBehaviorReactiveResistance(FeatureDefinitionPower powerReactiveResistance)
        : ITryAlterOutcomeAttack, IMagicEffectBeforeHitConfirmedOnMe
    {
        private static readonly HashSet<string> DamageTypes =
            [DamageTypeAcid, DamageTypeCold, DamageTypeFire, DamageTypeLightning, DamageTypePoison];

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
            if (rulesetEffect.EffectDescription.RangeType is not (RangeType.MeleeHit or RangeType.RangeHit))
            {
                yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
            }
        }

        public int HandlerPriority => 10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager ||
                helper != defender)
            {
                yield break;
            }

            var actualEffectForms =
                attackMode?.EffectDescription.EffectForms ?? rulesetEffect?.EffectDescription.EffectForms ?? [];

            yield return HandleReaction(battleManager, attacker, defender, actualEffectForms);
        }

        private IEnumerator HandleReaction(
            GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            // ReSharper disable once ParameterTypeCanBeEnumerable.Local
            List<EffectForm> actualEffectForms)
        {
            var effectForm = actualEffectForms
                .FirstOrDefault(x =>
                    x.FormType == EffectForm.EffectFormType.Damage &&
                    DamageTypes.Contains(x.DamageForm.DamageType));

            if (effectForm == null)
            {
                yield break;
            }

            var rulesetDefender = defender.RulesetCharacter;

            if (!defender.CanReact() ||
                rulesetDefender.GetRemainingPowerUses(powerReactiveResistance) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var damageType = effectForm.DamageForm.DamageType;
            var damageTitle = Gui.Localize($"Rules/&{damageType}Title");
            var usablePower = PowerProvider.Get(powerReactiveResistance, rulesetDefender);
            var reactionParams =
                new CharacterActionParams(defender, ActionDefinitions.Id.PowerReaction)
                {
                    StringParameter = "ReactiveResistance",
                    StringParameter2 = "UseReactiveResistanceDescription".Formatted(
                        Category.Reaction, attacker.Name, damageTitle),
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetDefender, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { defender }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", defender);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var conditionName = $"ConditionGiftOfTheChromaticDragon{damageType}";

            rulesetDefender.InflictCondition(
                conditionName,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetDefender.guid,
                rulesetDefender.CurrentFaction.Name,
                1,
                conditionName,
                0,
                0,
                0);
        }
    }

    #endregion

    #region Healer

    private static FeatDefinition BuildHealer()
    {
        var spriteMedKit = Sprites.GetSprite("PowerMedKit", Resources.PowerMedKit, 256, 128);
        var powerFeatHealerMedKit = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerMedKit")
            .SetGuiPresentation(Category.Feature, spriteMedKit)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(
                                HealingComputation.Dice,
                                4,
                                DieType.D6,
                                1,
                                false,
                                HealingCap.MaximumHitPoints)
                            .Build())
                    .SetParticleEffectParameters(MagicWeapon)
                    .Build())
            .AddToDB();

        powerFeatHealerMedKit.AddCustomSubFeatures(new ModifyEffectDescriptionMedKit(powerFeatHealerMedKit));

        var spriteResuscitate = Sprites.GetSprite("PowerResuscitate", Resources.PowerResuscitate, 256, 128);
        var powerFeatHealerResuscitate = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerResuscitate")
            .SetGuiPresentation(Category.Feature, spriteResuscitate)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetTargetFiltering(
                        TargetFilteringMethod.CharacterOnly,
                        TargetFilteringTag.No,
                        5,
                        DieType.D8)
                    .SetRequiredCondition(ConditionDefinitions.ConditionDead)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetReviveForm(12, ReviveHitPoints.One)
                            .Build())
                    .SetParticleEffectParameters(MagicWeapon)
                    .Build())
            .AddToDB();

        var spriteStabilize = Sprites.GetSprite("PowerStabilize", Resources.PowerStabilize, 256, 128);
        var powerFeatHealerStabilize = FeatureDefinitionPowerBuilder
            .Create("PowerFeatHealerStabilize")
            .SetGuiPresentation(Category.Feature, spriteStabilize)
            .SetUsesAbilityBonus(ActivationTime.Action, RechargeRate.LongRest, AttributeDefinitions.Wisdom)
            .SetEffectDescription(SpareTheDying.EffectDescription)
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
                proficiencyFeatHealerMedicine,
                powerFeatHealerMedKit,
                powerFeatHealerResuscitate,
                powerFeatHealerStabilize,
                proficiencyFeatHealerMedicine)
            .AddToDB();
    }

    private sealed class ModifyEffectDescriptionMedKit(BaseDefinition baseDefinition) : IModifyEffectDescription
    {
        public bool IsValid(BaseDefinition definition, RulesetCharacter character, EffectDescription effectDescription)
        {
            return definition == baseDefinition;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var characterLevel = character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            var medicineBonus = character
                .ComputeBaseAbilityCheckBonus(
                    AttributeDefinitions.Wisdom, rulesetEffect?.MagicAttackTrends, "Medicine");

            effectDescription.EffectForms[0].HealingForm.bonusHealing = characterLevel + medicineBonus;

            return effectDescription;
        }
    }

    #endregion

    #region Lucky

    private static FeatDefinition BuildFeatLucky()
    {
        const string Name = "FeatLucky";

        var power = FeatureDefinitionPowerBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentation(Name, Category.Feat, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest, 1, 3)
            .SetShowCasting(false)
            .AddToDB();

        power.AddCustomSubFeatures(new CustomBehaviorLucky(power));

        var feat = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(power)
            .AddToDB();

        return feat;
    }

    private sealed class CustomBehaviorLucky(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower powerLucky)
        : ITryAlterOutcomeAttack, ITryAlterOutcomeAttributeCheck, ITryAlterOutcomeSavingThrow, IRollSavingThrowFinished
    {
        private int _modifier;

        private int _saveDC;

        public void OnSavingThrowFinished(
            RulesetCharacter caster,
            RulesetCharacter defender,
            int saveBonus,
            string abilityScoreName,
            BaseDefinition sourceDefinition,
            List<TrendInfo> modifierTrends,
            List<TrendInfo> advantageTrends,
            int rollModifier,
            int saveDC,
            bool hasHitVisual,
            ref RollOutcome outcome,
            ref int outcomeDelta,
            List<EffectForm> effectForms)
        {
            _saveDC = saveDC;
            _modifier = saveBonus + rollModifier;
        }

        public int HandlerPriority => -10;

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            RulesetEffect rulesetEffect)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerLucky, rulesetHelper);

            if (rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            string stringParameter;

            if (helper == attacker &&
                action.AttackRollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                stringParameter = "LuckyAttack";
            }
            else if (helper == defender &&
                     action.AttackRollOutcome is RollOutcome.Success or RollOutcome.CriticalSuccess)
            {
                stringParameter = "LuckyEnemyAttack";
            }
            else
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var reactionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerNoCost)
                {
                    StringParameter = stringParameter,
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { helper }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _, out _);
            var previousRoll = action.AttackRoll;

            switch (stringParameter)
            {
                case "LuckyAttack" when dieRoll <= action.AttackRoll:
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feat/&FeatLuckyTitle",
                        "Feedback/&IsNotLuckyLower",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Positive, action.AttackRoll.ToString())
                        ]);

                    yield break;
                case "LuckyEnemyAttack" when dieRoll >= action.AttackRoll:
                    rulesetHelper.LogCharacterActivatesAbility(
                        "Feat/&FeatLuckyTitle",
                        "Feedback/&IsNotLuckyHigher",
                        extra:
                        [
                            (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()),
                            (ConsoleStyleDuplet.ParameterType.Negative, action.AttackRoll.ToString())
                        ]);

                    yield break;
            }

            action.AttackSuccessDelta += dieRoll - action.AttackRoll;
            action.AttackRoll = dieRoll;

            if (action.AttackSuccessDelta >= 0)
            {
                action.AttackRollOutcome = dieRoll == 20 ? RollOutcome.CriticalSuccess : RollOutcome.Success;
            }
            else
            {
                action.AttackRollOutcome = dieRoll == 1 ? RollOutcome.CriticalFailure : RollOutcome.Failure;
            }

            rulesetHelper.LogCharacterActivatesAbility(
                "Feat/&FeatLuckyTitle",
                "Feedback/&LuckyAttackToHitRoll",
                extra:
                [
                    (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        dieRoll.ToString()),
                    (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        previousRoll.ToString())
                ]);
        }

        public IEnumerator OnTryAlterAttributeCheck(
            GameLocationBattleManager battleManager,
            AbilityCheckData abilityCheckData,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier abilityCheckModifier)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerLucky, rulesetHelper);

            if (abilityCheckData.AbilityCheckRoll == 0 ||
                abilityCheckData.AbilityCheckRollOutcome != RollOutcome.Failure ||
                helper != defender ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var reactionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerNoCost)
                {
                    StringParameter = "LuckyCheck",
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { helper }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", helper);

            yield return battleManager.WaitForReactions(defender, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _, out _);
            var previousRoll = abilityCheckData.AbilityCheckRoll;

            if (dieRoll <= abilityCheckData.AbilityCheckRoll)
            {
                rulesetHelper.LogCharacterActivatesAbility(
                    "Feat/&FeatLuckyTitle",
                    "Feedback/&IsNotLuckyLower",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Positive, abilityCheckData.AbilityCheckRoll.ToString())
                    ]);

                yield break;
            }

            abilityCheckData.AbilityCheckSuccessDelta += dieRoll - abilityCheckData.AbilityCheckRoll;
            abilityCheckData.AbilityCheckRoll = dieRoll;
            abilityCheckData.AbilityCheckRollOutcome = abilityCheckData.AbilityCheckSuccessDelta >= 0
                ? RollOutcome.Success
                : RollOutcome.Failure;

            rulesetHelper.LogCharacterActivatesAbility(
                "Feat/&FeatLuckyTitle",
                "Feedback/&LuckyCheckToHitRoll",
                extra:
                [
                    (dieRoll > previousRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        dieRoll.ToString()),
                    (previousRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        previousRoll.ToString())
                ]);
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier saveModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var rulesetHelper = helper.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerLucky, rulesetHelper);

            if (!action.RolledSaveThrow ||
                action.SaveOutcome != RollOutcome.Failure ||
                helper != defender ||
                rulesetHelper.GetRemainingUsesOfPower(usablePower) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var reactionParams =
                new CharacterActionParams(helper, ActionDefinitions.Id.PowerNoCost)
                {
                    StringParameter = "LuckySaving",
                    StringParameter2 = "UseLuckySavingDescription".Formatted(
                        Category.Reaction, defender.Name, attacker.Name, helper.Name),
                    ActionModifiers = { new ActionModifier() },
                    RulesetEffect = implementationManager
                        .MyInstantiateEffectPower(rulesetHelper, usablePower, false),
                    UsablePower = usablePower,
                    TargetCharacters = { attacker }
                };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToUsePower(reactionParams, "UsePower", attacker);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var dieRoll = rulesetHelper.RollDie(DieType.D20, RollContext.None, false, AdvantageType.None, out _, out _);
            var savingRoll = action.SaveOutcomeDelta - _modifier + _saveDC;

            if (dieRoll <= savingRoll)
            {
                rulesetHelper.LogCharacterActivatesAbility(
                    "Feat/&FeatLuckyTitle",
                    "Feedback/&IsNotLuckyLower",
                    extra:
                    [
                        (ConsoleStyleDuplet.ParameterType.Negative, dieRoll.ToString()),
                        (ConsoleStyleDuplet.ParameterType.Positive, savingRoll.ToString())
                    ]);

                yield break;
            }

            action.SaveOutcomeDelta += dieRoll - savingRoll;
            action.SaveOutcome = action.SaveOutcomeDelta >= 0 ? RollOutcome.Success : RollOutcome.Failure;

            rulesetHelper.LogCharacterActivatesAbility(
                "Feat/&FeatLuckyTitle",
                "Feedback/&LuckySavingToHitRoll",
                extra:
                [
                    (dieRoll > savingRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        dieRoll.ToString()),
                    (savingRoll > dieRoll ? ConsoleStyleDuplet.ParameterType.Positive : ConsoleStyleDuplet.ParameterType.Negative,
                        savingRoll.ToString())
                ]);
        }
    }

    #endregion

    #region Mage Slayer

    private const string FeatMageSlayerName = "FeatMageSlayer";

    private static readonly FeatureDefinitionPower PowerMageSlayerSaving = FeatureDefinitionPowerBuilder
        .Create($"Power{FeatMageSlayerName}Saving")
        .SetGuiPresentation(FeatMageSlayerName, Category.Feat, hidden: true)
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.LongRest)
        .AddToDB();

    internal static readonly FeatDefinition FeatMageSlayer = FeatDefinitionBuilder
        .Create(FeatMageSlayerName)
        .SetGuiPresentation(FeatMageSlayerName, Category.Feat)
        .AddFeatures(
            PowerMageSlayerSaving,
            FeatureDefinitionBuilder
                .Create($"Feature{FeatMageSlayerName}")
                .SetGuiPresentationNoContent(true)
                .AddCustomSubFeatures(new CustomBehaviorMageSlayer(
                    ConditionDefinitionBuilder
                        .Create($"Condition{FeatMageSlayerName}")
                        .SetGuiPresentation(FeatMageSlayerName, Category.Feat, Gui.NoLocalization)
                        .SetSilent(Silent.WhenAddedOrRemoved)
                        .AddFeatures(
                            FeatureDefinitionMagicAffinityBuilder
                                .Create($"MagicAffinity{FeatMageSlayerName}")
                                .SetGuiPresentation(FeatMageSlayerName, Category.Feat)
                                .SetConcentrationModifiers(ConcentrationAffinity.Disadvantage)
                                .AddToDB())
                        .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                        .AddToDB()))
                .AddToDB())
        .AddToDB();

    internal sealed class CustomBehaviorMageSlayer(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionConcentrationDisadvantage)
        : IMagicEffectBeforeHitConfirmedOnEnemy, IPhysicalAttackBeforeHitConfirmedOnEnemy, ITryAlterOutcomeSavingThrow
    {
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

            defender.RulesetActor.InflictCondition(
                conditionConcentrationDisadvantage.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionConcentrationDisadvantage.Name,
                0,
                0,
                0);

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
            var rulesetAttacker = attacker.RulesetCharacter;

            defender.RulesetActor.InflictCondition(
                conditionConcentrationDisadvantage.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionConcentrationDisadvantage.Name,
                0,
                0,
                0);

            yield break;
        }

        public IEnumerator OnTryAlterOutcomeSavingThrow(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier actionModifier,
            bool hasHitVisual,
            bool hasBorrowedLuck)
        {
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            var rulesetDefender = defender.RulesetCharacter;
            var effectDescription = action.ActionParams.AttackMode?.EffectDescription ??
                                    action.ActionParams.RulesetEffect?.EffectDescription;

            if (!actionManager ||
                helper != defender ||
                !action.RolledSaveThrow ||
                action.SaveOutcome != RollOutcome.Failure ||
                rulesetDefender.GetRemainingPowerUses(PowerMageSlayerSaving) == 0 ||
                effectDescription?.savingThrowAbility is not
                    (AttributeDefinitions.Intelligence or AttributeDefinitions.Wisdom or AttributeDefinitions.Charisma))
            {
                yield break;
            }

            var usablePower = PowerProvider.Get(PowerMageSlayerSaving, rulesetDefender);
            var reactionParams = new CharacterActionParams(defender, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
            {
                StringParameter =
                    "CustomReactionMageSlayerDescription".Formatted(Category.Reaction, attacker.Name),
                UsablePower = usablePower
            };
            var reactionRequest = new ReactionRequestCustom("MageSlayer", reactionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(attacker, actionManager, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            action.SaveOutcomeDelta = 0;
            action.SaveOutcome = RollOutcome.Success;

            rulesetDefender.UsePower(usablePower);
            rulesetDefender.LogCharacterUsedPower(PowerMageSlayerSaving);
        }

        internal static IEnumerator HandleEnemyCastSpellWithin5Ft(
            GameLocationCharacter caster,
            GameLocationCharacter defender)
        {
            var actionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battleManager =
                ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!actionManager || !battleManager)
            {
                yield break;
            }

            var (attackMode, actionModifier) = defender.GetFirstMeleeModeThatCanAttack(caster);

            if (attackMode == null ||
                !defender.CanReact())
            {
                yield break;
            }

            var actionParams = new CharacterActionParams(defender, ActionDefinitions.Id.AttackOpportunity)
            {
                StringParameter = defender.Name,
                ActionModifiers = { actionModifier },
                AttackMode = attackMode,
                TargetCharacters = { caster }
            };
            var reactionRequest = new ReactionRequestReactionAttack("MageSlayer", actionParams);
            var count = actionManager.PendingReactionRequestGroups.Count;

            actionManager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(caster, actionManager, count);
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
                FeatureDefinitionMovementAffinityBuilder
                    .Create("MovementAffinityFeatMobile")
                    .SetGuiPresentationNoContent(true)
                    .SetBaseSpeedAdditiveModifier(2)
                    .AddCustomSubFeatures(
                        new ActionFinishedByMeFeatMobileDash(
                            ConditionDefinitionBuilder
                                .Create("ConditionImmuneAoO")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .AddCustomSubFeatures(new IgnoreAoOOnMeFeatMobile())
                                .AddToDB(),
                            ConditionDefinitionBuilder
                                .Create(ConditionDefinitions.ConditionFreedomOfMovement, "ConditionFeatMobileAfterDash")
                                .SetOrUpdateGuiPresentation(Category.Condition)
                                .SetParentCondition(ConditionDefinitions.ConditionFreedomOfMovement)
                                .SetPossessive()
                                .SetFeatures()
                                .AddToDB(),
                            ConditionDefinitionBuilder
                                .Create("ConditionMobileMark")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .AddToDB()))
                    .AddToDB())
            .SetAbilityScorePrerequisite(AttributeDefinitions.Dexterity, 13)
            .AddToDB();
    }

    private sealed class ActionFinishedByMeFeatMobileDash(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionImmuneAoO,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition conditionMovement,
        ConditionDefinition conditionMark) : IActionFinishedByMe
    {
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            switch (action)
            {
                case CharacterActionAttack when
                    ValidatorsWeapon.IsMelee(action.ActionParams.AttackMode):
                {
                    rulesetAttacker.InflictCondition(
                        conditionImmuneAoO.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionImmuneAoO.Name,
                        0,
                        0,
                        0);

                    var defender = action.ActionParams.TargetCharacters[0];
                    var rulesetDefender = defender.RulesetCharacter;

                    rulesetDefender.InflictCondition(
                        conditionMark.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfSourceTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionMark.Name,
                        0,
                        0,
                        0);
                    break;
                }
                case CharacterActionDash:
                    rulesetAttacker.InflictCondition(
                        conditionMovement.Name,
                        DurationType.Round,
                        0,
                        TurnOccurenceType.EndOfTurn,
                        AttributeDefinitions.TagEffect,
                        rulesetAttacker.guid,
                        rulesetAttacker.CurrentFaction.Name,
                        1,
                        conditionMovement.Name,
                        0,
                        0,
                        0);
                    break;
            }

            yield break;
        }
    }

    private sealed class IgnoreAoOOnMeFeatMobile : IIgnoreAoOOnMe
    {
        public bool CanIgnoreAoOOnSelf(RulesetCharacter defender, RulesetCharacter attacker)
        {
            return attacker.HasConditionOfCategoryAndType(
                AttributeDefinitions.TagEffect, "ConditionMobileMark");
        }
    }

    #endregion

    #region Poisonous Skin

    private static readonly FeatureDefinitionPower PowerFeatPoisonousSkin = FeatureDefinitionPowerBuilder
        .Create("PowerFeatPoisonousSkin")
        .SetGuiPresentation(Category.Feature, hidden: true)
        .SetUsesFixed(ActivationTime.NoCost)
        .SetShowCasting(false)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Minute, 1)
                .SetTargetingData(Side.Enemy, RangeType.Distance, 1, TargetType.IndividualsUnique)
                .ExcludeCaster()
                .SetSavingThrowData(false,
                    AttributeDefinitions.Constitution, false,
                    EffectDifficultyClassComputation.AbilityScoreAndProficiency,
                    AttributeDefinitions.Constitution)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                        .SetConditionForm(ConditionDefinitions.ConditionPoisoned,
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .Build())
        .AddCustomSubFeatures(new CustomBehaviorFeatPoisonousSkin())
        .AddToDB();

    internal static readonly FeatDefinition FeatPoisonousSkin = FeatDefinitionBuilder
        .Create("FeatPoisonousSkin")
        .SetGuiPresentation(Category.Feat)
        .SetAbilityScorePrerequisite(AttributeDefinitions.Constitution, 13)
        .SetFeatures(PowerFeatPoisonousSkin)
        .AddToDB();

    private static IEnumerator PoisonTarget(GameLocationCharacter me, GameLocationCharacter target)
    {
        var rulesetMe = me.RulesetCharacter;
        var rulesetTarget = target.RulesetCharacter;

        if (rulesetTarget is not { IsDeadOrDyingOrUnconscious: false })
        {
            yield break;
        }

        var implementationManager =
            ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

        var usablePower = PowerProvider.Get(PowerFeatPoisonousSkin, rulesetMe);
        var actionParams = new CharacterActionParams(me, ActionDefinitions.Id.PowerNoCost)
        {
            ActionModifiers = { new ActionModifier() },
            RulesetEffect = implementationManager
                .MyInstantiateEffectPower(rulesetMe, usablePower, false),
            UsablePower = usablePower,
            TargetCharacters = { target }
        };

        // must enqueue actions whenever within an attack workflow otherwise game won't consume attack
        ServiceRepository.GetService<IGameLocationActionService>()?
            .ExecuteAction(actionParams, null, true);
    }

    //Poison character that shoves me
    internal static IEnumerator HandleFeatPoisonousSkin(CharacterAction action, GameLocationCharacter target)
    {
        if (action is not CharacterActionShove)
        {
            yield break;
        }

        if (action.ActionParams.TargetCharacters == null ||
            !action.ActionParams.TargetCharacters.Contains(target))
        {
            yield break;
        }

        yield return PoisonTarget(target, action.ActingCharacter);
    }

    private class CustomBehaviorFeatPoisonousSkin
        : IPhysicalAttackFinishedByMe, IPhysicalAttackFinishedOnMe, IActionFinishedByMe
    {
        //Poison characters that I shove
        public IEnumerator OnActionFinishedByMe(CharacterAction action)
        {
            if (action is not CharacterActionShove)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;

            foreach (var target in action.actionParams.TargetCharacters)
            {
                yield return PoisonTarget(actingCharacter, target);
            }
        }

        //Poison target if I attack with unarmed
        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter me,
            GameLocationCharacter target,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            //Missed: skipping
            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            //Not unarmed attack: skipping
            if (!ValidatorsWeapon.IsUnarmed(attackMode))
            {
                yield break;
            }

            yield return PoisonTarget(me, target);
        }

        //Poison melee attacker
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            //Missed: skipping
            if (rollOutcome is RollOutcome.Failure or RollOutcome.CriticalFailure)
            {
                yield break;
            }

            //Not melee attack: skipping
            if (!ValidatorsWeapon.IsMelee(attackMode))
            {
                yield break;
            }

            yield return PoisonTarget(me, attacker);
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
            CastSpellDruid,
            CastSpellSorcerer,
            CastSpellWarlock,
            CastSpellWizard,
            InventorClass.SpellCasting
        };

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var castSpell in castSpells)
        {
            var spellSniperSpells = castSpell.SpellListDefinition.SpellsByLevel
                .SelectMany(x => x.Spells)
                .Where(x =>
                    x.SpellLevel == 0 &&
                    x.EffectDescription.RangeType is RangeType.MeleeHit or RangeType.RangeHit &&
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

            var title = Gui.Format($"Feat/&{NAME}Title", classTitle);
            var featSpellSniper = FeatDefinitionBuilder
                .Create($"{NAME}{className}")
                .SetGuiPresentation(title, Gui.Format($"Feat/&{NAME}Description", classTitle))
                .SetFeatures(
                    FeatureDefinitionCombatAffinityBuilder
                        .Create($"CombatAffinity{NAME}{className}")
                        .SetGuiPresentation(title, Gui.NoLocalization)
                        .AddCustomSubFeatures(new ValidateContextInsteadOfRestrictedProperty((_, _, _, _, _, mode, _) =>
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
                        .AddCustomSubFeatures(new FeatHelpers.SpellTag(FeatSpellSniperTag))
                        .SetSpellList(spellList)
                        .AddToDB(),
                    FeatureDefinitionPointPoolBuilder
                        .Create($"PointPool{NAME}{className}Cantrip")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1, spellList,
                            FeatSpellSniperTag)
                        .AddToDB())
                .SetFeatFamily(NAME)
                .AddCustomSubFeatures(new ModifyEffectDescriptionFeatSpellSniper())
                .AddToDB();

            spellSniperFeats.Add(featSpellSniper);
        }

        var spellSniperGroup = GroupFeats.MakeGroup("FeatGroupSpellSniper", NAME, spellSniperFeats);

        feats.AddRange(spellSniperFeats);

        spellSniperGroup.mustCastSpellsPrerequisite = true;

        return spellSniperGroup;
    }

    private sealed class ModifyEffectDescriptionFeatSpellSniper : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition is SpellDefinition &&
                   effectDescription.rangeType is RangeType.MeleeHit or RangeType.RangeHit &&
                   effectDescription.HasDamageForm();
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            effectDescription.rangeParameter = effectDescription.RangeParameter * 2;

            return effectDescription;
        }
    }

    #endregion

    #region War Caster

    private static FeatDefinition BuildWarcaster()
    {
        return FeatDefinitionBuilder
            .Create(FeatWarCaster)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionMagicAffinityBuilder
                    .Create(MagicAffinityFeatWarCaster)
                    .SetGuiPresentation(FeatWarCaster, Category.Feat)
                    .SetCastingModifiers(0, SpellParamsModifierType.FlatValue, 0,
                        SpellParamsModifierType.None)
                    .SetConcentrationModifiers(ConcentrationAffinity.Advantage)
                    .SetHandsFullCastingModifiers(true, true, true)
                    .AddToDB())
            .SetMustCastSpellsPrerequisite()
            .AddCustomSubFeatures(WarCasterMarker.Mark)
            .AddToDB();
    }

    internal class WarCasterMarker
    {
        private WarCasterMarker()
        {
        }

        public static WarCasterMarker Mark { get; } = new();
    }

    #endregion
}
