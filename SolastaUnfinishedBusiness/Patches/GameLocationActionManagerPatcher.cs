using System.Collections;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

internal static class GameLocationActionManagerPatcher
{
    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendSpellSlot")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactToSpendSpellSlot_Patch
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendSpellSlot` reaction with custom one
            __instance.AddInterruptRequest(new ReactionRequestSpendSpellSlotExtended(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactForOpportunityAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactForOpportunityAttack_Patch
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `OpportunityAttack` reaction with warcaster one
            __instance.AddInterruptRequest(new ReactionRequestWarcaster(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendPower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ReactToSpendPower_Patch
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            //PATCH: replace `SpendPower` reaction for bundled powers
            if (reactionParams.RulesetEffect is not RulesetEffectPower powerEffect ||
                !powerEffect.PowerDefinition.IsBundlePower())
            {
                return true;
            }

            __instance.AddInterruptRequest(new ReactionRequestSpendBundlePower(reactionParams));
            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ExecuteActionAsync")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class ExecuteActionAsync_Patch
    {
        internal static IEnumerator Postfix(
            [NotNull] IEnumerator values,
            [NotNull] CharacterAction action)
        {
            Global.ActionStarted(action);

            //PATCH: calls handlers for `ICustomOnActionFeature`
            var features = action.ActingCharacter.RulesetCharacter.GetSubFeaturesByType<ICustomOnActionFeature>();

            foreach (var feature in features)
            {
                feature.OnBeforeAction(action);
            }

            while (values.MoveNext())
            {
                yield return values.Current;
            }

            foreach (var feature in features)
            {
                feature.OnAfterAction(action);
            }
        }
    }
}
