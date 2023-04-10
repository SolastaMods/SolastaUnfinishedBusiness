using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionMoveModeBuilder : FeatureDefinitionBuilder<FeatureDefinitionMoveMode,
    FeatureDefinitionMoveModeBuilder>

{
    internal FeatureDefinitionMoveModeBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionMoveModeBuilder(FeatureDefinitionMoveMode original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal FeatureDefinitionMoveModeBuilder SetMode(RuleDefinitions.MoveMode moveMode, int speed)
    {
        Definition.moveMode = moveMode;
        Definition.speed = speed;
        return this;
    }
}
