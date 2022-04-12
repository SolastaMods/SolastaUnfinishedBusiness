using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionAttackModifierBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionAttackModifier, FeatureDefinitionAttackModifierBuilder>
    {
        #region Constructors
        protected FeatureDefinitionAttackModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        protected FeatureDefinitionAttackModifierBuilder(FeatureDefinitionAttackModifier original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion

        public FeatureDefinitionAttackModifierBuilder Configure(RuleDefinitions.AttackModifierMethod attackRollModifierMethod,
            int attackRollModifier, string attackRollAbilityScore, RuleDefinitions.AttackModifierMethod damageRollModifierMethod,
            int damageRollModifier, string damageRollAbilityScore, bool canAddAbilityBonusToSecondary, string additionalAttackTag)
        {
            Definition.SetAttackRollModifierMethod(attackRollModifierMethod);
            Definition.SetAttackRollModifier(attackRollModifier);
            Definition.SetAttackRollAbilityScore(attackRollAbilityScore);
            Definition.SetDamageRollModifierMethod(damageRollModifierMethod);
            Definition.SetDamageRollModifier(damageRollModifier);
            Definition.SetDamageRollAbilityScore(damageRollAbilityScore);
            Definition.SetCanAddAbilityBonusToSecondary(canAddAbilityBonusToSecondary);
            Definition.SetAdditionalAttackTag(additionalAttackTag);

            return This();
        }

        public FeatureDefinitionAttackModifierBuilder SetAbilityScoreReplacement(RuleDefinitions.AbilityScoreReplacement replacement)
        {
            Definition.SetAbilityScoreReplacement(replacement);

            return This();
        }

        public FeatureDefinitionAttackModifierBuilder SetAdditionalAttackTag(string tag)
        {
            Definition.SetAdditionalAttackTag(tag);

            return This();
        }
    }
}
