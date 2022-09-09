using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Patches;

internal static class UserMerchantInventoryPatcher
{
    //PATCH: supports adding custom items to dungeon maker traders
    [HarmonyPatch(typeof(UserMerchantInventory), "CreateMerchantDefinition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CreateMerchantDefinition_Patch
    {
        internal static void Postfix(ref MerchantDefinition __result)
        {
            CustomWeaponsContext.TryAddItemsToUserMerchant(__result);
        }
    }
}
