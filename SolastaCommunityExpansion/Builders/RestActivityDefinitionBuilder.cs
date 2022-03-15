using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public class RestActivityDefinitionBuilder : DefinitionBuilder<RestActivityDefinition, RestActivityDefinitionBuilder>
    {
        #region Constructors
        protected RestActivityDefinitionBuilder(RestActivityDefinition original) : base(original)
        {
        }

        public RestActivityDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected RestActivityDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected RestActivityDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected RestActivityDefinitionBuilder(RestActivityDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
 //       public static RestActivityDefinitionBuilder Create(string name, Guid namespaceGuid)
 //       {
 //           return new RestActivityDefinitionBuilder(name, namespaceGuid);
 //       }

        // TODO: add Create methods


        public RestActivityDefinitionBuilder Configure ( RestDefinitions.RestStage restStage, RuleDefinitions.RestType restType,
        RestActivityDefinition.ActivityCondition condition, string functor, string stringParameter, GuiPresentation guiPresentation)
        {
            Definition.SetRestStage(restStage);
            Definition.SetRestType(restType);
            Definition.SetCondition(condition);
            Definition.SetFunctor(functor);
            Definition.SetStringParameter(stringParameter);
            Definition.SetGuiPresentation(guiPresentation);
            return This();
        }

    }
}
