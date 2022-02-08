using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionOnAttackHitEffectBuilder : BaseDefinitionBuilder<FeatureDefinitionOnAttackHitEffect>
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
