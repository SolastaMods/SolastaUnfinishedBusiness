using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public class FeatDefinitionBuilder : DefinitionBuilder<FeatDefinition, FeatDefinitionBuilder>
    {
        protected FeatDefinitionBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        protected FeatDefinitionBuilder(FeatDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        protected FeatDefinitionBuilder(FeatDefinition original, string name, Guid namespaceGuid)
            : base(original, name, namespaceGuid)
        {
        }

        public static FeatDefinitionBuilder Create(FeatDefinition original, string name, string guid)
        {
            return new FeatDefinitionBuilder(original, name, guid);
        }

        public static FeatDefinitionBuilder Create(FeatDefinition original, string name, Guid namespaceGuid)
        {
            return new FeatDefinitionBuilder(original, name, namespaceGuid);
        }

        public static FeatDefinitionBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatDefinitionBuilder(name, namespaceGuid);
        }

        public FeatDefinitionBuilder SetFeatures(params FeatureDefinition[] features)
        {
            return SetFeatures(features.AsEnumerable());
        }

        public FeatDefinitionBuilder SetFeatures(IEnumerable<FeatureDefinition> features)
        {
            Definition.Features.SetRange(features);
            return this;
        }

        public FeatDefinitionBuilder AddFeatures(params FeatureDefinition[] features)
        {
            return AddFeatures(features.AsEnumerable());
        }

        public FeatDefinitionBuilder AddFeatures(IEnumerable<FeatureDefinition> features)
        {
            Definition.Features.AddRange(features);
            return this;
        }

        public FeatDefinitionBuilder SetAbilityScorePrerequisite(string abilityScore, int value)
        {
            Definition.SetMinimalAbilityScorePrerequisite(true);
            Definition.SetMinimalAbilityScoreName(abilityScore);
            Definition.SetMinimalAbilityScoreValue(value);
            return this;
        }

        public FeatDefinitionBuilder SetMustCastSpellsPrerequisite()
        {
            Definition.SetMustCastSpellsPrerequisite(true);
            return this;
        }

        public FeatDefinitionBuilder SetClassPrerequisite(params string[] classes)
        {
            return SetClassPrerequisite(classes.AsEnumerable());
        }

        public FeatDefinitionBuilder SetClassPrerequisite(IEnumerable<string> classes)
        {
            Definition.CompatibleClassesPrerequisite.SetRange(classes);
            return this;
        }

        public FeatDefinitionBuilder SetRacePrerequisite(params string[] races)
        {
            return SetRacePrerequisite(races.AsEnumerable());
        }

        public FeatDefinitionBuilder SetRacePrerequisite(IEnumerable<string> races)
        {
            Definition.CompatibleRacesPrerequisite.SetRange(races);
            return this;
        }

        public FeatDefinitionBuilder SetFeatPrerequisite(params string[] feats)
        {
            return SetFeatPrerequisite(feats.AsEnumerable());
        }

        public FeatDefinitionBuilder SetFeatPrerequisite(IEnumerable<string> feats)
        {
            Definition.KnownFeatsPrerequisite.SetRange(feats);
            return this;
        }

        public FeatDefinitionBuilder SetArmorProficiencyPrerequisite(ArmorCategoryDefinition category)
        {
            Definition.SetArmorProficiencyPrerequisite(true);
            Definition.SetArmorProficiencyCategory(category.Name);
            return this;
        }
    }
}
