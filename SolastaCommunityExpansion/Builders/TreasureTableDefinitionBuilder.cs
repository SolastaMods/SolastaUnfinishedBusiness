using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Builders
{
    public class TreasureTableDefinitionBuilder : DefinitionBuilder<TreasureTableDefinition, TreasureTableDefinitionBuilder>
    {
        protected TreasureTableDefinitionBuilder(TreasureTableDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        public static TreasureTableDefinitionBuilder CreateCopyFrom(TreasureTableDefinition original, string name, string guid)
        {
            return new TreasureTableDefinitionBuilder(original, name, guid);
        }

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
