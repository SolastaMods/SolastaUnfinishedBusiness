using System;

namespace SolastaCommunityExpansion.Builders
{
    public class CharacterFamilyDefinitionBuilder : DefinitionBuilder<CharacterFamilyDefinition, CharacterFamilyDefinitionBuilder>
    {
        #region Constructors
        protected CharacterFamilyDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected CharacterFamilyDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected CharacterFamilyDefinitionBuilder(CharacterFamilyDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected CharacterFamilyDefinitionBuilder(CharacterFamilyDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
