using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionBonusCantripsBuilder : BaseDefinitionBuilder<FeatureDefinitionBonusCantrips>
    {
        /*        private FeatureDefinitionBonusCantripsBuilder(string name, string guid)
                    : base(name, guid)
                {
                }

                private FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid, Category category = Category.None)
                    : base(name, namespaceGuid, category)
                {
                }

                private FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, string guid)
                    : base(original, name, guid)
                {
                }
        */
        private FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionBonusCantripsBuilder Create(FeatureDefinitionBonusCantrips original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionBonusCantripsBuilder(original, name, namespaceGuid);
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
