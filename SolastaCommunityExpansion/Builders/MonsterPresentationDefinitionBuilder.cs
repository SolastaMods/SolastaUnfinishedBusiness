using System;

namespace SolastaCommunityExpansion.Builders
{
    public class MonsterPresentationDefinitionBuilder : DefinitionBuilder<MonsterPresentationDefinition, MonsterPresentationDefinitionBuilder>
    {
        #region Constructors
        protected MonsterPresentationDefinitionBuilder(MonsterPresentationDefinition original) : base(original)
        {
        }

        protected MonsterPresentationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected MonsterPresentationDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected MonsterPresentationDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected MonsterPresentationDefinitionBuilder(MonsterPresentationDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected MonsterPresentationDefinitionBuilder(MonsterPresentationDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected MonsterPresentationDefinitionBuilder(MonsterPresentationDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
