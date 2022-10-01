using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

public abstract class WeaponTypeDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : WeaponTypeDefinition
    where TBuilder : WeaponTypeDefinitionBuilder<TDefinition, TBuilder>
{
    public TBuilder SetWeaponCategory(WeaponCategoryDefinition category)
    {
        Definition.weaponCategory = category.Name;
        return This();
    }

    public TBuilder SetAnimationTag(string tag)
    {
        Definition.animationTag = tag;
        return This();
    }

    #region Constructors

    protected WeaponTypeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected WeaponTypeDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected WeaponTypeDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    protected WeaponTypeDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class WeaponTypeDefinitionBuilder
    : WeaponTypeDefinitionBuilder<WeaponTypeDefinition, WeaponTypeDefinitionBuilder>
{
    #region Constructors

    public WeaponTypeDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    public WeaponTypeDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    public WeaponTypeDefinitionBuilder(WeaponTypeDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }

    public WeaponTypeDefinitionBuilder(WeaponTypeDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
