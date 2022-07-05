using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.Utils;
using UnityEngine;

namespace SolastaCommunityExpansion.Models;

internal class UserCampaignsTranslatorContext : MonoBehaviour
{
    internal static string CE2_TRANSLATION_TAG = "CE2 auto translation\n";

    private static UserCampaignsTranslatorContext _exporter;

    internal static readonly Dictionary<string, ExportStatus> CurrentExports = new();

    private static UserCampaignsTranslatorContext Exporter
    {
        get
        {
            if (_exporter != null)
            {
                return _exporter;
            }

            _exporter = new GameObject().AddComponent<UserCampaignsTranslatorContext>();
            DontDestroyOnLoad(_exporter.gameObject);

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
        yield return null;

        var current = 0;
        var total =
            userCampaign.UserDialogs
                .SelectMany(x => x.AllDialogStates)
                .SelectMany(x => x.DialogLines)
                .Count() +
            userCampaign.UserItems.Count +
            userCampaign.UserMonsters.Count +
            userCampaign.UserNpcs.Count +
            userCampaign.UserMerchantInventories.Count +
            userCampaign.UserLootPacks.Count +
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

        userCampaign.Description = Translations.Translate(userCampaign.Description, languageCode);
        userCampaign.TechnicalInfo = CE2_TRANSLATION_TAG
                                     + Translations.Translate(userCampaign.TechnicalInfo, languageCode);

        // USER DIALOGS
        foreach (var dialog in userCampaign.UserDialogs)
        {
            dialog.Description = Translations.Translate(dialog.Description, languageCode);

            foreach (var userDialogState in dialog.AllDialogStates
                         .Where(x =>
                             x.Type is "AnswerChoice" or "CharacterSpeech" or "NpcSpeech"))
            {
                foreach (var dialogLine in userDialogState.DialogLines)
                {
                    yield return Update();

                    dialogLine.TextLine = Translations.Translate(dialogLine.TextLine, languageCode);
                }
            }
        }

        // USER ITEMS
        foreach (var item in userCampaign.UserItems)
        {
            yield return Update();

            item.Title = Translations.Translate(item.Title, languageCode);
            item.Description = Translations.Translate(item.Title, languageCode);

            if (item.DocumentFragments.Count == 0)
            {
                continue;
            }

            var newDocumentFragment = item.DocumentFragments
                .Select(documentFragment => Translations.Translate(documentFragment, languageCode)).ToList();

            item.DocumentFragments = newDocumentFragment;
        }

        // USER LOCATIONS
        foreach (var userLocation in userCampaign.UserLocations)
        {
            userLocation.Title = Translations.Translate(userLocation.Title, languageCode);
            userLocation.Description = Translations.Translate(userLocation.Description, languageCode);

            foreach (var gadget in userLocation.GadgetsByName.Values)
            {
                yield return Update();

                foreach (var parameterValue in gadget.ParameterValues)
                {
                    if (parameterValue.GadgetParameterDescription.Type != GadgetBlueprintDefinitions.Type.Npc &&
                        parameterValue.GadgetParameterDescription.Type != GadgetBlueprintDefinitions.Type.Quest &&
                        parameterValue.GadgetParameterDescription.Type != GadgetBlueprintDefinitions.Type.Speech &&
                        parameterValue.GadgetParameterDescription.Type != GadgetBlueprintDefinitions.Type.SpeechList &&
                        parameterValue.GadgetParameterDescription.Type != GadgetBlueprintDefinitions.Type.LoreFormat)
                    {
                        continue;
                    }

                    var newStringsList = parameterValue.StringsList
                        .Select(stringValue => Translations.Translate(stringValue, languageCode)).ToList();

                    parameterValue.StringsList = newStringsList;

                    if (parameterValue.StringValue != string.Empty)
                    {
                        parameterValue.StringValue =
                            Translations.Translate(parameterValue.StringValue, languageCode);
                    }
                }
            }
        }

        // USER QUESTS
        foreach (var quest in userCampaign.UserQuests)
        {
            quest.Title = Translations.Translate(quest.Title, languageCode);
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

        // USER MONSTERS
        foreach (var monster in userCampaign.UserMonsters)
        {
            yield return Update();

            monster.Title = Translations.Translate(monster.Title, languageCode);
            monster.Description = Translations.Translate(monster.Description, languageCode);
        }

        // USER NPCs
        foreach (var npc in userCampaign.UserNpcs)
        {
            yield return Update();

            npc.Title = Translations.Translate(npc.Title, languageCode);
            npc.Description = Translations.Translate(npc.Description, languageCode);
        }

        // USER MERCHANT INVENTORIES
        foreach (var merchantInventory in userCampaign.UserMerchantInventories)
        {
            yield return Update();

            merchantInventory.Title = Translations.Translate(merchantInventory.Title, languageCode);
            merchantInventory.Description = Translations.Translate(merchantInventory.Description, languageCode);
        }

        // USER LOOT PACKS
        foreach (var lootPack in userCampaign.UserLootPacks)
        {
            yield return Update();

            lootPack.Title = Translations.Translate(lootPack.Title, languageCode);
            lootPack.Description = Translations.Translate(lootPack.Description, languageCode);
        }

        CurrentExports.Remove(exportName);

        var userCampaignPoolService = ServiceRepository.GetService<IUserCampaignPoolService>();

        userCampaignPoolService.SaveUserCampaign(userCampaign);
    }

    internal sealed class ExportStatus
    {
        internal Coroutine Coroutine;
        internal string LanguageCode;
        internal float PercentageComplete;
    }
}
