using HarmonyLib;

namespace SolastaMonsters.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    internal static class GameManagerBindPostDatabase
    {
        internal static void Postfix()
        {
            Models.MonsterContext.Load();
        }
    }
}
