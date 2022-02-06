using System;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionOnAttackHitEffectBuilder : BaseDefinitionBuilder<FeatureDefinitionOnAttackHitEffect>
    {
        private FeatureDefinitionOnAttackHitEffectBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        public static FeatureDefinitionOnAttackHitEffectBuilder Create(string name, Guid namespaceGuid, Category category = Category.None)
        {
            return new FeatureDefinitionOnAttackHitEffectBuilder(name, namespaceGuid, category);
        }

        public FeatureDefinitionOnAttackHitEffectBuilder SetOnAttackHitDelegate(OnAttackHitDelegate onHit)
        {
            Definition.SetOnAttackHitDelegate(onHit);
            return this;
        }
    }
}
