using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionProficiencyBuilder : BaseDefinitionBuilder<FeatureDefinitionProficiency>
    {
        public FeatureDefinitionProficiencyBuilder(string name, string guid, RuleDefinitions.ProficiencyType type,
        List<string> proficiencies, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetProficiencyType(type);
            Definition.Proficiencies.AddRange(proficiencies);
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
