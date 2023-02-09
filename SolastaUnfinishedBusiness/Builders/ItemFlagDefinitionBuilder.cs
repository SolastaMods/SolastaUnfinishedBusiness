using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class ItemFlagDefinitionBuilder : DefinitionBuilder<ItemFlagDefinition, ItemFlagDefinitionBuilder>
{
    internal ItemFlagDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal ItemFlagDefinitionBuilder(ItemFlagDefinition original, string name, Guid namespaceGuid) : base(original,
        name, namespaceGuid)
    {
    }
}
