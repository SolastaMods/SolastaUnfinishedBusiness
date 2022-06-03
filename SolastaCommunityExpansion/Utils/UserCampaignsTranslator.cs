using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Utils;

internal class UserCampaignsTranslator : MonoBehaviour
{
    private static UserCampaignsTranslator _exporter;

    internal static readonly Dictionary<string, ExportStatus> CurrentExports = new();

    internal static readonly string[] AvailableLanguages = {"de", "en", "es", "fr", "it", "pt", "ru", "zh-CN"};

    private static UserCampaignsTranslator Exporter
    {
        get
        {
            if (_exporter == null)
            {
                _exporter = new GameObject().AddComponent<UserCampaignsTranslator>();
                DontDestroyOnLoad(_exporter.gameObject);
            }

            return _exporter;
        }
    }

    internal static string Translate(string sourceText, string targetCode)
    {
        var translation = string.Empty;

        try
        {
            var translationFromGoogle = "";

            //Using secret translate.googleapis.com API that is internally used by the Google Translate extension for Chrome and requires no authentication
            var url = string.Format(
                "https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                "auto", targetCode, HttpUtility.UrlEncode(sourceText));

            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent",
                    "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2228.0 Safari/537.36");
                wc.Encoding = Encoding.UTF8;

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

    internal static void Cancel(string exportName)
    {
        if (!CurrentExports.ContainsKey(exportName) || CurrentExports[exportName].Coroutine == null)
        {
            return;
        }

        Exporter.StopCoroutine(CurrentExports[exportName].Coroutine);
        CurrentExports.Remove(exportName);
    }

    internal static void TranslateUserCampaign(string languageCode, string exportName, UserCampaign userCampaign)
    {
        var newUserCampaign = userCampaign.DeepCopy();
        var oldUserCampaignTitle = Regex.Replace(userCampaign.Title, @"\[.+\] - (.*)", "$1");

        newUserCampaign.Title = $"[{languageCode}] - {oldUserCampaignTitle}";
        newUserCampaign.IsWorkshopItem = false;

        var coroutine = TranslateUserCampaignRoutine(languageCode, exportName, newUserCampaign);

        CurrentExports.Add(exportName,
            new ExportStatus
            {
                Coroutine = Exporter.StartCoroutine(coroutine), PercentageComplete = 0f, LanguageCode = languageCode
            });
    }

    private static IEnumerator TranslateUserCampaignRoutine(string languageCode, string exportName,
        UserCampaign userCampaign)
    {
        var start = DateTime.UtcNow;

        Main.Log($"Export started: {DateTime.UtcNow:G}");

        yield return null;

        var current = 0;
        var total =
            userCampaign.UserDialogs
                .SelectMany(x => x.AllDialogStates)
                .SelectMany(x => x.DialogLines)
                .Count() +
            userCampaign.UserItems
                .Count() +
            userCampaign.UserLocations
                .SelectMany(x => x.GadgetsByName)
                .Count() +
            userCampaign.UserQuests
                .SelectMany(x => x.AllQuestStepDescriptions)
                .SelectMany(x => x.OutcomesTable)
                .Count();

        IEnumerator Update()
        {
            current++;
            CurrentExports[exportName].PercentageComplete = (float)current / total;

            yield return null;
        }

        userCampaign.Description = Translate(userCampaign.Description, languageCode);

        // USER DIALOGS
        foreach (var dialog in userCampaign.UserDialogs)
        {
            dialog.Description = Translate(dialog.Description, languageCode);

            foreach (var userDialogState in dialog.AllDialogStates
                         .Where(x =>
                             x.Type == "AnswerChoice"
                             || x.Type == "CharacterSpeech"
                             || x.Type == "NpcSpeech"))
            {
                foreach (var dialogLine in userDialogState.DialogLines)
                {
                    yield return Update();

                    dialogLine.TextLine = Translate(dialogLine.TextLine, languageCode);
                }
            }
        }

        // USER ITEMS
        foreach (var item in userCampaign.UserItems)
        {
            yield return Update();

            item.Title = Translate(item.Title, languageCode);
            item.Description = Translate(item.Title, languageCode);

            if (item.DocumentFragments.Count == 0)
            {
                continue;
            }

            var newdocumentFragment = new List<string>();

            foreach (var documentFragment in item.DocumentFragments)
            {
                newdocumentFragment.Add(Translate(documentFragment, languageCode));
            }

            item.DocumentFragments = newdocumentFragment;
        }

        // USER LOCATIONS
        foreach (var userLocation in userCampaign.UserLocations)
        {
            userLocation.Description = Translate(userLocation.Description, languageCode);

            foreach (var gadget in userLocation.GadgetsByName.Values)
            {
                yield return Update();

                foreach (var parameterValue in gadget.ParameterValues)
                {
                    if (parameterValue.GadgetParameterDescription.Type == GadgetBlueprintDefinitions.Type.Npc
                        || parameterValue.GadgetParameterDescription.Type == GadgetBlueprintDefinitions.Type.Speech
                        || parameterValue.GadgetParameterDescription.Type == GadgetBlueprintDefinitions.Type.SpeechList
                        || parameterValue.GadgetParameterDescription.Type == GadgetBlueprintDefinitions.Type.LoreFormat)
                    {
                        var newStringsList = new List<string>();

                        foreach (var stringValue in parameterValue.StringsList)
                        {
                            newStringsList.Add(Translate(stringValue, languageCode));
                        }

                        parameterValue.StringsList = newStringsList;

                        if (parameterValue.StringValue != string.Empty)
                        {
                            parameterValue.StringValue =
                                Translate(parameterValue.StringValue, languageCode);
                        }
                    }
                }
            }
        }

        // USER QUESTS
        foreach (var quest in userCampaign.UserQuests)
        {
            quest.Title = Translate(quest.Description, languageCode);
            quest.Description = Translate(quest.Description, languageCode);

            foreach (var userQuestStep in quest.AllQuestStepDescriptions)
            {
                userQuestStep.Title = Translate(userQuestStep.Title, languageCode);
                userQuestStep.Description = Translate(userQuestStep.Description, languageCode);

                foreach (var outcome in userQuestStep.OutcomesTable)
                {
                    yield return Update();

                    outcome.DescriptionText = Translate(outcome.DescriptionText, languageCode);
                }
            }
        }

        CurrentExports.Remove(exportName);

        var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();

        userCampaignPoolService.SaveUserCampaign(userCampaign);

        Main.Log($"Export finished: {DateTime.UtcNow}, {DateTime.UtcNow - start}.");
    }

    internal class ExportStatus
    {
        internal Coroutine Coroutine;
        internal string LanguageCode;
        internal float PercentageComplete;
    }
}
