using System;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

public class FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionIgnoreDynamicVisionImpairment,
    FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder>
{
    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder SetMaxRange(float range)
    {
        Definition.maxRange = range;
        return this;
    }

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder AddRequiredFeatures(
        params FeatureDefinition[] features)
    {
        Definition.requiredFeatures.AddRange(features);
        return this;
    }

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder AddRequiredFeatures(
        IEnumerable<FeatureDefinition> features)
    {
        Definition.requiredFeatures.AddRange(features);
        return this;
    }

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder AddForbiddenFeatures(
        params FeatureDefinition[] features)
    {
        Definition.forbiddenFeatures.AddRange(features);
        return this;
    }

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder AddForbiddenFeatures(
        IEnumerable<FeatureDefinition> features)
    {
        Definition.forbiddenFeatures.AddRange(features);
        return this;
    }

    #region Constructors

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder(
        FeatureDefinitionIgnoreDynamicVisionImpairment original,
        string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    public FeatureDefinitionIgnoreDynamicVisionImpairmentBuilder(
        FeatureDefinitionIgnoreDynamicVisionImpairment original,
        string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
