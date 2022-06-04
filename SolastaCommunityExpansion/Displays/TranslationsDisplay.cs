using System;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Models;

namespace SolastaCommunityExpansion.Displays;

public static class TranslationsDisplay
{
    internal static void DisplayTranslations()
    {
        UI.Label("");

        using (UI.HorizontalScope())
        {
            UI.Label(Gui.Format("ModUi/&TargetLanguage"), UI.Width(120));

            var intValue = Array.IndexOf(UserCampaignsTranslatorContext.AvailableLanguages,
                Main.Settings.SelectedLanguageCode);
            if (UI.SelectionGrid(
                    ref intValue,
                    UserCampaignsTranslatorContext.AvailableLanguages, UserCampaignsTranslatorContext.AvailableLanguages.Length,
                    3, UI.Width(300)))
            {
                Main.Settings.SelectedLanguageCode = UserCampaignsTranslatorContext.AvailableLanguages[intValue];
            }
        }

        UI.Label("");

        var toggle = Main.Settings.EnableOnTheFlyTranslations;
        if (UI.Toggle(Gui.Format("ModUi/&EnableOnTheFlyTranslations"), ref toggle, UI.AutoWidth()))
        {
            Main.Settings.EnableOnTheFlyTranslations = toggle;
        }

        UI.Label("");
        UI.Label(Gui.Format("ModUi/&ExecuteBatchTranslations"));
        UI.Label("");

        var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();

        foreach (var userCampaign in userCampaignPoolService.AllCampaigns
                     .Where(x => x.IsWorkshopItem)
                     .OrderBy(x => x.Title))
        {
            var exportName = userCampaign.Title;

            using (UI.HorizontalScope())
            {
                string buttonLabel;

                UI.Label(userCampaign.Author.bold().orange(), UI.Width(120));
                UI.Label(userCampaign.Title.bold().italic(), UI.Width(300));

                if (UserCampaignsTranslatorContext.CurrentExports.TryGetValue(exportName, out var status))
                {
                    buttonLabel = Gui.Format("ModUi/&TranslateCancel", status.LanguageCode.ToUpper(),
                        $"{status.PercentageComplete:00.0%}").bold().yellow();
                }
                else
                {
                    buttonLabel = Gui.Format("ModUi/&Translate");
                }

                UI.ActionButton(buttonLabel, () =>
                    {
                        if (status == null)
                        {
                            UserCampaignsTranslatorContext.TranslateUserCampaign(
                                Main.Settings.SelectedLanguageCode, userCampaign.Title, userCampaign);
                        }
                        else
                        {
                            UserCampaignsTranslatorContext.Cancel(userCampaign.Title);
                        }
                    },
                    UI.Width(200));
            }
        }

        UI.Label("");
    }
}
