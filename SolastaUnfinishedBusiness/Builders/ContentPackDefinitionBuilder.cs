using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class
    // ReSharper disable once ClassNeverInstantiated.Local
    ContentPackDefinitionBuilder : DefinitionBuilder<ContentPackDefinition, ContentPackDefinitionBuilder>
{
    #region Constructors

    internal ContentPackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    #endregion
}
