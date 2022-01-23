using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using HarmonyLib;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    // TODO verify if this is still needed. It's original goal was to help filter the Infusion selection lists during level up properly.
    [HarmonyPatch(typeof(CharacterBuildingManager), "LevelUpCharacter")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class CharacterBuildingManager_LevelUpClearActiveFeatures
    {
        internal static void Postfix(CharacterBuildingManager __instance)
        {
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            MethodInfo dynMethod = __instance.GetType().GetMethod("RefreshAllActiveFeatures",
                BindingFlags.NonPublic | BindingFlags.Instance);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
            dynMethod.Invoke(__instance, Array.Empty<object>());
        }
    }
}
