using ModKit;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Viewers.Displays.ItemsAndCraftingDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.RulesDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.ToolsDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class GameplayViewer : IMenuSelectablePage
    {
        public string Name => "Gameplay";

        public int Priority => 20;

        private static int selectedPane = 0;

        private static readonly NamedAction[] actions = new NamedAction[]
        {
            new NamedAction("Rules", DisplayRules),
            new NamedAction("Items & Crafting", DisplayItemsAndCrafting),
            new NamedAction("Tools", DisplayTools),
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
    }
}
