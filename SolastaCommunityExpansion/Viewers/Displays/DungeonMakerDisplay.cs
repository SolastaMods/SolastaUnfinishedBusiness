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
            UI.Label(". These 4 settings are safe to use as they don't require a player to have this mod installed");
            UI.Label("");

            toggle = Main.Settings.AllowGadgetsToBePlacedAnywhere;
            if (UI.Toggle("Allow gadgets to be placed anywhere on the map " + RequiresRestart, ref toggle))
            {
                Main.Settings.AllowGadgetsToBePlacedAnywhere = toggle;
            }

            toggle = Main.Settings.AllowPropsToBePlacedAnywhere;
            if (UI.Toggle("Allow props to be placed anywhere on the map " + RequiresRestart, ref toggle))
            {
                Main.Settings.AllowPropsToBePlacedAnywhere = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.UnleashNpcAsEnemy;
            if (UI.Toggle("Unleash NPCs as enemies " + "[press ".italic().yellow() + "SHIFT".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
            {
                Main.Settings.UnleashNpcAsEnemy = toggle;
            }

            toggle = Main.Settings.UnleashEnemyAsNpc;
            if (UI.Toggle("Unleash enemies as NPCs " + "[press ".italic().yellow() + "SHIFT".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
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
