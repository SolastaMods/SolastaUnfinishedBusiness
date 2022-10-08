using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class MorphotypeElementDefinitionBuilder : DefinitionBuilder<MorphotypeElementDefinition,
    MorphotypeElementDefinitionBuilder>
{
    internal MorphotypeElementDefinitionBuilder SetCategory(MorphotypeElementDefinition.ElementCategory value)
    {
        Definition.category = value;
        return This();
    }

    internal MorphotypeElementDefinitionBuilder SetMainColor(Color value)
    {
        Definition.mainColor = value;
        return this;
    }

    internal MorphotypeElementDefinitionBuilder SetSubClassFilterMask(
        GraphicsDefinitions.MorphotypeSubclassFilterTag value)
    {
        Definition.subClassFilterMask = value;
        return this;
    }

    internal MorphotypeElementDefinitionBuilder SetSortOrder(int value)
    {
        Definition.guiPresentation.sortOrder = value;
        return this;
    }

    #region Constructors

    protected MorphotypeElementDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(MorphotypeElementDefinition original, string name, Guid namespaceGuid)
        : base(original, name,
            namespaceGuid)
    {
    }

    protected MorphotypeElementDefinitionBuilder(MorphotypeElementDefinition original, string name,
        string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion
}
