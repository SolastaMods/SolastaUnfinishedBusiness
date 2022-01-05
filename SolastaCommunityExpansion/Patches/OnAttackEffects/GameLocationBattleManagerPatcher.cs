using HarmonyLib;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaCommunityExpansion.Helpers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SolastaCommunityExpansion.Patches.OnAttackEffects
{
    //
    // this patch shouldn't be protected
    //
    [HarmonyPatch(typeof(GameLocationBattleManager), "HandleCharacterAttackDamage")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationBattleManager_HandleCharacterAttackDamage
    {
        internal static void Postfix(GameLocationCharacter attacker,
            GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode,
            bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget)
        {
            if (attacker.RulesetCharacter == null)
            {
                return;
            }

            foreach (IOnAttackHitEffect feature in attacker.RulesetCharacter.EnumerateFeaturesToBrowse<IOnAttackHitEffect>())
            {
                feature.OnAttackHit(attacker, defender, attackModifier, attackMode, rangedAttack, advantageType, actualEffectForms, rulesetEffect, criticalHit, firstTarget);
            }
        }
    }
}
