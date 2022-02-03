using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionBonusCantripsBuilder : BaseDefinitionBuilder<FeatureDefinitionBonusCantrips>
    {
        public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, string guid,
            GuiPresentation guiPresentation) : base(original, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionBonusCantripsBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionBonusCantripsBuilder ClearBonusCantrips()
        {
            Definition.BonusCantrips.Clear();
            return this;
        }

        public FeatureDefinitionBonusCantripsBuilder AddBonusCantrip(SpellDefinition spellDefinition)
        {
            Definition.BonusCantrips.Add(spellDefinition);
            return this;
        }
    }
}
