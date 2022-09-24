using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes.Artisan.Subclasses;
using static CharacterClassDefinition;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Classes.Artisan;

internal static class ArtisanClass
{
    private static SpellListDefinition _spellList;

    public static readonly Guid GuidNamespace = new("7aee1270-7a61-48d9-8670-cf087c551c16");

    public static readonly FeatureDefinitionPower InfusionPool = FeatureDefinitionPowerPoolBuilder
        .Create("AttributeModiferArtisanInfusionHealingPool", GuidNamespace)
        .Configure(2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
            RuleDefinitions.RechargeRate.LongRest)
        .SetGuiPresentation("HealingPoolArtisanInfusions", Category.Subclass)
        .AddToDB();

    private static readonly List<string> AbilityScores = new()
    {
        AttributeDefinitions.Strength,
        AttributeDefinitions.Dexterity,
        AttributeDefinitions.Constitution,
        AttributeDefinitions.Wisdom,
        AttributeDefinitions.Intelligence,
        AttributeDefinitions.Charisma
    };

    private static readonly List<FeatureDefinition> Level2InfusionList = new()
    {
        ArtisanInfusionHelper.ArtificialServant,
        ArtisanInfusionHelper.EnhancedDefense,
        ArtisanInfusionHelper.BagOfHolding,
        ArtisanInfusionHelper.GogglesOfNight,
        ArtisanInfusionHelper.EnhancedFocus,
        ArtisanInfusionHelper.EnhancedWeapon,
        ArtisanInfusionHelper.MindSharpener,
        ArtisanInfusionHelper.ArmorOfMagicalStrength
    };

    private static readonly List<FeatureDefinition> Level6InfusionList = new()
    {
        ArtisanInfusionHelper.ResistantArmor,
        ArtisanInfusionHelper.SpellRefuelingRing,
        ArtisanInfusionHelper.BlindingWeapon,
        ArtisanInfusionHelper.BootsOfElvenKind,
        ArtisanInfusionHelper.CloakOfElvenKind
    };

    private static readonly List<FeatureDefinition> Level10InfusionList = new()
    {
        ArtisanInfusionHelper.BootsOfStridingAndSpringing,
        ArtisanInfusionHelper.BootsOfTheWinterland,
        ArtisanInfusionHelper.BroochOfShielding,
        ArtisanInfusionHelper.BracesrOfArchery,
        ArtisanInfusionHelper.CloakOfProtection,
        ArtisanInfusionHelper.GauntletsOfOgrePower,
        ArtisanInfusionHelper.GlovesOfMissileSnaring,
        ArtisanInfusionHelper.HeadbandOfIntellect,
        ArtisanInfusionHelper.SlippersOfSpiderClimbing
    };

    private static readonly List<FeatureDefinition> Level14InfusionList = new()
    {
        ArtisanInfusionHelper.AmuletOfHealth,
        ArtisanInfusionHelper.BeltOfGiantHillStrength,
        ArtisanInfusionHelper.BracersOfDefense,
        ArtisanInfusionHelper.RingProtectionPlus1
    };

    public static SpellListDefinition SpellList =>
        _spellList ??=
            SpellListDefinitionBuilder.Create("SpellListArtisan", GuidNamespace)
                .SetGuiPresentation("SpellListArtisan", Category.Feature)
                .ClearSpells()
                // melee weapon attack (booming blade/green flame blade)
                // pull enemy towards you (thorn whip/lightning lure)
                // I'm surrounded (thunderclap/sword burst)
                .SetSpellsAtLevel(0, SpellDefinitions.AcidSplash, SpellDefinitions.DancingLights,
                    SpellDefinitions.FireBolt, SpellDefinitions.Guidance, SpellDefinitions.Light,
                    SpellDefinitions.PoisonSpray, SpellDefinitions.RayOfFrost,
                    SpellDefinitions.Resistance, /*ResonatingStrike,*/ SpellDefinitions.ShockingGrasp,
                    SpellDefinitions.SpareTheDying)
                // absorb elements, snare, catapult, tasha's caustic brew
                .SetSpellsAtLevel(1, SpellDefinitions.CureWounds, SpellDefinitions.DetectMagic,
                    SpellDefinitions.ExpeditiousRetreat, SpellDefinitions.FaerieFire, SpellDefinitions.FalseLife,
                    SpellDefinitions.FeatherFall,
                    SpellDefinitions.Grease, SpellDefinitions.Identify, SpellDefinitions.Jump,
                    SpellDefinitions.Longstrider)
                // web, pyrotechnics, heat metal, enlarge/reduce
                .SetSpellsAtLevel(2, SpellDefinitions.Aid, SpellDefinitions.Blur, SpellDefinitions.Darkvision,
                    SpellDefinitions.EnhanceAbility, SpellDefinitions.HeatMetal, SpellDefinitions.Invisibility,
                    SpellDefinitions.LesserRestoration,
                    SpellDefinitions.Levitate,
                    SpellDefinitions.MagicWeapon, SpellDefinitions.ProtectionFromPoison,
                    SpellDefinitions.SeeInvisibility, SpellDefinitions.SpiderClimb)
                // blink, elemental weapon, flame arrows
                .SetSpellsAtLevel(3, SpellDefinitions.CreateFood, SpellDefinitions.DispelMagic, SpellDefinitions.Fly,
                    SpellDefinitions.Haste, SpellDefinitions.ProtectionFromEnergy, SpellDefinitions.Revivify)
                // everything
                .SetSpellsAtLevel(4, SpellDefinitions.FreedomOfMovement, SpellDefinitions.Stoneskin)
                // everything
                .SetSpellsAtLevel(5, SpellDefinitions.GreaterRestoration)
                .FinalizeSpells()
                .AddToDB();

    public static CharacterClassDefinition BuildArtisanClass()
    {
        var artisanBuilder = CharacterClassDefinitionBuilder
            .Create("ClassArtisan", GuidNamespace)
            .SetHitDice(RuleDefinitions.DieType.D8);
        // .AddPersonality(PersonalityFlagDefinitions.GpSpellcaster, 2)
        // .AddPersonality(PersonalityFlagDefinitions.GpCombat, 3)
        // .AddPersonality(PersonalityFlagDefinitions.GpExplorer, 1)
        // .AddPersonality(PersonalityFlagDefinitions.Normal, 3);

        // Game background checks
        artisanBuilder.SetIngredientGatheringOdds(7);
        // I don't think this matters
        artisanBuilder.SetBattleAI(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
        artisanBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Cleric);
        // purposely left blank
        //public bool RequiresDeity { get; }
        //public List<string> ExpertiseAutolearnPreference { get; }

        // Auto select helpers
        artisanBuilder.AddToolPreferences(
            ToolTypeDefinitions.EnchantingToolType,
            ToolTypeDefinitions.ScrollKitType,
            ToolTypeDefinitions.ArtisanToolSmithToolsType,
            ToolTypeDefinitions.ThievesToolsType);

        artisanBuilder.SetAbilityScorePriorities(
            AttributeDefinitions.Intelligence,
            AttributeDefinitions.Constitution,
            AttributeDefinitions.Dexterity,
            AttributeDefinitions.Wisdom,
            AttributeDefinitions.Strength,
            AttributeDefinitions.Charisma);

        artisanBuilder.AddSkillPreferences(
            DatabaseHelper.SkillDefinitions.Investigation,
            DatabaseHelper.SkillDefinitions.Arcana,
            DatabaseHelper.SkillDefinitions.History,
            DatabaseHelper.SkillDefinitions.Perception,
            DatabaseHelper.SkillDefinitions.Stealth,
            DatabaseHelper.SkillDefinitions.SleightOfHand,
            DatabaseHelper.SkillDefinitions.Athletics,
            DatabaseHelper.SkillDefinitions.Insight,
            DatabaseHelper.SkillDefinitions.Persuasion,
            DatabaseHelper.SkillDefinitions.Nature);

        artisanBuilder.AddFeatPreferences(
            FeatDefinitions.Lockbreaker,
            FeatDefinitions.PowerfulCantrip,
            FeatDefinitions.Robust);

        // GUI
        artisanBuilder.SetPictogram(CharacterClassDefinitions.Wizard.ClassPictogramReference);
        artisanBuilder.SetGuiPresentation("Artisan", Category.Class,
            CharacterClassDefinitions.Wizard.GuiPresentation.SpriteReference, 1);

        // Complicated stuff

        // Starting equipment.
        artisanBuilder.AddEquipmentRow(
            new List<HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Mace,
                    EquipmentDefinitions.OptionWeapon, 1)
            },
            new List<HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Mace,
                    EquipmentDefinitions.OptionWeaponSimpleChoice, 1)
            }
        );
        artisanBuilder.AddEquipmentRow(
            new List<HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger,
                    EquipmentDefinitions.OptionWeapon, 1)
            },
            new List<HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(ItemDefinitions.Mace,
                    EquipmentDefinitions.OptionWeaponSimpleChoice, 1)
            }
        );

        var lightArmor = new List<HeroEquipmentOption>();
        var mediumArmor = new List<HeroEquipmentOption>();
        lightArmor.Add(EquipmentOptionsBuilder.Option(ItemDefinitions.StuddedLeather,
            EquipmentDefinitions.OptionArmor, 1));
        mediumArmor.Add(EquipmentOptionsBuilder.Option(ItemDefinitions.ScaleMail, EquipmentDefinitions.OptionArmor,
            1));
        artisanBuilder.AddEquipmentRow(mediumArmor, lightArmor);

        artisanBuilder.AddEquipmentRow(
            EquipmentOptionsBuilder.Option(ItemDefinitions.ArcaneFocusWand,
                EquipmentDefinitions.OptionArcaneFocusChoice, 1));

        artisanBuilder.AddEquipmentRow(
            EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
            EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1),
            EquipmentOptionsBuilder.Option(ItemDefinitions.ThievesTool, EquipmentDefinitions.OptionTool, 1),
            EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack,
                1));

        var armorProf = ArtisanHelpers
            .BuildProficiency("ProficiencyArmorArtisan", RuleDefinitions.ProficiencyType.Armor,
                EquipmentDefinitions.LightArmorCategory, EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .SetGuiPresentation("ArtisanArmorProficiency", Category.Feature)
            .AddToDB();

        var weaponProf = ArtisanHelpers
            .BuildProficiency("ProficiencyWeaponArtisan", RuleDefinitions.ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory)
            .SetGuiPresentation("ArtisanWeaponProficiency", Category.Feature)
            .AddToDB();

        var toolProf = ArtisanHelpers
            .BuildProficiency("ProficiencyToolsArtisan", RuleDefinitions.ProficiencyType.Tool,
                ToolTypeDefinitions.ThievesToolsType.Name, ToolTypeDefinitions.ScrollKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name, ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.EnchantingToolType.Name, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
            .SetGuiPresentation("ArtisanToolsProficiency", Category.Feature)
            .AddToDB();

        var saveProf = ArtisanHelpers
            .BuildProficiency("ProficiencyArtisanSavingThrow", RuleDefinitions.ProficiencyType.SavingThrow,
                AttributeDefinitions.Constitution, AttributeDefinitions.Intelligence)
            .SetGuiPresentation("SavingThrowArtisanProficiency", Category.Feature)
            .AddToDB();

        artisanBuilder.AddFeaturesAtLevel(1, armorProf, weaponProf, toolProf, saveProf);

        // skill point pool (1)
        var skillPoints = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolArtisanSkillPoints", GuidNamespace)
            .Configure(HeroDefinitions.PointsPoolType.Skill, 2, false,
                SkillDefinitions.Arcana, SkillDefinitions.History,
                SkillDefinitions.Investigation, SkillDefinitions.Medecine,
                SkillDefinitions.Nature, SkillDefinitions.Perception, SkillDefinitions.SleightOfHand)
            .SetGuiPresentation("Feature/&ArtisanSkillPointsTitle",
                "Feature/&ArtisanSkillGainChoicesPluralDescription")
            .AddToDB();

        artisanBuilder.AddFeatureAtLevel(1, skillPoints);

        // spell casting (1)
        var featureSpellCasting = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellArtisan", GuidNamespace)
            .SetGuiPresentation("ArtisanSpellcasting", Category.Subclass)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(SpellList)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.WholeList)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.Prepared)
            .SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusHalfLevel)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetSpellCastingLevel(1)
            .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .SetSlotsPerLevel(1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .AddToDB();

        // var infusionChoice = FeatureDefinitionFeatureSetCustomBuilder
        //     .Create("ArtisanInfusionChoice", GuidNamespace)
        //     .SetGuiPresentation(Category.Feature)
        //     .SetRequireClassLevels(true)
        //     .SetLevelFeatures(2, Level2InfusionList)
        //     .SetLevelFeatures(6, Level6InfusionList)
        //     .SetLevelFeatures(10, Level10InfusionList)
        //     .SetLevelFeatures(14, Level14InfusionList)
        //     .AddToDB();
        //
        // var infusionReplace = FeatureDefinitionFeatureSetReplaceCustomBuilder
        //     .Create("ArtisanInfusionReplace", GuidNamespace)
        //     .SetGuiPresentation(Category.Feature)
        //     .SetReplacedFeatureSet(infusionChoice)
        //     .AddToDB();

        artisanBuilder.AddFeatureAtLevel(1, featureSpellCasting);

        // ritual casting (1)
        artisanBuilder.AddFeatureAtLevel(1, FeatureDefinitionFeatureSets.FeatureSetClericRitualCasting);

        // Artisans can cast with "hands full" because they can cast while holding an infused item, just blanket saying ignore that requirement
        // is the closest reasonable option we have right now.
        artisanBuilder.AddFeatureAtLevel(1, BuildMagicAffinityHandsFull("MagicAffinityArtisanInfusionCasting",
            new GuiPresentationBuilder(
                "Feature/&ArtisanInfusionCastingTitle",
                "Feature/&ArtisanInfusionCastingDescription").Build()
        ));

        var bonusCantrips = FeatureDefinitionBonusCantripsBuilder
            .Create("ArtisanMagicalTinkering", GuidNamespace)
            .SetGuiPresentation("ArtisanMagicalTinkering", Category.Subclass)
            .SetBonusCantrips(SpellDefinitions.Shine, SpellDefinitions.Sparkle, SpellDefinitions.Dazzle)
            .AddToDB();

        artisanBuilder.AddFeatureAtLevel(2, bonusCantrips);

        // infuse item (level 2)
        // potentially give them "healing pool" points for the number of infusions, then abilities that provide a bonus for 24hrs which the player activates each day

        artisanBuilder.AddFeatureAtLevel(2, InfusionPool);

        // Infusions -- Focus, Weapon, Mind Sharpener, Armor of Magical Strength are given in subclasses
        // Defense

        // artisanBuilder.AddFeatureAtLevel(2, infusionChoice, 4);
        // artisanBuilder.AddFeatureAtLevel(3, infusionReplace);

        // Repeating Shot-- no point it seems
        // Returning Weapon-- not currently do-able

        // right tool for the job (level 3) (can I just give enchanting tool at level 3?)-- tools are available in the store, just skipping for now

        // ASI (4)
        artisanBuilder.AddFeatureAtLevel(4, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        // artisanBuilder.AddFeatureAtLevel(4, infusionReplace);
        // artisanBuilder.AddFeatureAtLevel(5, infusionReplace);
        // Tool expertise (level 6)
        var toolExpertise = ArtisanHelpers.BuildProficiency("ExpertiseToolsArtisan",
                RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ThievesToolsType.Name, ToolTypeDefinitions.ScrollKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name, ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.EnchantingToolType.Name, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
            .SetGuiPresentation("ArtisanToolsExpertise", Category.Feature)
            .AddToDB();

        artisanBuilder.AddFeatureAtLevel(6, toolExpertise);

        var infusionPoolIncrease = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtisanInfusionIncreaseHealingPool", GuidNamespace)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .SetGuiPresentation("HealingPoolArtisanInfusionsIncrease", Category.Subclass)
            .AddToDB();
        artisanBuilder.AddFeatureAtLevel(6, infusionPoolIncrease);

        // artisanBuilder.AddFeatureAtLevel(6, infusionChoice, 2);
        // artisanBuilder.AddFeatureAtLevel(6, infusionReplace);

        // Infusions
        // Repulsion Shield, +1 shield, reaction (charges) to push enemy away on hit, otherwise... unsure?

        // gloves of thievery-- should be do-able to add the skill bonuses -- all (maybe don't implement
        // Boots of the Winding Path-- probably not going to happen

        var noContent = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle");
        var geniusSaves = ArtisanHelpers.BuildSavingThrowAffinity("ArtisanFlashOfGeniusSavingThrow",
            AbilityScores, RuleDefinitions.CharacterSavingThrowAffinity.None,
            FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 1, RuleDefinitions.DieType.D4,
            false, noContent.Build());

        var geniusAbility = ArtisanHelpers.BuildAbilityAffinity("ArtisanFlashOfGeniusAbilityCheck",
            new List<Tuple<string, string>>
            {
                Tuple.Create(AttributeDefinitions.Strength, ""),
                Tuple.Create(AttributeDefinitions.Dexterity, ""),
                Tuple.Create(AttributeDefinitions.Constitution, ""),
                Tuple.Create(AttributeDefinitions.Wisdom, ""),
                Tuple.Create(AttributeDefinitions.Intelligence, ""),
                Tuple.Create(AttributeDefinitions.Charisma, "")
            }, 1, RuleDefinitions.DieType.D4,
            RuleDefinitions.CharacterAbilityCheckAffinity.None, noContent.Build());

        var flashOfGeniusConditionPresentation = new GuiPresentationBuilder(
            "Subclass/&ArtisanFlashOfGeniusConditionTitle",
            "Subclass/&ArtisanFlashOfGeniusConditionDescription");
        var flashCondition = ArtisanHelpers.BuildCondition("ArtisanFlashOfGeniusCondition",
            RuleDefinitions.DurationType.Hour, 1, true, flashOfGeniusConditionPresentation.Build(),
            geniusSaves, geniusAbility);

        var flashEffect = new EffectDescriptionBuilder();
        flashEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(flashCondition,
            ConditionForm.ConditionOperation.Add, true, false, new List<ConditionDefinition>()).Build());
        flashEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(flashCondition,
            ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
        flashEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 6,
            RuleDefinitions.TargetType.Sphere, 6, 6);
        flashEffect.SetTargetProximityData(true, 6);
        flashEffect.SetCreatedByCharacter();
        flashEffect.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnActivation |
                                       RuleDefinitions.RecurrentEffect.OnEnter |
                                       RuleDefinitions.RecurrentEffect.OnTurnStart);
        flashEffect.SetDurationData(RuleDefinitions.DurationType.Permanent, 0,
            RuleDefinitions.TurnOccurenceType.StartOfTurn);
        flashEffect.SetParticleEffectParameters(SpellDefinitions.Bless.EffectDescription.EffectParticleParameters);

        var flashOfGenius = new ArtisanHelpers.FeatureDefinitionPowerBuilder("ArtisanFlashOfGeniusPower",
                GuidNamespace,
                -1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.PermanentUnlessIncapacitated,
                -1, RuleDefinitions.RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence,
                flashEffect.Build())
            .SetGuiPresentation(Category.Subclass)
            .AddToDB();
        artisanBuilder.AddFeatureAtLevel(7, flashOfGenius);
        // artisanBuilder.AddFeatureAtLevel(7, infusionReplace);
        // ASI (8)
        artisanBuilder.AddFeatureAtLevel(8, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        // artisanBuilder.AddFeatureAtLevel(8, infusionReplace);

        // 09
        // artisanBuilder.AddFeatureAtLevel(9, infusionReplace);

        // Magic Item Adept (10)
        var craftingArtisanMagicItemAdeptPresentation = new GuiPresentationBuilder(
            "Subclass/&CraftingArtisanMagicItemAdeptTitle",
            "Subclass/&CraftingArtisanMagicItemAdeptDescription");
        var craftingAffinity = new ArtisanHelpers.FeatureDefinitionCraftingAffinityBuilder(
            "CraftingArtisanMagicItemAdept", GuidNamespace,
            new List<ToolTypeDefinition>
            {
                ToolTypeDefinitions.ThievesToolsType,
                ToolTypeDefinitions.ScrollKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.ArtisanToolSmithToolsType
            }, 0.25f, true, craftingArtisanMagicItemAdeptPresentation.Build()).AddToDB();
        artisanBuilder.AddFeatureAtLevel(10, craftingAffinity);
        // boost to infusions (many of the +1s become +2s)

        var infusionPoolIncrease10 = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtisanInfusionIncreaseHealingPool10", GuidNamespace)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .SetGuiPresentation("HealingPoolArtisanInfusionsIncrease", Category.Subclass)
            .AddToDB();
        artisanBuilder.AddFeatureAtLevel(10, infusionPoolIncrease10);

        artisanBuilder.AddFeaturesAtLevel(10,
            // infusionChoice,
            // infusionChoice,
            // infusionReplace,
            ArtisanInfusionHelper.ImprovedEnhancedDefense,
            ArtisanInfusionHelper.ImprovedEnhancedFocus,
            ArtisanInfusionHelper.ImprovedEnhancedWeapon);
        // helm of awareness
        // winged boots-- probably not- it's a real complicated item

        // 11 spell storing item- no clue what to do
        var spellEffect = new EffectDescriptionBuilder();
        spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
            RuleDefinitions.TargetType.Self);
        spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());
        var spellStoringItem = new ArtisanHelpers.FeatureDefinitionPowerBuilder("ArtisanSpellStoringItem",
                GuidNamespace,
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1, RuleDefinitions.RechargeRate.LongRest, false, false, AttributeDefinitions.Intelligence,
                spellEffect.Build())
            .SetGuiPresentation("PowerArtisanSpellStoringItem", Category.Subclass,
                FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation
                    .SpriteReference)
            .AddToDB();
        artisanBuilder.AddFeatureAtLevel(11, spellStoringItem);
        // artisanBuilder.AddFeatureAtLevel(11, infusionReplace);

        artisanBuilder.AddFeatureAtLevel(12, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        // artisanBuilder.AddFeatureAtLevel(12, infusionReplace);

        // 13
        // artisanBuilder.AddFeatureAtLevel(13, infusionReplace);

        // 14- magic item savant another attunement slot and ignore requirements on magic items
        // also another infusion slot
        var infusionPoolIncrease14 = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtisanInfusionIncreaseHealingPool14", GuidNamespace)
            .SetGuiPresentation("HealingPoolArtisanInfusionsIncrease", Category.Subclass)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .AddToDB();

        artisanBuilder.AddFeatureAtLevel(14, infusionPoolIncrease14);
        // artisanBuilder.AddFeatureAtLevel(14, infusionChoice, 2);
        // artisanBuilder.AddFeatureAtLevel(14, infusionReplace);
        // probably give several infusions another boost here
        // arcane propulsion armor

        // 15
        // artisanBuilder.AddFeatureAtLevel(15, infusionReplace);

        // 16
        artisanBuilder.AddFeatureAtLevel(16, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        // artisanBuilder.AddFeatureAtLevel(16, infusionReplace);

        // 17
        // artisanBuilder.AddFeatureAtLevel(17, infusionReplace);

        // 18 - magic item master another attunement slot
        // also another infusion slot
        var infusionPoolIncrease18 = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtisanInfusionIncreaseHealingPool18", GuidNamespace)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .SetGuiPresentation("HealingPoolArtisanInfusionsIncrease", Category.Subclass)
            .AddToDB();

        artisanBuilder.AddFeatureAtLevel(18, infusionPoolIncrease18);
        // artisanBuilder.AddFeatureAtLevel(18, infusionChoice, 2);
        // artisanBuilder.AddFeatureAtLevel(18, infusionReplace);

        artisanBuilder.AddFeatureAtLevel(19, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        // artisanBuilder.AddFeatureAtLevel(19, infusionReplace);

        var soulOfArtificeGui = new GuiPresentationBuilder(
            "Subclass/&PowerArtisanSoulOfArtificeSavesTitle",
            "Subclass/&PowerArtisanSoulOfArtificeSavesDescription");
        var soulOfArtificeSaves = ArtisanHelpers.BuildSavingThrowAffinity("ArtisanSoulOfArtificeSavingThrow",
            AbilityScores, RuleDefinitions.CharacterSavingThrowAffinity.None,
            FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 3, RuleDefinitions.DieType.D4,
            false, soulOfArtificeGui.Build());
        artisanBuilder.AddFeatureAtLevel(20, soulOfArtificeSaves);
        // artisanBuilder.AddFeatureAtLevel(20, infusionReplace);

        // 20 - soul of artifice, +1 to saving throws for each attuned item (probably just give +6)
        // also an ability that lets you drop to 1 instead of 0 as an reaction, supposed to end one of your infusions, but maybe just use some other resource?

        var artisan = artisanBuilder.AddToDB();

        // Subclasses
        var subclasses = FeatureDefinitionSubclassChoiceBuilder
            .Create("SubclassChoiceArtisanSpecialistArchetypes", GuidNamespace)
            .SetGuiPresentation("ArtisanSpecialistArchetypes", Category.Feature)
            .SetSubclassSuffix("Specialist")
            .SetFilterByDeity(false)
            .SetSubclasses(
                AlchemistBuilder.Build(artisan),
                BattleSmithBuilder.Build(artisan),
                ScoutSentinelBuilder.BuildAndAddSubclass())
            .AddToDB();

        artisanBuilder.AddFeatureAtLevel(3, subclasses);

        return artisan;
    }

    private static FeatureDefinitionMagicAffinity BuildMagicAffinityHandsFull(string name,
        GuiPresentation guiPresentation)
    {
        return new ArtisanHelpers.FeatureDefinitionMagicAffinityBuilder(name, GuidNamespace, guiPresentation)
            .SetRitualCasting(RuleDefinitions.RitualCasting.Prepared)
            .AddToDB();
    }
}
