using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    public class TreasureTableDefinitionBuilder : BaseDefinitionBuilder<TreasureTableDefinition>
    {
        protected TreasureTableDefinitionBuilder(TreasureTableDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        public static TreasureTableDefinition CreateCopyFrom(TreasureTableDefinition original, string name, string guid)
        {
            return new TreasureTableDefinitionBuilder(original, name, guid).AddToDB();
        }

        public static TreasureTableDefinition CreateCopyFrom(TreasureTableDefinition original, string name, string guid, string title, string description)
        {
            return new TreasureTableDefinitionBuilder(original, name, guid).SetGuiPresentation(title, description).AddToDB();
        }
    }
}
