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
        // TODO: remove this constructor
        public FeatDefinitionBuilder(string name, string guid, IEnumerable<FeatureDefinition> features, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.Features.SetRange(features);
            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatDefinitionBuilder(string name, string guid) : base(name, guid)
        {
        }

        public FeatDefinitionBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public FeatDefinitionBuilder(FeatDefinition original, string name, string guid)
            : base(original, name, guid)
        {
        }

        public FeatDefinitionBuilder(FeatDefinition original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        public static FeatDefinitionBuilder CreateCopyFrom(FeatDefinition original, string name, string guid)
        {
            return new FeatDefinitionBuilder(original, name, guid);
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
