using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.AdditionalExtensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaModApi;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells
{
    //Checks if attack cantrip is valid to be cast as readied action on a target
    internal static class GameLocationBattleManagerPatcher
    {
        [HarmonyPatch(typeof(GameLocationBattleManager), "IsValidAttackForReadiedAction")]
        [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
        internal static class GameLocationBattleManager_IsValidAttackForReadiedAction
        {
            internal static void Postfix(GameLocationBattleManager __instance, ref bool __result,
                BattleDefinitions.AttackEvaluationParams attackParams,
                bool forbidDisadvantage)
            {
                if (DatabaseHelper.TryGetDefinition<SpellDefinition>(attackParams.effectName, null, out var cantrip))
                {
                    var attack = cantrip.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
                    if (attack != null)
                    {
                        var canAttack = attack.CanAttack;
                        if (canAttack != null)
                        {
                            __result = canAttack(attackParams.attacker, attackParams.defender);
                        }
                    }
                }
            }
        }
    }
}
