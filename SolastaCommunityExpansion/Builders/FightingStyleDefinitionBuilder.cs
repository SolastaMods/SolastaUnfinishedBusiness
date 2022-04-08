using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public abstract class FightingStyleDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FightingStyleDefinition
        where TBuilder : FightingStyleDefinitionBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FightingStyleDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FightingStyleDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FightingStyleDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FightingStyleDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public TBuilder SetFeatures(IEnumerable<FeatureDefinition> features)
        {
            Definition.SetFeatures(features.OrderBy(f => f.Name));
            Definition.Features.Sort(Sorting.Compare);
            return This();
        }

        public TBuilder SetFeatures(params FeatureDefinition[] features)
        {
            return SetFeatures(features.AsEnumerable());
        }
    }
}
