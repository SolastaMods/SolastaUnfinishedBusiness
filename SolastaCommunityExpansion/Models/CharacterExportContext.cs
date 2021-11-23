using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolastaModApi.Extensions;
using UnityEngine;

namespace SolastaCommunityExpansion.Models
{
    internal static class CharacterExportContext
    {
        /// <summary>
        /// Required during de-serialization in the character inspection screen to prevent null-ref exceptions
        /// </summary>
        internal class DummyRulesetEntityService : IRulesetEntityService
        {
            public static IRulesetEntityService Instance => new DummyRulesetEntityService();

            internal DummyRulesetEntityService()
            {
                // Nothing to do
            }

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

        internal const string INPUT_MODAL_MARK = "\n\n\n";

        internal static bool InputModalVisible { get; set; }

        internal static void Load()
        {
            ServiceRepository.GetService<IInputService>().RegisterCommand(Settings.CTRL_E, (int)KeyCode.E, (int)KeyCode.LeftControl, -1, -1, -1, -1);
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
                string newFirstName = Patches.MessageModalPatcher.InputField.text.Trim();
                string newSurname = string.Empty;
                bool hasSurname = hero.RaceDefinition.RacePresentation.HasSurName;

                HashSet<string> usedNames = Directory
                    .EnumerateFiles(TacticalAdventuresApplication.GameCharactersDirectory, $"*.chr")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (newFirstName == string.Empty)
                {
                    Gui.GuiService.ShowAlert("Message/&CharacterExportEmptyNameErrorDescription", "EA7171", 5);
                }
                else
                {
                    if (newFirstName.Contains(" "))
                    {
                        var a = newFirstName.Split(new char[] { ' ' }, 2);

                        newFirstName = a[0];
                        newSurname = hasSurname ? a[1] : string.Empty;
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

            // record current conditions, powers, spells and attunements
            var conditions = heroCharacter.ConditionsByCategory.ToList();
            var powers = heroCharacter.PowersUsedByMe.ToList();
            var spells = heroCharacter.SpellsCastByMe.ToList();

            var inventoryItems = new List<RulesetItem>();

            heroCharacter.CharacterInventory.EnumerateAllItems(inventoryItems);

            var attunedItems = inventoryItems.Select(i => new { Item = i, Name = i.AttunedToCharacter }).ToList();

            // record item guids
            var heroItemGuids = heroCharacter.Items.Select(i => new { Item = i, i.Guid }).ToList();
            var inventoryItemGuids = inventoryItems.Select(i => new { Item = i, i.Guid }).ToList();

            try
            {
                heroCharacter.Name = newFirstName;
                heroCharacter.SurName = newSurname;
                heroCharacter.BuiltIn = false;

                foreach (var item in attunedItems)
                {
                    item.Item.AttunedToCharacter = string.Empty;
                }

                foreach (var item in heroItemGuids)
                {
                    item.Item.SetGuid(0);
                }

                foreach (var item in inventoryItemGuids)
                {
                    item.Item.SetGuid(0);
                }

                heroCharacter.SetCurrentHitPoints(heroCharacter.CurrentHitPoints + heroCharacter.MissingHitPoints);
                heroCharacter.Unregister();
                heroCharacter.ResetForOutgame();
                heroCharacter.SetGuid(0);

                ServiceRepository.GetService<ICharacterPoolService>().SaveCharacter(heroCharacter, true);
            }
            finally
            {
                // restore guids
                heroCharacter.SetGuid(guid);

                // restore original values
                heroCharacter.Name = firstName;
                heroCharacter.SurName = surName;
                heroCharacter.BuiltIn = builtin;

                // restore conditions
                foreach (var kvp in conditions)
                {
                    heroCharacter.ConditionsByCategory.Add(kvp.Key, kvp.Value);
                }

                // restore active spells and effects
                heroCharacter.PowersUsedByMe.AddRange(powers);
                heroCharacter.SpellsCastByMe.AddRange(spells);

                // restore item status
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
            }
        }
    }
}
