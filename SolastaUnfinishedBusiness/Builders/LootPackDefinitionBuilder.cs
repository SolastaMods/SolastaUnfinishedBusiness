using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class LootPackDefinitionBuilder : DefinitionBuilder<LootPackDefinition, LootPackDefinitionBuilder>
{
    internal LootPackDefinitionBuilder SetItemOccurrencesList([NotNull] params ItemOccurence[] occurrences)
    {
        Definition.ItemOccurencesList.SetRange(occurrences);
        return this;
    }

    #region Constructors

    protected LootPackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected LootPackDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected LootPackDefinitionBuilder(LootPackDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected LootPackDefinitionBuilder(LootPackDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
