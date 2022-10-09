using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionOnCharacterKillBuilder : DefinitionBuilder<FeatureDefinitionOnCharacterKill,
    FeatureDefinitionOnCharacterKillBuilder>
{
    [NotNull]
    internal FeatureDefinitionOnCharacterKillBuilder SetOnCharacterKill(OnCharacterKillDelegate onCharacterKillDelegate)
    {
        Definition.SetOnCharacterKill(onCharacterKillDelegate);
        return this;
    }

    #region Constructors

    protected FeatureDefinitionOnCharacterKillBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionOnCharacterKillBuilder(FeatureDefinitionOnCharacterKill original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
