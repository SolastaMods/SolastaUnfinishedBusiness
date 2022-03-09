using HarmonyLib;

namespace SolastaCommunityExpansion.Multiclass.Patches.LevelUp
{
    internal static class CharacterLevelUpScreenPatcher
    {
        // binds the hero
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "OnBeginShow")]
        internal static class CharacterLevelUpScreenOnBeginShow
        {
            internal static void Postfix(CharacterLevelUpScreen __instance)
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.LevelUpContext.SelectedHero = __instance.CharacterBuildingService.CurrentLocalHeroCharacter;
            }
        }

        // unbinds the hero
        [HarmonyPatch(typeof(CharacterLevelUpScreen), "OnBeginHide")]
        internal static class CharacterLevelUpScreenOnBeginHide
        {
            internal static void Postfix()
            {
                if (!Main.Settings.EnableMulticlass)
                {
                    return;
                }

                Models.LevelUpContext.SelectedHero = null;
            }
        }
    }
}
