using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Displays
{
    internal static class ToolsDisplay
    {
        internal static void SetFactionRelation(string name, int value)
        {
            var service = ServiceRepository.GetService<IGameFactionService>();
            if (service != null)
            {
                service.ExecuteFactionOperation(name, FactionDefinition.FactionOperation.Increase, value - service.FactionRelations[name], "", null /* this string and monster doesn't matter if we're using "SetValue" */);
            }
        }

        internal static void DisplayTools()
        {
            bool toggle;
            int intValue;

            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableSaveByLocation;
            if (UI.Toggle("Enable save by campaigns / locations", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableSaveByLocation = toggle;
            }

            toggle = Main.Settings.EnableCharacterChecker;
            if (UI.Toggle("Enable the character checker button on the character pool", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCharacterChecker = toggle;
            }

            toggle = Main.Settings.EnableRespec;
            if (UI.Toggle("Enable the RESPEC and Level Down after rest actions", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableRespec = toggle;
                RespecContext.Switch();
            }

            toggle = Main.Settings.EnableCheatMenu;
            if (UI.Toggle("Enable the cheats menu", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCheatMenu = toggle;
            }

            toggle = Main.Settings.NoExperienceOnLevelUp;
            if (UI.Toggle("No experience is required to Level Up", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.NoExperienceOnLevelUp = toggle;
            }

            UI.Label("");

            intValue = Main.Settings.MaxBackupFilesPerLocationCampaign;
            if (UI.Slider("Max. backup files per location or campaign".white(), ref intValue, 0, 20, 10))
            {
                Main.Settings.MaxBackupFilesPerLocationCampaign = intValue;
            }

            UI.Label("");
            UI.Label(". Backup files are saved under " + "GAME_FOLDER/Mods/SolastaCommunityExpansion/DungeonMakerBackups".italic().yellow());

            UI.Label("");
            UI.Label("Faction Relations:".yellow());
            UI.Label("");

            var flip = true;
            var gameCampaign = Gui.GameCampaign;
            var gameFactionService = ServiceRepository.GetService<IGameFactionService>();

            // NOTE: don't use gameCampaign?. which bypasses Unity object lifetime check
            if (gameFactionService != null && gameCampaign != null && gameCampaign.CampaignDefinitionName != "UserCampaign")
            {
                foreach (var faction in gameFactionService.RegisteredFactions)
                {
                    if (faction.BuiltIn)
                    {
                        // These are things like monster factions, generally set to a specific relation and can't be changed.
                        continue;
                    }

                    if (faction.GuiPresentation.Hidden)
                    {
                        // These are things like Silent Whispers and Church Of Einar that are not fully implemented factions.
                        continue;
                    }

                    var title = faction.FormatTitle();

                    if (flip)
                    {
                        title = title.yellow();
                    }
                    else
                    {
                        title = title.white();
                    }

                    intValue = gameFactionService.FactionRelations[faction.Name];

                    if (UI.Slider("                              " + title, ref intValue, faction.MinRelationCap, faction.MaxRelationCap, 0, "", UI.AutoWidth()))
                    {
                        SetFactionRelation(faction.Name, intValue);
                    }

                    flip = !flip;
                }
            }
            else
            {
                UI.Label("Load an official campaign game to modify faction relations...".red());
            }

            UI.Label("");
        }
    }
}
