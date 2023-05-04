using System;
using JetBrains.Annotations;
using TA.AI;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class DamageDefinitionBuilder
    : DefinitionBuilder<DamageDefinition, DamageDefinitionBuilder>
{
    #region Constructors

    protected DamageDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected DamageDefinitionBuilder(DamageDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
