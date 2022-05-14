using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.CustomUI;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Patches.GameUi.Battle
{
    internal static class GameLocationActionManagerPatcher
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
    }
}
