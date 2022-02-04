using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionBonusCantripsBuilder : BaseDefinitionBuilder<FeatureDefinitionBonusCantrips>
    {
        public FeatureDefinitionBonusCantripsBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, Guid namespaceGuid, Category category = Category.None)
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

        public FeatureDefinitionBonusCantripsBuilder SetBonusCantrips(params SpellDefinition[] spellDefinitions)
        {
            SetBonusCantrips(spellDefinitions.AsEnumerable());
            return this;
        }

        public FeatureDefinitionBonusCantripsBuilder SetBonusCantrips(IEnumerable<SpellDefinition> spellDefinitions)
        {
            Definition.BonusCantrips.SetRange(spellDefinitions);
            return this;
        }
    }
}
