using System.Diagnostics.CodeAnalysis;
using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro
{
    // ensures custom props display the proper icon
    [HarmonyPatch(typeof(BaseBlueprint), "GetAssetKey")]
    [SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Patch")]
    internal static class BaseBlueprint_GetAssetKey
    {
        internal static bool Prefix(
            BaseBlueprint __instance,
            ref string __result,
            BaseBlueprint.PrefabByEnvironmentDescription prefabByEnvironmentDescription,
            EnvironmentDefinition environmentDefinition,
            bool perspective)
        {
            if (!Main.Settings.EnableDungeonMakerPro
                || !Main.Settings.EnableDungeonMakerModdedContent
                || __instance is not PropBlueprint propBlueprint
                || !propBlueprint.Name.EndsWith("MOD"))
            {
                return true;
            }

            var a = propBlueprint.Name.Split(new[] {'~'}, 3);

            if (a.Length != 3)
            {
                return true;
            }

            var propName = a[0];
            var environmentName = a[1];
            var str1 = "Gui/Bitmaps/Blueprints/Props/";
            var str2 = "User_Props_" + propName;
            var postfix = perspective ? "_Pers" : "_Top";

            if (environmentDefinition != null &&
                prefabByEnvironmentDescription.Environment == environmentDefinition.Name)
            {
                str1 = "Gui/Bitmaps/Props-" + environmentName + "/";
                str2 = str2 + "_" + environmentName;
            }

            __result = str1 + str2 + postfix;

            return false;
        }
    }
}
