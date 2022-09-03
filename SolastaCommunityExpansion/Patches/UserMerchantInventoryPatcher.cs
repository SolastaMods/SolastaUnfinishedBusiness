using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class UserMerchantInventoryPatcher
{
    [HarmonyPatch(typeof(UserMerchantInventory), "CreateMerchantDefinition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CreateMerchantDefinition_Patch
    {
        internal static void Postfix(UserMerchantInventory __instance, ref MerchantDefinition __result)
        {
            //PATCH: support for adding custom items to dungeon maker traders
            CustomWeaponsContext.TryAddItemsToUserMerchant(__result);
        }
    }
}
