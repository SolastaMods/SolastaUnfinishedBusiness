using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Warlock.Features;
using SolastaModApi;
using UnityEngine;
using static CharacterClassDefinition;
using static EquipmentDefinitions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.ToolTypeDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock
{
    public static class Warlock
    {
        public static CharacterClassDefinition ClassWarlock { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencyTool { get; private set; }

        public static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; private set; }

        public static FeatureDefinitionPointPool FeatureDefinitionSkillPoints { get; private set; }

        public static FeatureDefinitionCastSpell FeatureDefinitionClassWarlockCastSpell { get; private set; }

        private static void BuildEquipment(CharacterClassDefinitionBuilder classWarlockBuilder)
        {
            classWarlockBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow, OptionWeapon, 1),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Bolt, OptionAmmoPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.LightCrossbow, OptionWeaponSimpleChoice, 1),
                });

            classWarlockBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ScholarPack, OptionStarterPack, 1),
                },
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.DungeoneerPack, OptionStarterPack, 1),
                });

            classWarlockBuilder.AddEquipmentRow(
                new List<HeroEquipmentOption>
                {
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Leather, OptionArmor, 1),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.ComponentPouch, OptionFocus, 1),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger, OptionWeapon, 2),
                    EquipmentOptionsBuilder.Option(ItemDefinitions.Dagger, OptionWeaponSimpleChoice, 1),
                });
        }

        private static void BuildProficiencies()
        {
            static FeatureDefinitionProficiency Build(string name, RuleDefinitions.ProficiencyType type, params string[] proficiencies)
            {
                return FeatureDefinitionProficiencyBuilder
                    .Create(name, DefinitionBuilder.CENamespaceGuid)
                    .SetGuiPresentation(Category.Feature)
                    .SetProficiencies(type, proficiencies)
                    .AddToDB();
            }

            FeatureDefinitionProficiencyArmor =
                Build("ClassWarlockArmorProficiency", RuleDefinitions.ProficiencyType.Armor, LightArmorCategory);

            FeatureDefinitionProficiencyWeapon =
                Build("ClassWarlockWeaponProficiency", RuleDefinitions.ProficiencyType.Armor, SimpleWeaponCategory);

            FeatureDefinitionProficiencyTool =
                Build("ClassWarlockToolsProficiency", RuleDefinitions.ProficiencyType.Tool, EnchantingToolType.Name, HerbalismKitType.Name);

            FeatureDefinitionProficiencySavingThrow =
                Build("ClassWarlockSavingThrowProficiency", RuleDefinitions.ProficiencyType.SavingThrow, AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom);

            FeatureDefinitionSkillPoints = FeatureDefinitionPointPoolBuilder.Build(HeroDefinitions.PointsPoolType.Skill, 2,
                new List<string>
                {
                    SkillDefinitions.Arcana,
                    SkillDefinitions.Deception,
                    SkillDefinitions.History,
                    SkillDefinitions.Intimidation,
                    SkillDefinitions.Investigation,
                    SkillDefinitions.Nature,
                    SkillDefinitions.Religion
                },
                "ClassWarlockSkillProficiency",
                new GuiPresentationBuilder(
                    "Feature/&ClassWarlockSkillProficiencyDescription",
                    "Feature/&ClassWarlockSkillProficiencyTitle").Build());
        }

        private static void BuildSpells()
        {
            var castSpellName = "ClassWarlockCastSpell";
            var castSpellGuid = GuidHelper.Create(new Guid(Settings.GUID), castSpellName).ToString();
            var classWarlockCastSpell = FeatureDefinitionCastSpellBuilder.Create(castSpellName, castSpellGuid);

            // TODO: can't find ClassWarlockSpellListBuilder
            /*
            ClassWarlockSpellListBuilder.Build();
            var classWarlockSpellList = ClassWarlockSpellListBuilder.ClassWarlockSpellList;

            classWarlockCastSpell.SetGuiPresentation(new GuiPresentationBuilder("Feature/&ClassWarlockSpellcastingDescription", "Feature/&ClassWarlockSpellcastingTitle").Build());
            classWarlockCastSpell.SetKnownCantrips(new List<int>
            {
                2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4
            });
            classWarlockCastSpell.SetKnownSpells(new List<int>
            {
                2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15
            });

            // TODO: 
            //classWarlockCastSpell.SetSlotsPerLevel(Models.SharedSpellsContext.WarlockCastingSlots);
            classWarlockCastSpell.SetSlotsRecharge(RuleDefinitions.RechargeRate.ShortRest);
            classWarlockCastSpell.SetSpellCastingAbility(AttributeDefinitions.Charisma);
            classWarlockCastSpell.SetSpellCastingLevel(5);
            classWarlockCastSpell.SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class);
            classWarlockCastSpell.SetSpellList(classWarlockSpellList);
            classWarlockCastSpell.SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection);
            classWarlockCastSpell.SetSpellPreparationCount(RuleDefinitions.SpellPreparationCount.AbilityBonusPlusLevel);
            classWarlockCastSpell.SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);

            FeatureDefinitionClassWarlockCastSpell = classWarlockCastSpell.AddToDB();
            */
        }

        private static void BuildSubclasses(CharacterClassDefinitionBuilder classWarlockBuilder)
        {
            var subClassChoiceName = "ClassWarlockSubclassChoice";
            var subClassChoiceGuid = GuidHelper.Create(new Guid(Settings.GUID), subClassChoiceName).ToString();
            var classWarlockPatronPresentationBuilder = new GuiPresentationBuilder("Subclass/&ClassWarlockPatronDescription", "Subclass/&ClassWarlockPatronTitle");

            // TODO
            //var subclassChoices = classWarlockBuilder.BuildSubclassChoice(1, "Patron", false, subClassChoiceName, classWarlockPatronPresentationBuilder.Build(), subClassChoiceGuid);

            //DHWarlockSubclassRiftWalker.Build();
            //subclassChoices.Subclasses.Add(DHWarlockSubclassRiftWalker.Name);
            //DHWarlockSubclassElementalPatron.Build();
            //subclassChoices.Subclasses.Add(DHWarlockSubclassElementalPatron.Name);
        }

        private static void BuildProgression(CharacterClassDefinitionBuilder classWarlockBuilder)
        {
            classWarlockBuilder.AddFeatureAtLevel(1, FeatureDefinitionProficiencySavingThrow);
            classWarlockBuilder.AddFeatureAtLevel(1, FeatureDefinitionProficiencyArmor);
            classWarlockBuilder.AddFeatureAtLevel(1, FeatureDefinitionProficiencyWeapon);
            classWarlockBuilder.AddFeatureAtLevel(1, FeatureDefinitionProficiencyTool);
            classWarlockBuilder.AddFeatureAtLevel(1, FeatureDefinitionSkillPoints);
            classWarlockBuilder.AddFeatureAtLevel(1, FeatureDefinitionClassWarlockCastSpell);
            //level 1 - subclass feature
            classWarlockBuilder.AddFeatureAtLevel(2, WarlockEldritchInvocationSetBuilderLevel2.WarlockEldritchInvocationSetLevel2);
            classWarlockBuilder.AddFeatureAtLevel(2, WarlockEldritchInvocationSetBuilderLevel2.WarlockEldritchInvocationSetLevel2);
            classWarlockBuilder.AddFeatureAtLevel(2, AHWarlockClassPactBoonSetBuilder.AHWarlockClassPactBoonSet);
            classWarlockBuilder.AddFeatureAtLevel(2, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
            classWarlockBuilder.AddFeatureAtLevel(2, WarlockEldritchInvocationSetBuilderLevel5.WarlockEldritchInvocationSetLevel5);
            //level 6 - subclass feature
            classWarlockBuilder.AddFeatureAtLevel(7, WarlockEldritchInvocationSetBuilderLevel7.WarlockEldritchInvocationSetLevel7);
            classWarlockBuilder.AddFeatureAtLevel(8, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
            classWarlockBuilder.AddFeatureAtLevel(9, WarlockEldritchInvocationSetBuilderLevel9.WarlockEldritchInvocationSetLevel9);
            //level 10 - subclass feature
            classWarlockBuilder.AddFeatureAtLevel(11, WarlockMysticArcanumSetBuilder.WarlockMysticArcanumSetLevel11);
            classWarlockBuilder.AddFeatureAtLevel(12, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
            classWarlockBuilder.AddFeatureAtLevel(12, WarlockEldritchInvocationSetBuilderLevel12.WarlockEldritchInvocationSetLevel12);
            classWarlockBuilder.AddFeatureAtLevel(13, WarlockMysticArcanumSetBuilder.WarlockMysticArcanumSetLevel13);
            //level 14 - subclass feature
            classWarlockBuilder.AddFeatureAtLevel(15, WarlockMysticArcanumSetBuilder.WarlockMysticArcanumSetLevel15);
            classWarlockBuilder.AddFeatureAtLevel(15, WarlockEldritchInvocationSetBuilderLevel15.WarlockEldritchInvocationSetLevel15);
            classWarlockBuilder.AddFeatureAtLevel(16, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
            classWarlockBuilder.AddFeatureAtLevel(17, WarlockMysticArcanumSetBuilder.WarlockMysticArcanumSetLevel17);
            classWarlockBuilder.AddFeatureAtLevel(18, WarlockEldritchInvocationSetBuilderLevel18.WarlockEldritchInvocationSetLevel18);
            classWarlockBuilder.AddFeatureAtLevel(19, FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice);
            classWarlockBuilder.AddFeatureAtLevel(20, WarlockEldritchMasterBuilder.WarlockEldritchMaster);
        }

        internal static void BuildWarlockClass()
        {
            var className = "ClassWarlock";
            var classGuid = GuidHelper.Create(new Guid(Settings.GUID), className).ToString();
            var classWarlockBuilder = CharacterClassDefinitionBuilder.Create(className, classGuid);
            var classWarlockGuiPresentationBuilder = new GuiPresentationBuilder("Class/&ClassWarlockDescription", "Class/&ClassWarlockTitle");

            classWarlockGuiPresentationBuilder.SetColor(new Color(1.0f, 1.0f, 1.0f, 1.0f));

            // TODO: add setting
            //classWarlockGuiPresentationBuilder.SetHidden(!Main.Settings.EnableClassWarlock);
            classWarlockGuiPresentationBuilder.SetSortOrder(1);
            classWarlockGuiPresentationBuilder.SetSpriteReference(Cleric.GuiPresentation.SpriteReference);

            classWarlockBuilder.AddFeatPreference(FeatDefinitions.PowerfulCantrip);
            classWarlockBuilder.AddFeatPreference(FeatDefinitions.FlawlessConcentration);
            classWarlockBuilder.AddFeatPreference(FeatDefinitions.Robust);

            classWarlockBuilder.AddPersonality(PersonalityFlagDefinitions.Violence, 3);
            classWarlockBuilder.AddPersonality(PersonalityFlagDefinitions.Self_Preservation, 3);
            classWarlockBuilder.AddPersonality(PersonalityFlagDefinitions.Normal, 3);
            classWarlockBuilder.AddPersonality(PersonalityFlagDefinitions.GpSpellcaster, 5);
            classWarlockBuilder.AddPersonality(PersonalityFlagDefinitions.GpExplorer, 1);

            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Deception);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Intimidation);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Arcana);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.History);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Investigation);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Religion);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Nature);
            classWarlockBuilder.AddSkillPreference(DatabaseHelper.SkillDefinitions.Persuasion);

            classWarlockBuilder.AddToolPreference(EnchantingToolType);
            classWarlockBuilder.AddToolPreference(HerbalismKitType);

            classWarlockBuilder.SetAbilityScorePriorities(
                AttributeDefinitions.Charisma,
                AttributeDefinitions.Constitution,
                AttributeDefinitions.Dexterity,
                AttributeDefinitions.Strength,
                AttributeDefinitions.Wisdom,
                AttributeDefinitions.Intelligence);

            classWarlockBuilder.SetAnimationId(AnimationDefinitions.ClassAnimationId.Wizard);
            classWarlockBuilder.SetBattleAI(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions);
            classWarlockBuilder.SetGuiPresentation(classWarlockGuiPresentationBuilder.Build());
            classWarlockBuilder.SetHitDice(RuleDefinitions.DieType.D8);
            classWarlockBuilder.SetIngredientGatheringOdds(Sorcerer.IngredientGatheringOdds);
            classWarlockBuilder.SetPictogram(Wizard.ClassPictogramReference);

            DHEldritchInvocationsBuilder.Build();
            BuildEquipment(classWarlockBuilder);
            BuildProficiencies();
            BuildSpells();
            BuildProgression(classWarlockBuilder);
            BuildSubclasses(classWarlockBuilder);

            ClassWarlock = classWarlockBuilder.AddToDB();



            var itemlist = new List<ItemDefinition>
            {
                ItemDefinitions.WandOfLightningBolts,
                ItemDefinitions.StaffOfFire,
                ItemDefinitions.ArcaneShieldstaff,
                ItemDefinitions.WizardClothes_Alternate
            };

            foreach (ItemDefinition item in itemlist)
            {
                item.RequiredAttunementClasses.Add(ClassWarlock);
            };
        }
    }
}
