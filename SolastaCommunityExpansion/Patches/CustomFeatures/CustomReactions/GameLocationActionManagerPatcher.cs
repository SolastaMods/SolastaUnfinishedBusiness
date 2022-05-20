using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.CustomReactions
{
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
}
