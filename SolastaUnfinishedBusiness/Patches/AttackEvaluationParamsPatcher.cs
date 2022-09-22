using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TA;

namespace SolastaUnfinishedBusiness.Patches;

internal static class AttackEvaluationParamsPatcher
{
    [HarmonyPatch(typeof(BattleDefinitions.AttackEvaluationParams), "FillForMagicTouchAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FillForMagicTouchAttack_Patch
    {
        internal static void Postfix(
            ref BattleDefinitions.AttackEvaluationParams __instance,
            GameLocationCharacter attacker,
            int3 attackPosition,
            EffectDescription effectDescription,
            string effectName,
            GameLocationCharacter defender,
            int3 defenderPosition,
            ActionModifier attackModifier,
            MetamagicOptionDefinition metamagicOption
        )
        {
            //PATCH: allow for `Touch` effects to have reach changed, unless `Distant Spell` metamagic is used
            if (metamagicOption is {Type: RuleDefinitions.MetamagicType.DistantSpell})
                return;

            __instance.maxRange = Math.Max(effectDescription.rangeParameter, 1f);
        }
    }

    [HarmonyPatch(typeof(BattleDefinitions.AttackEvaluationParams), "FillForMagicReachAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class FillForMagicReachAttack_Patch
    {
        internal static void Postfix(
            ref BattleDefinitions.AttackEvaluationParams __instance,
            GameLocationCharacter attacker,
            int3 attackPosition,
            EffectDescription effectDescription,
            string effectName,
            GameLocationCharacter defender,
            int3 defenderPosition,
            ActionModifier attackModifier,
            MetamagicOptionDefinition metamagicOption
        )
        {
            //PATCH: allow for `MeleeHit` effects to have reach changed, unless `Distant Spell` metamagic is used
            if (metamagicOption is {Type: RuleDefinitions.MetamagicType.DistantSpell})
                return;

            __instance.maxRange = Math.Max(effectDescription.rangeParameter, 1f);
        }
    }
}