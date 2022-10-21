using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaUnfinishedBusiness.Patches;

public static class AttackEvaluationParamsPatcher
{
    [HarmonyPatch(typeof(BattleDefinitions.AttackEvaluationParams), "FillForMagicTouchAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class FillForMagicTouchAttack_Patch
    {
        public static void Postfix(
            // Since `AttackEvaluationParams` is a struct, we need to use ref to get actual object, instead of a copy
            ref BattleDefinitions.AttackEvaluationParams __instance,
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
    public static class FillForMagicReachAttack_Patch
    {
        public static void Postfix(
            // Since `AttackEvaluationParams` is a struct, we need to use ref to get actual object, instead of a copy
            ref BattleDefinitions.AttackEvaluationParams __instance,
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
