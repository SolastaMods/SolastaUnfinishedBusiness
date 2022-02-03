using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionDieRollModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionDieRollModifier>
    {
        public FeatureDefinitionDieRollModifierBuilder(string name, string guid,
                RuleDefinitions.RollContext context, int rerollCount, int minRerollValue,
                string consoleLocalizationKey, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetValidityContext(context);
            Definition.SetRerollLocalizationKey(consoleLocalizationKey);
            Definition.SetRerollCount(rerollCount);
            Definition.SetMinRerollValue(minRerollValue);
            Definition.SetMinRollValue(minRerollValue);
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionDieRollModifierBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionDieRollModifierBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionDieRollModifierBuilder(FeatureDefinitionDieRollModifier original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionDieRollModifierBuilder(FeatureDefinitionDieRollModifier original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }
    }
}
