using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionMoveThroughEnemyModifierBuilder : FeatureDefinitionBuilder<
    FeatureDefinitionMoveThroughEnemyModifier,
    FeatureDefinitionMoveThroughEnemyModifierBuilder>

{
    internal FeatureDefinitionMoveThroughEnemyModifierBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    internal FeatureDefinitionMoveThroughEnemyModifierBuilder(FeatureDefinitionMoveThroughEnemyModifier original,
        string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal FeatureDefinitionMoveThroughEnemyModifierBuilder SetMinSizeDifference(int minSizeDifference)
    {
        Definition.minSizeDifference = minSizeDifference;
        return this;
    }
}
