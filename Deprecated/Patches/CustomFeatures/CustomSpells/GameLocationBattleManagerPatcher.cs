using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using SolastaCommunityExpansion.Api;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

//Checks if attack cantrip is valid to be cast as readied action on a target
[HarmonyPatch(typeof(GameLocationBattleManager), "IsValidAttackForReadiedAction")]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
internal static class GameLocationBattleManager_IsValidAttackForReadiedAction
{
    internal static void Postfix(
        GameLocationBattleManager __instance,
        ref bool __result,
        BattleDefinitions.AttackEvaluationParams attackParams,
        bool forbidDisadvantage)
    {
        if (!DatabaseHelper.TryGetDefinition<SpellDefinition>(attackParams.effectName, null, out var cantrip))
        {
            return;
        }

        var attack = cantrip.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>();
        var canAttack = attack?.CanAttack;

        if (canAttack != null)
        {
            __result = canAttack(attackParams.attacker, attackParams.defender);
        }
    }
}
