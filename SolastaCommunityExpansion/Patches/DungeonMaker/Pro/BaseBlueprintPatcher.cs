using HarmonyLib;

namespace SolastaCommunityExpansion.Patches.DungeonMaker.Pro
{
    internal static class BaseBlueprintPatcher
    {
        // ensures custom props display the proper icon
        [HarmonyPatch(typeof(BaseBlueprint), "GetAssetKey")]
        internal static class BaseBlueprintGetAssetKey
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
                    || !(__instance is PropBlueprint propBlueprint)
                    || !propBlueprint.Name.EndsWith("MOD"))
                {
                    return true;
                }

                var a = propBlueprint.Name.Split(new[] { '~' }, 3);

                if (a.Length != 3)
                {
                    return true;
                }

                var propName = a[0];
                var environmentName = a[1];
                var str1 = "Gui/Bitmaps/Blueprints/Props/";
                var str2 = "User_Props_" + propName;
                var postfix = (perspective ? "_Pers" : "_Top");

                if (environmentDefinition != null && prefabByEnvironmentDescription.Environment == environmentDefinition.Name)
                {
                    str1 += environmentName + "/";
                    str2 += "_" + environmentName;
                }

                __result = str1 + str2 + postfix;

                return false;
            }
        }
    }
}
