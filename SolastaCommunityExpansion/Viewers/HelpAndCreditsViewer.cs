using System.Text;
using ModKit;
using UnityModManagerNet;
using static SolastaCommunityExpansion.Viewers.Displays.CreditsDisplay;
using static SolastaCommunityExpansion.Viewers.Displays.Level20HelpDisplay;

namespace SolastaCommunityExpansion.Viewers
{
    public class HelpAndCreditsViewer : IMenuSelectablePage
    {
        public string Name => "Help & Credits";

        public int Priority => 40;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            UI.Label("Welcome to Solasta Community Expansion".yellow().bold());
            UI.Div();

            DisplayLevel20Help();
            DisplayCredits();
            //AddDumpDescriptionToLogButton();
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable S1144 // Remove unused private members
        private static void AddDumpDescriptionToLogButton()
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore S1144 // Remove unused private members
        {
            UI.ActionButton("Dump Description to Logs", () =>
            {
                var collectedString = new StringBuilder();
                collectedString.Append("[heading][size=5] [b] [i] Solasta Community Expansion[/i][/b][/size][/heading]");
                collectedString.Append("\nThis is a collection of work from the Solasta modding community. It includes feats, subclasses, items, crafting recipes, gameplay options, UI improvements, and more. The general philosophy is everything is optional to enable, so you can install the mod and then enalbe the pieces you want. There are some minor bug fixes that are enabled by default.");
                collectedString.Append("\n\n[b] ATTENTION:[/b] This mod is a collection of previously released mods in addition to some new components. If any of the mods this is replacing is still installed, you will have errors on startup. It is highly suggested to delete all mods from [b]GAME_FOLDER\\Mods[/b] and add the ones you need. No previous mod from the list at the end of this document should be installed unless the author specifically says it is supported. [b]ChrisJohnDigital[/b], [b]ImpPhil[/b] and [b]Zappastuff[/b] put many hours consolidating all previous work to offer the best we created over the last year in a simple set of 4 basic mods:");
                collectedString.Append("\n\n[list=1]");
                collectedString.Append("\n[*] [b]Solasta Mod API[/b] - Provides the basis for all other mods to work");
                collectedString.Append("\n[*] [b]Solasta Community Expansion[/b] - About 40 mods from the community were consolidated here. 40 Feats, 6 Subclasses, Bug Fixes, etc.");
                collectedString.Append("\n[*] [b]Solasta Dungeon Maker PRO[/b] - Offers multiplayer with up to 4 users, additional design options for Dungeon Creators, Lua Scripting, etc.");
                collectedString.Append("\n[*] [b]Solasta Unfinished Business[/b] - Brings SRD official multiclassing rules into Solasta");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[heading] How to Report Bugs[/heading]");
                collectedString.Append("\n[list]");
                collectedString.Append("\n[*] The versions of Solasta, the Solasta Mod API, and Solasta Community Expansion.");
                collectedString.Append("\n[*] A list of other mods you have installed.");
                collectedString.Append("\n[*] A short description of the bug");
                collectedString.Append("\n[*] A step-by-step procedure to reproduce it");
                collectedString.Append("\n[*] The save, character and log files");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[heading][size=5]Features[/size][/heading]");
                collectedString.Append("\n[heading]Character Options[/heading]\n[list]");
                collectedString.Append("\n[*]Set level cap to 20");
                collectedString.Append("\n[*]Allow respecing characters");
                collectedString.Append("\n[*]Remove darkvision");
                collectedString.Append("\n[*]Alternate Human [+1 feat / +2 attribute choices / +1 skill]");
                collectedString.Append("\n[*]Flexible races [Assign ability score points instead of the racial defaults (example: High Elf has 3 points to assign instead of +2 Dex/+1 Int)]");
                collectedString.Append("\n[*]Flexible backgrounds [Select skill and tool proficiencies from backgrounds]");
                collectedString.Append("\n[*]Receive both ASI and Feat every 4 levels");
                collectedString.Append("\n[*]Epic [17,15,13,12,10,8] array");
                collectedString.Append("\n[*]Feats available at level 1");
                collectedString.Append("\n[*]Face unlock");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.FeatsContext.GenerateFeatsDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.SubclassesContext.GenerateSubclassDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.FightingStyleContext.GenerateFightingStyleDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Gameplay Options[/heading]\n[list]");
                collectedString.Append("\n[*]Use official advantage/disadvantage rules");
                collectedString.Append("\n[*]Use official surprise rules");
                collectedString.Append("\n[*]Blinded condition doesn't allow attack of opportunity");
                collectedString.Append("\n[*]Add pickpocketable loot");
                collectedString.Append("\n[*]Allow Druids to wear metal armor");
                collectedString.Append("\n[*]Disable auto-equip");
                collectedString.Append("\n[*]Scale merchant prices correctly/exactly");
                collectedString.Append("\n[*]Remove identification requirement from items");
                collectedString.Append("\n[*]Remove attunement requirement from items");
                collectedString.Append("\n[*]Improved handling of conjuration spells (dismiss conjuration doesn't trigger hostility, player control both in and out of combat)");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.ItemCraftingContext.GenerateItemsDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]UI Improvements and bug Fixes[/heading]\n[list]");
                collectedString.Append("\n[*]Inventory filtering and sorting");
                collectedString.Append("\n[*]Save by location");
                collectedString.Append("\n[*]Replace bug monsters with other creatures");
                collectedString.Append("\n[*]Allow extra keyboard characters in names");
                collectedString.Append("\n[*]Monsters's health in steps of 25/50/75/100%");
                collectedString.Append("\n[*]Invert ALT behavior on tooltips");
                collectedString.Append("\n[*]Shows crafting recipe in detailed tooltips");
                collectedString.Append("\n[*]Pause the UI when victorious in battle");
                collectedString.Append("\n[*]Additional lore friendly names");
                collectedString.Append("\n[*]Speed up battles");
                collectedString.Append("\n[*]Reduce battle camera movement");
                collectedString.Append("\n[*]Multi line spell casting selection");
                collectedString.Append("\n[*]Multi line power activation selection");
                collectedString.Append("\n[*]Keep spell UI open when switching weapons");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Tools[/heading]\n[list]");
                collectedString.Append("\n[*]Enable the cheats menu");
                collectedString.Append("\n[*]Enable the debug camera");
                collectedString.Append("\n[*]Enable the debug overlay");
                collectedString.Append("\n[*]No experience required for level up");
                collectedString.Append("\n[*]Set Faction relation levels");
                collectedString.Append("\n[*]Multiplier for earned experience");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Credits[/heading]\n[list]");
                collectedString.Append("\n[*]Chris John Digital");
                foreach (var kvp in Displays.CreditsDisplay.CreditsTable)
                {
                    collectedString.Append("\n[*]" + kvp.Key + ": " + kvp.Value);
                }
                collectedString.Append("\n[/list]");
                collectedString.Append("\nSource code on [url=https://github.com/ChrisPJohn/SolastaCommunityExpansion]GitHub[/url].");
                collectedString.Append("\n[heading]DEPRECATED MODS LIST[/heading]");
                collectedString.Append("\n[list]");
                collectedString.Append("\n[*]Alternate Human");
                collectedString.Append("\n[*]AlwaysAlt - Auto expand tooltips");
                collectedString.Append("\n[*]Armor Feats");
                collectedString.Append("\n[*]ASI and Feat");
                collectedString.Append("\n[*]Caster Feats -Telekinetic - Fey Teleportation - Shadow Touched");
                collectedString.Append("\n[*]Character Export");
                collectedString.Append("\n[*]Crafty Feats");
                collectedString.Append("\n[*]Custom Merchants");
                collectedString.Append("\n[*]Darkvision");
                collectedString.Append("\n[*]Data Viewer");
                collectedString.Append("\n[*]Druid Class by DubhHerder");
                collectedString.Append("\n[*]Dungeon Maker Merchants");
                collectedString.Append("\n[*]ElAntonius's Feat Pack");
                collectedString.Append("\n[*]Enchanting Crafting Ingredients");
                collectedString.Append("\n[*]Enhanced Vision");
                collectedString.Append("\n[*]Faster Time Scale");
                collectedString.Append("\n[*]Feats - Savage Attacker - Tough - War Caster");
                collectedString.Append("\n[*]Fighter Spell Shield");
                collectedString.Append("\n[*]Fighter Subclass - Tactician");
                collectedString.Append("\n[*]Fighting Style Feats");
                collectedString.Append("\n[*]Flexible Ancestries");
                collectedString.Append("\n[*]Flexible Backgrounds");
                collectedString.Append("\n[*]Healing Feats -Inspiring Leader - Chef - Healer");
                collectedString.Append("\n[*]Hot Seat Multiplayer Dungeon Master Mode");
                collectedString.Append("\n[*]Identify all items");
                collectedString.Append("\n[*]Level 1 Feat All Races");
                collectedString.Append("\n[*]Level 20");
                collectedString.Append("\n[*]Magic Crossbows");
                collectedString.Append("\n[*]More Magic Items");
                collectedString.Append("\n[*]Multiclass");
                collectedString.Append("\n[*]No Level Constraint");
                collectedString.Append("\n[*]Primed Recipes");
                collectedString.Append("\n[*]Ranger Arcanist -Ranger Subclass");
                collectedString.Append("\n[*]Respec");
                collectedString.Append("\n[*]Rogue Con Artist");
                collectedString.Append("\n[*]Save by Location");
                collectedString.Append("\n[*]Skip Tutorials");
                collectedString.Append("\n[*]Solastanomicon");
                collectedString.Append("\n[*]Telema Campaign");
                collectedString.Append("\n[*]Tinkerer Subclass - Scout Sentinel [in the Tinkerer Mod]");
                collectedString.Append("\n[*]Two Feats - Power Attack and Reckless Fury");
                collectedString.Append("\n[*]UI Updates");
                collectedString.Append("\n[*]Unofficial Hotfixes");
                collectedString.Append("\n[*]Wizard Arcane Fighter");
                collectedString.Append("\n[*]Wizard Life Transmuter");
                collectedString.Append("\n[*]Wizard Master Manipulator");
                collectedString.Append("\n[*]Wizard Spell Master");
                collectedString.Append("\n[/list]");
                // items
                Main.Error(collectedString.ToString());
            }, 
            UI.AutoWidth());
        }
    }
}
