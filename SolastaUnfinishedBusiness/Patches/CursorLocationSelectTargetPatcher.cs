using HarmonyLib;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomFeatures;

namespace SolastaUnfinishedBusiness.Patches;

internal static class CursorLocationSelectTargetPatcher
{
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
