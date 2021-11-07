using UnityModManagerNet;
using ModKit;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Viewers
{
    public class HelpAndCreditsViewer : IMenuSelectablePage
    {
        public string Name => "Help & Credits";

        public int Priority => 10;

        private static Dictionary<string, string> creditsTable = new Dictionary<string, string>
        {
            { "Zappastuff", "mod UI work, integration, community organization, level 20, respec" },
            { "ImpPhil", "monster's health, pause UI, stocks prices, no attunement" },
            { "DubhHerder", "crafty feats migration" },
            { "View619", "darkvision, superior dark vision" },
            { "SilverGriffon", "pickpocket, lore friendly names, crafty feats" },
            { "Boofat", "alwaysAlt" },
            { "Myztikrice", "faster time scale" },
            { "AceHigh", "power attack, reckless fury, no identification" },
            { "Narria", "ModKit creator, developer" }
        };

        private static bool level20HelpToggle;

        public void DisplayCredits()
        {
            UI.Div();
            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");
            using (UI.HorizontalScope())
            {
                UI.Label("ChrisJohnDigital".orange().bold(), UI.Width(110));
                UI.Label("head developer, feats, items, subclasses, progression, etc.", UI.Width(400));
            }
            foreach (var kvp in creditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(kvp.Key.orange(), UI.Width(110));
                    UI.Label(kvp.Value, UI.Width(400));
                }
            }
            UI.Label("");
        }

        //
        // Level 20 Help
        //

        private static readonly string PLANNED = " [" + "planned".cyan() + "] ";

        private const float DEFAULT_WIDTH = 250;

        private static void DisplayBarbarian(float width = DEFAULT_WIDTH)
        {
            using (UI.VerticalScope(UI.Width(width)))
            {
                UI.Label("Barbarian".green());
                UI.Label("13 - Brutal Critical (2 dices)");
                UI.Label("15 - Persistent Rage" + PLANNED);
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

        private static void DisplayLevel20Help()
        {
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

        public void DisplayHelp()
        {
            UI.Label("");
            UI.Label("Help: ".yellow() + "[click on triangles to expand entries]".red().italic().bold(), UI.AutoWidth());
            UI.Label("");

            DisplayLevel20Help();
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            DisplayHelp();
            DisplayCredits();

            //AddDumpDescriptionToLogButton();
        }

        private void AddDumpDescriptionToLogButton()
        {
            UI.ActionButton("Dump Description to Logs", () => {
                string collectedString = "";
                collectedString += "[heading][size=5] [b] [i] Solasta Community Expansion[/i][/b][/size][/heading]";
                collectedString += "\nThis is a collection of work from the Solasta modding community. It includes feats, subclasses, items, crafting recipes, gameplay options, UI improvements, and more. The general philosophy is everything is optional to enable, so you can install the mod and then enalbe the pieces you want. There are some minor bug fixes that are enabled by default.";
                collectedString += "\n\n[b] ATTENTION:[/b] This mod is a collection of previously released mods in addition to some new components. If any of the mods this is replacing is still installed, you will have errors on startup. It is highly suggested to delete all mods from [b]GAME_FOLDER\\Mods[/b] and add the ones you need. No previous mod from the list at the end of this document should be installed unless the author specifically says it is supported. [b]ChrisJohnDigital[/b], [b]ImpPhil[/b] and [b]Zappastuff[/b] put many hours consolidating all previous work to offer the best we created over the last year in a simple set of 4 basic mods:";
                collectedString += "\n\n[list=1]";
                collectedString += "\n[*] [b]Solasta Mod API[/b] - Provides the basis for all other mods to work";
                collectedString += "\n[*] [b]Solasta Community Expansion[/b] - About 40 mods from the community were consolidated here. 40 Feats, 6 Subclasses, Bug Fixes, etc.";
                collectedString += "\n[*] [b]Solasta Dungeon Maker PRO [Multiplayer][/b] - Offers multiplayer with up to 4 users, additional design options for Dungeon Creators, Lua Scripting, etc.";
                collectedString += "\n[*] [b]Solasta Unfinished Business [Multiclass][/b] - Brings SRD official multiclassing rules into Solasta";
                collectedString += "\n[/list]";
                collectedString += "\n[heading] How to Report Bugs[/heading]";
                collectedString += "\n[list]";
                collectedString += "\n[*] The versions of Solasta, the Solasta Mod API, and Solasta Community Expansion.";
                collectedString += "\n[*] A list of other mods you have installed.";
                collectedString += "\n[*] A short description of the bug";
                collectedString += "\n[*] A step-by-step procedure to reproduce it";
                collectedString += "\n[*] The save, character and log files";
                collectedString += "\n[/list]";
                collectedString += "\n[heading][size=5]Features[/size][/heading]";
                collectedString += "\n[heading]Character Creation Options[/heading]\n[list]";
                collectedString += "\n[*]Epic [17,15,13,12,10,8] array";
                collectedString += "\n[*]Alternate Human [+1 feat / +2 attribute choices / +1 skill]";
                collectedString += "\n[*]Feats available at level 1";
                collectedString += "\n[*]Flexible backgrounds [Select skill and tool proficiencies from backgrounds]";
                collectedString += "\n[*]Flexible races [Assign ability score points instead of the racial defaults (High Elf has 3 points to assign instead of +2 Dex/+1 Int)]";
                collectedString += "\n[*]Remove darkvision";
                collectedString += "\n[*]Additional lore friendly names";
                collectedString += "\n[/list]";
                collectedString += "\n[line]\n";
                collectedString += "[heading]UI Improvements and bug Fixes[/heading]\n[list]";
                collectedString += "\n[*]Invert ALT behavior on tooltips";
                collectedString += "\n[*]Monsters's health in steps of 25/50/75/100%";
                collectedString += "\n[*]Pause the UI when victorious in battle";
                collectedString += "\n[*]Scale merchant prices correctly/exactly";
                collectedString += "\n[*]Disable auto-equip";
                collectedString += "\n[*]Speed up battles";
                collectedString += "\n[*]Multi line spell casting selection";
                collectedString += "\n[*]Multi line power activation selection";
                collectedString += "\n[*]Keep spell UI open when switching weapons";
                collectedString += "\n[/list]";
                collectedString += "\n[line]\n";
                collectedString += "[heading]Other Options[/heading]\n[list]";
                collectedString += "\n[*]Receive both ASI and Feat every 4 levels";
                collectedString += "\n[*]Add pickpocketable loot";
                collectedString += "\n[*]Remove identification requirement from items";
                collectedString += "\n[*]Remove attunement requirement from items";
                collectedString += "\n[/list]";
                collectedString += "\n[line]\n";
                collectedString += Models.SubclassesContext.GenerateSubclassDescription();
                collectedString += "\n[line]\n";
                collectedString += Models.FeatsContext.GenerateFeatsDescription();
                collectedString += "\n[line]\n";
                collectedString += Models.ItemCraftingContext.GenerateItemsDescription();
                collectedString += "\n[line]\n";

                collectedString += "[heading]Credits[/heading]\n[list]";
                collectedString += "\n[*]Chris John Digital";
                foreach (var kvp in creditsTable)
                {
                    collectedString += "\n[*]" + kvp.Key + ": " + kvp.Value;
                }
                collectedString += "\n[/list]";
                collectedString += "\nSource code on [url=https://github.com/ChrisPJohn/SolastaCommunityExpansion]GitHub[/url].";
                collectedString += "\n[heading]DEPRECATED MODS LIST[/heading]";
                collectedString += "\n[list]";
                collectedString += "\n[*]Alternate Human";
                collectedString += "\n[*]AlwaysAlt - Auto expand tooltips";
                collectedString += "\n[*]Armor Feats";
                collectedString += "\n[*]ASI and Feat";
                collectedString += "\n[*]Caster Feats -Telekinetic - Fey Teleportation - Shadow Touched";
                collectedString += "\n[*]Character Export [to-be imported by @impPhil]";
                collectedString += "\n[*]Crafty Feats";
                collectedString += "\n[*]Custom Merchants";
                collectedString += "\n[*]Darkvision";
                collectedString += "\n[*]Data Viewer";
                collectedString += "\n[*]Druid Class by DubhHerder";
                collectedString += "\n[*]Dungeon Maker Merchants";
                collectedString += "\n[*]Enchanting Crafting Ingredients";
                collectedString += "\n[*]Enhanced Vision";
                collectedString += "\n[*]Faster Time Scale";
                collectedString += "\n[*]Feats - Savage Attacker - Tough - War Caster";
                collectedString += "\n[*]Fighter Spell Shield";
                collectedString += "\n[*]Fighting Style Feats";
                collectedString += "\n[*]Flexible Ancestries";
                collectedString += "\n[*]Flexible Backgrounds";
                collectedString += "\n[*]Healing Feats -Inspiring Leader - Chef - Healer";
                collectedString += "\n[*]Hot Seat Multiplayer Dungeon Master Mode";
                collectedString += "\n[*]Identify all items";
                collectedString += "\n[*]Level 1 Feat All Races";
                collectedString += "\n[*]Level 20";
                collectedString += "\n[*]Magic Crossbows";
                collectedString += "\n[*]More Magic Items";
                collectedString += "\n[*]Multiclass";
                collectedString += "\n[*]No Level Constraint";
                collectedString += "\n[*]Primed Recipes";
                collectedString += "\n[*]Respec";
                collectedString += "\n[*]Rogue Con Artist";
                collectedString += "\n[*]Save by Location [to-be imported by @impPhil]";
                collectedString += "\n[*]Skip Tutorials";
                collectedString += "\n[*]Solastanomicon";
                collectedString += "\n[*]Telema Campaign";
                collectedString += "\n[*]Tinkerer Subclass - Scout Sentinel [to-be imported by @dubhHerder]";
                collectedString += "\n[*]Two Feats - Power Attack and Reckless Fury";
                collectedString += "\n[*]UI Updates";
                collectedString += "\n[*]Unofficial Hotfixes";
                collectedString += "\n[*]Wizard Arcane Fighter";
                collectedString += "\n[*]Wizard Life Transmuter";
                collectedString += "\n[*]Wizard Master Manipulator";
                collectedString += "\n[*]Wizard Spell Master";
                collectedString += "\n[/list]";
                // items
                Main.Error(collectedString);
            }, UI.AutoWidth());
           
        }
    }
}
