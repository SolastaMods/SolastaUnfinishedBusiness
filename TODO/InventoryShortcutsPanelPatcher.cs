//using HarmonyLib;

//namespace SolastaDungeonMakerPro.Patches.EncountersSpawn
//{
//    // this patch allows the away party to fully utilize the shortcuts during their turn
//    class InventoryShortcutsPanelPatcher
//    {
//        [HarmonyPatch(typeof(InventoryShortcutsPanel), "Refresh")]
//        internal static class InventoryShortcutsPanelRefresh
//        {
//            internal static bool flip = false;

//            internal static void Prefix(InventoryShortcutsPanel __instance)
//            {
//                if (__instance.GuiCharacter.RulesetCharacter is RulesetCharacterHero && 
//                    __instance.GuiCharacter.RulesetCharacterHero.Side == RuleDefinitions.Side.Enemy && 
//                    Main.Settings.GameMasterMode)
//                {
//                    flip = true;
//                    __instance.GuiCharacter.RulesetCharacterHero.ChangeSide(RuleDefinitions.Side.Ally);
//                }
//            }

//            internal static void Postfix(InventoryShortcutsPanel __instance)
//            {
//                if (flip)
//                {
//                    flip = false;
//                    __instance.GuiCharacter.RulesetCharacterHero.ChangeSide(RuleDefinitions.Side.Enemy);
//                }
//            }
//        }
//    }
//}