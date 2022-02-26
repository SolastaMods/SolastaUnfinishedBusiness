using System.Diagnostics;
using System.Linq;
using System.Text;
using ModKit;
using SolastaCommunityExpansion.DataMiner;
using SolastaCommunityExpansion.Models;
using UnityEngine;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Viewers.Displays.BlueprintDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.CreditsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.DiagnosticsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.GameServicesDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.Level20HelpDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.PatchesDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class HelpAndCreditsViewer : IMenuSelectablePage
    {
        public string Name => "Help & Credits";

        public int Priority => 999;

        private static bool IsUnityExplorerEnabled { get; set; }

        private static int selectedPane;

        private static readonly NamedAction[] actions =
        {
            new NamedAction("Help & Credits", DisplayHelpAndCredits),
            new NamedAction("Blueprints", DisplayBlueprints),
            new NamedAction("Services", DisplayGameServices),
            new NamedAction("Patches", DisplayPatches),
        };

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (Main.Enabled || Main.Settings.EnableDataMinerStandalone)
            {
                var titles = actions.Select((a, i) => i == selectedPane ? a.name.orange().bold() : a.name).ToArray();

                UI.SelectionGrid(ref selectedPane, titles, titles.Length, UI.ExpandWidth(true));
                GUILayout.BeginVertical("box");
                actions[selectedPane].action();
                GUILayout.EndVertical();
            }
        }

        public static void DisplayHelpAndCredits()
        {
            DisplayModdingTools();
            DisplayDiagnostics();
            DisplayDumpDescription();
            DisplayLevel20Help();
            DisplayCredits();
        }

        private static void DisplayModdingTools()
        {
            UI.Label("");

            UI.ActionButton("Enable the Unity Explorer UI", () =>
            {
                if (!IsUnityExplorerEnabled)
                {
                    IsUnityExplorerEnabled = true;
                    UnityExplorer.ExplorerStandalone.CreateInstance();
                }
            }, UI.Width(200));

            UI.Label("");

            var exportTaLabel = "Export TA blueprints";
            var exportCeLabel = "Export CE blueprints";

            if (OfficialBlueprintExporter.Shared.PercentageComplete > 0)
            {
                exportTaLabel += $" {OfficialBlueprintExporter.Shared.PercentageComplete:00.00%}".yellow().bold();
            }

            if (DiagnosticsContext.HasDiagnosticsFolder && ModBlueprintExporter.Shared.PercentageComplete > 0)
            {
                exportCeLabel += $" {ModBlueprintExporter.Shared.PercentageComplete:00.00%}".yellow().bold();
            }

            using (UI.HorizontalScope())
            {
                UI.ActionButton(exportTaLabel, () => DiagnosticsContext.ExportOfficialBlueprints(), UI.Width(200));

                if (DiagnosticsContext.HasDiagnosticsFolder)
                {
                    UI.ActionButton(exportCeLabel, () => DiagnosticsContext.ExportModBlueprints(), UI.Width(200));
                }
            }
        }
    }
}
