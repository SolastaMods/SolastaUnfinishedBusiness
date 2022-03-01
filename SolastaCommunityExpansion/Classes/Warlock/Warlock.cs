using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Warlock.Features;
using SolastaModApi;
using static EquipmentDefinitions;
using static SolastaModApi.DatabaseHelper;
using static SolastaModApi.DatabaseHelper.CharacterClassDefinitions;
using static SolastaModApi.DatabaseHelper.ToolTypeDefinitions;
using static SolastaCommunityExpansion.Builders.EquipmentOptionsBuilder;

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
            classWarlockBuilder
                .AddEquipmentRow(
                    Column(
                        Option(ItemDefinitions.LightCrossbow, OptionWeapon, 1),
                        Option(ItemDefinitions.Bolt, OptionAmmoPack, 1)
                    ),
                    Column(Option(ItemDefinitions.LightCrossbow, OptionWeaponSimpleChoice, 1)))
                .AddEquipmentRow(
                    Column(Option(ItemDefinitions.ScholarPack, OptionStarterPack, 1)),
                    Column(Option(ItemDefinitions.DungeoneerPack, OptionStarterPack, 1)))
                .AddEquipmentRow(
                    Column(
                        Option(ItemDefinitions.Leather, OptionArmor, 1),
                        Option(ItemDefinitions.ComponentPouch, OptionFocus, 1),
                        Option(ItemDefinitions.Dagger, OptionWeapon, 2),
                        Option(ItemDefinitions.Dagger, OptionWeaponSimpleChoice, 1)
                    ));
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

            FeatureDefinitionPointPoolBuilder
                .Create("ClassWarlockSkillProficiency", DefinitionBuilder.CENamespaceGuid)
                .SetPool(HeroDefinitions.PointsPoolType.Skill, 2)
                .OnlyUniqueChoices()
                .RestrictChoices(
                    SkillDefinitions.Arcana,
                    SkillDefinitions.Deception,
                    SkillDefinitions.History,
                    SkillDefinitions.Intimidation,
                    SkillDefinitions.Investigation,
                    SkillDefinitions.Nature,
                    SkillDefinitions.Religion)
                .SetGuiPresentation(Category.Feature);
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
            // TODO: is this required?
            //classWarlockGuiPresentationBuilder.SetHidden(!Main.Settings.EnableClassWarlock);

            var classWarlockBuilder = CharacterClassDefinitionBuilder
                .Create("ClassWarlock", DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Class, Cleric.GuiPresentation.SpriteReference, 1 /*hidden=true/false?*/)
                .AddFeatPreferences(FeatDefinitions.PowerfulCantrip, FeatDefinitions.FlawlessConcentration, FeatDefinitions.Robust)
                .AddPersonalityWeights(
                    (PersonalityFlagDefinitions.Violence, 3),
                    (PersonalityFlagDefinitions.Self_Preservation, 3),
                    (PersonalityFlagDefinitions.Normal, 3),
                    (PersonalityFlagDefinitions.GpSpellcaster, 5),
                    (PersonalityFlagDefinitions.GpExplorer, 1))
                .AddSkillPreferences(DatabaseHelper.SkillDefinitions.Deception,
                    DatabaseHelper.SkillDefinitions.Intimidation,
                    DatabaseHelper.SkillDefinitions.Arcana,
                    DatabaseHelper.SkillDefinitions.History,
                    DatabaseHelper.SkillDefinitions.Investigation,
                    DatabaseHelper.SkillDefinitions.Religion,
                    DatabaseHelper.SkillDefinitions.Nature,
                    DatabaseHelper.SkillDefinitions.Persuasion)
                .AddToolPreferences(EnchantingToolType, HerbalismKitType)
                .SetAbilityScorePriorities(
                    AttributeDefinitions.Charisma,
                    AttributeDefinitions.Constitution,
                    AttributeDefinitions.Dexterity,
                    AttributeDefinitions.Strength,
                    AttributeDefinitions.Wisdom,
                    AttributeDefinitions.Intelligence)
                .SetAnimationId(AnimationDefinitions.ClassAnimationId.Wizard)
                .SetBattleAI(DecisionPackageDefinitions.DefaultSupportCasterWithBackupAttacksDecisions)
                .SetHitDice(RuleDefinitions.DieType.D8)
                .SetIngredientGatheringOdds(Sorcerer.IngredientGatheringOdds)
                .SetPictogram(Wizard.ClassPictogramReference);

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
