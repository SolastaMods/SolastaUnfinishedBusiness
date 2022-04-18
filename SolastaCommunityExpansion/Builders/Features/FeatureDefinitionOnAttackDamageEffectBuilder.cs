using System;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionOnAttackDamageEffectBuilder : FeatureDefinitionBuilder<FeatureDefinitionOnAttackDamageEffect, FeatureDefinitionOnAttackDamageEffectBuilder>
    {
        #region Constructors
        protected FeatureDefinitionOnAttackDamageEffectBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionOnAttackDamageEffectBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionOnAttackDamageEffectBuilder(FeatureDefinitionOnAttackDamageEffect original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionOnAttackDamageEffectBuilder(FeatureDefinitionOnAttackDamageEffect original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionOnAttackDamageEffectBuilder SetOnAttackDamageDelegate(OnAttackDamageDelegate del)
        {
            Definition.SetOnAttackDamageDelegate(del);
            return this;
        }
    }
}
