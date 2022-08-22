using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Tinkerer.Subclasses;
using SolastaCommunityExpansion.CustomDefinitions;
using static CharacterClassDefinition;
using static SolastaCommunityExpansion.Api.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Tinkerer;

internal static class TinkererClass
{
    public static readonly Guid GuidNamespace = new("7aee1270-7a61-48d9-8670-cf087c551c16");

    public static readonly FeatureDefinitionPower InfusionPool = FeatureDefinitionPowerPoolBuilder
        .Create("AttributeModiferArtificerInfusionHealingPool", GuidNamespace)
        .Configure(2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
            RuleDefinitions.RechargeRate.LongRest)
        .SetGuiPresentation("HealingPoolArtificerInfusions", Category.Subclass)
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
        InfusionHelpers.ArtificialServant,
        InfusionHelpers.EnhancedDefense,
        InfusionHelpers.BagOfHolding,
        InfusionHelpers.GogglesOfNight,
        InfusionHelpers.EnhancedFocus,
        InfusionHelpers.EnhancedWeapon,
        InfusionHelpers.MindSharpener,
        InfusionHelpers.ArmorOfMagicalStrength
    };

    private static readonly List<FeatureDefinition> Level6InfusionList = new()
    {
        InfusionHelpers.ResistantArmor,
        InfusionHelpers.SpellRefuelingRing,
        InfusionHelpers.BlindingWeapon,
        InfusionHelpers.BootsOfElvenKind,
        InfusionHelpers.CloakOfElvenKind
    };

    private static readonly List<FeatureDefinition> Level10InfusionList = new()
    {
        InfusionHelpers.BootsOfStridingAndSpringing,
        InfusionHelpers.BootsOfTheWinterland,
        InfusionHelpers.BroochOfShielding,
        InfusionHelpers.BracesrOfArchery,
        InfusionHelpers.CloakOfProtection,
        InfusionHelpers.GauntletsOfOgrePower,
        InfusionHelpers.GlovesOfMissileSnaring,
        InfusionHelpers.HeadbandOfIntellect,
        InfusionHelpers.SlippersOfSpiderClimbing
    };

    private static readonly List<FeatureDefinition> Level14InfusionList = new()
    {
        InfusionHelpers.AmuletOfHealth,
        InfusionHelpers.BeltOfGiantHillStrength,
        InfusionHelpers.BracersOfDefense,
        InfusionHelpers.RingProtectionPlus1
    };

    public static CharacterClassDefinition BuildTinkererClass()
    {
        var artificerBuilder = CharacterClassDefinitionBuilder
            .Create("ClassTinkerer", GuidNamespace)
            .SetHitDice(RuleDefinitions.DieType.D8)
            .AddPersonality(PersonalityFlagDefinitions.GpSpellcaster, 2)
            .AddPersonality(PersonalityFlagDefinitions.GpCombat, 3)
            .AddPersonality(PersonalityFlagDefinitions.GpExplorer, 1)
            .AddPersonality(PersonalityFlagDefinitions.Normal, 3);

        // Game background checks
        artificerBuilder.SetIngredientGatheringOdds(7);
        // I don't think this matters
        artificerBuilder.SetBattleAI(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
        artificerBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Cleric);
        // purposely left blank
        //public bool RequiresDeity { get; }
        //public List<string> ExpertiseAutolearnPreference { get; }

        // Auto select helpers
        artificerBuilder.AddToolPreferences(
            ToolTypeDefinitions.EnchantingToolType,
            ToolTypeDefinitions.ScrollKitType,
            ToolTypeDefinitions.ArtisanToolSmithToolsType,
            ToolTypeDefinitions.ThievesToolsType);

        artificerBuilder.SetAbilityScorePriorities(
            AttributeDefinitions.Intelligence,
            AttributeDefinitions.Constitution,
            AttributeDefinitions.Dexterity,
            AttributeDefinitions.Wisdom,
            AttributeDefinitions.Strength,
            AttributeDefinitions.Charisma);

        artificerBuilder.AddSkillPreferences(
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

        artificerBuilder.AddFeatPreferences(
            FeatDefinitions.Lockbreaker,
            FeatDefinitions.PowerfulCantrip,
            FeatDefinitions.Robust);

        // GUI
        artificerBuilder.SetPictogram(CharacterClassDefinitions.Wizard.ClassPictogramReference);
        artificerBuilder.SetGuiPresentation("Tinkerer", Category.Class,
            CharacterClassDefinitions.Wizard.GuiPresentation.SpriteReference, 1);

        // Complicated stuff

        // Starting equipment.
        artificerBuilder.AddEquipmentRow(
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
        artificerBuilder.AddEquipmentRow(
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
        artificerBuilder.AddEquipmentRow(mediumArmor, lightArmor);

        artificerBuilder.AddEquipmentRow(
            EquipmentOptionsBuilder.Option(ItemDefinitions.ArcaneFocusWand,
                EquipmentDefinitions.OptionArcaneFocusChoice, 1));

        artificerBuilder.AddEquipmentRow(
            EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
            EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1),
            EquipmentOptionsBuilder.Option(ItemDefinitions.ThievesTool, EquipmentDefinitions.OptionTool, 1),
            EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack,
                1));

        var armorProf = FeatureHelpers
            .BuildProficiency("ProficiencyArmorTinkerer", RuleDefinitions.ProficiencyType.Armor,
                EquipmentDefinitions.LightArmorCategory, EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory)
            .SetGuiPresentation("TinkererArmorProficiency", Category.Feature)
            .AddToDB();

        var weaponProf = FeatureHelpers
            .BuildProficiency("ProficiencyWeaponTinkerer", RuleDefinitions.ProficiencyType.Weapon,
                EquipmentDefinitions.SimpleWeaponCategory)
            .SetGuiPresentation("TinkererWeaponProficiency", Category.Feature)
            .AddToDB();

        var toolProf = FeatureHelpers
            .BuildProficiency("ProficiencyToolsTinkerer", RuleDefinitions.ProficiencyType.Tool,
                ToolTypeDefinitions.ThievesToolsType.Name, ToolTypeDefinitions.ScrollKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name, ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.EnchantingToolType.Name, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
            .SetGuiPresentation("TinkererToolsProficiency", Category.Feature)
            .AddToDB();

        var saveProf = FeatureHelpers
            .BuildProficiency("ProficiencyTinkererSavingThrow", RuleDefinitions.ProficiencyType.SavingThrow,
                AttributeDefinitions.Constitution, AttributeDefinitions.Intelligence)
            .SetGuiPresentation("SavingThrowTinkererProficiency", Category.Feature)
            .AddToDB();

        artificerBuilder.AddFeaturesAtLevel(1, armorProf, weaponProf, toolProf, saveProf);

        // skill point pool (1)
        var skillPoints = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolTinkererSkillPoints", GuidNamespace)
            .Configure(HeroDefinitions.PointsPoolType.Skill, 2, false,
                SkillDefinitions.Arcana, SkillDefinitions.History,
                SkillDefinitions.Investigation, SkillDefinitions.Medecine,
                SkillDefinitions.Nature, SkillDefinitions.Perception, SkillDefinitions.SleightOfHand)
            .SetGuiPresentation("Feature/&TinkererSkillPointsTitle",
                "Feature/&TinkererSkillGainChoicesPluralDescription")
            .AddToDB();

        artificerBuilder.AddFeatureAtLevel(1, skillPoints);

        // spell casting (1)
        var featureSpellCasting = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellTinkerer", GuidNamespace)
            .SetGuiPresentation("ArtificerSpellcasting", Category.Subclass)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(TinkererSpellList.SpellList)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.WholeList)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.Prepared)
            .SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusHalfLevel)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetSpellCastingLevel(1)
            .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .SetSlotsPerLevel(1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
            .AddToDB();

        var infusionChoice = FeatureDefinitionFeatureSetCustomBuilder
            .Create("TinkererInfusionChoice", GuidNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetRequireClassLevels(true)
            .SetLevelFeatures(2, Level2InfusionList)
            .SetLevelFeatures(6, Level6InfusionList)
            .SetLevelFeatures(10, Level10InfusionList)
            .SetLevelFeatures(14, Level14InfusionList)
            .AddToDB();

        var infusionReplace = FeatureDefinitionFeatureSetReplaceCustomBuilder
            .Create("TinkererInfusionReplace", GuidNamespace)
            .SetGuiPresentation(Category.Feature)
            .SetReplacedFeatureSet(infusionChoice)
            .AddToDB();

        artificerBuilder.AddFeatureAtLevel(1, featureSpellCasting);

        // ritual casting (1)
        artificerBuilder.AddFeatureAtLevel(1, FeatureDefinitionFeatureSets.FeatureSetClericRitualCasting);

        // Artificers can cast with "hands full" because they can cast while holding an infused item, just blanket saying ignore that requirement
        // is the closest reasonable option we have right now.
        artificerBuilder.AddFeatureAtLevel(1, BuildMagicAffinityHandsFull("MagicAffinityArtificerInfusionCasting",
            new GuiPresentationBuilder(
                "Feature/&ArtificerInfusionCastingTitle",
                "Feature/&ArtificerInfusionCastingDescription").Build()
        ));

        var bonusCantrips = FeatureDefinitionBonusCantripsBuilder
            .Create("TinkererMagicalTinkering", GuidNamespace)
            .SetGuiPresentation("TinkererMagicalTinkering", Category.Subclass)
            .SetBonusCantrips(SpellDefinitions.Shine, SpellDefinitions.Sparkle, SpellDefinitions.Dazzle)
            .AddToDB();

        artificerBuilder.AddFeatureAtLevel(2, bonusCantrips);

        // infuse item (level 2)
        // potentially give them "healing pool" points for the number of infusions, then abilities that provide a bonus for 24hrs which the player activates each day

        artificerBuilder.AddFeatureAtLevel(2, InfusionPool);

        // Infusions -- Focus, Weapon, Mind Sharpener, Armor of Magical Strength are given in subclasses
        // Defense

        artificerBuilder.AddFeatureAtLevel(2, infusionChoice, 4);
        artificerBuilder.AddFeatureAtLevel(3, infusionReplace);

        // Repeating Shot-- no point it seems
        // Returning Weapon-- not currently do-able

        // right tool for the job (level 3) (can I just give enchanting tool at level 3?)-- tools are available in the store, just skipping for now

        // ASI (4)
        artificerBuilder.AddFeatureAtLevel(4, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        artificerBuilder.AddFeatureAtLevel(4, infusionReplace);
        artificerBuilder.AddFeatureAtLevel(5, infusionReplace);
        // Tool expertise (level 6)
        var toolExpertise = FeatureHelpers.BuildProficiency("ExpertiseToolsTinkerer",
                RuleDefinitions.ProficiencyType.ToolOrExpertise,
                ToolTypeDefinitions.ThievesToolsType.Name, ToolTypeDefinitions.ScrollKitType.Name,
                ToolTypeDefinitions.PoisonersKitType.Name, ToolTypeDefinitions.HerbalismKitType.Name,
                ToolTypeDefinitions.EnchantingToolType.Name, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name)
            .SetGuiPresentation("TinkererToolsExpertise", Category.Feature)
            .AddToDB();

        artificerBuilder.AddFeatureAtLevel(6, toolExpertise);

        var infusionPoolIncrease = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtificerInfusionIncreaseHealingPool", GuidNamespace)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .SetGuiPresentation("HealingPoolArtificerInfusionsIncrease", Category.Subclass)
            .AddToDB();
        artificerBuilder.AddFeatureAtLevel(6, infusionPoolIncrease);

        artificerBuilder.AddFeatureAtLevel(6, infusionChoice, 2);
        artificerBuilder.AddFeatureAtLevel(6, infusionReplace);

        // Infusions
        // Repulsion Shield, +1 shield, reaction (charges) to push enemy away on hit, otherwise... unsure?

        // gloves of thievery-- should be do-able to add the skill bonuses -- all (maybe don't implement
        // Boots of the Winding Path-- probably not going to happen

        var noContent = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle");
        var geniusSaves = FeatureHelpers.BuildSavingThrowAffinity("TinkererFlashOfGeniusSavingThrow",
            AbilityScores, RuleDefinitions.CharacterSavingThrowAffinity.None,
            FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 1, RuleDefinitions.DieType.D4,
            false, noContent.Build());

        var geniusAbility = FeatureHelpers.BuildAbilityAffinity("TinkererFlashOfGeniusAbilityCheck",
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
            "Subclass/&TinkererFlashOfGeniusConditionTitle",
            "Subclass/&TinkererFlashOfGeniusConditionDescription");
        var flashCondition = FeatureHelpers.BuildCondition("TinkererFlashOfGeniusCondition",
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

        var flashOfGenius = new FeatureHelpers.FeatureDefinitionPowerBuilder("TinkererFlashOfGeniusPower",
                GuidNamespace,
                -1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.PermanentUnlessIncapacitated,
                -1, RuleDefinitions.RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence,
                flashEffect.Build())
            .SetGuiPresentation(Category.Subclass)
            .AddToDB();
        artificerBuilder.AddFeatureAtLevel(7, flashOfGenius);
        artificerBuilder.AddFeatureAtLevel(7, infusionReplace);
        // ASI (8)
        artificerBuilder.AddFeatureAtLevel(8, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        artificerBuilder.AddFeatureAtLevel(8, infusionReplace);

        // 09
        artificerBuilder.AddFeatureAtLevel(9, infusionReplace);

        // Magic Item Adept (10)
        var craftingTinkererMagicItemAdeptPresentation = new GuiPresentationBuilder(
            "Subclass/&CraftingTinkererMagicItemAdeptTitle",
            "Subclass/&CraftingTinkererMagicItemAdeptDescription");
        var craftingAffinity = new FeatureHelpers.FeatureDefinitionCraftingAffinityBuilder(
            "CraftingTinkererMagicItemAdept", GuidNamespace,
            new List<ToolTypeDefinition>
            {
                ToolTypeDefinitions.ThievesToolsType,
                ToolTypeDefinitions.ScrollKitType,
                ToolTypeDefinitions.PoisonersKitType,
                ToolTypeDefinitions.HerbalismKitType,
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.ArtisanToolSmithToolsType
            }, 0.25f, true, craftingTinkererMagicItemAdeptPresentation.Build()).AddToDB();
        artificerBuilder.AddFeatureAtLevel(10, craftingAffinity);
        // boost to infusions (many of the +1s become +2s)

        var infusionPoolIncrease10 = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtificerInfusionIncreaseHealingPool10", GuidNamespace)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .SetGuiPresentation("HealingPoolArtificerInfusionsIncrease", Category.Subclass)
            .AddToDB();
        artificerBuilder.AddFeatureAtLevel(10, infusionPoolIncrease10);

        artificerBuilder.AddFeaturesAtLevel(10,
            infusionChoice,
            infusionChoice,
            infusionReplace,
            InfusionHelpers.ImprovedEnhancedDefense,
            InfusionHelpers.ImprovedEnhancedFocus,
            InfusionHelpers.ImprovedEnhancedWeapon);
        // helm of awareness
        // winged boots-- probably not- it's a real complicated item

        // 11 spell storing item- no clue what to do
        var spellEffect = new EffectDescriptionBuilder();
        spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1,
            RuleDefinitions.TurnOccurenceType.EndOfTurn);
        spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
            RuleDefinitions.TargetType.Self);
        spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());
        var spellStoringItem = new FeatureHelpers.FeatureDefinitionPowerBuilder("TinkererSpellStoringItem",
                GuidNamespace,
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.BonusAction,
                1, RuleDefinitions.RechargeRate.LongRest, false, false, AttributeDefinitions.Intelligence,
                spellEffect.Build())
            .SetGuiPresentation("PowerTinkererSpellStoringItem", Category.Subclass,
                FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation
                    .SpriteReference)
            .AddToDB();
        artificerBuilder.AddFeatureAtLevel(11, spellStoringItem);
        artificerBuilder.AddFeatureAtLevel(11, infusionReplace);

        artificerBuilder.AddFeatureAtLevel(12, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        artificerBuilder.AddFeatureAtLevel(12, infusionReplace);

        // 13
        artificerBuilder.AddFeatureAtLevel(13, infusionReplace);

        // 14- magic item savant another attunement slot and ignore requirements on magic items
        // also another infusion slot
        var infusionPoolIncrease14 = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtificerInfusionIncreaseHealingPool14", GuidNamespace)
            .SetGuiPresentation("HealingPoolArtificerInfusionsIncrease", Category.Subclass)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .AddToDB();

        artificerBuilder.AddFeatureAtLevel(14, infusionPoolIncrease14);
        artificerBuilder.AddFeatureAtLevel(14, infusionChoice, 2);
        artificerBuilder.AddFeatureAtLevel(14, infusionReplace);
        // probably give several infusions another boost here
        // arcane propulsion armor

        // 15
        artificerBuilder.AddFeatureAtLevel(15, infusionReplace);

        // 16
        artificerBuilder.AddFeatureAtLevel(16, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        artificerBuilder.AddFeatureAtLevel(16, infusionReplace);

        // 17
        artificerBuilder.AddFeatureAtLevel(17, infusionReplace);

        // 18 - magic item master another attunement slot
        // also another infusion slot
        var infusionPoolIncrease18 = FeatureDefinitionPowerPoolModifierBuilder
            .Create("AttributeModiferArtificerInfusionIncreaseHealingPool18", GuidNamespace)
            .Configure(1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool)
            .SetGuiPresentation("HealingPoolArtificerInfusionsIncrease", Category.Subclass)
            .AddToDB();

        artificerBuilder.AddFeatureAtLevel(18, infusionPoolIncrease18);
        artificerBuilder.AddFeatureAtLevel(18, infusionChoice, 2);
        artificerBuilder.AddFeatureAtLevel(18, infusionReplace);

        artificerBuilder.AddFeatureAtLevel(19, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
        artificerBuilder.AddFeatureAtLevel(19, infusionReplace);

        var soulOfArtificeGui = new GuiPresentationBuilder(
            "Subclass/&PowerTinkererSoulOfArtificeSavesTitle",
            "Subclass/&PowerTinkererSoulOfArtificeSavesDescription");
        var soulOfArtificeSaves = FeatureHelpers.BuildSavingThrowAffinity("TinkererSoulOfArtificeSavingThrow",
            AbilityScores, RuleDefinitions.CharacterSavingThrowAffinity.None,
            FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 3, RuleDefinitions.DieType.D4,
            false, soulOfArtificeGui.Build());
        artificerBuilder.AddFeatureAtLevel(20, soulOfArtificeSaves);
        artificerBuilder.AddFeatureAtLevel(20, infusionReplace);

        // 20 - soul of artifice, +1 to saving throws for each attuned item (probably just give +6)
        // also an ability that lets you drop to 1 instead of 0 as an reaction, supposed to end one of your infusions, but maybe just use some other resource?

        var tinkerer = artificerBuilder.AddToDB();

        // Subclasses
        var subclasses = FeatureDefinitionSubclassChoiceBuilder
            .Create("SubclassChoiceArtificerSpecialistArchetypes", GuidNamespace)
            .SetGuiPresentation("ArtificerSpecialistArchetypes", Category.Feature)
            .SetSubclassSuffix("Specialist")
            .SetFilterByDeity(false)
            .SetSubclasses(
                AlchemistBuilder.Build(tinkerer),
                ArtilleristBuilder.Build(tinkerer, featureSpellCasting),
                BattleSmithBuilder.Build(tinkerer),
                ScoutSentinelTinkererSubclassBuilder.BuildAndAddSubclass())
            .AddToDB();

        artificerBuilder.AddFeatureAtLevel(3, subclasses);

        return tinkerer;
    }

    private static FeatureDefinitionMagicAffinity BuildMagicAffinityHandsFull(string name,
        GuiPresentation guiPresentation)
    {
        return new FeatureHelpers.FeatureDefinitionMagicAffinityBuilder(name, GuidNamespace, guiPresentation)
            .SetRitualCasting(RuleDefinitions.RitualCasting.Prepared)
            .AddToDB();
    }
}
