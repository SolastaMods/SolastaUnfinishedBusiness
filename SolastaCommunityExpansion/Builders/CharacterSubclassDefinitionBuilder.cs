using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public class CharacterSubclassDefinitionBuilder : BaseDefinitionBuilder<CharacterSubclassDefinition>
    {
        public CharacterSubclassDefinitionBuilder(string name, string guid)
            : base(name, guid)
        {
        }
        public CharacterSubclassDefinitionBuilder(string name, Guid namespaceGuid, string category = null)
            : base(name, namespaceGuid, category)
        {
        }

        public CharacterSubclassDefinitionBuilder(CharacterSubclassDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }
        public CharacterSubclassDefinitionBuilder(CharacterSubclassDefinition original, string name, Guid namespaceGuid, string category = null)
            : base(original, name, namespaceGuid, category)
        {
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
