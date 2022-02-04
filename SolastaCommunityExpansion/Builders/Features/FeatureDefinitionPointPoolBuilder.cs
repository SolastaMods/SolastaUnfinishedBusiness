using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionPointPoolBuilder : BaseDefinitionBuilder<FeatureDefinitionPointPool>
    {
        public FeatureDefinitionPointPoolBuilder(string name, string guid, HeroDefinitions.PointsPoolType poolType, int poolAmount,
             GuiPresentation guiPresentation) : base(name, guid, guiPresentation)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);
        }

        public FeatureDefinitionPointPoolBuilder(string name, Guid baseGuid,
            HeroDefinitions.PointsPoolType poolType, int poolAmount, Category category = Category.None) : base(name, baseGuid, category)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);
        }

        public FeatureDefinitionPointPoolBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        public FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
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
