using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

internal static class AttackEvaluationParamsPatcher
{
    [HarmonyPatch(typeof(BattleDefinitions.AttackEvaluationParams), "FillForMagicTouchAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FillForMagicTouchAttack_Patch
    {
        internal static void Postfix(
            BattleDefinitions.AttackEvaluationParams __instance,
            EffectDescription effectDescription,
            MetamagicOptionDefinition metamagicOption
        )
        {
            //PATCH: allow for `Touch` effects to have reach changed, unless `Distant Spell` metamagic is used
            if (metamagicOption is { Type: RuleDefinitions.MetamagicType.DistantSpell })
            {
                return;
            }

            __instance.maxRange = Math.Max(effectDescription.rangeParameter, 1f);
        }
    }

    [HarmonyPatch(typeof(BattleDefinitions.AttackEvaluationParams), "FillForMagicReachAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FillForMagicReachAttack_Patch
    {
        internal static void Postfix(
            BattleDefinitions.AttackEvaluationParams __instance,
            EffectDescription effectDescription,
            MetamagicOptionDefinition metamagicOption
        )
        {
            //PATCH: allow for `MeleeHit` effects to have reach changed, unless `Distant Spell` metamagic is used
            if (metamagicOption is { Type: RuleDefinitions.MetamagicType.DistantSpell })
            {
                return;
            }

            __instance.maxRange = Math.Max(effectDescription.rangeParameter, 1f);
        }
    }
}
