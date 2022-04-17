using HarmonyLib;

namespace SolastaMonsters.Patchers
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
