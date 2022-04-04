using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionPointPoolBuilder : FeatureDefinitionBuilder<FeatureDefinitionPointPool, FeatureDefinitionPointPoolBuilder>
    {
        #region Constructors
        protected FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original) : base(original)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionPointPoolBuilder Configure(HeroDefinitions.PointsPoolType poolType, int poolAmount,
            bool uniqueChoices, params string[] choices)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);
            Definition.RestrictedChoices.AddRange(choices);
            Definition.SetUniqueChoices(uniqueChoices);
            Definition.RestrictedChoices.Sort();

            return this;
        }

        public FeatureDefinitionPointPoolBuilder SetPool(HeroDefinitions.PointsPoolType poolType, int poolAmount)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);
            return this;
        }

        public FeatureDefinitionPointPoolBuilder RestrictChoices(params string[] choices)
        {
            return RestrictChoices(choices.AsEnumerable());
        }

        public FeatureDefinitionPointPoolBuilder RestrictChoices(IEnumerable<string> choices)
        {
            Definition.RestrictedChoices.AddRange(choices);
            Definition.RestrictedChoices.Sort();
            return this;
        }

        public FeatureDefinitionPointPoolBuilder OnlyUniqueChoices()
        {
            Definition.SetUniqueChoices(true);
            return this;
        }
    }
}
