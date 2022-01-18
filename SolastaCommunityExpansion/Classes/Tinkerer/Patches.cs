
using HarmonyLib;
using System.Reflection;

namespace SolastaArtificerMod
{
    internal class Patches
    {
        // TODO verify if this is still needed.
        [HarmonyPatch(typeof(CharacterBuildingManager), "LevelUpCharacter")]
        internal static class CharacterBuildingManager_LevelUpClearActiveFeatures
        {
            internal static void Postfix(CharacterBuildingManager __instance)
            {
                MethodInfo dynMethod = __instance.GetType().GetMethod("RefreshAllActiveFeatures",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                dynMethod.Invoke(__instance, new object[] { });
            }
        }
    }
}
