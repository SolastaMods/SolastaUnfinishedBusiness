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

    protected WeaponTypeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected WeaponTypeDefinitionBuilder(WeaponTypeDefinition original, string name, Guid namespaceGuid) : base(
        original,
        name, namespaceGuid)
    {
    }

    #endregion
}
