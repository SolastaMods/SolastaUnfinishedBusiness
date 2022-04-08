using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Builders
{
    public class TreasureTableDefinitionBuilder : DefinitionBuilder<TreasureTableDefinition, TreasureTableDefinitionBuilder>
    {
        #region Constructors
        protected TreasureTableDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected TreasureTableDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected TreasureTableDefinitionBuilder(TreasureTableDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected TreasureTableDefinitionBuilder(TreasureTableDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public TreasureTableDefinitionBuilder AddTreasureOptions(params TreasureOption[] treasureOptions)
        {
            return AddTreasureOptions(treasureOptions.AsEnumerable());
        }

        public TreasureTableDefinitionBuilder AddTreasureOptions(IEnumerable<TreasureOption> treasureOptions)
        {
            Definition.TreasureOptions.AddRange(treasureOptions);
            return this;
        }
    }
}
