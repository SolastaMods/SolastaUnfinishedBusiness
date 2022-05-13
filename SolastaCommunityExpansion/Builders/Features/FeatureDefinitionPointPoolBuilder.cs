using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public abstract class
        FeatureDefinitionPointPoolBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : FeatureDefinitionPointPool
        where TBuilder : FeatureDefinitionPointPoolBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
            original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(TDefinition original, string name, string definitionGuid) : base(
            original, name, definitionGuid)
        {
        }
        #endregion

        public TBuilder Configure(HeroDefinitions.PointsPoolType poolType, int poolAmount,
            bool uniqueChoices, params string[] choices)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);
            Definition.RestrictedChoices.AddRange(choices);
            Definition.SetUniqueChoices(uniqueChoices);
            Definition.RestrictedChoices.Sort();

            return This();
        }

        public TBuilder SetPool(HeroDefinitions.PointsPoolType poolType, int poolAmount)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);
            return This();
        }

        public TBuilder RestrictChoices(params string[] choices)
        {
            return RestrictChoices(choices.AsEnumerable());
        }

        public TBuilder RestrictChoices(IEnumerable<string> choices)
        {
            Definition.RestrictedChoices.AddRange(choices);
            Definition.RestrictedChoices.Sort();
            return This();
        }

        public TBuilder OnlyUniqueChoices()
        {
            Definition.SetUniqueChoices(true);
            return This();
        }
    }

    public class FeatureDefinitionPointPoolBuilder : FeatureDefinitionPointPoolBuilder<FeatureDefinitionPointPool,
        FeatureDefinitionPointPoolBuilder>
    {
        #region Constructors
        public FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionPointPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        public FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid) :
            base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
