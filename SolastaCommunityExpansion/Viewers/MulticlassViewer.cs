using ModKit;
using UnityModManagerNet;

namespace SolastaCommunityExpansion.Viewers
{
    public class MulticlassViewer : IMenuSelectablePage
    {
        public string Name => "Multiclass";

        public int Priority => 30;

        private static void DisplayMainSettings()
        {
            bool toggle;

            UI.Label("");

            toggle = Main.Settings.EnableMulticlass;
            if (UI.Toggle("Enable Multiclass " + "[requires restart]".italic().red(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMulticlass = toggle;
            }

            UI.Label("");

            if (!Main.Settings.EnableMulticlass)
            {
                UI.Label(". combines up to 4 different classes");
                UI.Label(". implements the shared slots system");
                UI.Label(". enforces ability scores minimum in & out pre-requisites");
                UI.Label(". enforces the correct subset of starting proficiencies");
                UI.Label(". enforces non-stacking extra attacks and channel divinity");
                UI.Label("");

                return;
            }

            UI.Label(". press the " + "up".cyan() + " arrow to hide / unhide the character panel selector " + "[useful with more than 2 spell repertoires]".yellow().italic());
            UI.Label(". press the " + "down".cyan() + " arrow to browse other classes details");
            UI.Label(". " + "SHIFT".cyan() + " click on a spell consumes a long rest slot instead of default short rest");

            UI.Label("");

            int value = Main.Settings.MaxAllowedClasses;
            if (UI.Slider("Max allowed classes".white(), ref value, 1, 4, 3, "", UI.Width(50)))
            {
                Main.Settings.MaxAllowedClasses = value;
            }

            UI.Label("");

            toggle = Main.Settings.EnableMinInOutAttributes;
            if (UI.Toggle("Enforce ability scores minimum in & out pre-requisites", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMinInOutAttributes = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableRelearnSpells;
            if (UI.Toggle("Cantrips or spells can be re-learned by another class", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableRelearnSpells = toggle;
            }

            toggle = Main.Settings.EnableDisplayAllKnownSpellsOnLevelUp;
            if (UI.Toggle("Display known spells from all classes during level up", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableDisplayAllKnownSpellsOnLevelUp = toggle;
            }

            UI.Label("");


            toggle = Main.Settings.EnableLevelDown;
            if (UI.Toggle("Enable the Level Down after rest action", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableLevelDown = toggle;
            }

            UI.Label("");
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion - Multiclass".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                if (Main.IsMulticlassInstalled)
                {
                    DisplayMainSettings();
                }
                else
                {
                    UI.Label("");
                    UI.Label(". Multiclass DLL cannot be found in the SolastaCommunityExpansion mod folder");
                    UI.Label("");
                }

            }
        }
    }
}
