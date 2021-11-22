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
                Main.Log($"Creating new DummyRulesetEntityService");
            }

            public bool Dirty { get; set; }

            public Dictionary<ulong, RulesetEntity> RulesetEntities => new Dictionary<ulong, RulesetEntity>();

            public ulong GenerateGuid()
            {
                Main.Log("GenerateGuid");
                return 0;
            }

            public void RegisterEntity(RulesetEntity rulesetEntity)
            {
                try
                {
                    if (rulesetEntity is RulesetItem)
                    {
                        var ri = rulesetEntity as RulesetItem;
                        Main.Log($"RegisterEntity: Type= {rulesetEntity.GetType().FullName}, Name={ri?.ItemDefinition?.Name}, Guid={ri?.ItemDefinition?.GUID}");
                    }
                    else if (rulesetEntity is RulesetItemProperty)
                    {
                        var ri = rulesetEntity as RulesetItemProperty;
                        Main.Log($"RegisterEntity: Type= {rulesetEntity.GetType().FullName}, Name={ri?.FeatureDefinition?.Name}, Guid={ri?.FeatureDefinition?.GUID}");
                    }
                    else
                    {
                        Main.Log($"RegisterEntity: Type={rulesetEntity.GetType().FullName}, {rulesetEntity?.Name}");
                    }

                    RulesetEntities.Add(rulesetEntity.Guid, rulesetEntity);
                }
                catch (Exception ex)
                {
                    Main.Log($"RegisterEntity: {ex}");
                }
            }

            public bool TryGetEntityByGuid(ulong guid, out RulesetEntity rulesetEntity)
            {
                Main.Log($"TryGetEntityByGuid: {guid}");
                return RulesetEntities.TryGetValue(guid, out rulesetEntity);
            }

            public void UnregisterEntity(RulesetEntity rulesetEntity)
            {
                Main.Log($"UnregisterEntity: {rulesetEntity?.Guid}");

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

        internal static void Load()
        {
            ServiceRepository.GetService<IInputService>().RegisterCommand(Settings.CTRL_E, (int)KeyCode.E, (int)KeyCode.LeftControl, -1, -1, -1, -1);
        }

        internal static void ExportInspectedCharacter(RulesetCharacterHero hero)
        {
            IGuiService guiService = ServiceRepository.GetService<IGuiService>();
            MessageModal messageModal = guiService.GetScreen<MessageModal>();

            messageModal.Show(MessageModal.Severity.Informative1, "Enter the hero name", INPUT_MODAL_MARK, "Ok", "Cancel", messageValidated, null, true);

            void messageValidated()
            {
                Main.Log("Ok pressed");

                string newFirstName = Patches.MessageModalPatcher.InputField.text;
                string newSurname = string.Empty;
                RacePresentation racePresentation = hero.RaceDefinition.RacePresentation;
                HashSet<string> usedNames = Directory
                    .EnumerateFiles(TacticalAdventuresApplication.GameCharactersDirectory, $"*.chr")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (racePresentation.HasSurName && newFirstName.Contains(" "))
                {
                    var a = newFirstName.Split(new char[] { ' ' }, 2);

                    newFirstName = a[0];
                    newSurname = a[1];
                }

                if (!usedNames.Contains(newFirstName))
                {
                    DoExportInspectedCharacter(hero, newFirstName, newSurname);
                }
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

                // TODO: -- need help
                // TODO: remove weapon modifiers and effects
                // TODO: fully rest and restore hit points 

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

                // finally, save the character
                ServiceRepository
                    .GetService<ICharacterPoolService>()
                    .SaveCharacter(heroCharacter, true);
            }
            finally
            {
                // and finally, finally, restore everything.

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
