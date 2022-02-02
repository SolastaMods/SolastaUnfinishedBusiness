using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    public class TreasureTableDefinitionBuilder : BaseDefinitionBuilder<TreasureTableDefinition>
    {
        protected TreasureTableDefinitionBuilder(string name, string guid, string title, string description, TreasureTableDefinition base_table) : base(base_table, name, guid)
        {
            if (title != "")
            {
                Definition.GuiPresentation.Title = title;
            }
            if (description != "")
            {
                Definition.GuiPresentation.Description = description;
            }
        }

        public static TreasureTableDefinition createCopyFrom(string name, string guid, string title, string description, TreasureTableDefinition base_table)
        {
            return new TreasureTableDefinitionBuilder(name, guid, title, description, base_table).AddToDB();
        }
    }
}
