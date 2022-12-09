using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Patches;

public static class PlayerInfoGroupPatcher
{
    //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
    [HarmonyPatch(typeof(PlayerInfoGroup), "RefreshPlayerAvatar")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    public static class RefreshPlayerAvatar_Patch
    {
        public static void Prefix(List<AssetReferenceSprite> defaultPlayerSpriteReferences)
        {
            while (defaultPlayerSpriteReferences.Count < Main.Settings.OverridePartySize)
            {
                defaultPlayerSpriteReferences.Add(defaultPlayerSpriteReferences.ElementAt(0));
            }
        }
    }
}
