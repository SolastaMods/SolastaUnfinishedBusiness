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

#pragma warning disable IDE0051, S1144, RCS1213 // Remove unused private members
        private static void AddDumpDescriptionToLogButton()
#pragma warning restore IDE0051, S1144, RCS1213 // Remove unused private members
        {
            UI.ActionButton("Dump Description to Logs", () =>
            {
                var collectedString = new StringBuilder();
                collectedString.Append("[heading][size=5] [b] [i] Solasta Community Expansion[/i][/b][/size][/heading]");
                collectedString.Append("\nThis is a collection of work from the Solasta modding community. It includes feats, subclasses, items, crafting recipes, gameplay options, UI improvements, and more. The general philosophy is everything is optional to enable, so you can install the mod and then enalbe the pieces you want. There are some minor bug fixes that are enabled by default.");
                collectedString.Append("\n\n[b] ATTENTION:[/b] This mod is a collection of previously released mods in addition to some new components. If any of the mods this is replacing is still installed, you will have errors on startup. [b]ChrisJohnDigital[/b], [b]ImpPhil[/b] and [b]Zappastuff[/b] put many hours consolidating all previous work to offer the best we created over the last year in a simple set of 4 basic mods:");
                collectedString.Append("\n\n[list=1]");
                collectedString.Append("\n[*] [b]Solasta Mod API[/b] - Provides the basis for all other mods to work");
                collectedString.Append("\n[*] [b]Solasta Community Expansion[/b] - About 40 mods from the community were consolidated here. 40 Feats, 10 Subclasses, Bug Fixes, etc.");
                collectedString.Append("\n[*] [b]Solasta Community Expansion - Multiclass[/b] - Brings SRD official multiclassing rules into Solasta.");
                collectedString.Append("\n[*] [b]Solasta Dungeon Maker PRO[/b] - Offers multiplayer with up to 4 users, additional design options for Dungeon Creators, Lua Scripting, etc.");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[heading]Installation[/heading]");
                collectedString.Append("\nThis mod requires [url=https://www.nexusmods.com/solastacrownofthemagister/mods/48]Solasta Mod API[/url].");
                collectedString.Append("\nFor more installation information visit our [url=https://github.com/ChrisPJohn/SolastaCommunityExpansion/wiki/Installation-Instructions]wiki[/url].");
                collectedString.Append("\n[heading] How to Report Bugs[/heading]");
                collectedString.Append("\n[list]");
                collectedString.Append("\n[*] The versions of Solasta, the Solasta Mod API, and Solasta Community Expansion.");
                collectedString.Append("\n[*] A list of other mods you have installed.");
                collectedString.Append("\n[*] A short description of the bug");
                collectedString.Append("\n[*] A step-by-step procedure to reproduce it");
                collectedString.Append("\n[*] The save, character and log files");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[heading][size=5]Features[/size][/heading]");
                collectedString.Append("\n[heading]Character General[/heading]\n[list]");
                collectedString.Append("\n[*]Set level cap to 20");
                collectedString.Append("\n[*]Allow respecing characters");
                collectedString.Append("\n[*]Disable sense and superior darkvision");
                collectedString.Append("\n[*]Alternate Human [+1 feat / +2 attribute choices / +1 skill]");
                collectedString.Append("\n[*]Flexible races [Assign ability score points instead of the racial defaults (example: High Elf has 3 points to assign instead of +2 Dex/+1 Int)]");
                collectedString.Append("\n[*]Flexible backgrounds [Select skill and tool proficiencies from backgrounds]");
                collectedString.Append("\n[*]Receive both ASI and Feat every 4 levels");
                collectedString.Append("\n[*]Epic 35 points buy system");
                collectedString.Append("\n[*]Epic [17,15,13,12,10,8] array");
                collectedString.Append("\n[*]Feats available at level 1");
                collectedString.Append("\n[*]Allow extra keyboard characters in names");
                collectedString.Append("\n[*]Additional lore friendly names");
                collectedString.Append("\n[*]Face unlock");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.FeatsContext.GenerateFeatsDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.ClassesContext.GenerateClassDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.SubclassesContext.GenerateSubclassDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.FightingStyleContext.GenerateFightingStyleDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.SpellsContext.GenerateSpellsDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Encounters General[/heading]\n[list]");
                collectedString.Append("\n[*]Switch party AI from human to computer");
                collectedString.Append("\n[*]Provide a custom ad-hoc encounter tool");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Bestiary[/heading]\n[list]");
                collectedString.Append("\n[*]Select monsters for ad-hoc encounters");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Characters Pool[/heading]\n[list]");
                collectedString.Append("\n[*]Select heroes as enemies for ad-hoc encounters");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Gameplay Rules[/heading]\n[list]");
                collectedString.Append("\n[*]Use official advantage/disadvantage rules");
                collectedString.Append("\n[*]Use official surprise rules");
                collectedString.Append("\n[*]Roll different Stealth checks for each surprised / surprising pairs");
                collectedString.Append("\n[*]Allow target selection when casting the Chain Lightning Spell");
                collectedString.Append("\n[*]Fully control conjurations [animals, elementals, etc]");
                collectedString.Append("\n[*]Improved handling of conjuration spells (dismiss conjuration doesn't trigger hostility, player control both in and out of combat)");
                collectedString.Append("\n[*]Blinded condition doesn't allow attack of opportunity");
                collectedString.Append("\n[*]Allow any class to wear sylvan armor");
                collectedString.Append("\n[*]Allow Druids to wear metal armor");
                collectedString.Append("\n[*]Disable auto-equip");
                collectedString.Append("\n[*]Make all magic staves arcane foci");
                collectedString.Append("\n[*]Increase normal vision range");
                collectedString.Append("\n[*]Add pickpocketable loot");
                collectedString.Append("\n[*]Allow stackable material components");
                collectedString.Append("\n[*]Scale merchant prices correctly/exactly");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Items & Merchants[/heading]\n[list]");
                collectedString.Append("\n[*]Remove identification requirement from items");
                collectedString.Append("\n[*]Remove attunement requirement from items");
                collectedString.Append("\n[*]Show crafting recipe in detailed tooltips");
                collectedString.Append("\n[*]Set the chances of a beard appearing on belt of dwarvenkin");
                collectedString.Append("\n[*]Stock Hugo with all clothing");
                collectedString.Append("\n[*]Stock Hugo with new foci items");
                collectedString.Append("\n[*]Enable all merchants to restock over time");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append(Models.ItemCraftingContext.GenerateItemsDescription());
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Tools[/heading]\n[list]");
                collectedString.Append("\n[*]Enable the Telema Kickstarter demo");
                collectedString.Append("\n[*]Enable the hothey ctrl-shit-T to teleport the party to the cursor location");
                collectedString.Append("\n[*]Overrides party size from 1 to 6");
                collectedString.Append("\n[*]Removes dungeons min/max levels prereqs");
                collectedString.Append("\n[*]Automatically create backup files for content created by user");
                collectedString.Append("\n[*]Enable the cheats menu");
                collectedString.Append("\n[*]Enable the debug camera");
                collectedString.Append("\n[*]Enable the debug overlay");
                collectedString.Append("\n[*]No experience required for level up");
                collectedString.Append("\n[*]Multiplier for earned experience");
                collectedString.Append("\n[*]Set Faction relation levels");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]UI Improvements and bug Fixes[/heading]\n[list]");
                collectedString.Append("\n[*]Enable character export from inventory screen");
                collectedString.Append("\n[*]Enable Inventory filtering and sorting");
                collectedString.Append("\n[*]Enable Save by location");
                collectedString.Append("\n[*]Enable hotkeys to toggle HUG elements");
                collectedString.Append("\n[*]Invert ALT behavior on tooltips");
                collectedString.Append("\n[*]Enable adventure log on custom dungeons");
                collectedString.Append("\n[*]Reduce battle camera movement");
                collectedString.Append("\n[*]Pause the UI when victorious in battle");
                collectedString.Append("\n[*]Speed up battles");
                collectedString.Append("\n[*]Dungeon Maker editor quality of life settings");
                collectedString.Append("\n[*]Item display settings");
                collectedString.Append("\n[*]Monsters's health in steps of 25/50/75/100%");
                collectedString.Append("\n[*]Replace bug monsters with other creatures");
                collectedString.Append("\n[*]Multi line spell casting selection");
                collectedString.Append("\n[*]Multi line power activation selection");
                collectedString.Append("\n[*]Keep spell UI open when switching weapons");
                collectedString.Append("\n[/list]");
                collectedString.Append("\n[line]\n");
                collectedString.Append("[heading]Credits[/heading]\n[list]");
                foreach (var kvp in CreditsTable)
                {
                    collectedString.Append("\n[*]").Append(kvp.Key).Append(": ").Append(kvp.Value);
                }
                collectedString.Append("\n[/list]");
                collectedString.Append("\nSource code on [url=https://github.com/ChrisPJohn/SolastaCommunityExpansion]GitHub[/url].");
                collectedString.Append("\nTo see the list of deprecated mods go [url=https://github.com/ChrisPJohn/SolastaCommunityExpansion/wiki/Mods-included-in-Solasta-Community-Expansion]here[/url]");
                // items
                Main.Error(collectedString.ToString());
            },
            UI.AutoWidth());
        }
    }
}
