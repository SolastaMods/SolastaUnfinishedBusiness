using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.CustomBehaviors;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SolastaUnfinishedBusiness.Models;

internal static class CharacterExportContext
{
    internal const string InputModalMark = "Message/&CharacterExportModalContentDescription";

    private static readonly char[] Separator = { ' ' };

    internal static TMP_InputField InputField { get; private set; }

    internal static void Load()
    {
        var messageModal = Gui.GuiService.GetScreen<MessageModal>();
        var contentText = messageModal.transform.FindChildRecursive("Content").GetComponent<TMP_Text>();
        var contentTextParent = contentText.transform.parent;

        var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
        var firstNameInputField = characterCreationScreen.transform.FindChildRecursive("FirstNameInputField")
            .GetComponent<TMP_InputField>();

        InputField = Object.Instantiate(firstNameInputField, contentTextParent.parent);

        InputField.characterLimit = 20;
        InputField.onValueChanged = null;
        InputField.fontAsset = contentText.font;
        InputField.pointSize = contentText.fontSize;
        InputField.transform.localPosition = new Vector3(-50,
            contentTextParent.localPosition.y - contentText.fontSize, 0);
    }

    [NotNull]
    private static string ParseText(string text)
    {
        return Gui.TrimInvalidCharacterNameSymbols(text).Trim();
    }

    internal static void ExportInspectedCharacter()
    {
        if (Global.CurrentCharacter is not RulesetCharacterHero hero)
        {
            return;
        }

        var messageModal = Gui.GuiService.GetScreen<MessageModal>();

        messageModal.Show(MessageModal.Severity.Informative1,
            Gui.Format("Message/&CharacterExportModalTitleDescription", hero.Name), InputModalMark,
            "Message/&MessageOkTitle", "Message/&MessageCancelTitle", MessageValidated, null);

        return;

        void MessageValidated()
        {
            var newFirstName = InputField.text;
            var newSurname = string.Empty;
            var hasSurname = hero.RaceDefinition.RacePresentation.HasSurName;

            var usedNames = Directory
                .EnumerateFiles(TacticalAdventuresApplication.GameCharactersDirectory, "*.chr")
                .Select(Path.GetFileNameWithoutExtension)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            newFirstName = newFirstName.TrimStart();

            if (string.IsNullOrEmpty(newFirstName))
            {
                Gui.GuiService.ShowAlert("Message/&CharacterExportEmptyNameErrorDescription", Gui.ColorFailure, 5);
            }
            else
            {
                if (newFirstName.Contains(" "))
                {
                    var a = newFirstName.Split(Separator, 2);

                    newFirstName = ParseText(a[0]);
                    newSurname = hasSurname ? ParseText(a[1]) : string.Empty;
                }
                else
                {
                    newFirstName = ParseText(newFirstName);
                }

                if (usedNames.Contains(newFirstName))
                {
                    Gui.GuiService.ShowAlert("Message/&CharacterExportDuplicateNameErrorDescription", Gui.ColorFailure,
                        5);
                }
                else
                {
                    ExportCharacter(hero, newFirstName, newSurname);
                }
            }
        }
    }

    private static void ExportCharacter([NotNull] RulesetCharacterHero heroCharacter, string newFirstName,
        string newSurname)
    {
        // record current name, etc..
        var guid = heroCharacter.Guid;
        var firstName = heroCharacter.Name;
        var surName = heroCharacter.SurName;
        var builtin = heroCharacter.BuiltIn;
        var hitPoints = heroCharacter.CurrentHitPoints;

        // record current conditions, powers, spells and attunements
        var conditions = heroCharacter.ConditionsByCategory.ToList();
        var powers = heroCharacter.PowersUsedByMe.ToList();
        var spells = heroCharacter.SpellsCastByMe.ToList();
        var inventoryItems = new List<RulesetItem>();

        heroCharacter.CharacterInventory.EnumerateAllItems(inventoryItems);

        var attunedItems = inventoryItems.ConvertAll(i => new { Item = i, Name = i.AttunedToCharacter });

        // NOTE: don't use Gui.GameLocation?. which bypasses Unity object lifetime check
        var customItems = (Gui.GameLocation
            ? inventoryItems.FindAll(i =>
                Gui.GameLocation.UserCampaign?.UserItems?.Exists(ui =>
                    ui.ReferenceItemDefinition == i.ItemDefinition) == true)
            : Enumerable.Empty<RulesetItem>()).ToList();

        var heroItemGuids = heroCharacter.Items.ConvertAll(i => new { Item = i, i.Guid });
        var inventoryItemGuids = inventoryItems.ConvertAll(i => new { Item = i, i.Guid });

        try
        {
            heroCharacter.Name = newFirstName;
            heroCharacter.SurName = newSurname;
            heroCharacter.BuiltIn = false;

            customItems.ForEach(x => inventoryItems.Remove(x));

            // change attunement to the new character name
            foreach (var item in attunedItems)
            {
                // change items attuned to this character name to the new name
                // unattune items attuned to another character in this characters inventory
                item.Item.AttunedToCharacter =
                    item.Item.AttunedToCharacter == firstName ? newFirstName : string.Empty;
            }

            heroCharacter.currentHitPoints = heroCharacter.TryGetAttributeValue(AttributeDefinitions.HitPoints);
            heroCharacter.Unregister();
            heroCharacter.ResetForOutgame();

            ServiceRepository.GetService<ICharacterPoolService>().SaveCharacter(heroCharacter, true);
        }
        finally
        {
            // restore original values
            heroCharacter.guid = guid;
            heroCharacter.Name = firstName;
            heroCharacter.SurName = surName;
            heroCharacter.BuiltIn = builtin;

            customItems.ForEach(x => inventoryItems.Add(x));

            // restore conditions
            foreach (var kvp in conditions)
            {
                heroCharacter.ConditionsByCategory.Add(kvp.Key, kvp.Value);
            }

            // restore items
            foreach (var item in attunedItems)
            {
                item.Item.AttunedToCharacter = item.Name;
            }

            foreach (var item in heroItemGuids)
            {
                item.Item.guid = item.Guid;
            }

            foreach (var item in inventoryItemGuids)
            {
                item.Item.guid = item.Guid;
            }

            // restore active spells and effects
            heroCharacter.PowersUsedByMe.AddRange(powers);
            heroCharacter.SpellsCastByMe.AddRange(spells);
            heroCharacter.currentHitPoints = hitPoints;

            heroCharacter.Register(false);
        }
    }
}
