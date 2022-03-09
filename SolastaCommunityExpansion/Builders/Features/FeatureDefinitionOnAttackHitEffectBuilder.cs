using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionOnAttackHitEffectBuilder : FeatureDefinitionBuilder<FeatureDefinitionOnAttackHitEffect, FeatureDefinitionOnAttackHitEffectBuilder>
    {
        protected FeatureDefinitionOnAttackHitEffectBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionOnAttackHitEffectBuilder SetOnAttackHitDelegate(OnAttackHitDelegate onHit)
        {
            Definition.SetOnAttackHitDelegate(onHit);
            return this;
        }
    }
}
