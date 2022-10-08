using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class WeaponTypeDefinitionBuilder
    : DefinitionBuilder<WeaponTypeDefinition, WeaponTypeDefinitionBuilder>
{
    internal WeaponTypeDefinitionBuilder SetWeaponCategory(WeaponCategoryDefinition category)
    {
        Definition.weaponCategory = category.Name;
        return this;
    }

    internal WeaponTypeDefinitionBuilder SetAnimationTag(string tag)
    {
        Definition.animationTag = tag;
        return this;
    }
    
    #region Constructors

    internal WeaponTypeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal WeaponTypeDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal WeaponTypeDefinitionBuilder(WeaponTypeDefinition original, string name, Guid namespaceGuid) : base(
        original,
        name, namespaceGuid)
    {
    }

    internal WeaponTypeDefinitionBuilder(WeaponTypeDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
