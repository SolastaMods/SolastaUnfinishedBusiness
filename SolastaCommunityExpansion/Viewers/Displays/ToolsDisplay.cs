using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class ToolsDisplay
    {
        private static bool enableDebugCamera = false;
        private static bool enableDebugOverlay = false;

        internal static void DisplayTools()
        {
            UI.Label("");

            bool toggle = Main.Settings.EnableCheatMenuDuringGameplay;
            if (UI.Toggle("Enables the cheats menu", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCheatMenuDuringGameplay = toggle;
            }

            if (UI.Toggle("Enables the debug camera", ref enableDebugCamera, UI.AutoWidth()))
            {
                IViewService viewService = ServiceRepository.GetService<IViewService>();
                ICameraService cameraService = ServiceRepository.GetService<ICameraService>();

                if (viewService == null || cameraService == null)
                {
                    enableDebugCamera = false;
                }
                else
                {
                    cameraService.DebugCameraEnabled = enableDebugCamera;
                }
            }

            if (UI.Toggle("Enables the debug overlay", ref enableDebugOverlay, UI.AutoWidth()))
            {
                ServiceRepository.GetService<IDebugOverlayService>().ToggleActivation();
            }

            UI.Label("");

            toggle = Main.Settings.NoExperienceOnLevelUp;
            if (UI.Toggle("No experience is required to level up", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.NoExperienceOnLevelUp = toggle;
            }


            int intValue = Main.Settings.ExperienceModifier;
            if (UI.Slider("Multiplies the experience gained by ".white() + "[%]".red(), ref intValue, 50, 200, 100, "", UI.Width(100)))
            {
                Main.Settings.ExperienceModifier = intValue;
            }

            UI.Label("");
            UI.Label("Faction Relations:");

            bool flip = true;
            var gameService = ServiceRepository.GetService<IGameService>();
            var gameFactionService = ServiceRepository.GetService<IGameFactionService>();

            if (gameFactionService != null && gameService?.Game?.GameCampaign?.CampaignDefinitionName?.Contains("UserCampaign") == false)
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
                    string title = Gui.Format(faction.GuiPresentation.Title);
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
                UI.Label("");
                UI.Label("Load an official campaign game to modify faction relations...".red());
            }
        }
    }
}
