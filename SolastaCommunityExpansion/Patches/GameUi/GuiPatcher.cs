using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using SolastaCommunityExpansion.Models;
using SolastaCommunityExpansion.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaCommunityExpansion.Patches.GameUi
{
    // allows extra characters on campaign names
    [HarmonyPatch(typeof(Gui), "TrimInvalidCharacterNameSymbols")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class GUI_TrimInvalidCharacterNameSymbols
    {
        private static readonly HashSet<char> InvalidFilenameChars = Path.GetInvalidFileNameChars().ToHashSet();

        public static bool Prefix(string originString, ref string __result)
        {
            if (!Main.Settings.AllowExtraKeyboardCharactersInAllNames || originString == null)
            {
                return true;
            }

            // Solasta original code disallows invalid filename chars and an additional list of illegal chars.
            // We're disallowing invalid filename chars only.
            // We're trimming whitespace from start only as per original method.
            // This allows the users to create a name with spaces inside, but also allows trailing space.
            __result = new string(originString
                    .Where(n => !InvalidFilenameChars.Contains(n))
                    .ToArray())
                .TrimStart();

            return false;
        }
    }

    //
    // custom resources enablement patch
    //

    [HarmonyPatch]
    internal static class Gui_LoadAssetAsync
    {
        internal static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(Gui), "LoadAssetSync", new Type[] { typeof(AssetReference) }, new Type[] { typeof(Sprite) });
        }

        internal static bool Prefix(AssetReference asset)
        {
            // If it's a CEAssetReferenceSprite prevent async load
            return asset is not CEAssetReferenceSprite;
        }

        internal static void Postfix(AssetReference asset, ref Sprite __result)
        {
            if (asset is CEAssetReferenceSprite reference)
            {
                Main.Log($"Providing sprite {reference.Sprite.name}");

                // Return our sprite
                __result = reference.Sprite;
            }
        }
    }

    //
    // custom resources enablement patch
    //

    [HarmonyPatch(typeof(Gui), "ReleaseAddressableAsset")]
    internal static class Gui_ReleaseAddressableAsset
    {
        internal static bool Prefix(UnityEngine.Object asset)
        {
            // If it's a CE provided sprite stop it being unloaded
            bool retval = !CustomIcons.IsCachedSprite(asset as Sprite);

            if (!retval)
            {
                Main.Log($"Not releasing {asset.name}");
            }

            return retval;
        }
    }
}
