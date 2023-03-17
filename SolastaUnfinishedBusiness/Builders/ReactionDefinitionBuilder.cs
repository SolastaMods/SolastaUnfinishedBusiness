using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class ReactionDefinitionBuilder : DefinitionBuilder<ReactionDefinition, ReactionDefinitionBuilder>
{
    internal ReactionDefinitionBuilder SetReact(string react)
    {
        Definition.reactTitle = $"Reaction/&{react}ReactTitle";
        Definition.reactDescription = $"Reaction/&Reaction{react}ReactDescription";;
        return this;
    }
    
    protected ReactionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected ReactionDefinitionBuilder(ReactionDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }
}
