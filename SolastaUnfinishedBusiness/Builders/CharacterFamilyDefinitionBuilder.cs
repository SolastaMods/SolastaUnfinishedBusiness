using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CharacterFamilyDefinitionBuilder
    : DefinitionBuilder<CharacterFamilyDefinition, CharacterFamilyDefinitionBuilder>
{
    internal CharacterFamilyDefinitionBuilder IsExtraPlanar()
    {
        Definition.extraplanar = true;
        return this;
    }

    #region Constructors

    protected CharacterFamilyDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected CharacterFamilyDefinitionBuilder(CharacterFamilyDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
