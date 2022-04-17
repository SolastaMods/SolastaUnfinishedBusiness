using ModKit;
using static SolastaCommunityExpansion.Displays.Shared;

namespace SolastaCommunityExpansion.Displays
{
    internal static class DungeonMakerDisplay
    {
        internal static void DisplayDungeonMaker()
        {
            bool toggle;

            #region DungeonMaker
            UI.Label("");
            UI.Label("Basic:".yellow());
            UI.Label("");

            UI.Label(". These 4 settings won't require the player to have this mod installed");
            UI.Label("");

            toggle = Main.Settings.AllowDungeonsMaxLevel20;
            if (UI.Toggle("Allow dungeons with max level 20 " + RequiresRestart, ref toggle))
            {
                Main.Settings.AllowDungeonsMaxLevel20 = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere;
            if (UI.Toggle("Allow gadgets and props to be placed anywhere on the map " + "[press ".italic().yellow() + "CTRL".italic().cyan() + " during placement to bypass any check]".italic().yellow(), ref toggle))
            {
                Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.UnleashNpcAsEnemy;
            if (UI.Toggle("Allow NPCs to be selected on monster gadgets " + "[press ".italic().yellow() + "CTRL".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
            {
                Main.Settings.UnleashNpcAsEnemy = toggle;
            }

            toggle = Main.Settings.UnleashEnemyAsNpc;
            if (UI.Toggle("Allow Monsters to be selected on NPC gadgets " + "[press ".italic().yellow() + "CTRL".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
            {
                Main.Settings.UnleashEnemyAsNpc = toggle;
            }
            #endregion

            UI.Label("");

            if (!Main.Settings.EnableDungeonMakerPro)
            {
                return;
            }

            UI.Label("Advanced:".yellow());

            UI.Label("");
            UI.Label(". ATTENTION:".bold().red() + " This setting will require the player to have this mod installed");
            UI.Label("");

            toggle = Main.Settings.EnableDungeonMakerModdedContent;
            if (UI.Toggle("Enable Dungeon Maker Pro " 
                + RequiresRestart
                + "\n\ninclude flat rooms, 150x150 & 200x200 dungeon sizes and no frills mixing assets from all environments"
                + "\nyou must have at least one outdoor room if you pick an outdoor environment", ref toggle))
            {
                Main.Settings.EnableDungeonMakerModdedContent = toggle;
            }

            UI.Label("");
            UI.Label("");
            UI.Label("");

            toggle = Main.Settings.EnableExtraHighLevelMonsters;
            if (UI.Toggle("Enable additional high level monsters (+20 CR) for tier 3 and 4 campaigns " + RequiresRestart, ref toggle))
            {
                Main.Settings.EnableExtraHighLevelMonsters = toggle;
            }

            UI.Label("");
        }
    }
}
