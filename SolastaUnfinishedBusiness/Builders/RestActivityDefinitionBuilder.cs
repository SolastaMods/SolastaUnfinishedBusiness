using System;

namespace SolastaUnfinishedBusiness.Builders;

public class
    RestActivityDefinitionBuilder : DefinitionBuilder<RestActivityDefinition, RestActivityDefinitionBuilder>
{
    internal RestActivityDefinitionBuilder SetRestData(
        RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType,
        RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter)
    {
        Definition.restStage = restStage;
        Definition.restType = restType;
        Definition.condition = condition;
        Definition.functor = functor;
        Definition.stringParameter = stringParameter;

        return This();
    }

    #region Constructors

    protected RestActivityDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected RestActivityDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}
