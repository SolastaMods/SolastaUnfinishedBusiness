using System;
using TA.AI;

namespace SolastaCommunityExpansion.Builders
{
    public class DecisionDefinitionBuilder : DefinitionBuilder<DecisionDefinition, DecisionDefinitionBuilder>
    {
        #region Constructors

        protected DecisionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected DecisionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected DecisionDefinitionBuilder(DecisionDefinition original, string name, Guid namespaceGuid) : base(
            original, name, namespaceGuid)
        {
        }

        protected DecisionDefinitionBuilder(DecisionDefinition original, string name, string definitionGuid) : base(
            original, name, definitionGuid)
        {
        }

        #endregion
    }
}
