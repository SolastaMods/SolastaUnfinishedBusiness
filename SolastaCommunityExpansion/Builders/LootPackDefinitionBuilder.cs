using System;

namespace SolastaCommunityExpansion.Builders;

public class LootPackDefinitionBuilder : DefinitionBuilder<LootPackDefinition, LootPackDefinitionBuilder>
{
    public static LootPackDefinitionBuilder CreateCopyFrom(LootPackDefinition original, string name, string guid)
    {
        return new LootPackDefinitionBuilder(original, name, guid);
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
