using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
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
            HeroDefinitions.PointsPoolType poolType, int poolAmount, string keyPrefix = null) : base(name, baseGuid, keyPrefix)
        {
            Definition.SetPoolType(poolType);
            Definition.SetPoolAmount(poolAmount);
        }

        public FeatureDefinitionPointPoolBuilder(string name, string guid) : base(name, guid)
        {
        }

        public FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, string guid) : base(original, name, guid)
        {
        }

        public FeatureDefinitionPointPoolBuilder SetGuiPresentation(GuiPresentation guiPresentation)
        {
            Definition.SetGuiPresentation(guiPresentation);
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
            return this;
        }

        public FeatureDefinitionPointPoolBuilder OnlyUniqueChoices()
        {
            Definition.SetUniqueChoices(true);
            return this;
        }
    }
}
