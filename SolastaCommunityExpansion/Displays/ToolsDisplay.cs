using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Displays;

internal static class ToolsDisplay
{
    private static bool displayArmor;
    private static bool displayWeapons;
    private static bool displayAmmunition;
    private static bool displayUsableDevices;
    private static Vector2 armorScrollPosition = Vector2.zero;
    private static Vector2 weaponsScrollPosition = Vector2.zero;
    private static Vector2 ammunitionScrollPosition = Vector2.zero;
    private static Vector2 usableDevicesScrollPosition = Vector2.zero;

    internal static void SetFactionRelation(string name, int value)
    {
        var service = ServiceRepository.GetService<IGameFactionService>();
        if (service != null)
        {
            service.ExecuteFactionOperation(name, FactionDefinition.FactionOperation.Increase,
                value - service.FactionRelations[name], "",
                null /* this string and monster doesn't matter if we're using "SetValue" */);
        }
    }

    internal static void DisplayTools()
    {
        bool toggle;
        int intValue;

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&General"));
        UI.Label("");

        toggle = Main.Settings.EnableSaveByLocation;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSaveByLocation"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSaveByLocation = toggle;
        }

        toggle = Main.Settings.EnableCharacterChecker;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCharacterChecker"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCharacterChecker = toggle;
        }

        toggle = Main.Settings.EnableCheatMenu;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableCheatMenu"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCheatMenu = toggle;
        }

        toggle = Main.Settings.EnableRespec;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableRespec"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRespec = toggle;
            RespecContext.Switch();
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&Adventure"));
        UI.Label("");

        toggle = Main.Settings.EnableTogglesToOverwriteDefaultTestParty;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableTogglesToOverwriteDefaultTestParty"), ref toggle))
        {
            Main.Settings.EnableTogglesToOverwriteDefaultTestParty = toggle;
        }

        toggle = Main.Settings.NoExperienceOnLevelUp;
        if (UI.Toggle(Gui.Localize("ModUi/&NoExperienceOnLevelUp"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.NoExperienceOnLevelUp = toggle;
        }

        toggle = Main.Settings.OverrideMinMaxLevel;
        if (UI.Toggle(Gui.Localize("ModUi/&OverrideMinMaxLevel"), ref toggle))
        {
            Main.Settings.OverrideMinMaxLevel = toggle;
        }

        UI.Label("");

        intValue = Main.Settings.MultiplyTheExperienceGainedBy;
        if (UI.Slider(Gui.Localize("ModUi/&MultiplyTheExperienceGainedBy"), ref intValue, 0, 200, 100, "",
                UI.Width(100)))
        {
            Main.Settings.MultiplyTheExperienceGainedBy = intValue;
        }

        intValue = Main.Settings.OverridePartySize;
        if (UI.Slider(Gui.Localize("ModUi/&OverridePartySize"), ref intValue,
                DungeonMakerContext.MIN_PARTY_SIZE, DungeonMakerContext.MAX_PARTY_SIZE,
                DungeonMakerContext.GAME_PARTY_SIZE, "", UI.AutoWidth()))
        {
            Main.Settings.OverridePartySize = intValue;
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&DungeonMaker"));
        UI.Label("");

        intValue = Main.Settings.MaxBackupFilesPerLocationCampaign;
        if (UI.Slider(Gui.Localize("ModUi/&MaxBackupFilesPerLocationCampaign"), ref intValue, 0, 20, 10))
        {
            Main.Settings.MaxBackupFilesPerLocationCampaign = intValue;
        }

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&MaxBackupHelp"));

        DisplayItems();
        DisplayFactionRelations();
    }

    private static void DisplayFactionRelations()
    {
        int intValue;

        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&FactionRelations"));
        UI.Label("");

        var flip = true;
        var gameCampaign = Gui.GameCampaign;
        var gameFactionService = ServiceRepository.GetService<IGameFactionService>();

        // NOTE: don't use gameCampaign?. which bypasses Unity object lifetime check
        if (gameFactionService != null && gameCampaign != null &&
            gameCampaign.CampaignDefinitionName != "UserCampaign")
        {
            foreach (var faction in gameFactionService.RegisteredFactions)
            {
                if (faction.BuiltIn)
                {
                    // These are things like monster factions, generally set to a specific relation and can't be changed.
                    continue;
                }

                if (faction.GuiPresentation.Hidden)
                {
                    // These are things like Silent Whispers and Church Of Einar that are not fully implemented factions.
                    continue;
                }

                var title = faction.FormatTitle();

                if (flip)
                {
                    title = title.yellow();
                }
                else
                {
                    title = title.white();
                }

                intValue = gameFactionService.FactionRelations[faction.Name];

                if (UI.Slider("                              " + title, ref intValue, faction.MinRelationCap,
                        faction.MaxRelationCap, 0, "", UI.AutoWidth()))
                {
                    SetFactionRelation(faction.Name, intValue);
                }

                flip = !flip;
            }
        }
        else
        {
            UI.Label(Gui.Localize("ModUi/&FactionHelp"));
        }

        UI.Label("");
    }

    private static void DisplayItems()
    {
        UI.Label("");
        UI.Label(Gui.Localize("ModUi/&Items"));
        UI.Label("");

        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();

        if (!characterInspectionScreen.Visible || characterInspectionScreen.externalContainer == null)
        {
            UI.Label(Gui.Localize("ModUi/&ItemsHelp1"));

            return;
        }

        UI.Label(Gui.Localize("ModUi/&ItemsHelp2"));

        DisplayItemGroup("Armor", Gui.Localize("ModUi/&Armor"), ref displayArmor, ref armorScrollPosition);
        DisplayItemGroup("Weapon", Gui.Localize("ModUi/&Weapon"), ref displayWeapons, ref weaponsScrollPosition);
        DisplayItemGroup("Ammunition", Gui.Localize("ModUi/&Ammunition"), ref displayAmmunition,
            ref ammunitionScrollPosition);
        DisplayItemGroup("UsableDevice", Gui.Localize("ModUi/&UsableDevice"), ref displayUsableDevices,
            ref usableDevicesScrollPosition);
    }

    private static void DisplayItemGroup(string group, string title, ref bool displayGroup, ref Vector2 scrollPosition)
    {
        var characterInspectionScreen = Gui.GuiService.GetScreen<CharacterInspectionScreen>();
        var rulesetItemFactoryService = ServiceRepository.GetService<IRulesetItemFactoryService>();
        var characterName = characterInspectionScreen.InspectedCharacter.Name;

        UI.Label("");
        UI.DisclosureToggle(title.yellow(), ref displayGroup, 200);

        if (!displayGroup)
        {
            return;
        }

        var items = DatabaseRepository.GetDatabase<ItemDefinition>()
            .Where(x => !x.guiPresentation.Hidden)
            .Where(x => x.GetField<bool>($"is{group}"))
            .OrderBy(x => x.FormatTitle());

        using var scrollView =
            new GUILayout.ScrollViewScope(scrollPosition, GUILayout.Width(350), GUILayout.Height(300));

        scrollPosition = scrollView.scrollPosition;

        foreach (var item in items)
        {
            using (UI.HorizontalScope())
            {
                UI.ActionButton("+".bold().red(), () =>
                    {
                        var rulesetItem = rulesetItemFactoryService.CreateStandardItem(item, true, characterName);

                        characterInspectionScreen.externalContainer.AddSubItem(rulesetItem);
                    },
                    UI.Width(30));
                UI.Label(item.FormatTitle(), UI.Width(300));
            }
        }
    }
}
