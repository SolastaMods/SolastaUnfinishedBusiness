using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    // TODO move complete builders to ModAPI, move reusable builders to the Features folder so they can be shared.
    public class LootPackDefinitionBuilder : BaseDefinitionBuilder<LootPackDefinition>
    {
        protected LootPackDefinitionBuilder(string name, string guid, string title, string description, LootPackDefinition base_loot) : base(base_loot, name, guid)
        {
            // ?? would these be better as !string.IsNullOr...
            if (title != "")
            {
                Definition.GuiPresentation.Title = title;
            }
            if (description != "")
            {
                Definition.GuiPresentation.Description = description;
            }
        }

        public static LootPackDefinition CreateCopyFrom(string name, string guid, string title, string description, LootPackDefinition base_loot)
        {
            return new LootPackDefinitionBuilder(name, guid, title, description, base_loot).AddToDB();
        }
    }
}
