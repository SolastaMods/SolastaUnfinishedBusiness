using ModKit;
using System.Linq;
using UnityEngine;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Viewers.Displays.CharacterDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.FeatsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.FightingStylesDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.SubClassesDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class CharacterViewer : IMenuSelectablePage
    {
        public string Name => "Character";

        public int Priority => 10;

        private static int selectedPane;

        private static readonly NamedAction[] actions = 
        {
            new NamedAction("General", DisplayCharacter),
            new NamedAction("Feats", DisplayFeats),
            new NamedAction("Subclasses", DisplaySubclasses),
            new NamedAction("Fighting Styles", DisplayFightingStyles),
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
