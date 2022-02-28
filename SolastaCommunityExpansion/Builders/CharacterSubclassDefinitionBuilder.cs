using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public sealed class CharacterSubclassDefinitionBuilder : DefinitionBuilder<CharacterSubclassDefinition, CharacterSubclassDefinitionBuilder>
    {
        private CharacterSubclassDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private CharacterSubclassDefinitionBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        private CharacterSubclassDefinitionBuilder(CharacterSubclassDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        private CharacterSubclassDefinitionBuilder(CharacterSubclassDefinition original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public static CharacterSubclassDefinitionBuilder Create(string name, string guid)
        {
            return new CharacterSubclassDefinitionBuilder(name, guid);
        }

        public static CharacterSubclassDefinitionBuilder Create(string name, Guid namespaceGuid)
        {
            return new CharacterSubclassDefinitionBuilder(name, namespaceGuid);
        }

        public static CharacterSubclassDefinitionBuilder Create(CharacterSubclassDefinition original, string name, string guid)
        {
            return new CharacterSubclassDefinitionBuilder(original, name, guid);
        }

        public static CharacterSubclassDefinitionBuilder Create(CharacterSubclassDefinition original, string name, Guid namespaceGuid)
        {
            return new CharacterSubclassDefinitionBuilder(original, name, namespaceGuid);
        }

        public CharacterSubclassDefinitionBuilder AddPersonality(PersonalityFlagDefinition personalityType, int weight)
        {
            Definition.PersonalityFlagOccurences.Add(
              new PersonalityFlagOccurence(DatabaseHelper.CharacterSubclassDefinitions.MartialChampion.PersonalityFlagOccurences[0])
                .SetWeight(weight)
                .SetPersonalityFlag(personalityType.Name));
            return this;
        }

        public CharacterSubclassDefinitionBuilder AddFeatureAtLevel(FeatureDefinition feature, int level)
        {
            Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(feature, level));
            return this;
        }
    }
}
