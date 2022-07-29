using System;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionReplaceAttackWithCantripBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionReplaceAttackWithCantrip,
        FeatureDefinitionReplaceAttackWithCantripBuilder>
{
    #region Constructors

    protected FeatureDefinitionReplaceAttackWithCantripBuilder(string name, string guid)
        : base(name, guid)
    {
    }

    protected FeatureDefinitionReplaceAttackWithCantripBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionReplaceAttackWithCantripBuilder(FeatureDefinitionReplaceAttackWithCantrip original,
        string name, string guid)
        : base(original, name, guid)
    {
    }

    protected FeatureDefinitionReplaceAttackWithCantripBuilder(FeatureDefinitionReplaceAttackWithCantrip original,
        string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
