using ModKit;

namespace SolastaCommunityExpansion.Displays
{
    internal static class DungeonMakerDisplay
    {
        internal static void DisplayDungeonMaker()
        {
            bool toggle;

            #region DungeonMaker

            UI.Label("");
            UI.Label(Gui.Format("ModUi/&Basic"));
            UI.Label("");

            UI.Label(Gui.Format("ModUi/&DungeonMakerBasicHelp"));
            UI.Label("");

            toggle = Main.Settings.AllowDungeonsMaxLevel20;
            if (UI.Toggle(Gui.Format("ModUi/&AllowDungeonsMaxLevel20"), ref toggle))
            {
                Main.Settings.AllowDungeonsMaxLevel20 = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere;
            if (UI.Toggle(Gui.Format("ModUi/&AllowGadgetsAndPropsToBePlacedAnywhere"), ref toggle))
            {
                Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.UnleashNpcAsEnemy;
            if (UI.Toggle(Gui.Format("ModUi/&UnleashNpcAsEnemy"), ref toggle))
            {
                Main.Settings.UnleashNpcAsEnemy = toggle;
            }

            toggle = Main.Settings.UnleashEnemyAsNpc;
            if (UI.Toggle(Gui.Format("ModUi/&UnleashEnemyAsNpc"), ref toggle))
            {
                Main.Settings.UnleashEnemyAsNpc = toggle;
            }

            #endregion

            UI.Label("");

            if (!Main.Settings.EnableDungeonMakerPro)
            {
                return;
            }

            UI.Label(Gui.Format("ModUi/&Advanced"));

            UI.Label("");
            UI.Label(Gui.Format("ModUi/&AdvancedHelp"));
            UI.Label("");

            toggle = Main.Settings.EnableDungeonMakerModdedContent;
            if (UI.Toggle(Gui.Format("ModUi/&EnableDungeonMakerModdedContent"), ref toggle))
            {
                Main.Settings.EnableDungeonMakerModdedContent = toggle;
            }

            UI.Label("");
            UI.Label("");
            UI.Label("");

            toggle = Main.Settings.EnableExtraHighLevelMonsters;
            if (UI.Toggle(Gui.Format("ModUi/&EnableExtraHighLevelMonsters"), ref toggle))
            {
                Main.Settings.EnableExtraHighLevelMonsters = toggle;
            }

            UI.Label("");
        }
    }
}
