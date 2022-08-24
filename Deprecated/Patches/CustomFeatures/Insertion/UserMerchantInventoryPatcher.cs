using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches.CustomFeatures.Insertion;

internal static class UserMerchantInventoryPatcher
{
    [HarmonyPatch(typeof(UserMerchantInventory), "CreateMerchantDefinition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CreateMerchantDefinition
    {
        internal static void Postfix(UserMerchantInventory __instance, ref MerchantDefinition __result)
        {
            CustomWeaponsContext.TryAddItemsToUserMerchant(__result);
        }
    }
}
