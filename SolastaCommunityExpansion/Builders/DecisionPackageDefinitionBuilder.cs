using System;
using TA.AI;

namespace SolastaCommunityExpansion.Builders
{
    public class DecisionPackageDefinitionBuilder : DefinitionBuilder<DecisionPackageDefinition, DecisionPackageDefinitionBuilder>
    {
        #region Constructors
        protected DecisionPackageDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected DecisionPackageDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected DecisionPackageDefinitionBuilder(DecisionPackageDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected DecisionPackageDefinitionBuilder(DecisionPackageDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
