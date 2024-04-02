using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionTerrainTypeAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionTerrainTypeAffinity, FeatureDefinitionTerrainTypeAffinityBuilder>
{
    internal FeatureDefinitionTerrainTypeAffinityBuilder IgnoreTravelPacePerceptionMalus(string terrainType)
    {
        Definition.ignoreTravelPacePerceptionMalus = true;
        Definition.terrainType = terrainType;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionTerrainTypeAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionTerrainTypeAffinityBuilder(FeatureDefinitionTerrainTypeAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
