using HarmonyLib;
using SolastaCommunityExpansion.Utils;
using SolastaMonsters.Models;

namespace SolastaMonsters.Patches
{
    [HarmonyPatch(typeof(GameManager), "BindPostDatabase")]
    internal static class GameManagerBindPostDatabase
    {
        internal static void Postfix()
        {
            if (SolastaCommunityExpansion.Main.Settings.EnableExtraHighLevelMonsters)
            {
                Translations.LoadTranslations("monsters");
                MonsterContext.Load();
            }
        }
    }
}
