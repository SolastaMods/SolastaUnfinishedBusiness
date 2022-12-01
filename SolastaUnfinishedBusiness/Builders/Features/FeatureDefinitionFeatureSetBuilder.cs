using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionFeatureSetBuilder
    : DefinitionBuilder<FeatureDefinitionFeatureSet, FeatureDefinitionFeatureSetBuilder>
{
    internal FeatureDefinitionFeatureSetBuilder AddFeatureSet(params FeatureDefinition[] featureDefinitions)
    {
        Definition.FeatureSet.AddRange(featureDefinitions);
        Definition.FeatureSet.Sort(Sorting.CompareTitle);
        return this;
    }
    
    internal FeatureDefinitionFeatureSetBuilder AddFeatureSetNoSort(params FeatureDefinition[] featureDefinitions)
    {
        Definition.FeatureSet.AddRange(featureDefinitions);
        return this;
    }

    internal FeatureDefinitionFeatureSetBuilder ClearFeatureSet()
    {
        Definition.FeatureSet.Clear();
        return this;
    }

    internal FeatureDefinitionFeatureSetBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
    {
        Definition.mode = mode;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
