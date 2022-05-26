using System;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionActionAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionActionAffinity,
        FeatureDefinitionActionAffinityBuilder>
    {
        #region Constructors

        protected FeatureDefinitionActionAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name,
            Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionActionAffinityBuilder(FeatureDefinitionActionAffinity original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion

        public FeatureDefinitionActionAffinityBuilder SetAuthorizedActions(params ActionDefinitions.Id[] actions)
        {
            Definition.SetAuthorizedActions(actions);
            Definition.AuthorizedActions.Sort();
            return This();
        }

        public FeatureDefinitionActionAffinityBuilder SetActionExecutionModifiers(
            params ActionDefinitions.ActionExecutionModifier[] modifiers)
        {
            Definition.ActionExecutionModifiers.SetRange(modifiers);
            return This();
        }

        public FeatureDefinitionActionAffinityBuilder SetDefaultAllowedActonTypes()
        {
            Definition.AllowedActionTypes = new[] {true, true, true, true, true, true};
            return This();
        }
    }
}
