using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class UserLootPackPatcher
{
    [HarmonyPatch(typeof(UserLootPack), nameof(UserLootPack.PostLoadJson))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class PostLoadJson_UserLootPack_Patch
    {
        //BUGFIX: game on boot up validates if items in loot packs exist but by the time it does custom items don't exist
        [UsedImplicitly]
        public static bool Prefix(
            UserLootPack __instance,
            out bool someContentRemoved,
            out bool someContentFixed,
            List<string> removedContent)
        {
            someContentRemoved = false;
            someContentFixed = false;

            //PATCH: comment out code that removes items
#if false
            __instance.toRemove.Clear();
            
            foreach (var itemOccurence in __instance.itemOccurences
                         .Where(itemOccurence => 
                             !DatabaseRepository
                                 .GetDatabase<ItemDefinition>()
                                 .HasElement(itemOccurence.ItemDefinitionName)))
            {
                __instance.toRemove.Add(itemOccurence);
            }

            foreach (var userItemOccurence in __instance.toRemove)
            {
                __instance.itemOccurences.Remove(userItemOccurence);
            }
#endif

            return false;
        }
    }
}
