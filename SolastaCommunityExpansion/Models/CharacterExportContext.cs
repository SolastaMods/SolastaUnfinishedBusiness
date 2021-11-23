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

            messageModal.Show(MessageModal.Severity.Informative1, "Enter the hero name to export:", INPUT_MODAL_MARK, "Ok", "Cancel", messageValidated, messageCancelled, true);

            void messageCancelled()
            {
                InputModalVisible = false;
            }

            void messageValidated()
            {
                string newFirstName = Patches.MessageModalPatcher.InputField.text.Trim();
                string newSurname = string.Empty;
                RacePresentation racePresentation = hero.RaceDefinition.RacePresentation;
                HashSet<string> usedNames = Directory
                    .EnumerateFiles(TacticalAdventuresApplication.GameCharactersDirectory, $"*.chr")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (newFirstName == string.Empty)
                {
                    Gui.GuiService.ShowAlert("Export Cancelled: Please enter a valid name.", "EA7171", 5);
                }
                else
                {
                    if (newFirstName.Contains(" "))
                    {
                        var a = newFirstName.Split(new char[] { ' ' }, 2);

                        newFirstName = a[0];

                        if (racePresentation.HasSurName)
                        {
                            newSurname = a[1];
                        }
                    }

                    if (usedNames.Contains(newFirstName))
                    {
                        Gui.GuiService.ShowAlert("Export Cancelled: A hero with this name already exists in the pool.\nPlease try again with a different name.", "EA7171", 5);
                    }
                    else
                    {
                        DoExportInspectedCharacter(hero, newFirstName, newSurname);
                    }
                }

                InputModalVisible = false;
            }
        }

        internal static void DoExportInspectedCharacter(RulesetCharacterHero heroCharacter, string newFirstName, string newSurname)
        {
            // record current name, etc..
            var firstName = heroCharacter.Name;
            var surName = heroCharacter.SurName;
            var builtin = heroCharacter.BuiltIn;
            var guid = heroCharacter.Guid;

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

                // remove active conditions (or filter out during serialization)
                heroCharacter.ConditionsByCategory.Clear();

                // remove spells and effects (or filter out during serialization)
                heroCharacter.PowersUsedByMe.Clear();
                heroCharacter.SpellsCastByMe.Clear();

                // TODO: remove weapon modifiers and effects

                // remove attunement, attuned items don't work well in the character inspection screen out of game
                foreach (var item in attunedItems)
                {
                    item.Item.AttunedToCharacter = string.Empty;
                }

                // clear guids
                heroCharacter.SetGuid(0);

                foreach (var item in heroItemGuids)
                {
                    item.Item.SetGuid(0);
                }

                foreach (var item in inventoryItemGuids)
                {
                    item.Item.SetGuid(0);
                }

                // TODO: check if below rest will affect in game hero

                // TODO: fully rest but below code throwns an exception on the finally block
                //Game game = ServiceRepository.GetService<IGameService>().Game;
                //GameTime gameTime = game.GameCampaign.GameTime;
                //TimeInfo timeInfo = gameTime.TimeInfo;
                //heroCharacter.ApplyRest(RuleDefinitions.RestType.LongRest, false, timeInfo);

                // finally, save the character
                ServiceRepository
                    .GetService<ICharacterPoolService>()
                    .SaveCharacter(heroCharacter, true);
            }
            finally
            {
                // and finally, finally, restore everything

                // TODO: check these things are really restored

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

                // restore attunements
                foreach (var item in attunedItems) { item.Item.AttunedToCharacter = item.Name; }

                // restore guids
                heroCharacter.SetGuid(guid);

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
