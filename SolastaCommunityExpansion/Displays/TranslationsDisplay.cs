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
            var intValue = Array.IndexOf(TranslationsExporter.AvailableLanguages, SelectedLanguage);

            if (UI.SelectionGrid(ref intValue,
                    TranslationsExporter.AvailableLanguages, TranslationsExporter.AvailableLanguages.Length, 6,
                    UI.Width(500)))
            {
                SelectedLanguage = TranslationsExporter.AvailableLanguages[intValue];
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

                if (TranslationsExporter.CurrentExports.TryGetValue(exportName, out var status))
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
                            TranslationsExporter.TranslateUserCampaign(SelectedLanguage, userCampaign.Title,
                                userCampaign);
                        }
                        else
                        {
                            TranslationsExporter.Cancel(userCampaign.Title);
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
