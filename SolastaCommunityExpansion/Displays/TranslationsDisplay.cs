using System;
using System.Linq;
using ModKit;
using SolastaCommunityExpansion.Utils;

namespace SolastaCommunityExpansion.Displays;

public static class TranslationsDisplay
{
    internal static string SelectedLanguage = "en";

    internal static void DisplayTranslations()
    {
        var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();

        UI.Label("");
        UI.Label(Gui.Format("ModUi/&TranslationHelp", SelectedLanguage.yellow().bold()));
        UI.Label("");

        using (UI.HorizontalScope())
        {
            var intValue = Array.IndexOf(UserCampaignsTranslator.AvailableLanguages, SelectedLanguage);

            if (UI.SelectionGrid(ref intValue,
                    UserCampaignsTranslator.AvailableLanguages, UserCampaignsTranslator.AvailableLanguages.Length, 6,
                    UI.Width(500)))
            {
                SelectedLanguage = UserCampaignsTranslator.AvailableLanguages[intValue];
            }
        }

        UI.Label("");

        foreach (var userCampaign in userCampaignPoolService.AllCampaigns
                     .Where(x => x.IsWorkshopItem)
                     .OrderBy(x => x.Title))
        {
            var exportName = userCampaign.Title;

            using (UI.HorizontalScope())
            {
                string buttonLabel;

                if (UserCampaignsTranslator.CurrentExports.TryGetValue(exportName, out var status))
                {
                    buttonLabel = Gui.Format("ModUi/&CancelTranslate", status.LanguageCode.ToUpper(),
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
                            UserCampaignsTranslator.TranslateUserCampaign(SelectedLanguage, userCampaign.Title,
                                userCampaign);
                        }
                        else
                        {
                            UserCampaignsTranslator.Cancel(userCampaign.Title);
                        }
                    },
                    UI.Width(200));

                UI.Label(userCampaign.Author.bold().orange(), UI.Width(100));
                UI.Label(userCampaign.Title.bold().italic(), UI.Width(400));
            }
        }

        UI.Label("");
    }
}
