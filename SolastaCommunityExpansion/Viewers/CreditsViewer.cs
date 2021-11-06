using UnityModManagerNet;
using ModKit;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.Viewers
{
    public class CreditsViewer : IMenuSelectablePage
    {
        public string Name => "Credits";

        public int Priority => 10;

        private static Dictionary<string, string> creditsTable = new Dictionary<string, string>
        {
            { "Zappastuff", "mod UI work, integration, community organization" },
            { "ImpPhil", "monster's health, pause UI, stocks prices, no attunement" },
            { "DubhHerder", "crafty feats migration" },
            { "View619", "darkvision, superior dark vision" },
            { "SilverGriffon", "pickpocket, lore friendly names, crafty feats" },
            { "Boofat", "alwaysAlt" },
            { "Myztikrice", "faster time scale" },
            { "AceHigh", "power attack, reckless fury, no identification" },
        };

        public void DisplayHelp()
        {
            UI.Label("");
            UI.Label("Author:".yellow());
            UI.Label("");
            UI.Label(". ChrisJohnDigital".bold() + " - feats, items, subclasses, progression, etc.");
            UI.Label("");
            UI.Label("Credits:".yellow());
            UI.Label("");

            foreach (var kvp in creditsTable)
            {
                using (UI.HorizontalScope())
                {
                    UI.Label(kvp.Key.orange(), UI.Width(100));
                    UI.Label(kvp.Value, UI.Width(400));
                }
            }
            UI.Label("");
        }

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            DisplayHelp();

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
                collectedString += "\n[*] Solasta Mod API - Provides the basis for all other mods to work";
                collectedString += "\n[*] Solasta Community Expansion - About 40 mods from the community were consolidated here. 40 Feats, 6 Subclasses, Bug Fixes, etc.";
                collectedString += "\n[*] DM Multiplayer PRO - Offers REAL Multiplayer with up to 4 users, additional design options for Dungeon Creators, Lua Scripting, etc.";
                collectedString += "\n[*] Solasta Unfinished Business - Offers progression to level 20, multiclassing and respecing";
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
