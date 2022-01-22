using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.BuilderHelpers;
using System;
using System.Collections.Generic;
using UnityEngine;
using static CharacterClassDefinition;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal static class TinkererClass
    {
        public static Guid GuidNamespace = new Guid("7aee1270-7a61-48d9-8670-cf087c551c16");

        public static readonly FeatureDefinitionPower InfusionPool = new FeatureDefinitionPowerPoolBuilder("AttributeModiferArtificerInfusionHealingPool",
            GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionHealingPool").ToString(),
                2, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, RuleDefinitions.RechargeRate.LongRest,
                new GuiPresentationBuilder("Subclass/&HealingPoolArtificerInfusionsDescription",
                "Subclass/&HealingPoolArtificerInfusionsTitle").Build()).AddToDB();

        private static readonly List<string> AbilityScores = new List<string>
            {
                AttributeDefinitions.Strength,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence,
                AttributeDefinitions.Charisma,
            };

        private static readonly List<FeatureDefinition> Level2InfusionList = new List<FeatureDefinition>
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

        private static readonly List<FeatureDefinition> Level6InfusionList = new List<FeatureDefinition>(Level2InfusionList)
        {
                    InfusionHelpers.ResistantArmor,
                    InfusionHelpers.SpellRefuelingRing,
                    InfusionHelpers.BlindingWeapon,
                    InfusionHelpers.BootsOfElvenKind,
                    InfusionHelpers.CloakOfElvenKind,
        };

        private static readonly List<FeatureDefinition> Level10InfusionList = new List<FeatureDefinition>(Level6InfusionList)
                {
                    InfusionHelpers.BracesrOfArchery,
                    InfusionHelpers.CloakOfProtection,
                    InfusionHelpers.GauntletsOfOgrePower,
                    InfusionHelpers.HeadbandOfIntellect,
                    InfusionHelpers.SlippersOfSpiderClimbing,
                };

        private static readonly List<FeatureDefinition> Level14InfusionList = new List<FeatureDefinition>(Level10InfusionList)
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
            ArtificerBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpSpellcaster, 2);
            ArtificerBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpCombat, 3);
            ArtificerBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.GpExplorer, 1);
            ArtificerBuilder.AddPersonality(DatabaseHelper.PersonalityFlagDefinitions.Normal, 3);

            // Game background checks
            ArtificerBuilder.SetIngredientGatheringOdds(7);
            // I don't think this matters
            ArtificerBuilder.SetBattleAI(DatabaseHelper.DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
            ArtificerBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Cleric);
            // purposely left blank
            //public bool RequiresDeity { get; }
            //public List<string> ExpertiseAutolearnPreference { get; }

            // Auto select helpers
            ArtificerBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.EnchantingToolType);
            ArtificerBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.ScrollKitType);
            ArtificerBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType);
            ArtificerBuilder.AddToolPreference(DatabaseHelper.ToolTypeDefinitions.ThievesToolsType);
            ArtificerBuilder.SetAbilityScorePriorities(AttributeDefinitions.Intelligence, AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity, AttributeDefinitions.Wisdom, AttributeDefinitions.Strength, AttributeDefinitions.Charisma);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Investigation);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Arcana);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.History);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Perception);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Stealth);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.SleightOfHand);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Athletics);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Insight);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Persuasion);
            ArtificerBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Nature);
            ArtificerBuilder.AddFeatPreference(DatabaseHelper.FeatDefinitions.Lockbreaker);
            ArtificerBuilder.AddFeatPreference(DatabaseHelper.FeatDefinitions.PowerfulCantrip);
            ArtificerBuilder.AddFeatPreference(DatabaseHelper.FeatDefinitions.Robust);

            // GUI
            ArtificerBuilder.SetPictogram(DatabaseHelper.CharacterClassDefinitions.Wizard.ClassPictogramReference);
            GuiPresentationBuilder guiBuilder = new GuiPresentationBuilder(
                "Class/&TinkererDescription",
                "Class/&TinkererTitle");
            guiBuilder.SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));
            guiBuilder.SetSortOrder(1);
            guiBuilder.SetSpriteReference(DatabaseHelper.CharacterClassDefinitions.Wizard.GuiPresentation.SpriteReference);
            ArtificerBuilder.SetGuiPresentation(guiBuilder.Build());

            // Complicated stuff

            // Starting equipment.
            List<HeroEquipmentOption> simpleWeaponList = new List<HeroEquipmentOption>();
            HeroEquipmentOption simpleWeaponOption = EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.Mace, EquipmentDefinitions.OptionWeaponSimpleChoice, 1);
            simpleWeaponList.Add(simpleWeaponOption);
            simpleWeaponList.Add(simpleWeaponOption);
            ArtificerBuilder.AddEquipmentRow(simpleWeaponList);

            List<HeroEquipmentOption> lightArmor = new List<HeroEquipmentOption>();
            List<HeroEquipmentOption> mediumArmor = new List<HeroEquipmentOption>();
            lightArmor.Add(EquipmentOptionsBuilder.Option(
                DatabaseHelper.ItemDefinitions.StuddedLeather, EquipmentDefinitions.OptionArmor, 1));
            mediumArmor.Add(EquipmentOptionsBuilder.Option(
                DatabaseHelper.ItemDefinitions.ScaleMail, EquipmentDefinitions.OptionArmor, 1));
            ArtificerBuilder.AddEquipmentRow(lightArmor, mediumArmor);

            List<HeroEquipmentOption> focus = new List<HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(DatabaseHelper.ItemDefinitions.ArcaneFocusWand, EquipmentDefinitions.OptionArcaneFocusChoice, 1)
            };
            ArtificerBuilder.AddEquipmentRow(focus);

            List<HeroEquipmentOption> providedGear = new List<HeroEquipmentOption>
            {
                EquipmentOptionsBuilder.Option(
                DatabaseHelper.ItemDefinitions.LightCrossbow,
                EquipmentDefinitions.OptionWeapon, 1),
                EquipmentOptionsBuilder.Option(
                DatabaseHelper.ItemDefinitions.Bolt,
                EquipmentDefinitions.OptionAmmoPack, 1),
                EquipmentOptionsBuilder.Option(
                DatabaseHelper.ItemDefinitions.ThievesTool,
                EquipmentDefinitions.OptionTool, 1),
                EquipmentOptionsBuilder.Option(
                DatabaseHelper.ItemDefinitions.DungeoneerPack,
                EquipmentDefinitions.OptionStarterPack, 1)
            };
            ArtificerBuilder.AddEquipmentRow(providedGear);


            //public List<FeatureUnlockByLevel> FeatureUnlocks { get; }
            FeatureDefinitionProficiency armorProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.Armor,
                new List<string>() { EquipmentDefinitions.LightArmorCategory, EquipmentDefinitions.MediumArmorCategory, EquipmentDefinitions.ShieldCategory },
                "ProficiencyArmorTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererArmorTrainingShortDescription",
                    "Feature/&TinkererArmorProficiencyTitle").Build());
            ArtificerBuilder.AddFeatureAtLevel(armorProf, 1);

            FeatureDefinitionProficiency weaponProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.Weapon,
                new List<string>() { EquipmentDefinitions.SimpleWeaponCategory },
                "ProficiencyWeaponTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererWeaponTrainingShortDescription",
                    "Feature/&TinkererWeaponProficiencyTitle").Build());
            ArtificerBuilder.AddFeatureAtLevel(weaponProf, 1);

            FeatureDefinitionProficiency toolProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.Tool,
                new List<string>() { DatabaseHelper.ToolTypeDefinitions.ThievesToolsType.Name, DatabaseHelper.ToolTypeDefinitions.ScrollKitType.Name,
                    DatabaseHelper.ToolTypeDefinitions.PoisonersKitType.Name, DatabaseHelper.ToolTypeDefinitions.HerbalismKitType.Name,
                    DatabaseHelper.ToolTypeDefinitions.EnchantingToolType.Name, DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType.Name},
                "ProficiencyToolsTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererToolProficiencyPluralShortDescription",
                    "Feature/&TinkererToolsProficiencyTitle").Build());
            ArtificerBuilder.AddFeatureAtLevel(toolProf, 1);

            FeatureDefinitionProficiency saveProf = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.SavingThrow,
                new List<string>() { AttributeDefinitions.Constitution, AttributeDefinitions.Intelligence },
                "ProficiencyTinkererSavingThrow",
                new GuiPresentationBuilder(
                    "Feature/&SavingThrowTinkererProficiencyDescription",
                    "Feature/&SavingThrowTinkererProficiencyTitle").Build());
            ArtificerBuilder.AddFeatureAtLevel(saveProf, 1);

            // skill point pool (1)
            FeatureDefinitionPointPool skillPoints = FeatureHelpers.BuildPointPool(HeroDefinitions.PointsPoolType.Skill, 2,
                new List<string>() { SkillDefinitions.Arcana, SkillDefinitions.History, SkillDefinitions.Investigation, SkillDefinitions.Medecine,
                    SkillDefinitions.Nature, SkillDefinitions.Perception, SkillDefinitions.SleightOfHand },
                "PointPoolTinkererSkillPoints",
                new GuiPresentationBuilder(
                    "Feature/&TinkererSkillGainChoicesPluralDescription",
                    "Feature/&TinkererSkillPointsTitle").Build());
            ArtificerBuilder.AddFeatureAtLevel(skillPoints, 1);

            SpellListDefinition spellList = TinkererSpellList.BuildSpellList();

            // spell casting (1)
            CastSpellBuilder spellCasting = new CastSpellBuilder("CastSpellTinkerer", GuidHelper.Create(GuidNamespace, "CastSpellTinkerer").ToString());
            spellCasting.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class);
            spellCasting.SetSpellCastingAbility(AttributeDefinitions.Intelligence);
            spellCasting.SetSpellList(spellList);
            spellCasting.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.WholeList);
            spellCasting.SetSpellReadyness(RuleDefinitions.SpellReadyness.Prepared);
            spellCasting.SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusHalfLevel);
            spellCasting.SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest);
            spellCasting.SetSpellCastingLevel(1);
            spellCasting.SetKnownCantrips(2, 1, CastSpellBuilder.CasterProgression.HALF_CASTER);
            spellCasting.SetSlotsPerLevel(1, CastSpellBuilder.CasterProgression.HALF_CASTER);
            GuiPresentationBuilder spellcastGui = new GuiPresentationBuilder(
                "Subclass/&ArtificerSpellcastingDescription",
                "Subclass/&ArtificerSpellcastingTitle");
            spellCasting.SetGuiPresentation(spellcastGui.Build());
            FeatureDefinitionCastSpell featureCasting = spellCasting.AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(featureCasting, 1);

            // ritual casting (1)
            ArtificerBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetClericRitualCasting, 1);

            // Artificers can cast with "hands full" because they can cast while holding an infused item, just blanket saying ignore that requirement
            // is the closest reasonable option we have right now.
            ArtificerBuilder.AddFeatureAtLevel(BuildMagicAffinityHandsFull("MagicAffinityArtificerInfusionCasting", new GuiPresentationBuilder(
                "Feature/&ArtificerInfusionCastingDescription",
                "Feature/&ArtificerInfusionCastingTitle").Build()
                ), 1);

            GuiPresentationBuilder magicalTinkeringGui = new GuiPresentationBuilder(
                "Subclass/&TinkererMagicalTinkeringDescription",
                "Subclass/&TinkererMagicalTinkeringTitle");
            ArtificerBuilder.AddFeatureAtLevel(FeatureHelpers.BuildBonusCantrips(new List<SpellDefinition>()
            {
                DatabaseHelper.SpellDefinitions.Shine,
                DatabaseHelper.SpellDefinitions.Sparkle,
                DatabaseHelper.SpellDefinitions.Dazzle,
            }, "TinkererMagicalTinkering", magicalTinkeringGui.Build()), 2);

            // infuse item (level 2)
            // potentially give them "healing pool" points for the number of infusions, then abilities that provide a bonus for 24hrs which the player activates each day

            ArtificerBuilder.AddFeatureAtLevel(InfusionPool, 2);


            // Infusions -- Focus, Weapon, Mind Sharpener, Armor of Magical Strength are given in subclasses
            // Defense
            GuiPresentationBuilder infusionChoiceGui = new GuiPresentationBuilder(
                "Subclass/&TinkererInfusionChoiceDescription",
                "Subclass/&TinkererInfusionChoiceTitle");
            FeatureDefinitionFeatureSet level2Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel2InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel2InfusionChoice").ToString(),
                Level2InfusionList, FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, false, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(level2Infusions, 2);
            ArtificerBuilder.AddFeatureAtLevel(level2Infusions, 2);
            ArtificerBuilder.AddFeatureAtLevel(level2Infusions, 2);
            ArtificerBuilder.AddFeatureAtLevel(level2Infusions, 2);

            // Repeating Shot-- no point it seems
            // Returning Weapon-- not currently do-able


            // right tool for the job (level 3) (can I just give enchanting tool at level 3?)-- tools are available in the store, just skipping for now


            // Subclasses
            FeatureDefinitionSubclassChoice subclasses = ArtificerBuilder.BuildSubclassChoice(3, "Specialist", false, "SubclassChoiceArtificerSpecialistArchetypes",
                new GuiPresentationBuilder(
                    "Feature/&ArtificerSpecialistArchetypesDescription",
                    "Feature/&AftificerSpecialistArchetypesTitle").Build(),
                GuidHelper.Create(GuidNamespace, "SubclassChoiceArtificerSpecialistArchetypes").ToString());

            // ASI (4)
            ArtificerBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 4);

            // Tool expertise (level 6)
            FeatureDefinitionProficiency toolExpertise = FeatureHelpers.BuildProficiency(RuleDefinitions.ProficiencyType.ToolOrExpertise,
                new List<string>() { DatabaseHelper.ToolTypeDefinitions.ThievesToolsType.Name, DatabaseHelper.ToolTypeDefinitions.ScrollKitType.Name,
                    DatabaseHelper.ToolTypeDefinitions.PoisonersKitType.Name, DatabaseHelper.ToolTypeDefinitions.HerbalismKitType.Name,
                    DatabaseHelper.ToolTypeDefinitions.EnchantingToolType.Name, DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType.Name},
                "ExpertiseToolsTinkerer",
                new GuiPresentationBuilder(
                    "Feature/&TinkererToolProficiencyPluralShortDescription",
                    "Feature/&TinkererToolsProficiencyTitle").Build());
            ArtificerBuilder.AddFeatureAtLevel(toolExpertise, 6);

            GuiPresentationBuilder InfusionPoolIncreaseGui = new GuiPresentationBuilder(
                "Subclass/&HealingPoolArtificerInfusionsIncreaseDescription",
                "Subclass/&HealingPoolArtificerInfusionsIncreaseTitle");
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(InfusionPoolIncrease, 6);

            FeatureDefinitionFeatureSet level6Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel6InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel6InfusionChoice").ToString(), Level6InfusionList,
                FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, true, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(level6Infusions, 6);
            ArtificerBuilder.AddFeatureAtLevel(level6Infusions, 6);
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
                "Subclass/&TinkererFlashOfGeniusConditionDescription",
                "Subclass/&TinkererFlashOfGeniusConditionTitle");
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
            flashEffect.SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.Bless.EffectDescription.EffectParticleParameters);

            GuiPresentationBuilder flashOfGeniusPresentation = new GuiPresentationBuilder(
                "Subclass/&TinkererFlashOfGeniusPowerDescription",
                "Subclass/&TinkererFlashOfGeniusPowerTitle");

            FeatureDefinitionPower flashOfGenius = new FeatureHelpers.FeatureDefinitionPowerBuilder("TinkererFlashOfGeniusPower", GuidHelper.Create(GuidNamespace, "TinkererFlashOfGeniusPower").ToString(),
                -1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.PermanentUnlessIncapacitated,
                -1, RuleDefinitions.RechargeRate.AtWill, false, false, AttributeDefinitions.Intelligence, flashEffect.Build(), flashOfGeniusPresentation.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(flashOfGenius, 7);

            // ASI (8)
            ArtificerBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 8);

            // Magic Item Adept (10)
            GuiPresentationBuilder CraftingTinkererMagicItemAdeptPresentation = new GuiPresentationBuilder(
                "Subclass/&CraftingTinkererMagicItemAdeptDescription",
                "Subclass/&CraftingTinkererMagicItemAdeptTitle");
            FeatureDefinitionCraftingAffinity craftingAffinity = new FeatureHelpers.FeatureDefinitionCraftingAffinityBuilder("CraftingTinkererMagicItemAdept", GuidHelper.Create(GuidNamespace, "CraftingTinkererMagicItemAdept").ToString(),
                new List<ToolTypeDefinition>()
                {
                    DatabaseHelper.ToolTypeDefinitions.ThievesToolsType, DatabaseHelper.ToolTypeDefinitions.ScrollKitType,
                    DatabaseHelper.ToolTypeDefinitions.PoisonersKitType, DatabaseHelper.ToolTypeDefinitions.HerbalismKitType,
                    DatabaseHelper.ToolTypeDefinitions.EnchantingToolType, DatabaseHelper.ToolTypeDefinitions.ArtisanToolSmithToolsType,
                }, 0.25f, true, CraftingTinkererMagicItemAdeptPresentation.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(craftingAffinity, 10);
            // boost to infusions (many of the +1s become +2s)
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease10 = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool10",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool10").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(InfusionPoolIncrease10, 10);

            FeatureDefinitionFeatureSet level10Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel10InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel10InfusionChoice").ToString(), Level10InfusionList,
                FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, true, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(level10Infusions, 10);
            ArtificerBuilder.AddFeatureAtLevel(level10Infusions, 10);
            ArtificerBuilder.AddFeatureAtLevel(InfusionHelpers.ImprovedEnhancedDefense, 10);
            ArtificerBuilder.AddFeatureAtLevel(InfusionHelpers.ImprovedEnhancedFocus, 10);
            ArtificerBuilder.AddFeatureAtLevel(InfusionHelpers.ImprovedEnhancedWeapon, 10);
            // helm of awareness
            // winged boots-- probably not- it's a real complicated item

            // 11 spell storing item- no clue what to do
            GuiPresentationBuilder SpellStoringItemGui = new GuiPresentationBuilder(
                "Subclass/&PowerTinkererSpellStoringItemDescription",
                "Subclass/&PowerTinkererSpellStoringItemTitle");
            SpellStoringItemGui.SetSpriteReference(DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalDiscipleOfTheElementsLightning.GuiPresentation.SpriteReference);

            EffectDescriptionBuilder spellEffect = new EffectDescriptionBuilder();
            spellEffect.SetDurationData(RuleDefinitions.DurationType.UntilLongRest, 1, RuleDefinitions.TurnOccurenceType.EndOfTurn);
            spellEffect.SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1, RuleDefinitions.TargetType.Self, 1, 1, ActionDefinitions.ItemSelectionType.None);
            spellEffect.AddEffectForm(new EffectFormBuilder().SetSpellForm(9).Build());
            FeatureDefinitionPower spellStoringItem = new FeatureHelpers.FeatureDefinitionPowerBuilder("TinkererSpellStoringItem", GuidHelper.Create(GuidNamespace, "TinkererSpellStoringItem").ToString(),
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence, RuleDefinitions.ActivationTime.BonusAction,
                1, RuleDefinitions.RechargeRate.LongRest, false, false, AttributeDefinitions.Intelligence, spellEffect.Build(),
                SpellStoringItemGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(spellStoringItem, 11);

            ArtificerBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 12);

            // 14- magic item savant another attunement slot and ignore requirements on magic items
            // also another infusion slot
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease14 = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool14",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool14").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(InfusionPoolIncrease14, 14);
            FeatureDefinitionFeatureSet level14Infusions = new FeatureHelpers.FeatureDefinitionFeatureSetBuilder("TinkererLevel14InfusionChoice",
                GuidHelper.Create(GuidNamespace, "TinkererLevel14InfusionChoice").ToString(), Level14InfusionList,
                FeatureDefinitionFeatureSet.FeatureSetMode.Exclusion, 0, true, false, infusionChoiceGui.Build()
                ).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(level14Infusions, 14);
            ArtificerBuilder.AddFeatureAtLevel(level14Infusions, 14);
            // probably give several infusions another boost here
            // arcane propulsion armor

            ArtificerBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 16);

            // 18 - magic item master another attunement slot
            // also another infusion slot
            FeatureDefinitionPowerPoolModifier InfusionPoolIncrease18 = new FeatureDefinitionPowerPoolModifierBuilder("AttributeModiferArtificerInfusionIncreaseHealingPool18",
                GuidHelper.Create(GuidNamespace, "AttributeModiferArtificerInfusionIncreaseHealingPool18").ToString(),
                1, RuleDefinitions.UsesDetermination.Fixed, AttributeDefinitions.Intelligence, InfusionPool, InfusionPoolIncreaseGui.Build()).AddToDB();
            ArtificerBuilder.AddFeatureAtLevel(InfusionPoolIncrease18, 18);
            ArtificerBuilder.AddFeatureAtLevel(level14Infusions, 18);
            ArtificerBuilder.AddFeatureAtLevel(level14Infusions, 18);

            ArtificerBuilder.AddFeatureAtLevel(DatabaseHelper.FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice, 19);


            GuiPresentationBuilder SoulOfArtificeGui = new GuiPresentationBuilder(
                "Subclass/&PowerTinkererSoulOfArtificeSavesDescription",
                "Subclass/&PowerTinkererSoulOfArtificeSavesTitle");
            FeatureDefinitionSavingThrowAffinity soulOfArtificeSaves = FeatureHelpers.BuildSavingThrowAffinity(AbilityScores, RuleDefinitions.CharacterSavingThrowAffinity.None, FeatureDefinitionSavingThrowAffinity.ModifierType.AddDice, 3, RuleDefinitions.DieType.D4, false,
            "TinkererSoulOfArtificeSavingThrow", SoulOfArtificeGui.Build());
            ArtificerBuilder.AddFeatureAtLevel(soulOfArtificeSaves, 20);

            // 20 - soul of artifice, +1 to saving throws for each attuned item (probably just give +6)
            // also an ability that lets you drop to 1 instead of 0 as an reaction, supposed to end one of your infusions, but maybe just use some other resource?

            CharacterClassDefinition tinkerer = ArtificerBuilder.AddToDB();

            CharacterSubclassDefinition alchemist = AlchemistBuilder.Build(tinkerer);
            subclasses.Subclasses.Add(alchemist.Name);

            CharacterSubclassDefinition artillerist = ArtilleristBuilder.Build(tinkerer, featureCasting);
            subclasses.Subclasses.Add(artillerist.Name);

            CharacterSubclassDefinition battleSmith = BattleSmithBuilder.Build(tinkerer);
            subclasses.Subclasses.Add(battleSmith.Name);

            ScoutSentinelTinkererSubclassBuilder.BuildAndAddSubclass();
            subclasses.Subclasses.Add(ScoutSentinelTinkererSubclassBuilder.Name);

            return tinkerer;
        }

        private static FeatureDefinitionMagicAffinity BuildMagicAffinityHandsFull(string name, GuiPresentation guiPresentation)
        {
            return new FeatureHelpers.FeatureDefinitionMagicAffinityBuilder(name, GuidHelper.Create(GuidNamespace, name).ToString(), guiPresentation).AddToDB();
        }
    }
}
