using ModKit;

namespace SolastaCommunityExpansion.Displays;

internal static class DungeonMakerDisplay
{
    internal static void DisplayDungeonMaker()
    {
        bool toggle;

        #region DungeonMaker

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&Basic"));
        UI.Label("");

        UI.Label(Gui.Localize("ModUi/&DungeonMakerBasicHelp"));
        UI.Label("");

        toggle = Main.Settings.AllowDungeonsMaxLevel20;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowDungeonsMaxLevel20"), ref toggle))
        {
            Main.Settings.AllowDungeonsMaxLevel20 = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowGadgetsAndPropsToBePlacedAnywhere"), ref toggle))
        {
            Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere = toggle;
        }

        #endregion

        UI.Label("");

        if (!Main.Settings.EnableDungeonMakerPro)
        {
            return;
        }

        UI.Label(Gui.Localize("ModUi/&Advanced"));

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&AdvancedHelp"));
        UI.Label("");

        toggle = Main.Settings.UnleashNpcAsEnemy;
        if (UI.Toggle(Gui.Localize("ModUi/&UnleashNpcAsEnemy"), ref toggle))
        {
            Main.Settings.UnleashNpcAsEnemy = toggle;
        }

        toggle = Main.Settings.UnleashEnemyAsNpc;
        if (UI.Toggle(Gui.Localize("ModUi/&UnleashEnemyAsNpc"), ref toggle))
        {
            Main.Settings.UnleashEnemyAsNpc = toggle;
        }

        UI.Label("");

        toggle = Main.Settings.EnableDungeonMakerModdedContent;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDungeonMakerModdedContent"), ref toggle))
        {
            Main.Settings.EnableDungeonMakerModdedContent = toggle;
        }

        UI.Label("");
        UI.Label("");
        UI.Label("");

        // toggle = Main.Settings.EnableExtraHighLevelMonsters;
        // if (UI.Toggle(Gui.Localize("ModUi/&EnableExtraHighLevelMonsters"), ref toggle))
        // {
        //     Main.Settings.EnableExtraHighLevelMonsters = toggle;
        // }

        // UI.Label("");
    }
}
