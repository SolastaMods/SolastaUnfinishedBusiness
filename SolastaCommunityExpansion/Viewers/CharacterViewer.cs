using UnityModManagerNet;
using ModKit;
using static SolastaCommunityExpansion.Viewers.Displays.CharacterDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.FeatsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.SubClassesDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class CharacterViewer : IMenuSelectablePage
    {
        public string Name => "Character";

        public int Priority => 10;

        private static int selectedPane = 0;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                UI.TabBar(ref selectedPane, null, new NamedAction[]
                {
                    new NamedAction("General", DisplayCharacter),
                    new NamedAction("Feats", DisplayFeats),
                    new NamedAction("Subclasses", DisplaySubclasses),
                });
            }
        }
    }
}