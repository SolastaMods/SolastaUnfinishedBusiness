using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionOnAttackHitEffectBuilder : FeatureDefinitionBuilder<FeatureDefinitionOnAttackHitEffect, FeatureDefinitionOnAttackHitEffectBuilder>
    {
        private FeatureDefinitionOnAttackHitEffectBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        public static FeatureDefinitionOnAttackHitEffectBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionOnAttackHitEffectBuilder(name, namespaceGuid);
        }

        public FeatureDefinitionOnAttackHitEffectBuilder SetOnAttackHitDelegate(OnAttackHitDelegate onHit)
        {
            Definition.SetOnAttackHitDelegate(onHit);
            return this;
        }
    }
}
