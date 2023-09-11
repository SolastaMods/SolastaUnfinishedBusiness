using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;

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

    internal FeatureDefinitionFeatureSetBuilder SetFeatureSet(params FeatureDefinition[] featureDefinitions)
    {
        Definition.featureSet.Clear();
        return AddFeatureSet(featureDefinitions);
    }

    internal FeatureDefinitionFeatureSetBuilder SetMode(FeatureDefinitionFeatureSet.FeatureSetMode mode)
    {
        Definition.mode = mode;
        return this;
    }

    internal FeatureDefinitionFeatureSetBuilder SetAncestryType(
        ExtraAncestryType ancestryType,
        params string[] ancestryDamageTypeMap)
    {
        Definition.ancestryType = (AncestryType)ancestryType;
        Definition.ancestryDamageTypeMap.AddRange(ancestryDamageTypeMap);
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
