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
            }
            #endregion

            #region Campaign
            UI.Label("");

            toggle = Main.Settings.DisplayCampaignToggle;
            if (UI.DisclosureToggle("Custom campaigns and locations:".yellow(), ref toggle, 200))
            {
                Main.Settings.DisplayCampaignToggle = toggle;
            }

            if (Main.Settings.DisplayCampaignToggle)
            {
                UI.Label("");

                toggle = Main.Settings.EnableAdventureLogDocuments;
                if (UI.Toggle("Record read documents and notes " + "[This setting only works in custom campaigns or locations]".yellow().italic(), ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdventureLogDocuments = toggle;
                }

                UI.Label("");

                toggle = Main.Settings.EnableAdditionalIconsOnLevelMap;
                if (UI.Toggle("Enable additional icons for camps, exits and teleporters on level map", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.EnableAdditionalIconsOnLevelMap = toggle;

                    if (toggle)
                    {
                        Main.Settings.MarkInvisibleTeleportersOnLevelMap = false;
                    }
                }

                if (Main.Settings.EnableAdditionalIconsOnLevelMap)
                {
                    toggle = Main.Settings.MarkInvisibleTeleportersOnLevelMap;
                    if (UI.Toggle("+ Also mark the location of invisible teleporters on level map after discovery".italic(), ref toggle, UI.AutoWidth()))
                    {
                        Main.Settings.MarkInvisibleTeleportersOnLevelMap = toggle;
                    }
                }

                toggle = Main.Settings.HideExitAndTeleporterGizmosIfNotDiscovered;
                if (UI.Toggle("Hide exits and teleporters visual effects if not discovered yet", ref toggle, UI.AutoWidth()))
                {
                    Main.Settings.HideExitAndTeleporterGizmosIfNotDiscovered = toggle;
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
