using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Validators;
using TA.AI;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAbilityCheckAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ItemDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRegenerations;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionFeatureSets;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionRestHealingModifiers;

namespace SolastaUnfinishedBusiness.Models;

internal static class SrdAndHouseRulesContext
{
    private const string InvisibleStalkerSubspellName = "ConjureElementalInvisibleStalker";

    internal static readonly HashSet<MonsterDefinition> ConjuredMonsters =
    [
        ConjuredOneBeastTiger_Drake,
        ConjuredTwoBeast_Direwolf,
        ConjuredFourBeast_BadlandsSpider,
        ConjuredEightBeast_Wolf,

        // Conjure minor elemental (4)
        SkarnGhoul, // CR 2
        WindSnake, // CR 2
        Fire_Jester, // CR 1

        // Conjure woodland beings (4) - not implemented

        // Conjure elemental (5)
        Air_Elemental, // CR 5
        Fire_Elemental, // CR 5
        Earth_Elemental, // CR 5

        InvisibleStalker, // CR 6

        // Conjure fey (6)
        FeyGiantApe, // CR 6
        FeyGiant_Eagle, // CR 5
        FeyBear, // CR 4
        Green_Hag, // CR 3
        FeyWolf, // CR 2
        FeyDriad
    ];

    private static readonly Dictionary<string, TagsDefinitions.Criticity> Tags = [];

    private static readonly List<MonsterDefinition> MonstersThatEmitLight =
    [
        CubeOfLight,
        Fire_Elemental,
        Fire_Jester,
        Fire_Osprey,
        Fire_Spider
    ];

    internal static readonly FeatureDefinitionActionAffinity ActionAffinityConditionBlind =
        FeatureDefinitionActionAffinityBuilder
            .Create("ActionAffinityConditionBlind")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(Id.AttackOpportunity)
            .AddToDB();

    private static readonly DecisionPackageDefinition DecisionPackageRestrained =
        AiContext.BuildDecisionPackageBreakFree(ConditionRestrainedByEntangle.Name);

    private static readonly FeatureDefinitionCombatAffinity CombatAffinityConditionSurprised =
        FeatureDefinitionCombatAffinityBuilder
            .Create("CombatAffinityConditionSurprised")
            .SetGuiPresentationNoContent(true)
            .SetInitiativeAffinity(AdvantageType.Disadvantage)
            .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolWizardScholar = FeatureDefinitionPointPoolBuilder
        .Create("PointPoolWizardScholar")
        .SetGuiPresentation(Category.Feature)
        .SetPool(HeroDefinitions.PointsPoolType.Expertise, 1)
        .RestrictChoices(
            SkillDefinitions.Arcana,
            SkillDefinitions.History,
            SkillDefinitions.Investigation,
            SkillDefinitions.Medecine,
            SkillDefinitions.Nature,
            SkillDefinitions.Religion)
        .AddToDB();

    private static readonly FeatureDefinitionPower PowerWarlockMagicalCunning = FeatureDefinitionPowerBuilder
        .Create("PowerWarlockMagicalCunning")
        .SetGuiPresentation(Category.Feature, PowerWizardArcaneRecovery)
        .SetUsesFixed(ActivationTime.Rest, RechargeRate.LongRest)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetSpellForm(5)
                        .Build())
                .SetParticleEffectParameters(PowerWizardArcaneRecovery)
                .Build())
        .AddToDB();

    private static readonly FeatureDefinitionPointPool PointPoolWarlockInvocation1 = FeatureDefinitionPointPoolBuilder
        .Create(FeatureDefinitionPointPools.PointPoolWarlockInvocation2, "PointPoolWarlockInvocation1")
        .SetGuiPresentation("PointPoolWarlockInvocationInitial", Category.Feature)
        .SetPool(HeroDefinitions.PointsPoolType.Invocation, 1)
        .AddToDB();

    private static readonly FeatureDefinitionFeatureSet FeatureSetDruidPrimalOrder = FeatureDefinitionFeatureSetBuilder
        .Create("FeatureSetDruidPrimalOrder")
        .SetGuiPresentation(Category.Feature)
        .SetMode(FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion)
        .SetFeatureSet(
            FeatureDefinitionFeatureSetBuilder
                .Create("FeatureSetDruidPrimalOrderMagician")
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionAbilityCheckAffinityBuilder
                        .Create("AbilityCheckDruidPrimalOrderMagician")
                        .SetGuiPresentation("FeatureSetDruidPrimalOrderMagician", Category.Feature)
                        .BuildAndSetAffinityGroups(CharacterAbilityCheckAffinity.None, DieType.D6, 1,
                            AbilityCheckGroupOperation.AddDie,
                            (AttributeDefinitions.Intelligence, SkillDefinitions.Arcana),
                            (AttributeDefinitions.Intelligence, SkillDefinitions.Nature))
                        .AddToDB(),
                    FeatureDefinitionPointPoolBuilder
                        .Create("PointPoolDruidPrimalOrderMagician")
                        .SetGuiPresentationNoContent(true)
                        .SetSpellOrCantripPool(HeroDefinitions.PointsPoolType.Cantrip, 1)
                        .AddToDB())
                .AddToDB(),
            FeatureDefinitionFeatureSetBuilder
                .Create("FeatureSetDruidPrimalOrderWarden")
                .SetGuiPresentation(Category.Feature)
                .SetFeatureSet(
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyDruidPrimalOrderWardenArmor")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Armor, EquipmentDefinitions.MediumArmorCategory)
                        .AddToDB(),
                    FeatureDefinitionProficiencyBuilder
                        .Create("ProficiencyDruidPrimalOrderWardenWeapon")
                        .SetGuiPresentationNoContent(true)
                        .SetProficiencies(ProficiencyType.Weapon, EquipmentDefinitions.MartialWeaponCategory)
                        .AddToDB())
                .AddToDB())
        .AddToDB();

    private static readonly List<string> DruidWeaponsCategories =
        [.. FeatureDefinitionProficiencys.ProficiencyDruidWeapon.Proficiencies];

    private static SpellDefinition ConjureElementalInvisibleStalker { get; set; }

    private static readonly List<(string, string)> GuidanceProficiencyPairs =
    [
        (AttributeDefinitions.Dexterity, SkillDefinitions.Acrobatics),
        (AttributeDefinitions.Wisdom, SkillDefinitions.AnimalHandling),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Arcana),
        (AttributeDefinitions.Strength, SkillDefinitions.Athletics),
        (AttributeDefinitions.Charisma, SkillDefinitions.Deception),
        (AttributeDefinitions.Intelligence, SkillDefinitions.History),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Insight),
        (AttributeDefinitions.Charisma, SkillDefinitions.Intimidation),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Investigation),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Medecine),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Nature),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Perception),
        (AttributeDefinitions.Charisma, SkillDefinitions.Performance),
        (AttributeDefinitions.Charisma, SkillDefinitions.Persuasion),
        (AttributeDefinitions.Intelligence, SkillDefinitions.Religion),
        (AttributeDefinitions.Dexterity, SkillDefinitions.SleightOfHand),
        (AttributeDefinitions.Dexterity, SkillDefinitions.Stealth),
        (AttributeDefinitions.Wisdom, SkillDefinitions.Survival)
    ];

    private static List<SpellDefinition> GuidanceSubSpells = new List<SpellDefinition>();

    internal static void LateLoad()
    {
        BuildConjureElementalInvisibleStalker();
        BuildOneDndGuidanceSubspells();
        LoadAfterRestIdentify();
        LoadAllowTargetingSelectionWhenCastingChainLightningSpell();
        LoadSenseNormalVisionRangeMultiplier();
        SwapOneDndBarkskinSpell();
        SwapOneDndGuidanceSpell();
        SwitchAddBleedingToLesserRestoration();
        SwitchAllowClubsToBeThrown();
        SwitchChangeSleetStormToCube();
        SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity();
        SwitchConditionBlindedShouldNotAllowOpportunityAttack();
        SwitchOneDnDEnableDruidToUseMetalArmor();
        SwitchDruidWeaponProficiencyToUseOneDnd();
        SwitchEnableDruidPrimalOrderAndRemoveMediumArmorProficiency();
        SwitchEldritchBlastRange();
        SwitchEnableUpcastConjureElementalAndFey();
        SwitchFilterOnHideousLaughter();
        SwitchFullyControlConjurations();
        SwitchAllowBladeCantripsToUseReach();
        SwitchHastedCasing();
        SwitchMagicStaffFoci();
        SwitchEnableRitualOnAllCasters();
        SwitchAllowTargetingSelectionWhenCastingChainLightningSpell();
        SwitchOfficialFoodRationsWeight();
        SwitchOneDndPreparedSpellsTables();
        SwitchOneDndPaladinLayOnHandAsBonusAction();
        SwitchOneDndEnableBardSuperiorInspirationAtLevel18();
        SwitchOneDndEnableBardWordsOfCreationAtLevel20();
        SwitchOneDndRemoveBardSongOfRest();
        SwitchOneDndRemoveBardMagicalSecretAt14And18();
        SwitchOneDndChangeBardicInspirationDurationToOneHour();
        SwitchOneDndEnableBardExpertiseOneLevelBefore();
        SwitchOneDndWarlockInvocationsProgression();
        SwitchOneDndWarlockMagicalCunningAtLevel2();
        SwitchOneDndHealingPotionBonusAction();
        SwitchOneDndHealingSpellsBuf();
        SwitchOneDndWizardScholar();
        SwitchOneDndWizardSchoolOfMagicLearningLevel();
        SwitchOneDndPaladinLearnSpellCastingAtOne();
        SwitchOneDndRangerLearnSpellCastingAtOne();
        SwitchOneDndSurprisedEnforceDisadvantage();
        SwitchRecurringEffectOnEntangle();
        SwitchRingOfRegenerationHealRate();
        SwitchSchoolRestrictionsFromShadowCaster();
        SwitchSchoolRestrictionsFromSpellBlade();
        SwitchUniversalSylvanArmorAndLightbringer();
        SwitchUseHeightOneCylinderEffect();
        NoTwinnedBladeCantrips();
        ModifyGravitySlam();
    }

    private static void LoadSenseNormalVisionRangeMultiplier()
    {
        _ = ConditionDefinitionBuilder
            .Create("ConditionSenseNormalVision24")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionSenseBuilder
                .Create(SenseNormalVision, "SenseNormalVision24")
                .SetSense(SenseMode.Type.NormalVision, 24)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();

        _ = ConditionDefinitionBuilder
            .Create("ConditionSenseNormalVision48")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(FeatureDefinitionSenseBuilder
                .Create(SenseNormalVision, "SenseNormalVision48")
                .SetSense(SenseMode.Type.NormalVision, 48)
                .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.BattleEnd)
            .AddToDB();
    }

    internal static void SwitchSchoolRestrictionsFromShadowCaster()
    {
        if (Main.Settings.RemoveSchoolRestrictionsFromShadowCaster)
        {
            FeatureDefinitionCastSpells.CastSpellShadowcaster.RestrictedSchools.Clear();
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellShadowcaster.RestrictedSchools.SetRange(
                SchoolAbjuration,
                SchoolDivination,
                SchoolIllusion,
                SchoolNecromancy);
        }
    }

    internal static void SwitchSchoolRestrictionsFromSpellBlade()
    {
        if (Main.Settings.RemoveSchoolRestrictionsFromSpellBlade)
        {
            FeatureDefinitionCastSpells.CastSpellMartialSpellBlade.RestrictedSchools.Clear();
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellMartialSpellBlade.RestrictedSchools.SetRange(
                SchoolConjuration,
                SchoolEnchantement,
                SchoolEvocation,
                SchoolTransmutation);
        }
    }

    internal static void AddLightSourceIfNeeded(GameLocationCharacter gameLocationCharacter)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
        {
            return;
        }

        if (!MonstersThatEmitLight.Contains(rulesetCharacterMonster.MonsterDefinition))
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacterMonster, "ShouldEmitLightFromMonster");
    }

    internal static void AddLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (!Main.Settings.EnableCharactersOnFireToEmitLight)
        {
            return;
        }

        if (rulesetCondition == null || !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter)
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        AddLightSource(gameLocationCharacter, rulesetCharacter, "ShouldEmitLightFromCondition");
    }

    private static void AddLightSource(
        GameLocationCharacter gameLocationCharacter,
        RulesetCharacter rulesetCharacter,
        string name)
    {
        var lightSourceForm = Shine.EffectDescription.EffectForms[0].LightSourceForm;

        rulesetCharacter.PersonalLightSource?.Unregister();
        rulesetCharacter.PersonalLightSource = new RulesetLightSource(
            lightSourceForm.Color,
            2,
            4,
            lightSourceForm.GraphicsPrefabAssetGUID,
            LightSourceType.Basic,
            name,
            rulesetCharacter.Guid);

        rulesetCharacter.PersonalLightSource.Register(true);

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .AddCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);
    }

    internal static void RemoveLightSourceIfNeeded(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        if (rulesetCondition == null ||
            !rulesetCondition.ConditionDefinition.IsSubtypeOf(ConditionOnFire.Name))
        {
            return;
        }

        if (rulesetActor is not RulesetCharacter rulesetCharacter ||
            rulesetCharacter.PersonalLightSource == null) // if using extinguish fire light source will come null here
        {
            return;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter == null)
        {
            return;
        }

        ServiceRepository.GetService<IGameLocationVisibilityService>()?
            .RemoveCharacterLightSource(gameLocationCharacter, rulesetCharacter.PersonalLightSource);

        rulesetCharacter.PersonalLightSource.Unregister();
        rulesetCharacter.PersonalLightSource = null;
    }

    internal static void SwitchUniversalSylvanArmorAndLightbringer()
    {
        GreenmageArmor.RequiredAttunementClasses.Clear();
        WizardClothes_Alternate.RequiredAttunementClasses.Clear();

        if (Main.Settings.AllowAnyClassToWearSylvanArmor)
        {
            return;
        }

        var allowedClasses = new[] { Wizard, Sorcerer, Warlock };

        GreenmageArmor.RequiredAttunementClasses.AddRange(allowedClasses);
        WizardClothes_Alternate.RequiredAttunementClasses.AddRange(allowedClasses);
    }

    internal static void SwitchOneDnDEnableDruidToUseMetalArmor()
    {
        var active = Main.Settings.EnableDruidToUseMetalArmor;

        if (active)
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Clear();
        }
        else
        {
            if (!FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Contains(
                    TagsDefinitions.ItemTagMetal))
            {
                FeatureDefinitionProficiencys.ProficiencyDruidArmor.ForbiddenItemTags.Add(
                    TagsDefinitions.ItemTagMetal);
            }
        }
    }

    internal static void SwitchEnableDruidPrimalOrderAndRemoveMediumArmorProficiency()
    {
        if (Main.Settings.EnableDruidPrimalOrderAndRemoveMediumArmorProficiency)
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.Proficiencies.Remove(
                EquipmentDefinitions.MediumArmorCategory);

            Druid.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetDruidPrimalOrder, 1));
        }
        else
        {
            FeatureDefinitionProficiencys.ProficiencyDruidArmor.Proficiencies.TryAdd(
                EquipmentDefinitions.MediumArmorCategory);

            Druid.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetDruidPrimalOrder);
        }

        Druid.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchDruidWeaponProficiencyToUseOneDnd()
    {
        FeatureDefinitionProficiencys.ProficiencyDruidWeapon.proficiencies =
            Main.Settings.SwapDruidWeaponProficiencyToUseOneDnd
                ? [WeaponCategoryDefinitions.SimpleWeaponCategory.Name]
                : DruidWeaponsCategories;
    }

    internal static void SwitchEnableRitualOnAllCasters()
    {
        var subclasses = SharedSpellsContext.SubclassCasterType.Keys.Select(GetDefinition<CharacterSubclassDefinition>);

        if (Main.Settings.EnableRitualOnAllCasters)
        {
            Paladin.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting,
                Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2));
            Ranger.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting,
                Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2));
            Sorcerer.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting, 1));
        }
        else
        {
            Paladin.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            Ranger.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            Sorcerer.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        Sorcerer.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        foreach (var subclass in subclasses)
        {
            if (Main.Settings.EnableRitualOnAllCasters)
            {
                subclass.FeatureUnlocks.Add(new FeatureUnlockByLevel(FeatureSetClericRitualCasting, 3));
            }
            else
            {
                subclass.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == FeatureSetClericRitualCasting);
            }

            subclass.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }
    }

    internal static void SwitchMagicStaffFoci()
    {
        if (!Main.Settings.MakeAllMagicStaveArcaneFoci)
        {
            return;
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>()
                     .Where(x => x.IsWeapon) // WeaponDescription could be null
                     .Where(x => x.WeaponDescription.WeaponType == EquipmentDefinitions.WeaponTypeQuarterstaff)
                     .Where(x => x.Magical && !x.Name.Contains("OfHealing")))
        {
            item.IsFocusItem = true;
            item.FocusItemDescription.focusType = EquipmentDefinitions.FocusType.Arcane;
        }
    }

    internal static void SwitchConditionBlindedShouldNotAllowOpportunityAttack()
    {
        if (Main.Settings.BlindedConditionDontAllowAttackOfOpportunity)
        {
            ConditionDefinitions.ConditionBlinded.Features.TryAdd(ActionAffinityConditionBlind);
            LightingAndObscurementContext.ConditionBlindedByDarkness.Features.TryAdd(ActionAffinityConditionBlind);
        }
        else
        {
            ConditionDefinitions.ConditionBlinded.Features.Remove(ActionAffinityConditionBlind);
            LightingAndObscurementContext.ConditionBlindedByDarkness.Features.Remove(ActionAffinityConditionBlind);
        }
    }

    private static void LoadAllowTargetingSelectionWhenCastingChainLightningSpell()
    {
        ChainLightning.AddCustomSubFeatures(new FilterTargetingCharacterChainLightning());
    }

    internal static void SwitchAllowTargetingSelectionWhenCastingChainLightningSpell()
    {
        var spell = ChainLightning.EffectDescription;

        if (Main.Settings.AllowTargetingSelectionWhenCastingChainLightningSpell)
        {
            spell.targetType = TargetType.IndividualsUnique;
            spell.targetParameter = 4;
            spell.effectAdvancement.additionalTargetsPerIncrement = 1;
        }
        else
        {
            spell.targetType = TargetType.ArcFromIndividual;
            spell.targetParameter = 3;
            spell.effectAdvancement.additionalTargetsPerIncrement = 0;
        }
    }

    internal static void SwitchOfficialFoodRationsWeight()
    {
        var foodSrdWeight = Food_Ration;
        var foodForagedSrdWeight = Food_Ration_Foraged;

        if (Main.Settings.UseOfficialFoodRationsWeight)
        {
            foodSrdWeight.weight = 2.0f;
            foodForagedSrdWeight.weight = 2.0f;
        }
        else
        {
            foodSrdWeight.weight = 3.0f;
            foodForagedSrdWeight.weight = 3.0f;
        }
    }

    internal static void SwapOneDndBarkskinSpell()
    {
        if (Main.Settings.SwapOneDndBarkskinSpell)
        {
            Barkskin.requiresConcentration = false;
            Barkskin.castingTime = ActivationTime.BonusAction;
            FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin.modifierValue = 17;
            Barkskin.GuiPresentation.description = "Spell/&BarkskinOneDndDescription";
            ConditionBarkskin.GuiPresentation.description = "Rules/&ConditionOneDndBarkskinDescription";
        }
        else
        {
            Barkskin.requiresConcentration = true;
            Barkskin.castingTime = ActivationTime.Action;
            FeatureDefinitionAttributeModifiers.AttributeModifierBarkskin.modifierValue = 16;
            Barkskin.GuiPresentation.description = "Spell/&BarkskinDescription";
            ConditionBarkskin.GuiPresentation.description = "Rules/&ConditionBarkskinDescription";
        }
    }

    private static void BuildOneDndGuidanceSubspells()
    {
        foreach (var (attribute, skill) in GuidanceProficiencyPairs)
        {
            var proficiencypair = (attribute, skill);
            var affinity = $"AbilityCheckAffinityGuidance{skill}";
            var condition = $"ConditionGuidance{skill}";

            GuidanceSubSpells.Add(
                SpellDefinitionBuilder
                    .Create($"Guidance{skill}")
                    .SetGuiPresentation(Category.Spell, Guidance.GuiPresentation.SpriteReference)
                    .SetSchoolOfMagic(SchoolOfMagicDefinitions.SchoolDivination)
                    .SetSpellLevel(0)
                    .SetCastingTime(ActivationTime.Action)
                    .SetMaterialComponent(MaterialComponentType.None)
                    .SetVerboseComponent(true)
                    .SetSomaticComponent(true)
                    .SetRequiresConcentration(true)
                    .SetVocalSpellSameType(VocalSpellSemeType.Buff)
                    .SetEffectDescription(
                        EffectDescriptionBuilder
                        .Create()
                        .SetDurationData(DurationType.Minute, 1)
                        .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                        .SetEffectForms(EffectFormBuilder.ConditionForm(
                                ConditionDefinitionBuilder
                                    .Create(ConditionGuided, condition)
                                    .SetGuiPresentation(Category.Condition, ConditionGuided)
                                    .SetSpecialInterruptions(ConditionInterruption.None)
                                    .SetFeatures(
                                        FeatureDefinitionAbilityCheckAffinityBuilder
                                            .Create(AbilityCheckAffinityGuided, affinity)
                                            .SetGuiPresentationNoContent(true)
                                            .BuildAndSetAffinityGroups(
                                                CharacterAbilityCheckAffinity.None, DieType.D4, 1, AbilityCheckGroupOperation.AddDie,
                                                abilityProficiencyPairs: (proficiencypair))
                                            .AddToDB())
                                    .AddToDB()))
                        .SetParticleEffectParameters(Guidance)
                        .Build())
                    .AddToDB());
        }
    }

    internal static void SwapOneDndGuidanceSpell()
    {
        foreach (var spell in GuidanceSubSpells)
        {
            spell.implemented = false;
        }

        if (Main.Settings.SwapOneDndGuidanceSpell)
        {
            Guidance.spellsBundle = true;
            Guidance.SubspellsList.SetRange(GuidanceSubSpells);
            Guidance.compactSubspellsTooltip = true;
            Guidance.EffectDescription.EffectForms.Clear();
            Guidance.GuiPresentation.description = "Spell/&OneDndGuidanceDescription";
        }
        else
        {
            Guidance.spellsBundle = false;
            Guidance.SubspellsList.Clear();
            Guidance.EffectDescription.EffectForms.SetRange(EffectFormBuilder.ConditionForm(ConditionGuided));
            Guidance.GuiPresentation.description = "Spell/&GuidanceDescription";
        }

    }

    internal static void SwitchOneDndWizardSchoolOfMagicLearningLevel()
    {
        var schools = DatabaseRepository.GetDatabase<CharacterSubclassDefinition>()
            .Where(x =>
                FeatureDefinitionSubclassChoices.SubclassChoiceWizardArcaneTraditions.Subclasses.Contains(x.Name) ||
                x.Name.StartsWith(WizardClass))
            .ToList();

        var fromLevel = 3;
        var toLevel = 2;

        if (Main.Settings.EnableWizardToLearnSchoolAtLevel3)
        {
            fromLevel = 2;
            toLevel = 3;
        }

        foreach (var featureUnlock in schools
                     .SelectMany(school => school.FeatureUnlocks
                         .Where(featureUnlock => featureUnlock.level == fromLevel)))
        {
            featureUnlock.level = toLevel;
        }

        foreach (var featureUnlock in Wizard.FeatureUnlocks.Where(featureUnlock => featureUnlock.level == fromLevel))
        {
            featureUnlock.level = toLevel;
        }

        foreach (var school in schools)
        {
            school.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
        }

        Wizard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndPaladinLearnSpellCastingAtOne()
    {
        var level = Main.Settings.EnablePaladinSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellPaladin))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters
        foreach (var featureUnlock in Paladin.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Paladin.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnablePaladinSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellPaladin.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[PaladinClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchOneDndRangerLearnSpellCastingAtOne()
    {
        var level = Main.Settings.EnableRangerSpellCastingAtLevel1 ? 1 : 2;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionCastSpells.CastSpellRanger))
        {
            featureUnlock.level = level;
        }

        // allows back and forth compatibility with EnableRitualOnAllCasters
        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureSetClericRitualCasting))
        {
            featureUnlock.level = level;
        }

        Ranger.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);

        if (Main.Settings.EnableRangerSpellCastingAtLevel1)
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfRoundUpCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.HalfRoundUp;
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellRanger.slotsPerLevels = SharedSpellsContext.HalfCastingSlots;
            SharedSpellsContext.ClassCasterType[RangerClass] =
                FeatureDefinitionCastSpellBuilder.CasterProgression.Half;
        }
    }

    internal static void SwitchOneDndSurprisedEnforceDisadvantage()
    {
        if (Main.Settings.EnableSurprisedToEnforceDisadvantage)
        {
            ConditionDefinitions.ConditionSurprised.Features.SetRange(CombatAffinityConditionSurprised);
            ConditionDefinitions.ConditionSurprised.GuiPresentation.Description = Gui.NoLocalization;
        }
        else
        {
            ConditionDefinitions.ConditionSurprised.Features.SetRange(
                FeatureDefinitionActionAffinitys.ActionAffinityConditionSurprised,
                FeatureDefinitionMovementAffinitys.MovementAffinityConditionSurprised);
            ConditionDefinitions.ConditionSurprised.GuiPresentation.Description =
                "Rules/&ConditionSurprisedDescription";
        }
    }

    internal static void SwitchOneDndPreparedSpellsTables()
    {
        if (Main.Settings.EnableOneDnDPreparedSpellsTables)
        {
            FeatureDefinitionCastSpells.CastSpellBard.knownSpells =
                [4, 5, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];
            FeatureDefinitionCastSpells.CastSpellSorcerer.knownSpells =
                [2, 4, 6, 7, 9, 10, 11, 12, 14, 15, 16, 16, 17, 17, 18, 18, 19, 20, 21, 22];
        }
        else
        {
            FeatureDefinitionCastSpells.CastSpellBard.knownSpells =
                [4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 15, 15, 16, 18, 19, 19, 20, 22, 22, 22];
            FeatureDefinitionCastSpells.CastSpellSorcerer.knownSpells =
                [2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 12, 13, 13, 14, 14, 15, 15, 15, 15];
        }
    }

    internal static void SwitchOneDndPaladinLayOnHandAsBonusAction()
    {
        PowerPaladinLayOnHands.activationTime = Main.Settings.EnablePaladinLayOnHandsAsBonusAction
            ? ActivationTime.BonusAction
            : ActivationTime.Action;
    }

    internal static void SwitchOneDndEnableBardExpertiseOneLevelBefore()
    {
        var level = Main.Settings.EnableBardExpertiseOneLevelBefore ? 2 : 3;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionPointPools.PointPoolBardExpertiseLevel3))
        {
            featureUnlock.level = level;
        }

        level = Main.Settings.EnableBardExpertiseOneLevelBefore ? 9 : 10;

        foreach (var featureUnlock in Ranger.FeatureUnlocks
                     .Where(x => x.FeatureDefinition == FeatureDefinitionPointPools.PointPoolBardExpertiseLevel10))
        {
            featureUnlock.level = level;
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndChangeBardicInspirationDurationToOneHour()
    {
        if (Main.Settings.ChangeBardicInspirationDurationToOneHour)
        {
            ConditionDefinitions.ConditionBardicInspiration.durationType = DurationType.Hour;
            ConditionDefinitions.ConditionBardicInspiration.durationParameter = 1;
        }
        else
        {
            ConditionDefinitions.ConditionBardicInspiration.durationType = DurationType.Minute;
            ConditionDefinitions.ConditionBardicInspiration.durationParameter = 10;
        }
    }

    internal static void SwitchOneDndRemoveBardSongOfRest()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == RestHealingModifierBardSongOfRest);

        if (!Main.Settings.RemoveBardSongOfRest)
        {
            Bard.FeatureUnlocks.Add(new FeatureUnlockByLevel(RestHealingModifierBardSongOfRest, 2));
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndRemoveBardMagicalSecretAt14And18()
    {
        Bard.FeatureUnlocks.RemoveAll(x =>
            x.FeatureDefinition == FeatureDefinitionPointPools.PointPoolBardMagicalSecrets14 ||
            x.FeatureDefinition == Level20Context.PointPoolBardMagicalSecrets18);

        if (!Main.Settings.RemoveBardMagicalSecretAt14And18)
        {
            Bard.FeatureUnlocks.AddRange(
                new FeatureUnlockByLevel(FeatureDefinitionPointPools.PointPoolBardMagicalSecrets14, 14),
                new FeatureUnlockByLevel(Level20Context.PointPoolBardMagicalSecrets18, 18));
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndEnableBardSuperiorInspirationAtLevel18()
    {
        if (Main.Settings.RemoveBardMagicalSecretAt14And18)
        {
            Bard.FeatureUnlocks.Add(
                new FeatureUnlockByLevel(Level20Context.FeatureBardSuperiorInspiration2024, 18));
        }
        else
        {
            Bard.FeatureUnlocks.RemoveAll(x =>
                x.FeatureDefinition == Level20Context.FeatureBardSuperiorInspiration2024);
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndEnableBardWordsOfCreationAtLevel20()
    {
        if (Main.Settings.RemoveBardMagicalSecretAt14And18)
        {
            Bard.FeatureUnlocks.Add(
                new FeatureUnlockByLevel(Level20Context.AutoPreparedSpellsBardWordOfCreation, 20));
        }
        else
        {
            Bard.FeatureUnlocks.RemoveAll(x =>
                x.FeatureDefinition == Level20Context.AutoPreparedSpellsBardWordOfCreation);
        }

        Bard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndHealingPotionBonusAction()
    {
        if (Main.Settings.OneDndHealingPotionBonusAction)
        {
            PowerFunctionPotionOfHealing.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfHealingOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfGreaterHealing.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfGreaterHealingOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfSuperiorHealing.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionOfSuperiorHealingOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionPotionRemedy.activationTime = ActivationTime.BonusAction;
            PowerFunctionRemedyOther.activationTime = ActivationTime.BonusAction;
            PowerFunctionAntitoxin.activationTime = ActivationTime.BonusAction;
        }
        else
        {
            PowerFunctionPotionOfHealing.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfHealingOther.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfGreaterHealing.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfGreaterHealingOther.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfSuperiorHealing.activationTime = ActivationTime.Action;
            PowerFunctionPotionOfSuperiorHealingOther.activationTime = ActivationTime.Action;
            PowerFunctionPotionRemedy.activationTime = ActivationTime.Action;
            PowerFunctionRemedyOther.activationTime = ActivationTime.Action;
            PowerFunctionAntitoxin.activationTime = ActivationTime.Action;
        }
    }

    internal static void SwitchOneDndHealingSpellsBuf()
    {
        var dice = Main.Settings.EnableOneDndHealingSpellsBuf ? 2 : 1;

        // Cure Wounds, Healing Word got buf on base damage and add dice
        CureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        CureWounds.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;
        HealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
        HealingWord.effectDescription.effectAdvancement.additionalDicePerIncrement = dice;

        // Mass Cure Wounds and Mass Healing Word only got buf on base damage
        MassHealingWord.effectDescription.EffectForms[0].healingForm.diceNumber = dice;

        dice = Main.Settings.EnableOneDndHealingSpellsBuf ? 5 : 3;

        MassCureWounds.effectDescription.EffectForms[0].healingForm.diceNumber = dice;
    }

    internal static void SwitchOneDndWizardScholar()
    {
        if (Main.Settings.EnableWizardToLearnScholarAtLevel2)
        {
            Wizard.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolWizardScholar, 2));
        }
        else
        {
            Wizard.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PointPoolWizardScholar);
        }

        Wizard.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockMagicalCunningAtLevel2()
    {
        if (Main.Settings.EnableWarlockMagicalCunningAtLevel2)
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(PowerWarlockMagicalCunning, 2));
        }
        else
        {
            Warlock.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PowerWarlockMagicalCunning);
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchOneDndWarlockInvocationsProgression()
    {
        if (Main.Settings.SwapWarlockToUseOneDndInvocationProgression)
        {
            Warlock.FeatureUnlocks.Add(new FeatureUnlockByLevel(PointPoolWarlockInvocation1, 1));
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationAdditionalTitle";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationAdditionalDescription";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation5.poolAmount = 2;
        }
        else
        {
            Warlock.FeatureUnlocks.RemoveAll(x => x.FeatureDefinition == PointPoolWarlockInvocation1);
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Title =
                "Feature/&PointPoolWarlockInvocationInitialTitle";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation2.GuiPresentation.Description =
                "Feature/&PointPoolWarlockInvocationInitialDescription";
            FeatureDefinitionPointPools.PointPoolWarlockInvocation5.poolAmount = 1;
        }

        Warlock.FeatureUnlocks.Sort(Sorting.CompareFeatureUnlock);
    }

    internal static void SwitchFilterOnHideousLaughter()
    {
        HideousLaughter.effectDescription.restrictedCreatureFamilies.Clear();

        if (!Main.Settings.RemoveHumanoidFilterOnHideousLaughter)
        {
            HideousLaughter.effectDescription.restrictedCreatureFamilies.Add(CharacterFamilyDefinitions.Humanoid.Name);
        }
    }

    internal static void SwitchRecurringEffectOnEntangle()
    {
        // Remove recurring effect on Entangle (as per SRD, any creature is only affected at cast time)
        if (Main.Settings.RemoveRecurringEffectOnEntangle)
        {
            Entangle.effectDescription.recurrentEffect = RecurrentEffect.OnActivation;
            Entangle.effectDescription.EffectForms[2].canSaveToCancel = false;
            ConditionRestrainedByEntangle.Features.Add(FeatureDefinitionActionAffinitys.ActionAffinityGrappled);
            ConditionRestrainedByEntangle.amountOrigin = ConditionDefinition.OriginOfAmount.Fixed;
            ConditionRestrainedByEntangle.baseAmount = (int)AiContext.BreakFreeType.DoStrengthCheckAgainstCasterDC;
            ConditionRestrainedByEntangle.addBehavior = true;
            ConditionRestrainedByEntangle.battlePackage = DecisionPackageRestrained;
        }
        else
        {
            Entangle.effectDescription.recurrentEffect =
                RecurrentEffect.OnActivation | RecurrentEffect.OnTurnEnd | RecurrentEffect.OnEnter;
            Entangle.effectDescription.EffectForms[2].canSaveToCancel = true;
            ConditionRestrainedByEntangle.Features.Remove(FeatureDefinitionActionAffinitys.ActionAffinityGrappled);
            ConditionRestrainedByEntangle.amountOrigin = ConditionDefinition.OriginOfAmount.None;
            ConditionRestrainedByEntangle.baseAmount = 0;
            ConditionRestrainedByEntangle.addBehavior = false;
            ConditionRestrainedByEntangle.battlePackage = null;
        }
    }

    internal static void SwitchChangeSleetStormToCube()
    {
        var sleetStormEffect = SleetStorm.EffectDescription;

        if (Main.Settings.ChangeSleetStormToCube)
        {
            // Set to Cube side 8, default height
            sleetStormEffect.targetType = TargetType.Cube;
            sleetStormEffect.targetParameter = 8;
            sleetStormEffect.targetParameter2 = 0;
        }
        else
        {
            // Restore to cylinder radius 4, height 3
            sleetStormEffect.targetType = TargetType.Cylinder;
            sleetStormEffect.targetParameter = 4;
            sleetStormEffect.targetParameter2 = 3;
        }
    }

    internal static void SwitchEldritchBlastRange()
    {
        EldritchBlast.effectDescription.rangeParameter = Main.Settings.FixEldritchBlastRange ? 24 : 16;
    }

    internal static void SwitchRingOfRegenerationHealRate()
    {
        var ringDefinition = RegenerationRing;

        if (Main.Settings.FixRingOfRegenerationHealRate)
        {
            // Heal by 1 hp per 3 minutes which is roughly the same as 
            // RAW of 1d6 (avg 3.5) every 10 minutes.
            ringDefinition.tickType = DurationType.Minute;
            ringDefinition.tickNumber = 3;
            ringDefinition.diceNumber = 1;
        }
        else
        {
            ringDefinition.tickType = DurationType.Round;
            ringDefinition.tickNumber = 1;
            ringDefinition.diceNumber = 2;
        }

        ringDefinition.dieType = DieType.D1;
    }

    internal static void SwitchUseHeightOneCylinderEffect()
    {
        // always applicable
        ClearTargetParameter2ForTargetTypeCube();

        // Change SpikeGrowth to be height 1 round cylinder/sphere
        var spikeGrowthEffect = SpikeGrowth.EffectDescription;

        spikeGrowthEffect.targetParameter = 4;

        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Set to Cylinder radius 4, height 1
            spikeGrowthEffect.targetType = TargetType.Cylinder;
            spikeGrowthEffect.targetParameter2 = 1;
        }
        else
        {
            // Restore default of Sphere radius 4
            spikeGrowthEffect.targetType = TargetType.Sphere;
            spikeGrowthEffect.targetParameter2 = 0;
        }

        // Spells with TargetType.Cube and defaults values of (tp, tp2)
        // Note that tp2 should be 0 for Cube and is ignored in game.
        // BlackTentacles: (4, 2)
        // Entangle: (4, 1)
        // FaerieFire: (4, 2)
        // FlamingSphere: (3, 2) <- a flaming sphere is a cube?
        // Grease: (2, 2)
        // HypnoticPattern: (6, 2)
        // Slow: (8, 2)
        // Thunderwave: (3, 2)

        // Change Black Tentacles, Entangle, Grease to be height 1 square cylinder/cube
        if (Main.Settings.UseHeightOneCylinderEffect)
        {
            // Setting height switches to square cylinder (if originally cube)
            SetHeight(BlackTentacles, 1);
            SetHeight(Entangle, 1);
            SetHeight(Grease, 1);
        }
        else
        {
            // Setting height to 0 restores original behaviour
            SetHeight(BlackTentacles, 0);
            SetHeight(Entangle, 0);
            SetHeight(Grease, 0);
        }

        return;

        static void SetHeight([NotNull] IMagicEffect spellDefinition, int height)
        {
            spellDefinition.EffectDescription.targetParameter2 = height;
        }

        static void ClearTargetParameter2ForTargetTypeCube()
        {
            foreach (var sd in DatabaseRepository
                         .GetDatabase<SpellDefinition>()
                         .Where(sd =>
                             sd.EffectDescription.TargetType is TargetType.Cube
                                 or TargetType.CubeWithOffset))
            {
                // TargetParameter2 is not used by TargetType.Cube but has random values assigned.
                // We are going to use it to create a square cylinder with height so set to zero for all spells with TargetType.Cube.
                sd.EffectDescription.targetParameter2 = 0;
            }
        }
    }

    internal static void SwitchAllowBladeCantripsToUseReach()
    {
        var db = DatabaseRepository.GetDatabase<SpellDefinition>();
        var cantrips = new List<string> { "BoomingBlade", "ResonatingStrike", "SunlightBlade" };

        foreach (var bladeCantrip in db.Where(x => cantrips.Contains(x.Name)))
        {
            var text = Main.Settings.AllowBladeCantripsToUseReach ? "Feedback/&WithinReach" : "Feedback/&Within5Ft";

            bladeCantrip.GuiPresentation.Description = Gui.Format($"Spell/&{bladeCantrip.Name}Description", text);
        }
    }

    internal static void SwitchHastedCasing()
    {
        var restrictedActions = FeatureDefinitionAdditionalActions.AdditionalActionHasted.RestrictedActions;
        if (Main.Settings.AllowHasteCasting)
        {
            restrictedActions.TryAdd(Id.CastMain);
        }
        else
        {
            restrictedActions.RemoveAll(id => id == Id.CastMain);
        }
    }

    internal static void SwitchAddBleedingToLesserRestoration()
    {
        var cf = LesserRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cf != null)
        {
            if (Main.Settings.AddBleedingToLesserRestoration)
            {
                cf.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
            }
            else
            {
                cf.ConditionForm.ConditionsList.Remove(ConditionBleeding);
            }
        }

        var cfg = GreaterRestoration.EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Condition);

        if (cfg == null)
        {
            return;
        }

        // NOTE: using the same setting as for Lesser Restoration for compatibility
        if (Main.Settings.AddBleedingToLesserRestoration)
        {
            cfg.ConditionForm.ConditionsList.TryAdd(ConditionBleeding);
        }
        else
        {
            cfg.ConditionForm.ConditionsList.Remove(ConditionBleeding);
        }
    }

    /// <summary>
    ///     Allow conjurations to fully controlled party members instead of AI controlled.
    /// </summary>
    private static void BuildConjureElementalInvisibleStalker()
    {
        ConjureElementalInvisibleStalker = SpellDefinitionBuilder
            .Create(ConjureElementalFire, InvisibleStalkerSubspellName)
            .SetOrUpdateGuiPresentation("Spell/&ConjureElementalInvisibleStalkerTitle",
                "Spell/&ConjureElementalDescription")
            .AddToDB();

        var summonForm = ConjureElementalInvisibleStalker
            .EffectDescription.GetFirstFormOfType(EffectForm.EffectFormType.Summon)?.SummonForm;

        if (summonForm != null)
        {
            summonForm.monsterDefinitionName = InvisibleStalker.Name;
        }
    }

    internal static void SwitchColdResistanceAndImmunityAlsoGrantsWeatherImmunity()
    {
        foreach (var featureSet in DatabaseRepository.GetDatabase<FeatureDefinitionFeatureSet>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                if (featureSet.FeatureSet.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance))
                {
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                // ReSharper disable once InvertIf
                if (featureSet.FeatureSet.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity))
                {
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                    featureSet.FeatureSet.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                featureSet.FeatureSet.Remove(ConditionAffinityWeatherFrozenImmunity);
            }
        }

        foreach (var condition in DatabaseRepository.GetDatabase<ConditionDefinition>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                if (condition.Features.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance))
                {
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                condition.Features.Remove(ConditionAffinityWeatherChilledImmunity);
                condition.Features.Remove(ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                // ReSharper disable once InvertIf
                if (condition.Features.Contains(FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity))
                {
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                    condition.Features.TryAdd(ConditionAffinityWeatherFrozenImmunity);
                }
            }
            else
            {
                condition.Features.Remove(ConditionAffinityWeatherChilledImmunity);
                condition.Features.Remove(ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                condition.Features.Remove(ConditionAffinityWeatherFrozenImmunity);
            }
        }

        foreach (var item in DatabaseRepository.GetDatabase<ItemDefinition>())
        {
            if (Main.Settings.ColdResistanceAlsoGrantsImmunityToChilledCondition)
            {
                var itemProperty = item.staticProperties.FirstOrDefault(x =>
                    x.FeatureDefinition == FeatureDefinitionDamageAffinitys.DamageAffinityColdResistance);

                if (itemProperty != null)
                {
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherFrozenImmunity
                    });
                }
            }
            else
            {
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherChilledImmunity);
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherFrozenImmunity);
            }

            if (Main.Settings.ColdImmunityAlsoGrantsImmunityToChilledAndFrozenCondition)
            {
                var itemProperty = item.staticProperties.FirstOrDefault(x =>
                    x.FeatureDefinition == FeatureDefinitionDamageAffinitys.DamageAffinityColdImmunity);

                // ReSharper disable once InvertIf
                if (itemProperty != null)
                {
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherChilledInsteadOfFrozenImmunity
                    });
                    item.staticProperties.TryAdd(new ItemPropertyDescription(itemProperty)
                    {
                        featureDefinition = ConditionAffinityWeatherFrozenImmunity
                    });
                }
            }
            else
            {
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherChilledImmunity);
                item.staticProperties.RemoveAll(x =>
                    x.FeatureDefinition == ConditionAffinityWeatherChilledInsteadOfFrozenImmunity);
                item.staticProperties.RemoveAll(x => x.FeatureDefinition == ConditionAffinityWeatherFrozenImmunity);
            }
        }
    }

    internal static void SwitchFullyControlConjurations()
    {
        foreach (var conjuredMonster in ConjuredMonsters)
        {
            conjuredMonster.fullyControlledWhenAllied = Main.Settings.FullyControlConjurations;
        }
    }

    internal static void SwitchAllowClubsToBeThrown()
    {
        var db = DatabaseRepository.GetDatabase<ItemDefinition>();

        foreach (var itemDefinition in db
                     .Where(x => x.IsWeapon &&
                                 x.WeaponDescription.WeaponTypeDefinition == WeaponTypeDefinitions.ClubType))
        {
            if (Main.Settings.AllowClubsToBeThrown)
            {
                itemDefinition.WeaponDescription.WeaponTags.Add(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 10;
            }
            else
            {
                itemDefinition.WeaponDescription.WeaponTags.Remove(TagsDefinitions.WeaponTagThrown);
                itemDefinition.WeaponDescription.maxRange = 5;
            }
        }
    }

    internal static void SwitchEnableUpcastConjureElementalAndFey()
    {
        if (!Main.Settings.EnableUpcastConjureElementalAndFey)
        {
            ConjureElemental.SubspellsList.Remove(ConjureElementalInvisibleStalker);

            return;
        }

        ConfigureAdvancement(ConjureFey);
        ConfigureAdvancement(ConjureElemental);
        ConfigureAdvancement(ConjureMinorElementals);
        ConjureElemental.SubspellsList.Add(ConjureElementalInvisibleStalker);

        return;

        // Set advancement at spell level, not sub-spell
        static void ConfigureAdvancement([NotNull] IMagicEffect spell)
        {
            var advancement = spell.EffectDescription.EffectAdvancement;

            advancement.effectIncrementMethod = EffectIncrementMethod.PerAdditionalSlotLevel;
            advancement.additionalSpellLevelPerIncrement = 1;
        }
    }

    private static void LoadAfterRestIdentify()
    {
        const string AfterRestIdentifyName = "PowerAfterRestIdentify";

        RestActivityDefinitionBuilder
            .Create("RestActivityShortRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        RestActivityDefinitionBuilder
            .Create("RestActivityLongRestIdentify")
            .SetGuiPresentation(AfterRestIdentifyName, Category.Feature)
            .AddCustomSubFeatures(new ValidateRestActivity(false, false))
            .SetRestData(
                RestDefinitions.RestStage.AfterRest,
                RestType.LongRest,
                RestActivityDefinition.ActivityCondition.None,
                PowerBundleContext.UseCustomRestPowerFunctorName,
                AfterRestIdentifyName)
            .AddToDB();

        var afterRestIdentifyCondition = ConditionDefinitionBuilder
            .Create("AfterRestIdentify")
            .SetGuiPresentation(Category.Condition)
            .AddCustomSubFeatures(OnConditionAddedOrRemovedIdentifyItems.Mark)
            .AddToDB();

        FeatureDefinitionPowerBuilder
            .Create(AfterRestIdentifyName)
            .SetGuiPresentation(Category.Feature, hidden: true)
            .AddCustomSubFeatures(CanIdentifyOnRest.Mark)
            .SetUsesFixed(ActivationTime.Rest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(
                        Side.Ally,
                        RangeType.Self,
                        1,
                        TargetType.Self)
                    .SetDurationData(
                        DurationType.Minute,
                        1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                afterRestIdentifyCondition,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();
    }

    internal static bool IsAttackModeInvalid(RulesetCharacter character, RulesetAttackMode mode)
    {
        if (character is not RulesetCharacterHero hero)
        {
            return false;
        }

        return IsHandCrossbowUseInvalid(mode.sourceObject as RulesetItem, hero,
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeMainHand),
            hero.GetItemInSlot(EquipmentDefinitions.SlotTypeOffHand));
    }

    internal static bool IsHandCrossbowUseInvalid(
        RulesetItem item,
        RulesetCharacterHero hero,
        RulesetItem main,
        RulesetItem off)
    {
        if (Main.Settings.IgnoreHandXbowFreeHandRequirements)
        {
            return false;
        }

        if (item == null || hero == null)
        {
            return false;
        }

        Tags.Clear();
        item.FillTags(Tags, hero, true);

        if (!Tags.ContainsKey(TagsDefinitions.WeaponTagAmmunition) ||
            Tags.ContainsKey(TagsDefinitions.WeaponTagTwoHanded))
        {
            return false;
        }

        if (main == item && off != null)
        {
            return true;
        }

        return off == item
               && main != null
               && main.ItemDefinition.WeaponDescription?.WeaponType != WeaponTypeDefinitions.UnarmedStrikeType.Name;
    }

    internal static void HandleSmallRaces(BattleDefinitions.AttackEvaluationParams evaluationParams)
    {
        if (!Main.Settings.UseOfficialSmallRacesDisWithHeavyWeapons)
        {
            return;
        }

        var hero = evaluationParams.attacker.RulesetCharacter.GetOriginalHero();

        if (hero?.RaceDefinition.SizeDefinition == CharacterSizeDefinitions.Small &&
            evaluationParams.attackMode is { SourceDefinition: ItemDefinition { IsWeapon: true } itemDefinition } &&
            itemDefinition.WeaponDescription.WeaponTags.Contains(TagsDefinitions.WeaponTagHeavy))
        {
            evaluationParams.attackModifier.AttackAdvantageTrends.Add(
                new TrendInfo(-1, FeatureSourceType.Unknown, "Feedback/&SmallRace", null));
        }
    }

    private static void NoTwinnedBladeCantrips()
    {
        MetamagicOptionDefinitions.MetamagicTwinnedSpell.AddCustomSubFeatures(NoTwinned.Validator);
    }

    private sealed class FilterTargetingCharacterChainLightning : IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            var selectedTargets = __instance.SelectionService.SelectedTargets;

            if (selectedTargets.Count == 0)
            {
                return true;
            }

            var firstTarget = selectedTargets[0];

            var isValid = firstTarget.IsWithinRange(target, 6);

            if (!isValid)
            {
                __instance.actionModifier.FailureFlags.Add("Failure/&SecondTargetNotWithinRange");
            }

            return isValid;
        }
    }

    private sealed class CanIdentifyOnRest : IValidatePowerUse
    {
        private CanIdentifyOnRest()
        {
        }

        public static CanIdentifyOnRest Mark { get; } = new();

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower power)
        {
            //does this work properly for wild-shaped heroes?
            if (character is not RulesetCharacterHero hero)
            {
                return false;
            }

            return Main.Settings.IdentifyAfterRest && hero.HasNonIdentifiedItems();
        }
    }

    private sealed class OnConditionAddedOrRemovedIdentifyItems : IOnConditionAddedOrRemoved
    {
        public static OnConditionAddedOrRemovedIdentifyItems Mark { get; } = new();

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            target.GetOriginalHero()?.AutoIdentifyInventoryItems();
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    internal sealed class NoTwinned
    {
        public static readonly ValidateMetamagicApplication Validator =
            (RulesetCharacter _, RulesetEffectSpell spell, MetamagicOptionDefinition _, ref bool result,
                ref string failure) =>
            {
                if (!spell.SpellDefinition.HasSubFeatureOfType<NoTwinned>())
                {
                    return;
                }

                result = false;
                failure = "Failure/&FailureFlagInvalidSingleTarget";
            };

        public static NoTwinned Mark { get; } = new();
    }

    #region Gravity Slam

    private static EffectDescription _gravitySlamVanilla;
    private static EffectDescription _gravitySlamModified;

    private static void ModifyGravitySlam()
    {
        _gravitySlamVanilla = GravitySlam.EffectDescription;

        _gravitySlamModified = EffectDescriptionBuilder.Create(_gravitySlamVanilla)
            .SetTargetingData(Side.All, RangeType.Distance, 20, TargetType.Cylinder, 4, 10)
            .AddEffectForms(EffectFormBuilder.MotionForm(ExtraMotionType.PushDown, 10))
            .Build();

        ToggleGravitySlamModification();
    }

    internal static void ToggleGravitySlamModification()
    {
        if (Main.Settings.EnablePullPushOnVerticalDirection && Main.Settings.ModifyGravitySlam)
        {
            GravitySlam.effectDescription = _gravitySlamModified;
        }
        else
        {
            GravitySlam.effectDescription = _gravitySlamVanilla;
        }

        Global.RefreshControlledCharacter();
    }

    #endregion
}
