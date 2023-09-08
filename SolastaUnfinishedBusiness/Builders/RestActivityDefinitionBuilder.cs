using System;
using JetBrains.Annotations;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class
    RestActivityDefinitionBuilder : DefinitionBuilder<RestActivityDefinition, RestActivityDefinitionBuilder>
{
    internal RestActivityDefinitionBuilder SetRestData(
        RestDefinitions.RestStage restStage,
        RestType restType,
        RestActivityDefinition.ActivityCondition condition,
        string functor,
        string stringParameter)
    {
        Definition.restStage = restStage;
        Definition.restType = restType;
        Definition.condition = condition;
        Definition.functor = functor;
        Definition.stringParameter = stringParameter;
        return this;
    }

    #region Constructors

    protected RestActivityDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    #endregion
}
