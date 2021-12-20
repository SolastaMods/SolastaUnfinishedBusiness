using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionActionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionActionAffinity>
    {
        public FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity toCopy, string name, string guid) : base(toCopy, name, guid)
        {
        }

    }
}
