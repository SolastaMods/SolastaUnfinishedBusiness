using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class
    FeatureDefinitionAncestryBuilder : DefinitionBuilder<FeatureDefinitionAncestry, FeatureDefinitionAncestryBuilder>
{
    internal FeatureDefinitionAncestryBuilder SetAncestry(ExtraAncestryType type)
    {
        Definition.type = (RuleDefinitions.AncestryType)type;
        return this;
    }

    #region Constructors

    internal FeatureDefinitionAncestryBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionAncestryBuilder(FeatureDefinitionAncestry original, string name, Guid namespaceGuid) :
        base(
            original, name, namespaceGuid)
    {
    }

    #endregion
}
