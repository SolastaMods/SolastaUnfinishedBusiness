using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using I2.Loc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaCommunityExpansion.Properties;

namespace SolastaCommunityExpansion.Utils;

public static class Translations
{
#if false
    internal static string targetCode = "pt";

    internal static void test(UserLocation userLocation, UserCampaign userCampaign)
    {
        foreach (var gadget in userLocation.GadgetsByName.Values)
        {
            foreach (var parameterValue in gadget.ParameterValues)
            {
                switch (parameterValue.GadgetParameterDescription.Type)
                {
                    case GadgetBlueprintDefinitions.Type.Npc:
                    case GadgetBlueprintDefinitions.Type.Speech:
                    case GadgetBlueprintDefinitions.Type.SpeechList:
                        var newStringsList = new List<string>();

                        foreach (var stringValue in parameterValue.StringsList)
                        {
                            newStringsList.Add(Translations.Translate(stringValue, targetCode));
                        }

                        parameterValue.StringsList = newStringsList;

                        if (parameterValue.StringValue != string.Empty)
                        {
                            parameterValue.StringValue = Translations.Translate(parameterValue.StringValue, targetCode);
                        }
                        
                        break;
                }
            }
        }

        foreach (var quest in userCampaign.UserQuests)
        {
            quest.Description = Translations.Translate(quest.Description, targetCode);

            foreach (var userQuestStep in quest.AllQuestStepDescriptions)
            {
                userQuestStep.Title = Translations.Translate(userQuestStep.Title, targetCode);
                userQuestStep.Description = Translations.Translate(userQuestStep.Description, targetCode);

                foreach (var outcome in userQuestStep.OutcomesTable)
                {
                    outcome.DescriptionText = Translations.Translate(outcome.DescriptionText, targetCode);
                }
            }
        }

        foreach (var dialog in userCampaign.UserDialogs)
        {
            dialog.Description = Translations.Translate(dialog.Description, targetCode);

            foreach (var userDialogState in dialog.AllDialogStates)
            {
                foreach (var dialogLine in userDialogState.DialogLines)
                {
                    dialogLine.TextLine = Translations.Translate(dialogLine.TextLine, targetCode);
                }
            }
        }
    }
#endif

    private static Dictionary<string, string> GetWordsDictionary()
    {
        var words = new Dictionary<string, string>();
        var path = Path.Combine(Main.MOD_FOLDER, "dictionary.txt");

        if (!File.Exists(path))
        {
            return words;
        }

        foreach (var line in File.ReadLines(path))
        {
            try
            {
                var splitted = line.Split(new[] {'\t', ' '}, 2);

                words.Add(splitted[0], splitted[1]);
            }
            catch
            {
                Main.Error($"invalid dictionary line \"{line}\".");
            }
        }

        return words;
    }

    public static void LoadTranslations(string category)
    {
        var languageSourceData = LocalizationManager.Sources[0];
        var languageIndex = languageSourceData.GetLanguageIndex(LocalizationManager.CurrentLanguage);
        var languageCode = LocalizationManager.CurrentLanguageCode.Replace("-", "_");
        var payload = (string)typeof(Resources).GetProperty(category + '_' + languageCode).GetValue(null);
        var lines = new List<string>(payload.Split(new[] {Environment.NewLine}, StringSplitOptions.None));

        foreach (var line in lines)
        {
            string term;
            string text;

            try
            {
                var splitted = line.Split(new[] {'\t', ' '}, 2);

                term = splitted[0];
                text = splitted[1];

                foreach (var kvp in GetWordsDictionary())
                {
                    text = text.Replace(kvp.Key, kvp.Value);
                }
            }
            catch
            {
                Main.Error($"invalid translation line \"{line}\".");

                continue;
            }

            var termData = languageSourceData.GetTermData(term);

            if (termData != null && termData.Languages[languageIndex] != null)
            {
                Main.Log($"term {term} overwritten with text {text}");

                termData.Languages[languageIndex] = text;
            }
            else
            {
                languageSourceData.AddTerm(term).Languages[languageIndex] = text;
            }
        }
    }

    internal static string Translate(string sourceText, string targetCode)
    {
        string translation = string.Empty;

        try
        {
            string translationFromGoogle = "";

            //Using secret translate.googleapis.com API that is internally used by the Google Translate extension for Chrome and requires no authentication
            string url = string.Format(
                "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                "auto", targetCode, HttpUtility.UrlEncode(sourceText));

            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                wc.Encoding = System.Text.Encoding.UTF8;

                translationFromGoogle = wc.DownloadString(url);
            }

            // Get translated text
            var json = JsonConvert.DeserializeObject(translationFromGoogle);

            translation = ((((json as JArray).First() as JArray).First() as JArray).First() as JValue).Value.ToString();
        }
        catch
        {
            Main.Logger.Log("Failed translating: " + sourceText);
        }

        return translation;
    }
}
