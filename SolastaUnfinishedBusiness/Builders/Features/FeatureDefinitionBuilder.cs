using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionBuilder : DefinitionBuilder<FeatureDefinition, FeatureDefinitionBuilder>
{
    #region Constructors

    protected FeatureDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionBuilder(FeatureDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    #endregion
}
