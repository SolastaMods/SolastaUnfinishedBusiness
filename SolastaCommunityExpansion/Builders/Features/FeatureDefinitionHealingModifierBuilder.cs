using System;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionHealingModifierBuilder
    : FeatureDefinitionAffinityBuilder<FeatureDefinitionHealingModifier, FeatureDefinitionHealingModifierBuilder>
{
    #region Constructors

    protected FeatureDefinitionHealingModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionHealingModifierBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionHealingModifierBuilder(FeatureDefinitionHealingModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionHealingModifierBuilder(FeatureDefinitionHealingModifier original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
