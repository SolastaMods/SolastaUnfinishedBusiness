using System;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionOnAttackEffectBuilder : FeatureDefinitionBuilder<FeatureDefinitionOnAttackEffect,
        FeatureDefinitionOnAttackEffectBuilder>
    {
        public FeatureDefinitionOnAttackEffectBuilder SetOnAttackDelegates(OnAttackDelegate before,
            OnAttackDelegate after)
        {
            Definition.SetOnAttackDelegates(before, after);
            return this;
        }

        #region Constructors

        protected FeatureDefinitionOnAttackEffectBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionOnAttackEffectBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        protected FeatureDefinitionOnAttackEffectBuilder(FeatureDefinitionOnAttackEffect original, string name,
            Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionOnAttackEffectBuilder(FeatureDefinitionOnAttackEffect original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion
    }
}
