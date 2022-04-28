using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders
{

    public abstract class SpellWithCustomFeaturesBuilder<TDefinition, TBuilder> : SpellDefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : SpellWithCustomFeatures
        where TBuilder : SpellWithCustomFeaturesBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected SpellWithCustomFeaturesBuilder(string name, string guid) : base(name, guid)
        {
        }

        protected SpellWithCustomFeaturesBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }

        protected SpellWithCustomFeaturesBuilder(TDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        protected SpellWithCustomFeaturesBuilder(TDefinition original, string name, Guid guidNamespace) : base(original, name, guidNamespace)
        {
        }
        #endregion

        public TBuilder AddCustomFeature(object feature)
        {
            Definition.CustomFeatures.Add(feature);
            return This();
        }

        public TBuilder AddCustomFeatures(params object[] features)
        {
            Definition.CustomFeatures.AddRange(features);
            return This();
        }

        public TBuilder AddCustomFeatures(IEnumerable<object> features)
        {
            Definition.CustomFeatures.AddRange(features);
            return This();
        }
    }

    public class SpellWithCustomFeaturesBuilder: SpellWithCustomFeaturesBuilder<SpellWithCustomFeatures, SpellWithCustomFeaturesBuilder>
    {
        public SpellWithCustomFeaturesBuilder(string name, string guid) : base(name, guid)
        {
        }

        public SpellWithCustomFeaturesBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }

        public SpellWithCustomFeaturesBuilder(SpellWithCustomFeatures original, string name, string guid) : base(original, name, guid)
        {
        }

        public SpellWithCustomFeaturesBuilder(SpellWithCustomFeatures original, string name, Guid guidNamespace) : base(original, name, guidNamespace)
        {
        }
    }
}
