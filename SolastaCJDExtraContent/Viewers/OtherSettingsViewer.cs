using UnityModManagerNet;
using ModKit;

namespace SolastaCJDExtraContent.Menus.Viewers
{
    public class OtherSettingsViewer : IMenuSelectablePage
    {
        public string Name => "Other Settings";

        public int Priority => 1;

        public static void DisplayOtherSettings()
        {
            bool toggle;

            UI.Div();
            UI.Label("Settings:".yellow());

            UI.Label("");
            toggle = Main.Settings.EnablesAsiAndFeat;
            if (UI.Toggle("Enables both ASI and Feat instead of ASI or Feat", ref toggle, 0, UI.AutoWidth()))
            {
                Main.Settings.EnablesAsiAndFeat = toggle;
                Models.AsiAndFeatContext.Switch(toggle);
            }
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Content Expansion".yellow().bold());

            DisplayOtherSettings();
        }
    }
}