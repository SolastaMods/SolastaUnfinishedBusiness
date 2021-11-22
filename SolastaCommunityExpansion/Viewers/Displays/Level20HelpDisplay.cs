using ModKit;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class Level20HelpDisplay
    {
        private static readonly string PLANNED = " [" + "planned".cyan() + "] ";

        private const float DEFAULT_WIDTH = 250;

        private static bool level20HelpToggle = false;

        private static void DisplayBarbarian(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Barbarian".green());
                UI.Label("13 - Brutal Critical (2 dices)");
                UI.Label("15 - Persistent Rage");
                UI.Label("16 - Ability score or feat");
                UI.Label("17 - Brutal Critical (3 dices)");
                UI.Label("18 - Indomitable Might" + PLANNED);
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Primal Champion");
                UI.Label("");
            }
        }

        private static void DisplayCleric(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Cleric".green());
                UI.Label("14 - Turn Undead");
                UI.Label("16 - Ability score or feat");
                UI.Label("17 - Turn Undead");
                UI.Label("18 - Channel Divinity");
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Divine Intervention Improvement");
                UI.Label("");
            }
        }

        private static void DisplayDruid(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Druid".green());
                UI.Label("16 - Ability score or feat");
                UI.Label("18 - Beast Spells" + PLANNED);
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Archdruid" + PLANNED);
                UI.Label("");
            }
        }

        private static void DisplayFighter(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Fighter".green());
                UI.Label("13 - Indomitable");
                UI.Label("14 - Ability score or feat");
                UI.Label("16 - Ability score or feat");
                UI.Label("17 - Action Surge / Indomitable");
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Extra Attack");
                UI.Label("");
            }
        }

        private static void DisplayPaladin(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Paladin".green());
                UI.Label("14 - Cleansing Touch");
                UI.Label("16 - Ability score or feat");
                UI.Label("18 - Aura of Courage / Aura of Protection");
                UI.Label("19 - Ability score or feat");
                UI.Label("");
            }
        }

        private static void DisplayRanger(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Ranger".green());
                UI.Label("14 - Favored Enemy");
                UI.Label("16 - Ability score or feat");
                UI.Label("18 - Feral Senses" + PLANNED);
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Foe Slayer" + PLANNED);
                UI.Label("");
            }
        }

        private static void DisplayRogue(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Rogue".green());
                UI.Label("14 - Blind Sense");
                UI.Label("15 - Slippery Mind");
                UI.Label("16 - Ability score or feat");
                UI.Label("18 - Elusive" + PLANNED);
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Stroke of Luck" + PLANNED);
                UI.Label("");
            }
        }

        private static void DisplaySorcerer(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Sorcerer".green());
                UI.Label("16 - Ability score or feat");
                UI.Label("17 - Metamagic");
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Sorcerous Restoration");
                UI.Label("");
            }
        }

        private static void DisplayWizard(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Wizard".green());
                UI.Label("16 - Ability score or feat");
                UI.Label("18 - Spell Mastery" + PLANNED);
                UI.Label("19 - Ability score or feat");
                UI.Label("20 - Signature Spells" + PLANNED);
                UI.Label("");
            }
        }

        internal static void DisplayLevel20Help()
        {
            UI.Label("");

            UI.DisclosureToggle("Level 20".orange().bold(), ref level20HelpToggle);
            if (level20HelpToggle)
            {
                UI.Label("");

                using (UI.HorizontalScope())
                {
                    DisplayBarbarian();
                    DisplayCleric(300);
                    DisplayDruid();
                }
                using (UI.HorizontalScope())
                {
                    DisplayFighter();
                    DisplayPaladin(300);
                    DisplayRanger();
                }
                using (UI.HorizontalScope())
                {
                    DisplayRogue();
                    DisplaySorcerer(300);
                    DisplayWizard();
                }
            }
            UI.Label("");
        }
    }
}
