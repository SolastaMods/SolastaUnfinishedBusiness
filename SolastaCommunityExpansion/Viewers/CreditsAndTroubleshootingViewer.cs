using System.Linq;
using ModKit;
using UnityEngine;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Viewers.Displays.BlueprintDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.CreditsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.DiagnosticsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.GameServicesDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.PatchesDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class CreditsAndTroubleshootingViewer : IMenuSelectablePage
    {
        public string Name => "Credits & Troubleshooting";

        public int Priority => 999;

        private static bool displayPatches;

        private static int selectedPane;

        private static readonly NamedAction[] actions =
        {
            new NamedAction("Credits", DisplayCredits),
            new NamedAction("Diagnostics", DisplayDiagnostics),
            new NamedAction("Blueprints", DisplayBlueprints),
            new NamedAction("Services", DisplayGameServices),
        };

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                var titles = actions.Select((a, i) => i == selectedPane ? a.name.orange().bold() : a.name).ToArray();

                UI.SelectionGrid(ref selectedPane, titles, titles.Length, UI.ExpandWidth(true));
                GUILayout.BeginVertical("box");
                actions[selectedPane].action();
                GUILayout.EndVertical();
            }
        }

        public static void DisplayDiagnostics()
        {
            DisplayModdingTools();
            UI.DisclosureToggle("Patches:".yellow(), ref displayPatches, 200);
            UI.Label("");

            if (displayPatches)
            {
                DisplayPatches();
            }
        }
    }
}
