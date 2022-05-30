using System;

namespace SolastaCommunityExpansion.Builders;

public class WeaponTypeDefinitionBuilder : DefinitionBuilder<WeaponTypeDefinition>
{
    #region Constructors

    internal WeaponTypeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal WeaponTypeDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal WeaponTypeDefinitionBuilder(WeaponTypeDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    internal WeaponTypeDefinitionBuilder(WeaponTypeDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    internal WeaponTypeDefinitionBuilder(WeaponTypeDefinition original) : base(original)
    {
    }

    #endregion
}
