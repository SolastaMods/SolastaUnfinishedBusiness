using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Models
{
    public static class MulticlassContext
    {
        public static CharacterClassDefinition DummyClass { get; private set; }

        public static RestActivityDefinition RestActivityLevelDown { get; private set; } = RestActivityDefinitionBuilder
            .Create("LevelDown", "fdb4d86eaef942d1a22dbf1fb5a7299f")
            .SetGuiPresentation("MainMenu/&ExportPdfTitle", "MainMenu/&ExportPdfDescription")
            .SetRestData(
                RestDefinitions.RestStage.AfterRest, RuleDefinitions.RestType.ShortRest,
                RestActivityDefinition.ActivityCondition.None, "LevelDown", string.Empty)
            .AddToDB();

        internal static void Load()
        {
            _ = ArmorProficiencyMulticlassBuilder.BarbarianArmorProficiencyMulticlass;
            _ = ArmorProficiencyMulticlassBuilder.FighterArmorProficiencyMulticlass;
            _ = ArmorProficiencyMulticlassBuilder.PaladinArmorProficiencyMulticlass;
            _ = ArmorProficiencyMulticlassBuilder.WardenArmorProficiencyMulticlass;
            _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolBardSkillPointsMulticlass;
            _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolRangerSkillPointsMulticlass;
            _ = SkillProficiencyPointPoolSkillsBuilder.PointPoolRogueSkillPointsMulticlass;
        }

        internal static void LaterLoad()
        {
            // don't refactor out of this method...
            DummyClass = CharacterClassDefinitionBuilder
                .Create("DummyClass", "062d696ab44146e0b316188f943d8079")
                .SetGuiPresentationNoContent()
                .AddToDB();

            DummyClass.GuiPresentation.SetHidden(true);
        }
    }

    internal sealed class ArmorProficiencyMulticlassBuilder : FeatureDefinitionProficiencyBuilder
    {
        private const string BarbarianArmorProficiencyMulticlassName = "BarbarianArmorProficiencyMulticlass";
        private const string BarbarianArmorProficiencyMulticlassGuid = "86558227b0cd4771b42978a60dc610db";

        private const string FighterArmorProficiencyMulticlassName = "FighterArmorProficiencyMulticlass";
        private const string FighterArmorProficiencyMulticlassGuid = "5df5ec907a424fccbfec103344421b51";

        private const string PaladinArmorProficiencyMulticlassName = "PaladinArmorProficiencyMulticlass";
        private const string PaladinArmorProficiencyMulticlassGuid = "69b18e44aabd4acca702c05f9d6c7fcb";

        private const string WardenArmorProficiencyMulticlassName = "WardenArmorProficiencyMulticlass";
        private const string WardenArmorProficiencyMulticlassGuid = "19666e846975401b819d1ae72c5d27ac";

        private ArmorProficiencyMulticlassBuilder(string name, string guid, string title, params string[] proficienciesToReplace) : base(ProficiencyFighterArmor, name, guid)
        {
            Definition.Proficiencies.SetRange(proficienciesToReplace);
            Definition.GuiPresentation.Title = title;
        }

        private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid, string title, params string[] proficienciesToReplace)
        {
            return new ArmorProficiencyMulticlassBuilder(name, guid, title, proficienciesToReplace).AddToDB();
        }

        internal static readonly FeatureDefinitionProficiency BarbarianArmorProficiencyMulticlass =
            CreateAndAddToDB(BarbarianArmorProficiencyMulticlassName, BarbarianArmorProficiencyMulticlassGuid, "Feature/&BarbarianArmorProficiencyTitle",
                EquipmentDefinitions.ShieldCategory
            );

        internal static readonly FeatureDefinitionProficiency FighterArmorProficiencyMulticlass =
            CreateAndAddToDB(FighterArmorProficiencyMulticlassName, FighterArmorProficiencyMulticlassGuid, "Feature/&FighterArmorProficiencyTitle",
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            );

        internal static readonly FeatureDefinitionProficiency PaladinArmorProficiencyMulticlass =
            CreateAndAddToDB(PaladinArmorProficiencyMulticlassName, PaladinArmorProficiencyMulticlassGuid, "Feature/&PaladinArmorProficiencyTitle",
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            );

        internal static readonly FeatureDefinitionProficiency WardenArmorProficiencyMulticlass =
            CreateAndAddToDB(WardenArmorProficiencyMulticlassName, WardenArmorProficiencyMulticlassGuid, "Feature/&WardenArmorProficiencyTitle",
                EquipmentDefinitions.LightArmorCategory,
                EquipmentDefinitions.MediumArmorCategory,
                EquipmentDefinitions.ShieldCategory
            );
    }

    internal static class SkillProficiencyPointPoolSkillsBuilder
    {
        internal static readonly FeatureDefinitionPointPool PointPoolBardSkillPointsMulticlass = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolBardSkillPointsMulticlass", "a69b2527569b4893abe57ad1f80e97ed")
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&BardSkillsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Arcana,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.History,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Medecine,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.Religion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth,
                SkillDefinitions.Survival
            )
            .AddToDB();

        internal static readonly FeatureDefinitionPointPool PointPoolRangerSkillPointsMulticlass = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRangerSkillPointsMulticlass", "096e4e01b52b490e807cf8d458845aa5")
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&RangerSkillsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Athletics,
                SkillDefinitions.Insight,
                SkillDefinitions.Investigation,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival,
                SkillDefinitions.Stealth
            )
            .AddToDB();

        internal static readonly FeatureDefinitionPointPool PointPoolRogueSkillPointsMulticlass = FeatureDefinitionPointPoolBuilder
            .Create("PointPoolRogueSkillPointsMulticlass", "451259da8c5c41f4b1b363f00b01be4e")
            // Non-standard pattern?
            .SetGuiPresentation("Feature/&RogueSkillPointsTitle", "Feature/&SkillGainChoicesPluralDescription")
            .SetPool(HeroDefinitions.PointsPoolType.Skill, 1)
            .RestrictChoices(
                SkillDefinitions.Acrobatics,
                SkillDefinitions.Athletics,
                SkillDefinitions.Deception,
                SkillDefinitions.Insight,
                SkillDefinitions.Intimidation,
                SkillDefinitions.Investigation,
                SkillDefinitions.Perception,
                SkillDefinitions.Performance,
                SkillDefinitions.Persuasion,
                SkillDefinitions.SleightOfHand,
                SkillDefinitions.Stealth
            )
            .AddToDB();
    }
}
