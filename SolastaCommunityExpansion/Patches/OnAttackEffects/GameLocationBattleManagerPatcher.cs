using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Patches.OnAttackEffects
{
    class GameLocationBattleManagerPatcher
    {
        [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
        internal static class GameLocationBattleManager_HandleCharacterAttackDamage_Patch
        {
            internal static void Postfix(GameLocationBattleManager __instance, GameLocationCharacter attacker, 
                GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode, 
                bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms, 
                RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget)
            {
                if (attacker.RulesetCharacter == null)
                {
                    return;
                }
                List<FeatureDefinition> features = new List<FeatureDefinition>();
                attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>(features);

                foreach (IOnAttackHitEffect feature in features)
                {
                    feature.OnAttackHit(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
                }
            }
        }

    }
}
