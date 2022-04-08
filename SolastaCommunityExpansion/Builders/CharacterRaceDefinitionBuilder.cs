using System;
using System.Linq;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders
{
    public class CharacterRaceDefinitionBuilder : DefinitionBuilder<CharacterRaceDefinition, CharacterRaceDefinitionBuilder>
    {
        #region Constructors
        protected CharacterRaceDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected CharacterRaceDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
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

        public CharacterRaceDefinitionBuilder AddFeatureAtLevel(FeatureDefinition feature, int level)
        {
            Definition.AddFeatureUnlocks(new FeatureUnlockByLevel(feature, level));
            Definition.FeatureUnlocks.Sort(Sorting.Compare);
            return this;
        }

        public CharacterRaceDefinitionBuilder AddFeaturesAtLevel(int level, params FeatureDefinition[] features)
        {
            Definition.AddFeatureUnlocks(features.Select(f => new FeatureUnlockByLevel(f, level)));
            Definition.FeatureUnlocks.Sort(Sorting.Compare);
            return this;
        }

        public CharacterRaceDefinitionBuilder SetFeaturesAtLevel(int level, params FeatureDefinition[] features)
        {
            Definition.SetFeatureUnlocks(
                features.Select(f => new FeatureUnlockByLevel(f, level)));
            Definition.FeatureUnlocks.Sort(Sorting.Compare);
            return this;
        }
    }
}
