using System;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public class FeatDefinitionBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        private void InitializePrerequisiteCollectionFields()
        {
            Definition.SetField("knownFeatsPrerequisite", new List<string>());
            Definition.SetField("compatibleRacesPrerequisite", new List<string>());
            Definition.SetField("compatibleClassesPrerequisite", new List<string>());
        }

        public FeatDefinitionBuilder(string name, string guid, IEnumerable<FeatureDefinition> features, GuiPresentation guiPresentation) : base(name, guid)
        {
            InitializePrerequisiteCollectionFields();

            Definition.SetField("features", features.ToList());
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatDefinitionBuilder(string name, Guid namespaceGuid, string category, params FeatureDefinition[] features)
            : this(name, namespaceGuid, category, features.AsEnumerable())
        {
        }

        public FeatDefinitionBuilder(string name, Guid namespaceGuid, string category, IEnumerable<FeatureDefinition> features)
            : base(name, namespaceGuid, category)
        {
            InitializePrerequisiteCollectionFields();

            Definition.SetField("features", features.ToList());
        }

        public FeatDefinitionBuilder(string name, string guid) : base(name, guid)
        {
            InitializePrerequisiteCollectionFields();
        }

        public FeatDefinitionBuilder(FeatDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        public FeatDefinitionBuilder SetFeatures(params FeatureDefinition[] features)
        {
            return SetFeatures(features.AsEnumerable());
        }

        public FeatDefinitionBuilder SetFeatures(IEnumerable<FeatureDefinition> features)
        {
            Definition.SetField("features", features.ToList());
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
