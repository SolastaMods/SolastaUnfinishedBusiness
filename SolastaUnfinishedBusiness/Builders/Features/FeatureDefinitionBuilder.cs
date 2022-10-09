using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

internal abstract class FeatureDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinition
    where TBuilder : FeatureDefinitionBuilder<TDefinition, TBuilder>
{
    #region Constructors

    protected FeatureDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class FeatureDefinitionBuilder : FeatureDefinitionBuilder<FeatureDefinition, FeatureDefinitionBuilder>
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
