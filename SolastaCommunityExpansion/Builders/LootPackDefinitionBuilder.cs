using System;

namespace SolastaCommunityExpansion.Builders
{
    public class LootPackDefinitionBuilder : DefinitionBuilder<LootPackDefinition, LootPackDefinitionBuilder>
    {
        #region Constructors
        protected LootPackDefinitionBuilder(LootPackDefinition original) : base(original)
        {
        }

        protected LootPackDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected LootPackDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected LootPackDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected LootPackDefinitionBuilder(LootPackDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected LootPackDefinitionBuilder(LootPackDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected LootPackDefinitionBuilder(LootPackDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public static LootPackDefinitionBuilder CreateCopyFrom(LootPackDefinition original, string name, string guid)
        {
            return new LootPackDefinitionBuilder(original, name, guid);
        }
    }
}
