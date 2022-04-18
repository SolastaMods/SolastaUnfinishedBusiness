using System;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionOnAttackFinishedEffectBuilder : FeatureDefinitionBuilder<FeatureDefinitionOnAttackFinishedEffect, FeatureDefinitionOnAttackFinishedEffectBuilder>
    {
        #region Constructors
        protected FeatureDefinitionOnAttackFinishedEffectBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionOnAttackFinishedEffectBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionOnAttackFinishedEffectBuilder(FeatureDefinitionOnAttackFinishedEffect original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionOnAttackFinishedEffectBuilder(FeatureDefinitionOnAttackFinishedEffect original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionOnAttackFinishedEffectBuilder SetOnAttackFinishedDelegate(OnAttackFinishedDelegate del)
        {
            Definition.SetOnAttackFinishedDelegate(del);
            return this;
        }
    }
}
