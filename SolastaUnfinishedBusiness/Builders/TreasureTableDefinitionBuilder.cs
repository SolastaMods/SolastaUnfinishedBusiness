using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class
    TreasureTableDefinitionBuilder : DefinitionBuilder<TreasureTableDefinition, TreasureTableDefinitionBuilder>
{
    internal TreasureTableDefinitionBuilder AddTreasureOptions(IEnumerable<TreasureOption> treasureOptions)
    {
        Definition.TreasureOptions.AddRange(treasureOptions);
        return this;
    }

    #region Constructors

    protected TreasureTableDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected TreasureTableDefinitionBuilder(TreasureTableDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
