using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaCJDExtraContent.Features
{
    public class FeatureDefinitionProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        public FeatureDefinitionProficiencyBuilder(string name, string guid, RuleDefinitions.ProficiencyType type,
        List<string> proficiencies, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetProficiencyType(type);
            foreach (string item in proficiencies)
            {
                Definition.Proficiencies.Add(item);
            }
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
