using System;
using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class GameUiDisplay
    {
        private static bool DisplayAdventureLog { get; set; }

        private static bool DisplayBattle { get; set; }

        private static bool DisplayDungeonMaker { get; set; }

        private static bool DisplayItem { get; set; }

        private static bool DisplayMonster { get; set; }

        private static bool DisplaySpell { get; set; }

        internal static void DisplayGameUi()
        {
            bool toggle;
            int intValue;
            float floatValue;

            UI.Label("");
            UI.Label("General:".yellow());
            UI.Label("");

            toggle = Main.Settings.EnableCharacterExport;
            if (UI.Toggle("Enable character export from inventory screen " + "[ctrl-(E)xport]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableCharacterExport = toggle;
            }

            toggle = Main.Settings.EnableHudToggleElementsHotkeys;
            if (UI.Toggle("Enable hotkeys to toggle HUD components visibility " + "[ctrl-(C)ontrol Panel / ctrl-(L)og / ctrl-(M)ap / ctrl-(P)arty]".italic().yellow(), ref toggle, UI.AutoWidth()))
            {
                Main.Settings.EnableHudToggleElementsHotkeys = toggle;
            }

            toggle = Main.Settings.InvertAltBehaviorOnTooltips;
            if (UI.Toggle("Invert ALT key behavior on tooltips", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.InvertAltBehaviorOnTooltips = toggle;
            }

            toggle = Main.Settings.RecipeTooltipShowsRecipe;
            if (UI.Toggle("Show crafting recipe in detailed tooltips", ref toggle, UI.AutoWidth()))
            {
                Main.Settings.RecipeTooltipShowsRecipe = toggle;
            }

            #region AdventureLog
            UI.Label("");

            toggle = DisplayAdventureLog;
            if (UI.DisclosureToggle("Adventure Log: ".yellow(), ref toggle, 200))
            {
                DisplayAdventureLog = toggle;
            }

            if (DisplayAdventureLog)
            {
                UI.Label("");
                UI.Label(". The settings below only work in custom campaigns or locations");
                UI.Label("");

                toggle = Main.Settings.EnableAdventureLogBanterLines;
                if (UI.Toggle("Record NPCs banter lines", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdventureLogBanterLines = toggle;
                }

                toggle = Main.Settings.EnableAdventureLogDocuments;
                if (UI.Toggle("Record read documents and notes", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdventureLogDocuments = toggle;
                }

                toggle = Main.Settings.EnableAdventureLogLore;
                if (UI.Toggle("Record full screen lore", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdventureLogLore = toggle;
                }

                toggle = Main.Settings.EnableAdventureLogTextFeedback;
                if (UI.Toggle("Record text feedback", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdventureLogTextFeedback = toggle;
                }

                toggle = Main.Settings.EnableAdventureLogPopups;
                if (UI.Toggle("Record bottom and header popups", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdventureLogPopups = toggle;
                }
            }
            #endregion

            #region Battle
            UI.Label("");

            toggle = DisplayBattle;
            if (UI.DisclosureToggle("Battle:".yellow(), ref toggle, 200))
            {
                DisplayBattle = toggle;
            }

            if (DisplayBattle)
            {
                UI.Label("");

                toggle = Main.Settings.DontFollowCharacterInBattle;
                if (UI.Toggle("Battle camera doesn't follow when character is already on screen", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.DontFollowCharacterInBattle = toggle;
                }

                if (Main.Settings.DontFollowCharacterInBattle)
                {
                    intValue = Main.Settings.DontFollowMargin;
                    if (UI.Slider("+ unless character is off or within % of screen edge".italic().yellow(), ref intValue, 0, 20, 1, "%", UI.AutoWidth()))
                    {
                        Main.Settings.DontFollowMargin = intValue;
                    }

                    UI.Label("");
                }

                toggle = Main.Settings.AutoPauseOnVictory;
                if (UI.Toggle("Pause the UI when victorious in battle", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.AutoPauseOnVictory = toggle;
                }

                toggle = Main.Settings.PermanentSpeedUp;
                if (UI.Toggle("Permanently speeds battle up", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.PermanentSpeedUp = toggle;
                }

                UI.Label("");
                floatValue = Main.Settings.CustomTimeScale;
                if (UI.Slider("Battle timescale modifier".white(), ref floatValue, 1f, 50f, 1f, 1, "", UI.AutoWidth()))
                {
                    Main.Settings.CustomTimeScale = floatValue;
                }
            }
            #endregion

            #region DungeonMaker
            UI.Label("");

            toggle = DisplayDungeonMaker;
            if (UI.DisclosureToggle("Dungeon Maker: ".yellow(), ref toggle, 200))
            {
                DisplayDungeonMaker = toggle;
            }

            if (DisplayDungeonMaker)
            {
                UI.Label("");

                toggle = Main.Settings.FlexibleGadgetsPlacement;
                if (UI.Toggle("Allow gadgets to be placed anywhere on the map " + RequiresRestart, ref toggle))
                {
                    Main.Settings.FlexibleGadgetsPlacement = toggle;
                }

                toggle = Main.Settings.FlexiblePropsPlacement;
                if (UI.Toggle("Allow props to be placed anywhere on the map " + RequiresRestart, ref toggle))
                {
                    Main.Settings.FlexiblePropsPlacement = toggle;
                }

                UI.Label("");

                toggle = Main.Settings.DungeonMakerEditorBetterTooltips;
                if (UI.Toggle("Enable better tooltip on dungeon maker editor " + "[selected items or monsters on gadget detail screen]".italic().yellow(), ref toggle))
                {
                    Main.Settings.DungeonMakerEditorBetterTooltips = toggle;
                }

                UI.Label("");

                toggle = Main.Settings.UnleashAllMonsters;
                if (UI.Toggle("Unleash NPCs as enemies " + "[press SHIFT while clicking Select on gadget panel]".italic().yellow(), ref toggle))
                {
                    Main.Settings.UnleashAllMonsters = toggle;
                }

                toggle = Main.Settings.UnleashAllNPCs;
                if (UI.Toggle("Unleash enemies as NPCs " + "[press SHIFT while clicking Select on gadget panel]".italic().yellow(), ref toggle))
                {
                    Main.Settings.UnleashAllNPCs = toggle;
                }
            }
            #endregion

            #region Item
            UI.Label("");

            toggle = DisplayItem;
            if (UI.DisclosureToggle("Inventory and items: ".yellow(), ref toggle, 200))
            {
                DisplayItem = toggle;
            }

            if (DisplayItem)
            {
                UI.Label("");

                toggle = Main.Settings.EnableInventoryFilterAndSort;
                if (UI.Toggle("Enable inventory filtering and sorting " + RequiresRestart, ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableInventoryFilterAndSort = toggle;
                }

                toggle = Main.Settings.EnableInvisibleCrownOfTheMagister;
                if (UI.Toggle("Hide the " + "Crown of the Magister".orange() + " on game UI", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableInvisibleCrownOfTheMagister = toggle;
                    ItemOptionsContext.SwitchCrownOfTheMagister();
                }

                UI.Label("");

                using (UI.HorizontalScope())
                {
                    UI.Label("Empress Garb".orange() + " appearance ".white(), UI.Width(325));

                    intValue = Array.IndexOf(ItemOptionsContext.EmpressGarbSkins, Main.Settings.EmpressGarbSkin);
                    if (UI.SelectionGrid(ref intValue, ItemOptionsContext.EmpressGarbSkins, ItemOptionsContext.EmpressGarbSkins.Length, UI.Width(600)))
                    {
                        Main.Settings.EmpressGarbSkin = ItemOptionsContext.EmpressGarbSkins[intValue];
                        ItemOptionsContext.SwitchEmpressGarb();
                    }
                }
            }
            #endregion

            #region Monster
            UI.Label("");

            toggle = DisplayMonster;
            if (UI.DisclosureToggle("Monsters: ".yellow(), ref toggle, 200))
            {
                DisplayMonster = toggle;
            }

            if (DisplayMonster)
            {
                UI.Label("");

                toggle = Main.Settings.HideMonsterHitPoints;
                if (UI.Toggle("Display Monsters's health in steps of 25% / 50% / 75% / 100% instead of exact hit points", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.HideMonsterHitPoints = toggle;
                }

                toggle = Main.Settings.RemoveBugVisualModels;
                if (UI.Toggle("Replace bug-like models with alternative visuals in the game " + "[must be switched on before maps are loaded] ".italic().yellow() + RequiresRestart, ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.RemoveBugVisualModels = toggle;
                }
            }
            #endregion

            #region Spell
            UI.Label("");

            toggle = DisplaySpell;
            if (UI.DisclosureToggle("Spells: ".yellow(), ref toggle, 200))
            {
                DisplaySpell = toggle;
            }

            if (DisplaySpell)
            {
                UI.Label("");

                intValue = Main.Settings.MaxSpellLevelsPerLine;
                if (UI.Slider("Max levels per line on Spell Panel".white(), ref intValue, 3, 7, 5, "", UI.AutoWidth()))
                {
                    Main.Settings.MaxSpellLevelsPerLine = intValue;
                }

                floatValue = Main.Settings.SpellPanelGapBetweenLines;
                if (UI.Slider("Gap between spell lines on Spell Panel".white(), ref floatValue, 0f, 200f, 50f, 0, "", UI.AutoWidth()))
                {
                    Main.Settings.SpellPanelGapBetweenLines = floatValue;
                }
            }
            #endregion

            UI.Label("");
        }
    }
}
