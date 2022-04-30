using System;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionPointPoolWithBonusBuilder : FeatureDefinitionPointPoolBuilder<
        FeatureDefinitionPointPoolWithBonus, FeatureDefinitionPointPoolWithBonusBuilder>
    {
        #region Constructors

        public FeatureDefinitionPointPoolWithBonusBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionPointPoolWithBonusBuilder(string name, string definitionGuid) : base(name,
            definitionGuid)
        {
        }

        public FeatureDefinitionPointPoolWithBonusBuilder(FeatureDefinitionPointPoolWithBonus original, string name,
            Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionPointPoolWithBonusBuilder(FeatureDefinitionPointPoolWithBonus original, string name,
            string definitionGuid) : base(original, name, definitionGuid)
        {
        }

        #endregion
    }
}
