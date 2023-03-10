using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

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

    protected LootPackDefinitionBuilder(LootPackDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
