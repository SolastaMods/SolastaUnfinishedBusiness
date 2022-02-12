namespace SolastaCommunityExpansion.Builders
{
    public class LootPackDefinitionBuilder : DefinitionBuilder<LootPackDefinition>
    {
        protected LootPackDefinitionBuilder(LootPackDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        public static LootPackDefinitionBuilder CreateCopyFrom(LootPackDefinition original, string name, string guid)
        {
            return new LootPackDefinitionBuilder(original, name, guid);
        }
    }
}
