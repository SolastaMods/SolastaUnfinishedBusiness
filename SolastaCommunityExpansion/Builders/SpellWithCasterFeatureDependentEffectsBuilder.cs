using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Builders
{
    public abstract class SpellWithCasterFeatureDependentEffectsBuilder<TDefinition, TBuilder> : SpellWithCustomFeaturesBuilder<
        TDefinition,
        TBuilder>
        where TDefinition : SpellWithCasterFeatureDependentEffects
        where TBuilder : SpellWithCustomFeaturesBuilder<TDefinition, TBuilder>
    {
        #region Constructors

        protected SpellWithCasterFeatureDependentEffectsBuilder(string name, string guid) : base(name, guid)
        {
        }

        protected SpellWithCasterFeatureDependentEffectsBuilder(string name, Guid guidNamespace) : base(name,
            guidNamespace)
        {
        }

        protected SpellWithCasterFeatureDependentEffectsBuilder(TDefinition original, string name, string guid) : base(
            original, name, guid)
        {
        }

        protected SpellWithCasterFeatureDependentEffectsBuilder(TDefinition original, string name, Guid guidNamespace) :
            base(original, name, guidNamespace)
        {
        }

        #endregion Constructors

        public TBuilder AddFeatureEffects(params (List<FeatureDefinition>, EffectDescription)[] featureEffects)
        {
            Definition.FeaturesEffectList.AddRange(featureEffects);
            return This();
        }

        public TBuilder SetFeatureEffects(params (List<FeatureDefinition>, EffectDescription)[] featureEffects)
        {
            Definition.FeaturesEffectList.SetRange(featureEffects);
            return This();
        }

        public TBuilder ClearFeatureEffects()
        {
            Definition.FeaturesEffectList.Clear();
            return This();
        }
    }

    public class SpellWithCasterFeatureDependentEffectsBuilder : SpellWithCasterFeatureDependentEffectsBuilder<
        SpellWithCasterFeatureDependentEffects, SpellWithCasterFeatureDependentEffectsBuilder>
    {
        #region Constructors

        public SpellWithCasterFeatureDependentEffectsBuilder(string name, string guid) : base(name, guid)
        {
        }

        public SpellWithCasterFeatureDependentEffectsBuilder(string name, Guid guidNamespace) : base(name,
            guidNamespace)
        {
        }

        public SpellWithCasterFeatureDependentEffectsBuilder(SpellWithCasterFeatureDependentEffects original,
            string name, string guid) : base(original, name, guid)
        {
        }

        public SpellWithCasterFeatureDependentEffectsBuilder(SpellWithCasterFeatureDependentEffects original,
            string name, Guid guidNamespace) : base(original, name, guidNamespace)
        {
        }

        #endregion Constructors
    }
}
