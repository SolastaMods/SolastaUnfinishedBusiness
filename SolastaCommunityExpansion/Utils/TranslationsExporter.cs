using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using SolastaModApi.Infrastructure;
using UnityEngine;

namespace SolastaCommunityExpansion.Utils;

internal class TranslationsExporter : MonoBehaviour
{
    private static TranslationsExporter _exporter;

    internal static readonly Dictionary<string, ExportStatus> CurrentExports = new();

    internal static readonly string[] AvailableLanguages = LocalizationManager.GetAllLanguagesCode().ToArray();

    private static TranslationsExporter Exporter
    {
        get
        {
            if (_exporter == null)
            {
                _exporter = new GameObject().AddComponent<TranslationsExporter>();
                DontDestroyOnLoad(_exporter.gameObject);
            }

            return _exporter;
        }
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

        newUserCampaign.Title = $"{languageCode}_{userCampaign.Title}";
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

        // USER DIALOGS
        foreach (var dialog in userCampaign.UserDialogs)
        {
            dialog.Description = Translations.Translate(dialog.Description, languageCode);

            foreach (var userDialogState in dialog.AllDialogStates)
            {
                yield return Update();

                foreach (var dialogLine in userDialogState.DialogLines)
                {
                    dialogLine.TextLine = Translations.Translate(dialogLine.TextLine, languageCode);
                }
            }
        }

        // USER LOCATIONS
        foreach (var userLocation in userCampaign.UserLocations)
        {
            foreach (var gadget in userLocation.GadgetsByName.Values)
            {
                yield return Update();

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
                                newStringsList.Add(Translations.Translate(stringValue, languageCode));
                            }

                            parameterValue.StringsList = newStringsList;

                            if (parameterValue.StringValue != string.Empty)
                            {
                                parameterValue.StringValue =
                                    Translations.Translate(parameterValue.StringValue, languageCode);
                            }

                            break;
                    }
                }
            }
        }

        // USER QUESTS
        foreach (var quest in userCampaign.UserQuests)
        {
            quest.Description = Translations.Translate(quest.Description, languageCode);

            foreach (var userQuestStep in quest.AllQuestStepDescriptions)
            {
                userQuestStep.Title = Translations.Translate(userQuestStep.Title, languageCode);
                userQuestStep.Description = Translations.Translate(userQuestStep.Description, languageCode);

                foreach (var outcome in userQuestStep.OutcomesTable)
                {
                    yield return Update();

                    outcome.DescriptionText = Translations.Translate(outcome.DescriptionText, languageCode);
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
