using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Tinkerer.Subclasses;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using static CharacterClassDefinition;
using static SolastaModApi.DatabaseHelper;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal static class TinkererClass
    {
        public static readonly Guid GuidNamespace = new("7aee1270-7a61-48d9-8670-cf087c551c16");

        public static readonly FeatureDefinitionPower InfusionPool = new FeatureDefinitionPowerPoolBuilder("AttributeModiferArtificerInfusionHealingPool",
            GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionHealingPool").ToString(),
                2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, RuleDefinitions.RechargeRate.LongRest,
                new GuiPresentationBuilder("Subclass/&HealingPoolArtificerInfusionsTitle",
                "Subclass/&HealingPoolArtificerInfusionsDescription").Build()).AddToDB();

        private static readonly List<string> AbilityScores = new()
        {
            AttributeDefinitions.Strength,
            AttributeDefinitions.Dexterity,
            AttributeDefinitions.Constitution,
            AttributeDefinitions.Wisdom,
            AttributeDefinitions.Intelligence,
            AttributeDefinitions.Charisma,
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
            InfusionHelpers.ArmorOfMagicalStrength,
        };

        private static readonly List<FeatureDefinition> Level6InfusionList = new(Level2InfusionList)
        {
            InfusionHelpers.ResistantArmor,
            InfusionHelpers.SpellRefuelingRing,
            InfusionHelpers.BlindingWeapon,
            InfusionHelpers.BootsOfElvenKind,
            InfusionHelpers.CloakOfElvenKind,
        };

        private static readonly List<FeatureDefinition> Level10InfusionList = new(Level6InfusionList)
        {
            InfusionHelpers.BracesrOfArchery,
            InfusionHelpers.CloakOfProtection,
            InfusionHelpers.GauntletsOfOgrePower,
            InfusionHelpers.HeadbandOfIntellect,
            InfusionHelpers.SlippersOfSpiderClimbing,
        };

        private static readonly List<FeatureDefinition> Level14InfusionList = new(Level10InfusionList)
        {
            InfusionHelpers.AmuletOfHealth,
            InfusionHelpers.BeltOfGiantHillStrength,
            InfusionHelpers.BracersOfDefense,
            InfusionHelpers.RingProtectionPlus1,
        };

        public static CharacterClassDefinition BuildTinkererClass()
        {
            CharacterClassDefinitionBuilder ArtificerBuilder = new CharacterClassDefinitionBuilder("ClassTinkerer", GuidHelper.Create(GuidNamespace, "ClassTinkerer").ToString());
            ArtificerBuilder.SetHitDice(RuleDefinitions.DieType.D8);
            ArtificerBuilder.AddPersonality(PersonalityFlagDefinitions.GpSpellcaster, 2);
            ArtificerBuilder.AddPersonality(PersonalityFlagDefinitions.GpCombat, 3);
            ArtificerBuilder.AddPersonality(PersonalityFlagDefinitions.GpExplorer, 1);
            ArtificerBuilder.AddPersonality(PersonalityFlagDefinitions.Normal, 3);

            // Game background checks
            ArtificerBuilder.SetIngredientGatheringOdds(7);
            // I don't think this matters
            ArtificerBuilder.SetBattleAI(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
            ArtificerBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Cleric);
            // purposely left blank
            //public bool RequiresDeity { get; }
            //public List<string> ExpertiseAutolearnPreference { get; }

            // Auto select helpers
            ArtificerBuilder.AddToolPreferences(
                ToolTypeDefinitions.EnchantingToolType,
                ToolTypeDefinitions.ScrollKitType,
                ToolTypeDefinitions.ArtisanToolSmithToolsType,
                ToolTypeDefinitions.ThievesToolsType);

            ArtificerBuilder.SetAbilityScorePriorities(
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Charisma);

            ArtificerBuilder.AddSkillPreferences(
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

            ArtificerBuilder.AddFeatPreferences(
                FeatDefinitions.Lockbreaker,
                FeatDefinitions.PowerfulCantrip,
                FeatDefinitions.Robust);

            // GUI
            ArtificerBuilder.SetPictogram(CharacterClassDefinitions.Wizard.ClassPictogramReference);
            ArtificerBuilder.SetGuiPresentation("Tinkerer", Category.Class,
                CharacterClassDefinitions.Wizard.GuiPresentation.SpriteReference, 1);

            // Complicated stuff

            // Starting equipment.
            List<HeroEquipmentOption> simpleWeaponList = new List<HeroEquipmentOption>();
            HeroEquipmentOption simpleWeaponOption = EquipmentOptionsBuilder.Option(ItemDefinitions.Mace, EquipmentDefinitions.OptionWeaponSimpleChoice, 1);
            simpleWeaponList.Add(simpleWeaponOption);
            simpleWeaponList.Add(simpleWeaponOption);
            ArtificerBuilder.AddEquipmentRow(simpleWeaponList);

            List<HeroEquipmentOption> lightArmor = new List<HeroEquipmentOption>();
            List<HeroEquipmentOption> mediumArmor = new List<HeroEquipmentOption>();
            lightArmor.Add(EquipmentOptionsBuilder.Option(ItemDefinitions.StuddedLeather, EquipmentDefinitions.OptionArmor, 1));
            mediumArmor.Add(EquipmentOptionsBuilder.Option(ItemDefinitions.ScaleMail, EquipmentDefinitions.OptionArmor, 1));
            ArtificerBuilder.AddEquipmentRow(lightArmor, mediumArmor);

            ArtificerBuilder.AddEquipmentRow(
                EquipmentOptionsBuilder.Option(ItemDefinitions.ArcaneFocusWand, EquipmentDefinitions.OptionArcaneFocusChoice, 1));

            ArtificerBuilder.AddEquipmentRow(
                EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow, EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt, EquipmentDefinitions.OptionAmmoPack, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.ThievesTool, EquipmentDefinitions.OptionTool, 1),
                EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack, EquipmentDefinitions.OptionStarterPack, 1));

            //public List<FeatureUnlockByLevel> FeatureUnlocks { get; }
            FeatureDefinitionProficiency armorProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.Armor,
                new List<string>() { EquipmentDefinitions.LightArmorCategory, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory },
                "ProficiencyArmorTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererArmorProficiencyTitle",
                    "Feature/&TinkererArmorTrainingShortDescription").Build());
            ArtificerBuilder.AddFeatureAtLevel(1, armorProf);

            FeatureDefinitionProficiency weaponProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.Weapon,
                new List<string>() { EquipmentDefinitions.SimpleWeaponCategory },
                "ProficiencyWeaponTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererWeaponProficiencyTitle",
                    "Feature/&TinkererWeaponTrainingShortDescription").Build());
            ArtificerBuilder.AddFeatureAtLevel(1, weaponProf);

            FeatureDefinitionProficiency toolProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.Tool,
                new List<string>() { ToolTypeDefinitions.ThievesToolsType.Name, ToolTypeDefinitions.ScrollKitType.Name,
                    ToolTypeDefinitions.PoisonersKitType.Name, ToolTypeDefinitions.HerbalismKitType.Name,
                    ToolTypeDefinitions.EnchantingToolType.Name, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name},
                "ProficiencyToolsTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererToolsProficiencyTitle",
                    "Feature/&TinkererToolProficiencyPluralShortDescription").Build());
            ArtificerBuilder.AddFeatureAtLevel(1, toolProf);

            FeatureDefinitionProficiency saveProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.SavingThrow,
                new List<string>() { AttributeDefinitions.Constitution, AttributeDefinitions.Intelligence },
                "ProficiencyTinkererSavingThrow",
                new GuiPresentationBuilder(
                    "Feature/&SavingThrowTinkererProficiencyTitle",
                    "Feature/&SavingThrowTinkererProficiencyDescription").Build());
            ArtificerBuilder.AddFeatureAtLevel(1, saveProf);

            // skill point pool (1)
            FeatureDefinitionPointPool skillPoints = FeatureHelpers.BuildPointPool(HeroDefinitions.PointsPoolType.Skill, 2,
                new List<string>() { SkillDefinitions.Arcana, SkillDefinitions.History, SkillDefinitions.Investigation, SkillDefinitions.Medecine,
                    SkillDefinitions.Nature, SkillDefinitions.Perception, SkillDefinitions.SleightOfHand },
                "PointPoolTinkererSkillPoints",
                new GuiPresentationBuilder(
                    "Feature/&TinkererSkillPointsTitle",
                    "Feature/&TinkererSkillGainChoicesPluralDescription").Build());
            ArtificerBuilder.AddFeatureAtLevel(1, skillPoints);

            SpellListDefinition spellList = TinkererSpellList.BuildAndAddToDB();

            // spell casting (1)
            var featureSpellCasting = FeatureDefinitionCastSpellBuilder
                .Create("CastSpellTinkerer", GuidNamespace)
                .SetGuiPresentation("ArtificerSpellcasting", Category.Subclass)
                .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
                .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
                .SetSpellList(spellList)
                .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.WholeList)
                .SetSpellReadyness(RuleDefinitions.SpellReadyness.Prepared)
                .SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusHalfLevel)
                .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
                .SetSpellCastingLevel(1)
                .SetKnownCantrips(2, 1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
                .SetSlotsPerLevel(1, FeatureDefinitionCastSpellBuilder.CasterProgression.HALF_CASTER)
                .AddToDB();

            ArtificerBuilder.AddFeatureAtLevel(1, featureSpellCasting);

            // ritual casting (1)
            ArtificerBuilder.AddFeatureAtLevel(1, FeatureDefinitionFeatureSets.FeatureSetClericRitualCasting);

            // Artificers can cast with "hands full" because they can cast while holding an infused item, just blanket saying ignore that requirement
            // is the closest reasonable option we have right now.
            ArtificerBuilder.AddFeatureAtLevel(1, BuildMagicAffinityHandsFull("MagicAffinityArtificerInfusionCasting", new GuiPresentationBuilder(
                "Feature/&ArtificerInfusionCastingTitle",
                "Feature/&ArtificerInfusionCastingDescription").Build()
                ));

            GuiPresentationBuilder magicalTinkeringGui = new GuiPresentationBuilder(
                "Subclass/&TinkererMagicalTinkeringTitle",
                "Subclass/&TinkererMagicalTinkeringDescription");
            ArtificerBuilder.AddFeatureAtLevel(2, FeatureHelpers.BuildBonusCantrips(new List<SpellDefinition>()
            {
                SpellDefinitions.Shine,
                SpellDefinitions.Sparkle,
                SpellDefinitions.Dazzle,
            }, "TinkererMagicalTinkering", magicalTinkeringGui.Build()));

            // infuse item (level 2)
            // potentially give them "healing pool" points for the number of infusions, then abilities that provide a bonus for 24hrs which the player activates each day

            ArtificerBuilder.AddFeatureAtLevel(2, InfusionPool);

            // Infusions -- Focus, Weapon, Mind Sharpener, Armor of Magical Strength are given in subclasses
            // Defense
            GuiPresentationBuilder infusionChoiceGui = new GuiPresentationBuilder(
                "Subclass/&TinkererInfusionChoiceTitle",
                "Subclass/&TinkererInfusionChoiceDescription");
            FeatureDefinitionFeatureSet level2Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel2InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel2InfusionChoice").ToString(),
                Level2InfusionList, FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, false, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(2, level2Infusions, 4);

            // Repeating Shot-- no point it seems
            // Returning Weapon-- not currently do-able

            // right tool for the job (level 3) (can I just give enchanting tool at level 3?)-- tools are available in the store, just skipping for now

            // ASI (4)
            ArtificerBuilder.AddFeatureAtLevel(4, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            // Tool expertise (level 6)
            FeatureDefinitionProficiency toolExpertise = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                new List<string>() { ToolTypeDefinitions.ThievesToolsType.Name, ToolTypeDefinitions.ScrollKitType.Name,
                    ToolTypeDefinitions.PoisonersKitType.Name, ToolTypeDefinitions.HerbalismKitType.Name,
                    ToolTypeDefinitions.EnchantingToolType.Name, ToolTypeDefinitions.ArtisanToolSmithToolsType.Name},
                "ExpertiseToolsTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererToolsExpertiseTitle",
                    "Feature/&TinkererToolsExpertisePluralShortDescription").Build());
            ArtificerBuilder.AddFeatureAtLevel(6, toolExpertise);

            GuiPresentationBuilder InfusionPoolIncreaseGui = new GuiPresentationBuilder(
                "Subclass/&HealingPoolArtificerInfusionsIncreaseTitle",
                "Subclass/&HealingPoolArtificerInfusionsIncreaseDescription");
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(6, InfusionPoolIncrease);

            FeatureDefinitionFeatureSet level6Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel6InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel6InfusionChoice").ToString(), Level6InfusionList,
                FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, true, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(6, level6Infusions, 2);
            // Infusions
            // Repulsion Shield, +1 shield, reaction (charges) to push enemy away on hit, otherwise... unsure?

            // gloves of thievery-- should be do-able to add the skill bonuses -- all (maybe don't implement
            // Boots of the Winding Path-- probably not going to happen

            GuiPresentationBuilder noContent = new GuiPresentationBuilder("Feature/&NoContentTitle", "Feature/&NoContentTitle");
            FeatureDefinitionSavingThrowAffinity geniusSaves = FeatureHelpers.BuildSavingThrowAffinity(AbilityScores,
                RuleDefinitions.CharacterSavingThrowAffinity.None, FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 1, RuleDefinitions.DieType.D4, false,
            "TinkererFlashOfGeniusSavingThrow", noContent.Build());

            FeatureDefinitionAbilityCheckAffinity geniusAbility = FeatureHelpers.BuildAbilityAffinity(new List<Tuple<string, string>>()
            {
                Tuple.Create(AttributeDefinitions.Strength, ""),
                Tuple.Create(AttributeDefinitions.Dexterity, ""),
                Tuple.Create(AttributeDefinitions.Constitution, ""),
                Tuple.Create(AttributeDefinitions.Wisdom, ""),
                Tuple.Create(AttributeDefinitions.Intelligence, ""),
                Tuple.Create(AttributeDefinitions.Charisma, ""),
            }, 1, RuleDefinitions.DieType.D4, RuleDefinitions.CharacterAbilityCheckAffinity.None,
            "TinkererFlashOfGeniusAbilityCheck", noContent.Build());

            GuiPresentationBuilder flashOfGeniusConditionPresentation = new GuiPresentationBuilder(
                "Subclass/&TinkererFlashOfGeniusConditionTitle",
                "Subclass/&TinkererFlashOfGeniusConditionDescription");
            ConditionDefinition flashCondition = FeatureHelpers.BuildCondition(new List<FeatureDefinition>() {
                geniusSaves,
                geniusAbility,
            },
                RuleDefinitions.DurationType.Hour, 1, true, "TinkererFlashOfGeniusCondition", flashOfGeniusConditionPresentation.Build());

            EffectDescriptionBuilder flashEffect = new EffectDescriptionBuilder();
            flashEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(flashCondition, ConditionForm.ConditionOperation.Add, true, false, new List<ConditionDefinition>()).Build());
            flashEffect.AddEffectForm(new EffectFormBuilder().SetConditionForm(flashCondition, ConditionForm.ConditionOperation.Add, false, false, new List<ConditionDefinition>()).Build());
            flashEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 6, RuleDefinitions.TargetType.Sphere, 6, 6, ActionDefinitions.ItemSelectionType.None);
            flashEffect.SetTargetProximityData(true, 6);
            flashEffect.SetCreatedByCharacter();
            flashEffect.SetRecurrentEffect(RuleDefinitions.RecurrentEffect.OnActivation | RuleDefinitions.RecurrentEffect.OnEnter | RuleDefinitions.RecurrentEffect.OnTurnStart);
            flashEffect.SetDurationData(RuleDefinitions.DurationType.Permanent, 0, RuleDefinitions.TurnOccurenceType.StartOfTurn);
            flashEffect.SetParticleEffectParameters(SpellDefinitions.Bless.EffectDescription.EffectParticleParameters);

            GuiPresentationBuilder flashOfGeniusPresentation = new GuiPresentationBuilder(
                "Subclass/&TinkererFlashOfGeniusPowerTitle",
                "Subclass/&TinkererFlashOfGeniusPowerDescription");

            FeatureDefinitionPower flashOfGenius = new FeatureHelpers.FeatureDefinitionPowerBuilder("TinkererFlashOfGeniusPower", GuidHelper.Create(GuidNamespace, "TinkererFlashOfGeniusPower").ToString(),
                -1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.PermanentUnlessIncapacitated,
                -1, RuleDefinitions.RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, flashEffect.Build(), flashOfGeniusPresentation.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(7, flashOfGenius);

            // ASI (8)
            ArtificerBuilder.AddFeatureAtLevel(8, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            // Magic Item Adept (10)
            GuiPresentationBuilder CraftingTinkererMagicItemAdeptPresentation = new GuiPresentationBuilder(
                "Subclass/&CraftingTinkererMagicItemAdeptTitle",
                "Subclass/&CraftingTinkererMagicItemAdeptDescription");
            FeatureDefinitionCraftingAffinity craftingAffinity = new FeatureHelpers.FeatureDefinitionCraftingAffinityBuilder("CraftingTinkererMagicItemAdept", GuidHelper.Create(GuidNamespace, "CraftingTinkererMagicItemAdept").ToString(),
                new List<ToolTypeDefinition>()
                {
                    ToolTypeDefinitions.ThievesToolsType, ToolTypeDefinitions.ScrollKitType,
                    ToolTypeDefinitions.PoisonersKitType, ToolTypeDefinitions.HerbalismKitType,
                    ToolTypeDefinitions.EnchantingToolType, ToolTypeDefinitions.ArtisanToolSmithToolsType,
                }, 0.25f, true, CraftingTinkererMagicItemAdeptPresentation.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(10, craftingAffinity);
            // boost to infusions (many of the +1s become +2s)
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease10 = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool10",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool10").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(10, InfusionPoolIncrease10);

            FeatureDefinitionFeatureSet level10Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel10InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel10InfusionChoice").ToString(), Level10InfusionList,
                FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, true, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeaturesAtLevel(10,
                level10Infusions,
                level10Infusions,
                InfusionHelpers.ImprovedEnhancedDefense,
                InfusionHelpers.ImprovedEnhancedFocus,
                InfusionHelpers.ImprovedEnhancedWeapon);
            // helm of awareness
            // winged boots-- probably not- it's a real complicated item

            // 11 spell storing item- no clue what to do
            GuiPresentationBuilder SpellStoringItemGui = new GuiPresentationBuilder(
                "Subclass/&PowerTinkererSpellStoringItemTitle",
                "Subclass/&PowerTinkererSpellStoringItemDescription");
            SpellStoringItemGui.SetSpriteReference(FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder spellEffect = new EffectDescriptionBuilder();
            spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());
            FeatureDefinitionPower spellStoringItem = new FeatureHelpers.FeatureDefinitionPowerBuilder("TinkererSpellStoringItem", GuidHelper.Create(GuidNamespace, "TinkererSpellStoringItem").ToString(),
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.BonusAction,
                1, RuleDefinitions.RechargeRate.LongRest, false, false, AttributeDefinitions.Intelligence, spellEffect.Build(),
                SpellStoringItemGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(11, spellStoringItem);

            ArtificerBuilder.AddFeatureAtLevel(12, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            // 14- magic item savant another attunement slot and ignore requirements on magic items
            // also another infusion slot
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease14 = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool14",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool14").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(14, InfusionPoolIncrease14);
            FeatureDefinitionFeatureSet level14Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel14InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel14InfusionChoice").ToString(), Level14InfusionList,
                FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, true, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(14, level14Infusions, 2);
            // probably give several infusions another boost here
            // arcane propulsion armor

            ArtificerBuilder.AddFeatureAtLevel(16, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            // 18 - magic item master another attunement slot
            // also another infusion slot
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease18 = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool18",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool18").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(18, InfusionPoolIncrease18);
            ArtificerBuilder.AddFeatureAtLevel(18, level14Infusions, 2);

            ArtificerBuilder.AddFeatureAtLevel(19, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);

            GuiPresentationBuilder SoulOfArtificeGui = new GuiPresentationBuilder(
                "Subclass/&PowerTinkererSoulOfArtificeSavesTitle",
                "Subclass/&PowerTinkererSoulOfArtificeSavesDescription");
            FeatureDefinitionSavingThrowAffinity soulOfArtificeSaves = FeatureHelpers.BuildSavingThrowAffinity(AbilityScores, RuleDefinitions.CharacterSavingThrowAffinity.None, FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 3, RuleDefinitions.DieType.D4, false,
            "TinkererSoulOfArtificeSavingThrow", SoulOfArtificeGui.Build());
            ArtificerBuilder.AddFeatureAtLevel(20, soulOfArtificeSaves);

            // 20 - soul of artifice, +1 to saving throws for each attuned item (probably just give +6)
            // also an ability that lets you drop to 1 instead of 0 as an reaction, supposed to end one of your infusions, but maybe just use some other resource?

            CharacterClassDefinition tinkerer = ArtificerBuilder.AddToDB();

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

            ArtificerBuilder.AddFeatureAtLevel(3, subclasses);

            return tinkerer;
        }

        private static FeatureDefinitionMagicAffinity BuildMagicAffinityHandsFull(string name, GuiPresentation guiPresentation)
        {
            return new FeatureHelpers.FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(GuidNamespace, name).ToString(), guiPresentation).AddToDB();
        }
    }
}
