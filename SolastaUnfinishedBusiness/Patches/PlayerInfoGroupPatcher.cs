using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Patches;

[UsedImplicitly]
public static class PlayerInfoGroupPatcher
{
    [HarmonyPatch(typeof(PlayerInfoGroup), nameof(PlayerInfoGroup.RefreshPlayerAvatar))]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    [UsedImplicitly]
    public static class RefreshPlayerAvatar_Patch
    {
        [UsedImplicitly]
        public static void Prefix(List<AssetReferenceSprite> defaultPlayerSpriteReferences)
        {
            //PATCH: allows up to 6 players to join the game if there are enough heroes available (PARTYSIZE)
            while (defaultPlayerSpriteReferences.Count < Main.Settings.OverridePartySize)
            {
                defaultPlayerSpriteReferences.Add(defaultPlayerSpriteReferences.ElementAt(0));
            }
        }
    }
}
