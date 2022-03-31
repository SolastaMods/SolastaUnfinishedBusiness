using System;
using System.Reflection;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public class CharacterRaceDefinitionBuilder : DefinitionBuilder<CharacterRaceDefinition, CharacterRaceDefinitionBuilder>
    {
        #region Constructors
        protected CharacterRaceDefinitionBuilder(CharacterRaceDefinition original) : base(original)
        {
        }

        protected CharacterRaceDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected CharacterRaceDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected CharacterRaceDefinitionBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        protected CharacterRaceDefinitionBuilder(CharacterRaceDefinition original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        protected CharacterRaceDefinitionBuilder(CharacterRaceDefinition original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected CharacterRaceDefinitionBuilder(CharacterRaceDefinition original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public CharacterRaceDefinitionBuilder SetSizeDefinition(CharacterSizeDefinition characterSizeDefinition)
        {
            Definition.SetSizeDefinition(characterSizeDefinition);
            return this;
        }

        public CharacterRaceDefinitionBuilder SetMinimalAge(int minimalAge)
        {
            Definition.SetMinimalAge(minimalAge);
            return this;
        }

        public CharacterRaceDefinitionBuilder SetMaximalAge(int maximalAge)
        {
            Definition.SetMinimalAge(maximalAge);
            return this;
        }

        public CharacterRaceDefinitionBuilder SetBaseHeight(int baseHeight)
        {
            Definition.SetBaseHeight(baseHeight);
            return this;
        }

        public CharacterRaceDefinitionBuilder SetBaseWeight(int baseWeight)
        {
            Definition.SetBaseWeight(baseWeight);
            return this;
        }

        public CharacterRaceDefinitionBuilder SetRacePresentation(RacePresentation racePresentation)
        {
            Definition.SetRacePresentation(racePresentation);
            return this;
        }

        public CharacterRaceDefinitionBuilder SetFeatures(params (FeatureDefinition featureDefinition, int level)[] featuresByLevel)
        {
            Definition.FeatureUnlocks.Clear();

            foreach (var (featureDefinition, level) in featuresByLevel)
            {
                Definition.FeatureUnlocks.Add(new FeatureUnlockByLevel(featureDefinition, level));
            }

            return this;
        }
    }
}
