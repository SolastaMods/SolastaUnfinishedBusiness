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
            LoadInputField();
            ServiceRepository.GetService<IInputService>().RegisterCommand(Settings.CTRL_E, (int)KeyCode.E, (int)KeyCode.LeftControl, -1, -1, -1, -1);
        }

        internal static void LoadInputField()
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
            InputField.transform.localPosition = new Vector3(0, contentText.transform.parent.localPosition.y - contentText.fontSize, 0);
        }

        private static string ParseText(string text)
        {
            if (Main.Settings.AllowExtraKeyboardCharactersInNames)
            {
                return new string(text.Where(n => !HeroNameContext.InvalidFilenameChars.Contains(n)).ToArray()).Trim();
            }
            else
            {
                return Gui.TrimInvalidCharacterNameSymbols(text).Trim();
            }
        }

        internal static void ExportInspectedCharacter(RulesetCharacterHero hero)
        {
            MessageModal messageModal = Gui.GuiService.GetScreen<MessageModal>();

            InputModalVisible = true;

            messageModal.Show(MessageModal.Severity.Informative1, "Message/&CharacterExportModalTitleDescription", INPUT_MODAL_MARK, "Message/&MessageOkTitle", "Message/&MessageCancelTitle", messageValidated, messageCancelled, true);

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
                    .EnumerateFiles(TacticalAdventuresApplication.GameCharactersDirectory, $"*.chr")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                newFirstName = newFirstName.TrimStart();

                if (newFirstName == string.Empty)
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

            var attunedItems = inventoryItems.Select(i => new { Item = i, Name = i.AttunedToCharacter }).ToList();
            var heroItemGuids = heroCharacter.Items.Select(i => new { Item = i, i.Guid }).ToList();
            var inventoryItemGuids = inventoryItems.Select(i => new { Item = i, i.Guid }).ToList();

            try
            {
                heroCharacter.Name = newFirstName;
                heroCharacter.SurName = newSurname;
                heroCharacter.BuiltIn = false;

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

        /// <summary>
        /// Required during de-serialization in the character inspection screen to prevent null-ref exceptions
        /// </summary>
        internal class DummyRulesetEntityService : IRulesetEntityService
        {
            public static IRulesetEntityService Instance => new DummyRulesetEntityService();

            public bool Dirty { get; set; }

            public Dictionary<ulong, RulesetEntity> RulesetEntities => new Dictionary<ulong, RulesetEntity>();

            public ulong GenerateGuid()
            {
                return 0;
            }

            public void RegisterEntity(RulesetEntity rulesetEntity)
            {
                try
                {
                    if (rulesetEntity is RulesetItem)
                    {
                        var ri = rulesetEntity as RulesetItem;
                    }
                    else if (rulesetEntity is RulesetItemProperty)
                    {
                        var ri = rulesetEntity as RulesetItemProperty;
                    }

                    RulesetEntities.Add(rulesetEntity.Guid, rulesetEntity);
                }
                catch (Exception)
                {

                }
            }

            public bool TryGetEntityByGuid(ulong guid, out RulesetEntity rulesetEntity)
            {
                return RulesetEntities.TryGetValue(guid, out rulesetEntity);
            }

            public void UnregisterEntity(RulesetEntity rulesetEntity)
            {
                if (rulesetEntity != null)
                {
                    RulesetEntities.Remove(rulesetEntity.Guid);
                }
            }

            public void SwapEntities(RulesetEntity oldRulesetEntity, RulesetEntity newRulesetEntity)
            {
                // Nothing to do
            }

            public void ResetCurrentGuid()
            {
                // Nothing to do
            }

            public bool TryGetEntityByGuidAndType<T>(ulong guid, out T rulesetEntity) where T : RulesetEntity
            {
                rulesetEntity = null;
                return false;
            }
        }
    }
}
