using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class ToolsDisplay
    {
        private static bool IsUnityExplorerEnabled { get; set; }

        internal static void DisplayTools()
        {
            bool toggle;
            int intValue;

            UI.Label("");
            UI.Label("Debug:".yellow());
            UI.Label("");

            UI.ActionButton("Enable Unity Explorer UI", () =>
            {
                if (!IsUnityExplorerEnabled)
                {
                    IsUnityExplorerEnabled = true;
                    UnityExplorer.ExplorerStandalone.CreateInstance();
                }
            });

            UI.Label("");

            toggle = Main.Settings.EnableCheatMenu;
            if (UI.Toggle("Enable the cheats menu", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCheatMenu = toggle;
            }

            toggle = Main.Settings.NoExperienceOnLevelUp;
            if (UI.Toggle("No experience is required to level up", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.NoExperienceOnLevelUp = toggle;
            }

            UI.Label("");
            UI.Label("Faction Relations:".yellow());
            UI.Label("");

            bool flip = true;
            var gameCampaign = Gui.GameCampaign;
            var gameFactionService = ServiceRepository.GetService<IGameFactionService>();

            if (gameFactionService != null && gameCampaign?.CampaignDefinitionName != "UserCampaign")
            {
                foreach (FactionDefinition faction in gameFactionService.RegisteredFactions)
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

                    string title = faction.FormatTitle();

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
                        SetFactionRelationsContext.SetFactionRelation(faction.Name, intValue);
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
