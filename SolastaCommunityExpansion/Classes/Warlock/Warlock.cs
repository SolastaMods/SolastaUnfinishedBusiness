using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaCommunityExpansion.Classes.Warlock.Features;
using SolastaCommunityExpansion.Classes.Warlock.Subclasses;
using SolastaCommunityExpansion.Properties;
using SolastaCommunityExpansion.Utils;
using static EquipmentDefinitions;
using static SolastaCommunityExpansion.Builders.EquipmentOptionsBuilder;
using static SolastaCommunityExpansion.Classes.Warlock.Features.WarlockFeatures;
using static SolastaCommunityExpansion.Api.DatabaseHelper;
using static SolastaCommunityExpansion.Api.DatabaseHelper.CharacterClassDefinitions;
using static SolastaCommunityExpansion.Api.DatabaseHelper.ToolTypeDefinitions;

namespace SolastaCommunityExpansion.Classes.Warlock;

public static class Warlock
{
    public static CharacterClassDefinition ClassWarlock { get; private set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyArmor { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyWeapon { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencyTool { get; set; }

    private static FeatureDefinitionProficiency FeatureDefinitionProficiencySavingThrow { get; set; }

    private static FeatureDefinitionPointPool FeatureDefinitionSkillPoints { get; set; }

    private static FeatureDefinitionCastSpell FeatureDefinitionClassWarlockCastSpell { get; set; }

    private static void BuildEquipment([NotNull] CharacterClassDefinitionBuilder classWarlockBuilder)
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
        static FeatureDefinitionProficiency BuildProficiency(string name, RuleDefinitions.ProficiencyType type,
            params string[] proficiencies)
        {
            return FeatureDefinitionProficiencyBuilder
                .Create(name, DefinitionBuilder.CENamespaceGuid)
                .SetGuiPresentation(Category.Feature)
                .SetProficiencies(type, proficiencies)
                .AddToDB();
        }

        FeatureDefinitionProficiencyArmor =
            BuildProficiency("ClassWarlockArmorProficiency", RuleDefinitions.ProficiencyType.Armor,
                LightArmorCategory);

        FeatureDefinitionProficiencyWeapon =
            BuildProficiency("ClassWarlockWeaponProficiency", RuleDefinitions.ProficiencyType.Weapon,
                SimpleWeaponCategory);

        FeatureDefinitionProficiencyTool =
            BuildProficiency("ClassWarlockToolsProficiency", RuleDefinitions.ProficiencyType.Tool,
                EnchantingToolType.Name, HerbalismKitType.Name);

        FeatureDefinitionProficiencySavingThrow =
            BuildProficiency("ClassWarlockSavingThrowProficiency", RuleDefinitions.ProficiencyType.SavingThrow,
                AttributeDefinitions.Charisma, AttributeDefinitions.Wisdom);

        FeatureDefinitionSkillPoints = FeatureDefinitionPointPoolBuilder
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
            .SetGuiPresentation(Category.Feature)
            .AddToDB();
    }

    private static void BuildSpells()
    {
        var classWarlockCastSpell = FeatureDefinitionCastSpellBuilder
            .Create(FeatureDefinitionCastSpells.CastSpellSorcerer, "ClassWarlockCastSpell",
                DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassWarlockSpellcasting", Category.Feature)
            .SetKnownCantrips(2, 2, 2, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4)
            .SetKnownSpells(2, 3, 4, 5, 6, 7, 8, 9, 10, 10, 11, 11, 12, 12, 13, 13, 14, 14, 15, 15)
            .SetReplacedSpells(0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0)
            .SetSlotsPerLevel(WarlockSpells.WarlockCastingSlots)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.ShortRest)
            .SetSpellCastingAbility(AttributeDefinitions.Charisma)
            .SetSpellCastingLevel(9)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Class)
            .SetSpellList(WarlockSpells.WarlockSpellList)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown);

        FeatureDefinitionClassWarlockCastSpell = classWarlockCastSpell.AddToDB();
    }

    private static void BuildProgression([NotNull] CharacterClassDefinitionBuilder classWarlockBuilder)
    {
        // DEPRECATED
        _ = WarlockSubclassRiftWalkerPatron.Build();

        var subclassChoices = FeatureDefinitionSubclassChoiceBuilder
            .Create("ClassWarlockSubclassChoice", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("ClassWarlockPatron", Category.Subclass)
            .SetSubclassSuffix("Patron")
            .SetFilterByDeity(false)
            .SetSubclasses(
                WarlockSubclassSoulBladePact.Build(),
                WarlockSubclassAncientForestPatron.Build(),
                WarlockSubclassElementalPatron.Build(),
                WarlockSubclassMoonLitPatron.Build()
            )
            .AddToDB();

        classWarlockBuilder
            .AddFeaturesAtLevel(1,
                FeatureDefinitionProficiencySavingThrow,
                FeatureDefinitionProficiencyArmor,
                FeatureDefinitionProficiencyWeapon,
                FeatureDefinitionProficiencyTool,
                FeatureDefinitionSkillPoints,
                FeatureDefinitionClassWarlockCastSpell,
                subclassChoices
            )
            .AddFeaturesAtLevel(2,
                WarlockEldritchInvocationSet,
                WarlockEldritchInvocationSet
            )
            .AddFeaturesAtLevel(3,
                WarlockClassPactBoonSetBuilder.WarlockClassPactBoonSet,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(4,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(5,
                WarlockEldritchInvocationReplacer,
                WarlockEldritchInvocationSet
            )
            .AddFeaturesAtLevel(6,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(7,
                WarlockEldritchInvocationReplacer,
                WarlockEldritchInvocationSet
            )
            .AddFeaturesAtLevel(8,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(9,
                WarlockEldritchInvocationReplacer,
                WarlockEldritchInvocationSet
            )
            .AddFeaturesAtLevel(10,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(11,
                WarlockMysticArcanumSet,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(12,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                WarlockEldritchInvocationReplacer,
                WarlockEldritchInvocationSet
            )
            .AddFeaturesAtLevel(13,
                WarlockMysticArcanumSet,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(14,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(15,
                WarlockMysticArcanumSet,
                WarlockEldritchInvocationReplacer,
                WarlockEldritchInvocationSet
            )
            .AddFeaturesAtLevel(16,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(17,
                WarlockMysticArcanumSet,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(18,
                WarlockEldritchInvocationReplacer,
                WarlockEldritchInvocationSet
            )
            .AddFeaturesAtLevel(19,
                FeatureDefinitionFeatureSets.FeatureSetAbilityScoreChoice,
                WarlockEldritchInvocationReplacer
            )
            .AddFeaturesAtLevel(20,
                WarlockEldritchMasterPower,
                WarlockEldritchInvocationReplacer
            );
    }

    internal static CharacterClassDefinition BuildWarlockClass()
    {
        var warlockSpriteReference =
            CustomIcons.CreateAssetReferenceSprite("Warlock", Resources.Warlock, 1024, 576);

        var classWarlockBuilder = CharacterClassDefinitionBuilder
            .Create("ClassWarlock", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Class, warlockSpriteReference, 1)
            .AddFeatPreferences(FeatDefinitions.PowerfulCantrip, FeatDefinitions.FlawlessConcentration,
                FeatDefinitions.Robust)
            .AddPersonality(PersonalityFlagDefinitions.Violence, 3)
            .AddPersonality(PersonalityFlagDefinitions.Self_Preservation, 3)
            .AddPersonality(PersonalityFlagDefinitions.Normal, 3)
            .AddPersonality(PersonalityFlagDefinitions.GpSpellcaster, 5)
            .AddPersonality(PersonalityFlagDefinitions.GpExplorer, 1)
            .AddSkillPreferences(
                DatabaseHelper.SkillDefinitions.Deception,
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

        EldritchInvocationsBuilder.Build();

        BuildEquipment(classWarlockBuilder);
        BuildProficiencies();
        BuildSpells();
        BuildProgression(classWarlockBuilder);

        ClassWarlock = classWarlockBuilder.AddToDB();

        var itemlist = new List<ItemDefinition>
        {
            ItemDefinitions.WandOfLightningBolts,
            ItemDefinitions.StaffOfFire,
            ItemDefinitions.ArcaneShieldstaff,
            ItemDefinitions.WizardClothes_Alternate
        };

        foreach (var item in itemlist)
        {
            item.RequiredAttunementClasses.Add(ClassWarlock);
        }

        return ClassWarlock;
    }
}
