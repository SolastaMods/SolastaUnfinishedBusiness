using HarmonyLib;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells;

//add suport for target spell filtering based on custom spell filters
//(i.e. preventing target from being selected)
[HarmonyPatch(typeof(CursorLocationSelectTarget), "IsFilteringValid")]
internal static class CursorLocationSelectTarget_IsFilteringValid
{
    internal static void Postfix(CursorLocationSelectTarget __instance, GameLocationCharacter target,
        ref bool __result)
    {
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
