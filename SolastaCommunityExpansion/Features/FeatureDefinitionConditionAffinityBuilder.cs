using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Features
{
    public class FeatureDefinitionConditionAffinityBuilder : BaseDefinitionBuilder<FeatureDefinitionConditionAffinity>
    {
        public FeatureDefinitionConditionAffinityBuilder(FeatureDefinitionConditionAffinity toCopy, string name, string guid,
            GuiPresentation guiPresentation) : base(toCopy, name, guid)
        {
            Definition.SetGuiPresentation(guiPresentation);
        }

    }
}
