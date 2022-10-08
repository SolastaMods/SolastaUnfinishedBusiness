using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionFeatureSetBuilder : FeatureDefinitionBuilder<FeatureDefinitionFeatureSet,
    FeatureDefinitionFeatureSetBuilder>
{
    internal FeatureDefinitionFeatureSetBuilder SetFeatureSet(params FeatureDefinition[] featureDefinitions)
    {
        Definition.FeatureSet.SetRange(featureDefinitions);
        Definition.FeatureSet.Sort(Sorting.CompareTitle);
        return this;
    }

    internal FeatureDefinitionFeatureSetBuilder AddFeatureSet(params FeatureDefinition[] featureDefinitions)
    {
        Definition.FeatureSet.AddRange(featureDefinitions);
        Definition.FeatureSet.Sort(Sorting.CompareTitle);
        return this;
    }

    internal FeatureDefinitionFeatureSetBuilder SetEnumerateInDescription(bool value)
    {
        Definition.enumerateInDescription = value;
        return this;
    }

    internal FeatureDefinitionFeatureSetBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
    {
        Definition.mode = mode;
        return this;
    }

    internal FeatureDefinitionFeatureSetBuilder SetUniqueChoices(bool uniqueChoice)
    {
        Definition.uniqueChoices = uniqueChoice;
        return this;
    }
    #region Constructors

    protected FeatureDefinitionFeatureSetBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionFeatureSetBuilder(FeatureDefinitionFeatureSet original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
