using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class ToolsDisplay
    {
        // private static readonly string reqRestart = "[requires restart]".italic().red();

        private static bool enableDebugCamera = false;
        private static bool enableDebugOverlay = false;

        internal static void DisplayTools()
        {
            bool toggle;

            UI.Label("");

            toggle = Main.Settings.EnableCheatMenuDuringGameplay;
            if (UI.Toggle("Enables the cheats menu", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnableCheatMenuDuringGameplay = toggle;
            }

            if (UI.Toggle("Enables the debug camera", ref enableDebugCamera, 0, UI.AutoWidth()))
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

            if (UI.Toggle("Enables the debug overlay", ref enableDebugOverlay, 0, UI.AutoWidth()))
            {
                ServiceRepository.GetService<IDebugOverlayService>().ToggleActivation();
            }

            UI.Label("");

            toggle = Main.Settings.NoExperienceOnLevelUp;
            if (UI.Toggle("No experience is required to level up", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.NoExperienceOnLevelUp = toggle;
            }

            UI.Label("");
            UI.Label("Faction Relations");

            bool flip = true;
            var service = ServiceRepository.GetService<IGameFactionService>();
            if (service != null) {
                foreach (FactionDefinition faction in service.RegisteredFactions)
                {
                    if (faction.BuiltIn)
                    {
                        // These are things like monster factions, generally set to a specific relation and can't be changed.
                        continue;
                    }
                    if (faction.GuiPresentation.Hidden)
                    {
                        // These are things like Silent Whipsers and Church Of Einar that are not fully implemented factions.
                        continue;
                    }
                    string title = Gui.Format(faction.GuiPresentation.Title);
                    if (flip)
                    {
                        title = title.yellow();
                    } else
                    {
                        title = title.white();
                    }
                    int intValue = service.FactionRelations[faction.Name];
                    if (UI.Slider("                              " + title, ref intValue, faction.MinRelationCap, faction.MaxRelationCap, 0, "", UI.AutoWidth()))
                    {
                        SetFactionRelationsContext.SetFactionRelation(faction.Name, intValue);
                    }
                    flip = !flip;
                }                 
            } else
            {
                UI.Label("Load a game to modify faction relations".red());
            }
        }
    }
}
