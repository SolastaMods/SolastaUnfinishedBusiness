//using HarmonyLib;

//namespace SolastaDungeonMakerPro.Patches.EncountersSpawn
//{
//    // this patch allows the away party to use the inventory inspect button
//    class CharacterControlPanelPatcher
//    {
//        [HarmonyPatch(typeof(CharacterControlPanel), "OnInspectCb")]
//        internal static class CharacterControlPanelOnInspectCb
//        {
//            private static RulesetCharacterHero hero = null;

//            internal static void Prefix(CharacterControlPanel __instance)
//            {
//                if (Main.Settings.GameMasterMode &&
//                    __instance?.GuiCharacter?.RulesetCharacter is RulesetCharacterHero && 
//                    __instance.GuiCharacter.RulesetCharacterHero.Side == RuleDefinitions.Side.Enemy)
//                {
//                    hero = __instance.GuiCharacter.RulesetCharacterHero;
//                    hero.ChangeSide(RuleDefinitions.Side.Ally);
//                }
//            }

//            internal static void Postfix(CharacterControlPanel __instance)
//            {
//                if (hero != null)
//                {
//                    hero.ChangeSide(RuleDefinitions.Side.Enemy);
//                    hero = null;
//                }
//            }
//        }
//    }
//}