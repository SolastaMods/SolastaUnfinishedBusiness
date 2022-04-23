using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders
{

    public abstract class SpellWithCustomFeatursBuilder<TDefinition, TBuilder> : SpellDefinitionBuilder<TDefinition, TBuilder>
        where TDefinition : SpellWithCustomFeatures
        where TBuilder : SpellWithCustomFeatursBuilder<TDefinition, TBuilder>
    {
        #region Constructors
        protected SpellWithCustomFeatursBuilder(string name, string guid) : base(name, guid)
        {
        }

        protected SpellWithCustomFeatursBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }

        protected SpellWithCustomFeatursBuilder(TDefinition original, string name, string guid) : base(original, name, guid)
        {
        }

        protected SpellWithCustomFeatursBuilder(TDefinition original, string name, Guid guidNamespace) : base(original, name, guidNamespace)
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

    public class SpellWithCustomFeatursBuilder: SpellWithCustomFeatursBuilder<SpellWithCustomFeatures, SpellWithCustomFeatursBuilder>
    {
        public SpellWithCustomFeatursBuilder(string name, string guid) : base(name, guid)
        {
        }

        public SpellWithCustomFeatursBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
        {
        }

        public SpellWithCustomFeatursBuilder(SpellWithCustomFeatures original, string name, string guid) : base(original, name, guid)
        {
        }

        public SpellWithCustomFeatursBuilder(SpellWithCustomFeatures original, string name, Guid guidNamespace) : base(original, name, guidNamespace)
        {
        }
    }
}
