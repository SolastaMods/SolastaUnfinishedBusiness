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
            { "ImpPhil", "monster's health, pause UI, stocks prices" },
            { "View619", "darkvision / superior dark vision" },
            { "SilverGriffon", "pickpocket, lore friendly names, crafty feats" },
            { "Boofat", "alwaysAlt, faster time scale" },
            { "AceHigh", "power attack, reckless fury" },
            { "DubhHerder", "crafty feats migration" }
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

            // AddDumpDescriptionToLogButton();
        }

        private void AddDumpDescriptionToLogButton()
        {
            UI.ActionButton("Dump Description to Logs", () => {
                string collectedString = "";
                // settings page
                collectedString += "[heading]Character Creation Options[/heading]\n[list]";
                collectedString += "\n[*]Epic [17,15,13,12,10,8] array";
                collectedString += "\n[*]Alternate Human [+2 attribute choices / +1 skill]";
                collectedString += "\n[*]Feats available at level 1";
                collectedString += "\n[*]Flexible backgrounds";
                collectedString += "\n[*]Flexible races";
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
                // items
                Main.Error(collectedString);
            }, UI.AutoWidth());
           
        }
    }
}