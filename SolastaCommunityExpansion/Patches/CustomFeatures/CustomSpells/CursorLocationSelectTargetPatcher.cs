using HarmonyLib;
using SolastaCommunityExpansion.CustomDefinitions;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomSpells
{
    internal static class CursorLocationSelectTargetPatcher
    {
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

                var actionParams = __instance.GetField<CharacterActionParams>("actionParams");

                var canBeUsedToAttack = CustomFeaturesContext
                    .GetFirstCustomFeature<IPerformAttackAfterMagicEffectUse>(actionParams?.RulesetEffect
                        ?.SourceDefinition)?.CanBeUsedToAttack;
                if (canBeUsedToAttack != null &&
                    !canBeUsedToAttack(actionParams.GetField<GameLocationCharacter>("actingCharacter"), target))
                {
                    __result = false;
                    __instance.GetField<ActionModifier>("actionModifier").FailureFlags
                        .Add("Failure/&FailureFlagTargetIncorrectWeapon"); //TODO: add proper message, maybe from the feature itself?
                }
            }
        }
    }
}
