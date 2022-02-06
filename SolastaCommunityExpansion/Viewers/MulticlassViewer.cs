using ModKit;
using UnityModManagerNet;

namespace SolastaCommunityExpansion.Viewers
{
    public class MulticlassViewer : IMenuSelectablePage
    {
        public string Name => "Multiclass";

        public int Priority => 30;

        private static void DisplayMulticlassHelp()
        {
            UI.Label("Features:".yellow());
            UI.Label("");
            UI.Label(". combines up to 4 different classes");
            UI.Label(". implements a shared slots system " + "[also supports Warlock pact magic integration]".yellow().italic());
            UI.Label(". enforces ability scores minimum in & out pre-requisites");
            UI.Label(". enforces a subset of starting proficiencies");
            UI.Label(". " + "extra attacks".orange() + ", " + "unarmored defenses".orange() + " and " + "channel divinity".orange() + " don't stack");
            UI.Label("");
        }

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
                return;
            }

            UI.Label(". press the " + "up".cyan() + " arrow to hide / unhide the character panel selector " + "[useful with more than 2 spell repertoires]".yellow().italic());
            UI.Label(". press the " + "down".cyan() + " arrow to switch to other classes details " + "[while in the character panel]".yellow().italic());
            UI.Label(". " + "SHIFT".cyan() + " click on a spell to consume a long rest slot " + "[default behavior consumes short level slots first]".yellow().italic());
            
            UI.Label("");

            int value = Main.Settings.MaxAllowedClasses;
            if (UI.Slider("Max allowed classes".white(), ref value, 1, 4, 3, "", UI.Width(50)))
            {
                Main.Settings.MaxAllowedClasses = value;
            }

            UI.Label("");

            if (Gui.Game == null)
            {
                toggle = Main.Settings.EnableSharedSpellCasting;
                if (UI.Toggle("Enable the shared slots system", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableSharedSpellCasting = toggle;
                    if (!toggle)
                    {
                        Main.Settings.EnableSharedSpellCasting = toggle;
                    }
                }

                if (Main.Settings.EnableSharedSpellCasting && Multiclass.Models.IntegrationContext.IsExtraContentInstalled)
                {
                    toggle = Main.Settings.EnableCombinedSpellCasting;
                    if (UI.Toggle("+ Combine the shared slots system and ".italic() + "Warlock".italic().orange() + " pact magic".italic(), ref toggle, UI.AutoWidth()))
                    {
                        Main.Settings.EnableCombinedSpellCasting = toggle;
                    }
                }
            }

            toggle = Main.Settings.EnableMinInOutAttributes;
            if (UI.Toggle("Enforce ability scores minimum in & out pre-requisites", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableMinInOutAttributes = toggle;
            }

            UI.Label("");

            toggle = Main.Settings.EnableGrantHolySymbol;
            if (UI.Toggle("Grant holy symbol to divine casters", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantHolySymbol = toggle;
            }

            toggle = Main.Settings.EnableGrantComponentPouch;
            if (UI.Toggle("Grant component pouch to arcane casters", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantComponentPouch = toggle;
            }

            toggle = Main.Settings.EnableGrantDruidicFocus;
            if (UI.Toggle("Grant druidic focus to " + "Druid".orange(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantDruidicFocus = toggle;
            }

            toggle = Main.Settings.EnableGrantCLothesWizard;
            if (UI.Toggle("Grant clothes to " + "Wizard".orange(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableGrantCLothesWizard = toggle;
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
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            if (Main.Enabled)
            {
                DisplayMainSettings();
                DisplayMulticlassHelp();
            }
        }
    }
}
