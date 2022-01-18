
using HarmonyLib;
using System.Reflection;

namespace SolastaCommunityExpansion.Classes.Tinkerer
{
    internal class Patches
    {
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
