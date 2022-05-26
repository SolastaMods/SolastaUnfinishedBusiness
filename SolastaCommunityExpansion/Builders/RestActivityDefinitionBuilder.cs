using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public class
        RestActivityDefinitionBuilder : DefinitionBuilder<RestActivityDefinition, RestActivityDefinitionBuilder>
    {
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

        internal RestActivityDefinitionBuilder SetRestData(
            RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType,
            RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter)
        {
            Definition.SetRestStage(restStage);
            Definition.SetRestType(restType);
            Definition.SetCondition(condition);
            Definition.SetFunctor(functor);
            Definition.SetStringParameter(stringParameter);

            return This();
        }
    }
}
