using System;
using ModKit;
using SolastaCommunityExpansion.Models;
using static SolastaCommunityExpansion.Viewers.Displays.Shared;

namespace SolastaCommunityExpansion.Viewers.Displays
{
    internal static class GameUiDisplay
    {
        internal static void DisplayGameUi()
        {
            bool toggle;
            int intValue;
            float floatValue;

            #region AdventureLog
            UI.Label("");

            toggle = Main.Settings.DisplayAdventureLogToggle;
            if (UI.DisclosureToggle("Adventure Log: ".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayAdventureLogToggle = toggle;
            }

            if (Main.Settings.DisplayAdventureLogToggle)
            {
                UI.Label("");
                UI.Label(". These settings only work in custom campaigns or locations");
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

            toggle = Main.Settings.DisplayBattleToggle;
            if (UI.DisclosureToggle("Battle:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayBattleToggle = toggle;
            }

            if (Main.Settings.DisplayBattleToggle)
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
                    if (UI.Slider("+ Unless hero is off or within % of screen edge".white().italic(), ref intValue, 0, 20, 1, "%", UI.AutoWidth()))
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

                toggle = Main.Settings.PermanentlySpeedBattleUp;
                if (UI.Toggle("Permanently speeds battle up", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.PermanentlySpeedBattleUp = toggle;
                }

                UI.Label("");
                floatValue = Main.Settings.BattleCustomTimeScale;
                if (UI.Slider("Battle timescale modifier".white(), ref floatValue, 1f, 50f, 1f, 1, "M", UI.AutoWidth()))
                {
                    Main.Settings.BattleCustomTimeScale = floatValue;
                }
            }
            #endregion

            #region DungeonMaker
            UI.Label("");

            toggle = Main.Settings.DisplayDungeonMakerToggle;
            if (UI.DisclosureToggle("Dungeon Maker: ".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayDungeonMakerToggle = toggle;
            }

            if (Main.Settings.DisplayDungeonMakerToggle)
            {
                UI.Label("");

                toggle = Main.Settings.AllowGadgetsToBePlacedAnywhere;
                if (UI.Toggle("Allow gadgets to be placed anywhere on the map " + RequiresRestart, ref toggle))
                {
                    Main.Settings.AllowGadgetsToBePlacedAnywhere = toggle;
                }

                toggle = Main.Settings.AllowPropsToBePlacedAnywhere;
                if (UI.Toggle("Allow props to be placed anywhere on the map " + RequiresRestart, ref toggle))
                {
                    Main.Settings.AllowPropsToBePlacedAnywhere = toggle;
                }

                UI.Label("");

                toggle = Main.Settings.UnleashNpcAsEnemy;
                if (UI.Toggle("Unleash NPCs as enemies " + "[press ".italic().yellow() + "SHIFT".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
                {
                    Main.Settings.UnleashNpcAsEnemy = toggle;
                }

                toggle = Main.Settings.UnleashEnemyAsNpc;
                if (UI.Toggle("Unleash enemies as NPCs " + "[press ".italic().yellow() + "SHIFT".italic().cyan() + " while clicking Select on gadget panel]".italic().yellow(), ref toggle))
                {
                    Main.Settings.UnleashEnemyAsNpc = toggle;
                }
            }
            #endregion

            #region Hotkeys
            UI.Label("");

            toggle = Main.Settings.DisplayHotkeysToggle;
            if (UI.DisclosureToggle("Hotkeys:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayHotkeysToggle = toggle;
            }

            if (Main.Settings.DisplayHotkeysToggle)
            {
                UI.Label("");

                toggle = Main.Settings.EnableCharacterExport;
                if (UI.Toggle("Enable character export from the inventory screen using " + "ctrl-(E)".cyan() + "xport", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableCharacterExport = toggle;
                }

                toggle = Main.Settings.EnableHotkeysToToggleHud;
                if (UI.Toggle("Enable the hotkeys " + "ctrl-(C)".cyan() + "ontrol Panel, " + "ctrl-(L)".cyan() + "og, " + "ctrl-(M)".cyan() + "ap, " + "ctrl-(P)".cyan() + "arty and " + "ctrl-(H)".cyan() + "ud to toggle the UI panels visibility", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableHotkeysToToggleHud = toggle;
                }

                toggle = Main.Settings.InvertAltBehaviorOnTooltips;
                if (UI.Toggle("Invert " + "ALT".cyan() + " key behavior on tooltips", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.InvertAltBehaviorOnTooltips = toggle;
                }
            }
            #endregion

            #region Item
            UI.Label("");

            toggle = Main.Settings.DisplayItemToggle;
            if (UI.DisclosureToggle("Inventory and items: ".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayItemToggle = toggle;
            }

            if (Main.Settings.DisplayItemToggle)
            {
                UI.Label("");

                toggle = Main.Settings.EnableInventoryFilteringAndSorting;
                if (UI.Toggle("Enable inventory filtering and sorting", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableInventoryFilteringAndSorting = toggle;
                    InventoryManagementContext.RefreshControlsVisibility();
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

                    intValue = Array.IndexOf(ItemOptionsContext.EmpressGarbAppearances, Main.Settings.EmpressGarbAppearance);
                    if (UI.SelectionGrid(ref intValue, ItemOptionsContext.EmpressGarbAppearances, ItemOptionsContext.EmpressGarbAppearances.Length, UI.Width(600)))
                    {
                        Main.Settings.EmpressGarbAppearance = ItemOptionsContext.EmpressGarbAppearances[intValue];
                        ItemOptionsContext.SwitchEmpressGarb();
                    }
                }
            }
            #endregion

            #region Monster
            UI.Label("");

            toggle = Main.Settings.DisplayMonsterToggle;
            if (UI.DisclosureToggle("Monsters: ".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayMonsterToggle = toggle;
            }

            if (Main.Settings.DisplayMonsterToggle)
            {
                UI.Label("");

                toggle = Main.Settings.HideMonsterHitPoints;
                if (UI.Toggle("Display Monsters's health in steps of 25% / 50% / 75% / 100% instead of exact hit points", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.HideMonsterHitPoints = toggle;
                }

                toggle = Main.Settings.RemoveBugVisualModels;
                if (UI.Toggle("Replace bug-like models with alternative visuals in the game " + RequiresRestart, ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.RemoveBugVisualModels = toggle;
                }
            }
            #endregion

            #region Spell
            UI.Label("");

            toggle = Main.Settings.DisplaySpellToggle;
            if (UI.DisclosureToggle("Spells: ".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplaySpellToggle = toggle;
            }

            if (Main.Settings.DisplaySpellToggle)
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
