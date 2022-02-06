using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionPointPoolBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
/*        private FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, string guid)
            : base(original, name, guid)
        {
        }

        private FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }*/

        private FeatureDefinitionPointPoolBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid, Category.None)
        {
        }

        public static FeatureDefinitionPointPoolBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionPointPoolBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionPointPoolBuilder Create(string name, string guid)
        {
            return new FeatureDefinitionPointPoolBuilder(name, guid);
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
            return this;
        }

        public FeatureDefinitionPointPoolBuilder OnlyUniqueChoices()
        {
            Definition.SetUniqueChoices(true);
            return this;
        }
    }
}
