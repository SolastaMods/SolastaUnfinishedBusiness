using HarmonyLib;
using SolastaCommunityExpansion;
using SolastaCommunityExpansion.Utils;
using SolastaMonsters.Models;

namespace SolastaMonsters.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    internal static class GameManagerBindPostDatabase
    {
        internal static void Postfix()
        {
            if (Main.Settings.EnableExtraHighLevelMonsters)
            {
                Translations.LoadTranslations("monsters");
                MonsterContext.Load();
            }
        }
    }
}
