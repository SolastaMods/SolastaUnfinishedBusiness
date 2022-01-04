using SolastaModApi;
using SolastaModApi.Extensions;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Builders.Features
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
        private OnAttackHitDelegate onHit;

        internal void SetOnAttackHitDelegate(OnAttackHitDelegate del)
        {
            onHit = del;
        }

        public void OnAttackHit(GameLocationCharacter attacker,
                GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode,
                bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms,
                RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget)
        {
            onHit?.Invoke(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
        }
    }

    public class FeatureDefinitionOnAttackHitEffectBuilder : BaseDefinitionBuilder<FeatureDefinitionOnAttackHitEffect>
    {
        public FeatureDefinitionOnAttackHitEffectBuilder(string name, string guid,
            OnAttackHitDelegate onHit, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.SetOnAttackHitDelegate(onHit);
            Definition.SetGuiPresentation(guiPresentation);
        }
    }
}
