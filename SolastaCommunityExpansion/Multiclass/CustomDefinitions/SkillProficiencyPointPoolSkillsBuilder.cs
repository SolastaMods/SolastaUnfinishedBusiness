using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionPointPools;

namespace SolastaMulticlass.CustomDefinitions
{
    internal sealed class SkillProficiencyPointPoolSkillsBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
        private const string BardClassSkillProficiencyMulticlassName = "BardClassSkillProficiencyMulticlass";
        private const string BardClassSkillProficiencyMulticlassGuid = "a69b2527569b4893abe57ad1f80e97ed";

        private const string PointPoolRangerSkillPointsMulticlassName = "PointPoolRangerSkillPointsMulticlass";
        private const string PointPoolRangerSkillPointsMulticlassGuid = "096e4e01b52b490e807cf8d458845aa5";

        private const string PointPoolRogueSkillPointsMulticlassName = "PointPoolRogueSkillPointsMulticlass";
        private const string PointPoolRogueSkillPointsMulticlassGuid = "451259da8c5c41f4b1b363f00b01be4e";

        [Obsolete]
        private SkillProficiencyPointPoolSkillsBuilder(string name, string guid, string title, params string[] restrictedChoices) : base(PointPoolRangerSkillPoints, name, guid)
        {
            Definition.SetPoolAmount(1);
            Definition.RestrictedChoices.Clear();
            Definition.RestrictedChoices.AddRange(restrictedChoices);
            Definition.GuiPresentation.Title = title;
            Definition.GuiPresentation.Description = "Feature/&SkillGainChoicesPluralDescription";
        }

        [Obsolete]
        private static FeatureDefinitionPointPool CreateAndAddToDB(string name, string guid, string title, params string[] restrictedChoices)
        {
            return new SkillProficiencyPointPoolSkillsBuilder(name, guid, title, restrictedChoices).AddToDB();
        }

        [Obsolete]
        public static readonly FeatureDefinitionPointPool PointPoolBardSkillPointsMulticlass =
            CreateAndAddToDB(BardClassSkillProficiencyMulticlassName, BardClassSkillProficiencyMulticlassGuid, "Feature/&BardClassSkillPointPoolTitle",
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
            );

        [Obsolete]
        public static readonly FeatureDefinitionPointPool PointPoolRangerSkillPointsMulticlass =
            CreateAndAddToDB(PointPoolRangerSkillPointsMulticlassName, PointPoolRangerSkillPointsMulticlassGuid, "Feature/&RangerSkillsTitle",
                SkillDefinitions.AnimalHandling,
                SkillDefinitions.Athletics,
                SkillDefinitions.Insight,
                SkillDefinitions.Investigation,
                SkillDefinitions.Nature,
                SkillDefinitions.Perception,
                SkillDefinitions.Survival,
                SkillDefinitions.Stealth
            );

        [Obsolete]
        public static readonly FeatureDefinitionPointPool PointPoolRogueSkillPointsMulticlass =
            CreateAndAddToDB(PointPoolRogueSkillPointsMulticlassName, PointPoolRogueSkillPointsMulticlassGuid, "Feature/&RogueSkillPointsTitle",
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
            );
    }
}
