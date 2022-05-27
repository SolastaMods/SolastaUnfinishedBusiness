using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendSpellSlot")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationActionManager_ReactToSpendSpellSlot
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            __instance.InvokeMethod("AddInterruptRequest",
                new ReactionRequestSpendSpellSlotExtended(reactionParams));
            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactForOpportunityAttack")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationActionManager_ReactForOpportunityAttack
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            __instance.InvokeMethod("AddInterruptRequest", new ReactionRequestWarcaster(reactionParams));

            return false;
        }
    }

    [HarmonyPatch(typeof(GameLocationActionManager), "ReactToSpendPower")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GameLocationActionManager_ReactToSpendPower
    {
        internal static bool Prefix(GameLocationActionManager __instance, CharacterActionParams reactionParams)
        {
            if (reactionParams.RulesetEffect is RulesetEffectPower powerEffect
                && powerEffect.PowerDefinition.IsBundlePower())
            {
                __instance.InvokeMethod("AddInterruptRequest", new ReactionRequestSpendBundlePower(reactionParams));
                return false;
            }

            return true;
        }
    }
}
