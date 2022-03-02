using System.Collections.Generic;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface IOnAttackHitEffect
    {
        void OnAttackHit(GameLocationCharacter attacker,
                GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode,
                bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms,
                RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget);
    }

    public delegate void OnAttackHitDelegate(GameLocationCharacter attacker,
                GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode,
                bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms,
                RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget);

    /**
     * Before using this, please consider if FeatureDefinitionAdditionalDamage can cover the desired use case.
     * This has much greater flexibility, so there are cases where it is appropriate, but when possible it is
     * better for future maintainability of features to use the features provided by TA.
     */

    public class FeatureDefinitionOnAttackHitEffect : FeatureDefinition, IOnAttackHitEffect
    {
        private OnAttackHitDelegate onAttackHitDelegate;

        internal static void Setup()
        {
            ModContext.HandleCharacterAttackDamageHandler += new ModContext.HandleCharacterAttackDamage(MyHandleCharacterAttackDamage);
        }

        private static void MyHandleCharacterAttackDamage(
            ref GameLocationCharacter attacker,
            ref GameLocationCharacter defender,
            ref ActionModifier attackModifier,
            ref RulesetAttackMode attackMode,
            ref bool rangedAttack,
            ref RuleDefinitions.AdvantageType advantageType,
            ref List<EffectForm> actualEffectForms,
            ref RulesetEffect rulesetEffect,
            ref bool criticalHit,
            ref bool firstTarget,
            bool isPrefix)
        {
            if (isPrefix || attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (IOnAttackHitEffect feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
            {
                feature.OnAttackHit(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
            }
        }

        internal void SetOnAttackHitDelegate(OnAttackHitDelegate onAttackHitDelegate)
        {
            this.onAttackHitDelegate = onAttackHitDelegate;
        }

        public void OnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            RuleDefinitions.AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool criticalHit,
            bool firstTarget)
        {
            onAttackHitDelegate?.Invoke(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
        }
    }
}
