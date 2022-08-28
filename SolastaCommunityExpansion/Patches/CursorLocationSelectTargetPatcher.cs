using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.CustomUI;

namespace SolastaCommunityExpansion.Patches;

internal static class CursorLocationSelectTargetPatcher
{
    [HarmonyPatch(typeof(CursorLocationSelectTarget), "IsValidAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class IsValidAttack_Patch
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var code = new List<CodeInstruction>(instructions);

            //PATCH: Uses attack mode from cursor's ActionParams, instead of first one matching action type
            //required for extra attacks on action panel to work properly
            ExtraAttacksOnActionPanel.ApplyCursorLocationSelectTargetTranspile(code);

            return code;
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), "IsFilteringValid")]
    internal static class IsFilteringValid_Patch
    {
        internal static void Postfix(CursorLocationSelectTarget __instance, GameLocationCharacter target,
            ref bool __result)
        {
            //PATCH: suport for target spell filtering based on custom spell filters
            // used for melee cantrips to limit targets to weapon attack range
            
            if (!__result)
            {
                return;
            }

            var actionParams = __instance.actionParams;

            var canBeUsedToAttack = actionParams?.RulesetEffect
                ?.SourceDefinition.GetFirstSubFeatureOfType<IPerformAttackAfterMagicEffectUse>()?.CanBeUsedToAttack;

            if (canBeUsedToAttack == null || canBeUsedToAttack(__instance, actionParams.actingCharacter, target,
                    out var failure))
            {
                return;
            }

            __result = false;
            __instance.actionModifier.FailureFlags.Add(failure);
        }
    }
}