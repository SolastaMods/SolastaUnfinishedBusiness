using System;
using JetBrains.Annotations;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Builders;

public class LootPackDefinitionBuilder : DefinitionBuilder<LootPackDefinition, LootPackDefinitionBuilder>
{
    public LootPackDefinitionBuilder SetItemOccurrencesList([NotNull] params ItemOccurence[] occurrences)
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
