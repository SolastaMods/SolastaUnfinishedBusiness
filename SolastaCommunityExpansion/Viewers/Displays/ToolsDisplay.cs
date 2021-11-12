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
            int intValue;
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

            intValue = Main.Settings.ExperienceModifier;
            if (UI.Slider("Multiplies the experience gained by ".white() + "[%]".red(), ref intValue, 50, 200, 100, "", UI.Width(100)))
            {
                Main.Settings.ExperienceModifier = intValue;
            }
        }
    }
}
