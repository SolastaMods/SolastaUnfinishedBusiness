using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class
    TutorialStepDefinitionBuilder : DefinitionBuilder<TutorialStepDefinition, TutorialStepDefinitionBuilder>
{
    #region Constructors

    protected TutorialStepDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected TutorialStepDefinitionBuilder(TutorialStepDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
