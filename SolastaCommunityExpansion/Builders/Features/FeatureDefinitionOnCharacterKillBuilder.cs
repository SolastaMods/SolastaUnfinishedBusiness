using System;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionOnCharacterKillBuilder : FeatureDefinitionBuilder<FeatureDefinitionOnCharacterKill,
    FeatureDefinitionOnCharacterKillBuilder>
{
    [NotNull]
    public FeatureDefinitionOnCharacterKillBuilder SetOnCharacterKill(OnCharacterKillDelegate onCharacterKillDelegate)
    {
        Definition.SetOnCharacterKill(onCharacterKillDelegate);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionOnCharacterKillBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionOnCharacterKillBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionOnCharacterKillBuilder(FeatureDefinitionOnCharacterKill original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionOnCharacterKillBuilder(FeatureDefinitionOnCharacterKill original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
