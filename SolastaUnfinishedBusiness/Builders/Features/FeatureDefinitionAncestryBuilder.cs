using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class
    FeatureDefinitionAncestryBuilder : DefinitionBuilder<FeatureDefinitionAncestry, FeatureDefinitionAncestryBuilder>
{
    internal FeatureDefinitionAncestryBuilder SetAncestry(ExtraAncestryType type)
    {
        Definition.type = (AncestryType)type;
        return this;
    }

    internal FeatureDefinitionAncestryBuilder SetDamageType(string damageType)
    {
        Definition.damageType = damageType;
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
