using SolastaCommunityExpansion.Builders.Features;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionProficiencys;

namespace SolastaCommunityExpansion.Level20.Features
{
    internal sealed class ProficiencyRogueSlipperyMindBuilder : FeatureDefinitionProficiencyBuilder
    {
        private const string ProficiencyRogueSlipperyMindName = "ZSProficiencyRogueSlipperyMind";
        private const string ProficiencyRogueSlipperyMindGuid = "b7eb00f96e13495ea4af1389fafca546";

        private ProficiencyRogueSlipperyMindBuilder(string name, string guid) : base(ProficiencyRogueSavingThrow, name, guid)
        {
            Definition.GuiPresentation.Title = "Feature/&ProficiencyRogueSlipperyMindTitle";
            Definition.GuiPresentation.Description = "Feature/&ProficiencyRogueSlipperyMindDescription";
            Definition.Proficiencies.Add("Wisdom");
        }

        private static FeatureDefinitionProficiency CreateAndAddToDB(string name, string guid)
        {
            return new ProficiencyRogueSlipperyMindBuilder(name, guid).AddToDB();
        }

        internal static readonly FeatureDefinitionProficiency ProficiencyRogueSlipperyMind
            = CreateAndAddToDB(ProficiencyRogueSlipperyMindName, ProficiencyRogueSlipperyMindGuid);
    }
}
