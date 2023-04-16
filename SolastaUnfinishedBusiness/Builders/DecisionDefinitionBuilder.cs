using System;
using JetBrains.Annotations;
using TA.AI;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class DecisionDefinitionBuilder
    : DefinitionBuilder<DecisionDefinition, DecisionDefinitionBuilder>
{
    #region Constructors

    protected DecisionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected DecisionDefinitionBuilder(DecisionDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
