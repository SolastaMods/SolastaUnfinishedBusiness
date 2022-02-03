using SolastaModApi;

namespace SolastaCommunityExpansion.Builders
{
    public class LootPackDefinitionBuilder : BaseDefinitionBuilder<LootPackDefinition>
    {
        protected LootPackDefinitionBuilder(LootPackDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        public static LootPackDefinition CreateCopyFrom(LootPackDefinition original, string name, string guid, string title, string description)
        {
            return new LootPackDefinitionBuilder(original, name, guid).SetGuiPresentation(title, description).AddToDB();
        }

        public static LootPackDefinition CreateCopyFrom(LootPackDefinition original, string name, string guid)
        {
            return new LootPackDefinitionBuilder(original, name, guid).AddToDB();
        }
    }
}
