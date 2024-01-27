using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionReplaceAttackWithCantripBuilder
    : DefinitionBuilder<FeatureDefinitionAttackReplaceWithCantrip, FeatureDefinitionReplaceAttackWithCantripBuilder>
{
    #region Constructors

    protected FeatureDefinitionReplaceAttackWithCantripBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionReplaceAttackWithCantripBuilder(FeatureDefinitionAttackReplaceWithCantrip original,
        string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
