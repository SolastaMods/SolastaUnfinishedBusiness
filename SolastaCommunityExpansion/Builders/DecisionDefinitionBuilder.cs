using System;
using TA.AI;

namespace SolastaCommunityExpansion.Builders
{
    public class DecisionDefinitionBuilder : DefinitionBuilder<DecisionDefinition, DecisionDefinitionBuilder>
    {
        #region Constructors
        protected DecisionDefinitionBuilder(DecisionDefinition original) : base(original)
        {
        }

        protected DecisionDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected DecisionDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected DecisionDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected DecisionDefinitionBuilder(DecisionDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected DecisionDefinitionBuilder(DecisionDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected DecisionDefinitionBuilder(DecisionDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
