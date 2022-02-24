using ModKit;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class DungeonMakerDisplay
    {
        internal static void DisplayDungeonMaker()
        {
            bool toggle;

            #region DungeonMaker
            UI.Label("");
            UI.Label(". These 5 settings are safe to use as they don't require a player to have this mod installed");
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
            if (UI.Toggle("Unleash NPCs as enemies " + "[press ".italic().yellow() + "CTRL".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
            {
                Main.Settings.UnleashNpcAsEnemy = toggle;
            }

            toggle = Main.Settings.UnleashEnemyAsNpc;
            if (UI.Toggle("Unleash enemies as NPCs " + "[press ".italic().yellow() + "CTRL".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
            {
                Main.Settings.UnleashEnemyAsNpc = toggle;
            }
            #endregion

            UI.Label("");

            if (!Main.Settings.EnableDungeonMakerPro)
            {
                return;
            }

            UI.Label("ATTENTION:".bold().yellow());
            UI.Label(". Any modded content used on a location will force the player to install this mod");
            UI.Label(". Can be easily identified in the editor as asset labels are " + "yellow".yellow());
            UI.Label(". Include flat rooms, 150x150 & 200x200 dungeon sizes and no frills mixing assets from all environments");
            UI.Label(". You must have at least one outdoor room if you pick an outdoor environment");
            UI.Label("");

            toggle = Main.Settings.EnableDungeonMakerModdedContent;
            if (UI.Toggle("Enable Dungeon Maker Pro " + "[MODDED CONTENT] ".italic().yellow() + RequiresRestart, ref toggle))
            {
                Main.Settings.EnableDungeonMakerModdedContent = toggle;
            }

            UI.Label("");
        }
    }
}
