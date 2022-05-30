using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Displays;

internal static class ToolsDisplay
{
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
        UI.Label(Gui.Format("ModUi/&General"));
        UI.Label("");

        toggle = Main.Settings.EnableSaveByLocation;
        if (UI.Toggle(Gui.Format("ModUi/&EnableSaveByLocation"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableSaveByLocation = toggle;
        }

        toggle = Main.Settings.EnableCharacterChecker;
        if (UI.Toggle(Gui.Format("ModUi/&EnableCharacterChecker"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCharacterChecker = toggle;
        }


        toggle = Main.Settings.EnableCheatMenu;
        if (UI.Toggle(Gui.Format("ModUi/&EnableCheatMenu"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableCheatMenu = toggle;
        }

        toggle = Main.Settings.EnableRespec;
        if (UI.Toggle(Gui.Format("ModUi/&EnableRespec"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableRespec = toggle;
            RespecContext.Switch();
        }


        UI.Label("");
        UI.Label(Gui.Format("ModUi/&Adventure"));
        UI.Label("");


        toggle = Main.Settings.EnableTogglesToOverwriteDefaultTestParty;
        if (UI.Toggle(Gui.Format("ModUi/&EnableTogglesToOverwriteDefaultTestParty"), ref toggle))
        {
            Main.Settings.EnableTogglesToOverwriteDefaultTestParty = toggle;
        }

        toggle = Main.Settings.NoExperienceOnLevelUp;
        if (UI.Toggle(Gui.Format("ModUi/&NoExperienceOnLevelUp"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.NoExperienceOnLevelUp = toggle;
        }

        toggle = Main.Settings.OverrideMinMaxLevel;
        if (UI.Toggle(Gui.Format("ModUi/&OverrideMinMaxLevel"), ref toggle))
        {
            Main.Settings.OverrideMinMaxLevel = toggle;
        }

        UI.Label("");

        intValue = Main.Settings.MultiplyTheExperienceGainedBy;
        if (UI.Slider(Gui.Format("ModUi/&MultiplyTheExperienceGainedBy"), ref intValue, 0, 200, 100, "",
                UI.Width(100)))
        {
            Main.Settings.MultiplyTheExperienceGainedBy = intValue;
        }

        intValue = Main.Settings.OverridePartySize;
        if (UI.Slider(Gui.Format("ModUi/&OverridePartySize"), ref intValue,
                DungeonMakerContext.MIN_PARTY_SIZE, DungeonMakerContext.MAX_PARTY_SIZE,
                DungeonMakerContext.GAME_PARTY_SIZE, "", UI.AutoWidth()))
        {
            Main.Settings.OverridePartySize = intValue;
        }

        UI.Label("");
        UI.Label(Gui.Format("ModUi/&DungeonMaker"));
        UI.Label("");

        intValue = Main.Settings.MaxBackupFilesPerLocationCampaign;
        if (UI.Slider(Gui.Format("ModUi/&MaxBackupFilesPerLocationCampaign"), ref intValue, 0, 20, 10))
        {
            Main.Settings.MaxBackupFilesPerLocationCampaign = intValue;
        }

        UI.Label("");
        UI.Label(Gui.Format("ModUi/&MaxBackupHelp"));

        UI.Label("");
        UI.Label(Gui.Format("ModUi/&FactionRelations"));
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
            UI.Label(Gui.Format("ModUi/&FactionHelp"));
        }

        UI.Label("");
    }
}
