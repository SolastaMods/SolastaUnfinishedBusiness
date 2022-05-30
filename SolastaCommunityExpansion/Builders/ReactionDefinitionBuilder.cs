using System;

namespace SolastaCommunityExpansion.Builders;

public class ReactionDefinitionBuilder : DefinitionBuilder<ReactionDefinition, ReactionDefinitionBuilder>
{
    internal ReactionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal ReactionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal ReactionDefinitionBuilder(ReactionDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    internal ReactionDefinitionBuilder(ReactionDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }
}
