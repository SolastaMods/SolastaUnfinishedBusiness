using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class LanguageDefinitionBuilder : DefinitionBuilder<LanguageDefinition, LanguageDefinitionBuilder>
{
    #region Constructors

    protected LanguageDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected LanguageDefinitionBuilder(LanguageDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
