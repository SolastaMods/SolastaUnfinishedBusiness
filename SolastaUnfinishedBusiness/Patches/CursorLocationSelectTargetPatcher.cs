using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class CursorLocationSelectTargetPatcher
{
    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.IsFilteringValid))]
    [UsedImplicitly]
    public static class IsFilteringValid_Patch
    {
        [UsedImplicitly]
        public static void Postfix(
            CursorLocationSelectTarget __instance,
            GameLocationCharacter target,
            ref bool __result)
        {
            //TODO: replace with IFilterTargetingMagicEffect
            //PATCH: support for target spell filtering based on custom spell filters
            // used for melee cantrips to limit targets to weapon attack range
            if (!__result)
            {
                return;
            }

            __result = IsFilteringValidMeleeCantrip(__instance, target);

            //PATCH: supports IFilterTargetingMagicEffect
            if (!__result)
            {
                return;
            }

            foreach (var filterTargetingMagicEffect in __instance.actionParams.actingCharacter.RulesetCharacter
                         .GetSubFeaturesByType<IFilterTargetingMagicEffect>())
            {
                __result = filterTargetingMagicEffect.IsValid(__instance, target);

                if (!__result)
                {
                    return;
                }
            }
        }

        private static bool IsFilteringValidMeleeCantrip(
            CursorLocationSelectTarget __instance,
            GameLocationCharacter target)
        {
            var actionParams = __instance.actionParams;
            var canBeUsedToAttack = actionParams?.RulesetEffect
                ?.SourceDefinition.GetFirstSubFeatureOfType<IAttackAfterMagicEffect>()?.CanBeUsedToAttack;

            if (canBeUsedToAttack == null || canBeUsedToAttack(__instance, actionParams.actingCharacter, target,
                    out var failure))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add(failure);

            return false;
        }
    }

    [HarmonyPatch(typeof(CursorLocationSelectTarget), nameof(CursorLocationSelectTarget.IsValidMagicAttack))]
    [UsedImplicitly]
    public static class IsValidMagicAttack_Patch
    {
        [UsedImplicitly]
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
}
