using System;
using System.Linq;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Api.ModKit;
using SolastaUnfinishedBusiness.Models;

namespace SolastaUnfinishedBusiness.Displays;

internal static class DungeonMakerDisplay
{
    internal static void DisplayDungeonMaker()
    {
        const float DOC_BUTTON_WIDTH = 147f;

        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Aberrations".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersAberration.md"),
                UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Beasts".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersBeast.md"), UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Celestials".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersCelestial.md"),
                UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Constructs".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersConstruct.md"),
                UI.Width(DOC_BUTTON_WIDTH));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Dragons".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersDragon.md"), UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Elementals".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersElemental.md"),
                UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Fey".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersFey.md"), UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Fiend".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersFiend.md"), UI.Width(DOC_BUTTON_WIDTH));
        }

        using (UI.HorizontalScope())
        {
            UI.ActionButton("Giants".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersGiant.md"), UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Humanoids".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersHumanoid.md"),
                UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Monstrosities".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersMonstrosity.md"),
                UI.Width(DOC_BUTTON_WIDTH));
            UI.ActionButton("Undead".Bold().Khaki(),
                () => UpdateContext.OpenDocumentation("Monsters/SolastaMonstersUndead.md"), UI.Width(DOC_BUTTON_WIDTH));
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Basic"));
        UI.Label();
        UI.Label(Gui.Localize("ModUi/&DungeonMakerBasicHelp"));
        UI.Label();

        var toggle = Main.Settings.EnableLoggingInvalidReferencesInUserCampaigns;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableLoggingInvalidReferencesInUserCampaigns"), ref toggle))
        {
            Main.Settings.EnableLoggingInvalidReferencesInUserCampaigns = toggle;
        }

        toggle = Main.Settings.EnableSortingDungeonMakerAssets;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableSortingDungeonMakerAssets"), ref toggle))
        {
            Main.Settings.EnableSortingDungeonMakerAssets = toggle;
        }

        toggle = Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere;
        if (UI.Toggle(Gui.Localize("ModUi/&AllowGadgetsAndPropsToBePlacedAnywhere"), ref toggle))
        {
            Main.Settings.AllowGadgetsAndPropsToBePlacedAnywhere = toggle;
        }

        toggle = Main.Settings.UnleashEnemyAsNpc;
        if (UI.Toggle(Gui.Localize("ModUi/&UnleashEnemyAsNpc"), ref toggle))
        {
            Main.Settings.UnleashEnemyAsNpc = toggle;
        }

        UI.Label();
        UI.Label(Gui.Localize("ModUi/&Advanced"));
        UI.Label();
        UI.Label(Gui.Localize("ModUi/&AdvancedHelp"));
        UI.Label();

        toggle = Main.Settings.AddNewWeaponsAndRecipesToEditor;
        if (UI.Toggle(Gui.Localize(Gui.Localize("ModUi/&EnableAdditionalItemsInDungeonMaker")), ref toggle,
                UI.AutoWidth()))
        {
            Main.Settings.AddNewWeaponsAndRecipesToEditor = toggle;
        }

        toggle = Main.Settings.UnleashNpcAsEnemy;
        if (UI.Toggle(Gui.Localize("ModUi/&UnleashNpcAsEnemy"), ref toggle))
        {
            Main.Settings.UnleashNpcAsEnemy = toggle;
        }

        toggle = Main.Settings.EnableVariablePlaceholdersOnTexts;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableVariablePlaceholdersOnTexts"), ref toggle))
        {
            Main.Settings.EnableVariablePlaceholdersOnTexts = toggle;
        }

        toggle = Main.Settings.EnableDungeonMakerModdedContent;
        if (UI.Toggle(Gui.Localize("ModUi/&EnableDungeonMakerModdedContent"), ref toggle))
        {
            Main.Settings.EnableDungeonMakerModdedContent = toggle;
        }

        UI.Label();

        UI.Label();
        UI.Label(Gui.Format("ModUi/&Translations"));
        UI.Label();

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Localize("ModUi/&TargetLanguage"), UI.Width(150f));

            var intValue = Array.IndexOf(TranslatorContext.AvailableLanguages, Main.Settings.SelectedLanguageCode);

            if (UI.SelectionGrid(
                    ref intValue,
                    TranslatorContext.AvailableLanguages,
                    TranslatorContext.AvailableLanguages.Length,
                    10, UI.Width(600f)))
            {
                Main.Settings.SelectedLanguageCode = TranslatorContext.AvailableLanguages[intValue];
            }
        }

        UI.Label();

        var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();

        foreach (var userCampaign in userCampaignPoolService.AllCampaigns
                     .Where(x => !x.TechnicalInfo.StartsWith(TranslatorContext.TranslatorBehaviour.UbTranslationTag))
                     .OrderBy(x => x.Author)
                     .ThenBy(x => x.Title))
        {
            var exportName = userCampaign.Title;

            using (UI.HorizontalScope())
            {
                string buttonLabel;

                UI.Label(
                    userCampaign.Author.Substring(0, Math.Min(20, userCampaign.Author.Length)).Bold().Orange(),
                    UI.Width(150f));
                UI.Label(userCampaign.Title.Bold().Italic(), UI.Width(400f));

                if (TranslatorContext.TranslatorBehaviour.CurrentExports.TryGetValue(exportName, out var status))
                {
                    buttonLabel = Gui.Format("ModUi/&TranslateCancel", status.LanguageCode.ToUpper(),
                        $"{status.PercentageComplete:00.0%}").Bold().Khaki();
                }
                else
                {
                    buttonLabel = Gui.Localize("ModUi/&Translate");
                }

                UI.ActionButton(buttonLabel, () =>
                    {
                        if (status == null)
                        {
                            TranslatorContext.TranslatorBehaviour.TranslateUserCampaign(
                                Main.Settings.SelectedLanguageCode, userCampaign.Title, userCampaign);
                        }
                        else
                        {
                            TranslatorContext.TranslatorBehaviour.Cancel(userCampaign.Title);
                        }
                    },
                    UI.Width(200f));
            }
        }

        UI.Label();
    }
}
