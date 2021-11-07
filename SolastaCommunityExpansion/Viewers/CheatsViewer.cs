using UnityModManagerNet;
using ModKit;

namespace SolastaCommunityExpansion.Viewers
{
    public class CheatsViewer : IMenuSelectablePage
    {
        public string Name => "Cheats";

        public int Priority => 5;

        private static readonly string reqRestart = "[requires restart to disable]".italic().red();


        public void DisplayCheats()
        {
            int intValue = Main.Settings.RecipeCost;

            UI.Label("");
            UI.Label("Cheats:".yellow());

            UI.Label("");

            //
            // WILL START ADDING CHEATS ON ANOTHER BRANCH
            //

            //bool toggle = Main.Settings.NoIdentification;
            //if (UI.Toggle("blah blah blah " + reqRestart, ref toggle, 0, UI.AutoWidth()))
            //{
            //    Main.Settings.NoIdentification = toggle;
            //    Models.RemoveIdentificationContext.Load();
            //}
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (!Main.Enabled) return;

            DisplayCheats();
        }
    }
}