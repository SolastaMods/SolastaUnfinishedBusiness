using System;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionOnRollAttackModeBuilder : FeatureDefinitionBuilder<FeatureDefinitionOnRollAttackMode,
    FeatureDefinitionOnRollAttackModeBuilder>
{
    public FeatureDefinitionOnRollAttackModeBuilder SetOnRollAttackModeDelegate(OnRollAttackModeDelegate del)
    {
        Definition.SetOnRollAttackModeDelegate(del);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionOnRollAttackModeBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionOnRollAttackModeBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionOnRollAttackModeBuilder(FeatureDefinitionOnRollAttackMode original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionOnRollAttackModeBuilder(FeatureDefinitionOnRollAttackMode original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
