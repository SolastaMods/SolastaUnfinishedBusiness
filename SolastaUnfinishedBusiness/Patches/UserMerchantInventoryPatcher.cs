using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Patches;

public static class UserMerchantInventoryPatcher
{
    [HarmonyPatch(typeof(UserMerchantInventory), "CreateMerchantDefinition")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class CreateMerchantDefinition_Patch
    {
        public static void Postfix(ref MerchantDefinition __result)
        {
            //PATCH: supports adding custom items to dungeon maker traders
            CustomWeaponsContext.TryAddItemsToUserMerchant(__result);
        }
    }
}
