using System.Linq;
using ModKit;
using UnityEngine;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Viewers.Displays.DungeonMakerDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.GameUiDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.HotKeysDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class InterfaceViewer : IMenuSelectablePage
    {
        public string Name => "Interface";

        public int Priority => 30;

        private static int selectedPane;

        private static readonly NamedAction[] actions =
        {
            new NamedAction("Dungeon Maker", DisplayDungeonMaker),
            new NamedAction("Game UI", DisplayGameUi),
            new NamedAction("Hotkeys", DisplayHotkeys),
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
