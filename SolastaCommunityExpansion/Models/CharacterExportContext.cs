using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolastaModApi.Extensions;
using TMPro;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    internal static class CharacterExportContext
    {
        internal const string INPUT_MODAL_MARK = "Message/&CharacterExportModalContentDescription";

        internal static TMP_InputField InputField { get; private set; }

        internal static bool InputModalVisible { get; set; }

        internal static void Load()
        {
            MessageModal messageModal = Gui.GuiService.GetScreen<MessageModal>();
            TMP_Text contentText = messageModal.transform.FindChildRecursive("Content").GetComponent<TMP_Text>();

            CharacterCreationScreen characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();
            TMP_InputField firstNameInputField = characterCreationScreen.transform.FindChildRecursive("FirstNameInputField").GetComponent<TMP_InputField>();

            InputField = UnityEngine.Object.Instantiate(firstNameInputField, contentText.transform.parent.parent);

            InputField.characterLimit = 20;
            InputField.onValueChanged = null;
            InputField.fontAsset = contentText.font;
            InputField.pointSize = contentText.fontSize;
            InputField.transform.localPosition = new Vector3(-50, contentText.transform.parent.localPosition.y - contentText.fontSize, 0);
        }

        private static string ParseText(string text)
        {
            return Gui.TrimInvalidCharacterNameSymbols(text).Trim();
        }

        internal static void ExportInspectedCharacter(RulesetCharacterHero hero)
        {
            MessageModal messageModal = Gui.GuiService.GetScreen<MessageModal>();

            InputModalVisible = true;

            messageModal.Show(MessageModal.Severity.Informative1,
                "Message/&CharacterExportModalTitleDescription", INPUT_MODAL_MARK,
                "Message/&MessageOkTitle", "Message/&MessageCancelTitle", messageValidated, messageCancelled, true);

            void messageCancelled()
            {
                InputModalVisible = false;
            }

            void messageValidated()
            {
                string newFirstName = InputField.text;
                string newSurname = string.Empty;
                bool hasSurname = hero.RaceDefinition.RacePresentation.HasSurName;

                HashSet<string> usedNames = Directory
                    .EnumerateFiles(TacticalAdventuresApplication.GameCharactersDirectory, "*.chr")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                newFirstName = newFirstName.TrimStart();

                if (string.IsNullOrEmpty(newFirstName))
                {
                    Gui.GuiService.ShowAlert("Message/&CharacterExportEmptyNameErrorDescription", "EA7171", 5);
                }
                else
                {
                    if (newFirstName.Contains(" "))
                    {
                        var a = newFirstName.Split(new char[] { ' ' }, 2);

                        newFirstName = ParseText(a[0]);
                        newSurname = hasSurname ? ParseText(a[1]) ?? string.Empty : string.Empty;
                    }
                    else
                    {
                        newFirstName = ParseText(newFirstName);
                    }

                    if (usedNames.Contains(newFirstName))
                    {
                        Gui.GuiService.ShowAlert("Message/&CharacterExportDuplicateNameErrorDescription", "EA7171", 5);
                    }
                    else
                    {
                        ExportCharacter(hero, newFirstName, newSurname);
                    }
                }

                InputModalVisible = false;
            }
        }

        internal static void ExportCharacter(RulesetCharacterHero heroCharacter, string newFirstName, string newSurname)
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
                ? inventoryItems.FindAll(i => Gui.GameLocation.UserCampaign?.UserItems?.Exists(ui => ui.ReferenceItemDefinition == i.ItemDefinition) == true)
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
                    item.Item.AttunedToCharacter = item.Item.AttunedToCharacter == firstName ? newFirstName : string.Empty;
                }

                heroCharacter.SetCurrentHitPoints(heroCharacter.GetAttribute("HitPoints").CurrentValue);
                heroCharacter.Unregister();
                heroCharacter.ResetForOutgame();

                ServiceRepository.GetService<ICharacterPoolService>().SaveCharacter(heroCharacter, true);
            }
            finally
            {
                // restore original values
                heroCharacter.SetGuid(guid);
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
                    item.Item.SetGuid(item.Guid);
                }

                foreach (var item in inventoryItemGuids)
                {
                    item.Item.SetGuid(item.Guid);
                }

                // restore active spells and effects
                heroCharacter.PowersUsedByMe.AddRange(powers);
                heroCharacter.SpellsCastByMe.AddRange(spells);
                heroCharacter.SetCurrentHitPoints(hitPoints);

                heroCharacter.Register(false);
            }
        }
    }
}
