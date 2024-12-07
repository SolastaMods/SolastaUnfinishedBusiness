using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using TA;
using UnityEngine;
using UnityEngine.UI;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class ToolsContext
{
    internal const int GamePartySize = 4;

    internal const int MinPartySize = 1;
    internal const int MaxPartySize = 6;

    internal const float CustomScale = 0.85f;

    internal const int DungeonMinLevel = 1;
    internal const int DungeonMaxLevel = 20;

    private const RestActivityDefinition.ActivityCondition ActivityConditionDisabled =
        (RestActivityDefinition.ActivityCondition)(-1001);

    private const string RespecName = "RestActivityRespec";

    private static List<string> BuiltInHeroNames { get; } = [];

    private static RestActivityDefinition RestActivityRespec { get; } = RestActivityDefinitionBuilder
        .Create(RespecName)
        .SetGuiPresentation(Category.RestActivity)
        .SetRestData(
            RestDefinitions.RestStage.AfterRest,
            RestType.LongRest,
            RestActivityDefinition.ActivityCondition.None,
            RespecName,
            string.Empty)
        .AddToDB();

    internal static bool IsBuiltIn(string name)
    {
        return BuiltInHeroNames.Contains(name);
    }

    internal static void Load()
    {
        var gameBuiltInCharactersDirectory = TacticalAdventuresApplication.GameBuiltInCharactersDirectory;

        if (Directory.Exists(gameBuiltInCharactersDirectory))
        {
            BuiltInHeroNames.AddRange(Directory
                .GetFiles(gameBuiltInCharactersDirectory)
                .Select(x => Path
                    .GetFileName(x)
                    .Replace(".chr", "")));
        }

        ServiceRepository.GetService<IFunctorService>().RegisterFunctor(RespecName, new FunctorRespec());
        SwitchRespec();
        // SwitchEncounterPercentageChance();
    }

    internal static void SwitchRespec()
    {
        RestActivityRespec.condition = Main.Settings.EnableRespecAction
            ? RestActivityDefinition.ActivityCondition.None
            : ActivityConditionDisabled;
    }

#if false
    internal static void SwitchEncounterPercentageChance()
    {
        foreach (var travelEventProbabilityDescription in DatabaseRepository.GetDatabase<TravelActivityDefinition>()
                     .SelectMany(x => x.RandomEvents)
                     .Where(x => x.EventDefinition.Name == "TravelEventEncounter"))
        {
            travelEventProbabilityDescription.basePercent = Main.Settings.EncounterPercentageChance;
        }
    }
#endif

    public static void Rebase(Transform parent, int max)
    {
        while (Main.Settings.DefaultPartyHeroes.Count > max)
        {
            var heroToDelete = Main.Settings.DefaultPartyHeroes.ElementAt(0);

            var child = parent.FindChildRecursive(heroToDelete);

            if (child)
            {
                child.GetComponentInChildren<Toggle>().isOn = false;
            }
        }
    }

    public static Transform CreateHeroCheckbox(Component character)
    {
        // ReSharper disable once Unity.UnknownResource
        var settingCheckboxItem = Resources.Load<GameObject>("Gui/Prefabs/Modal/Setting/SettingCheckboxItem");
        var smallToggleNoFrame = settingCheckboxItem.transform.Find("SmallToggleNoFrame");
        var checkBox = Object.Instantiate(smallToggleNoFrame, character.transform);
        var checkBoxRect = checkBox.GetComponent<RectTransform>();

        checkBox.name = "DefaultHeroToggle";
        checkBox.gameObject.SetActive(true);
        checkBox.Find("Background").gameObject.AddComponent<GuiTooltip>();

        checkBoxRect.anchoredPosition = new Vector2(160, 40);

        return checkBox;
    }

    internal static void Disable(RectTransform charactersTable)
    {
        for (var i = 0; i < charactersTable.childCount; i++)
        {
            var character = charactersTable.GetChild(i);
            var checkBoxToggle = character.GetComponentInChildren<Toggle>();

            if (checkBoxToggle)
            {
                checkBoxToggle.gameObject.SetActive(false);
            }
        }
    }

    internal sealed class FunctorRespec : Functor
    {
        internal static bool IsRespecing { get; private set; }
        internal static string OldHeroName { get; private set; }

        public override IEnumerator Execute(
            FunctorParametersDescription functorParameters, FunctorExecutionContext context)
        {
            var guiConsoleScreen = Gui.GuiService.GetScreen<GuiConsoleScreen>();
            var gameLocationScreenExploration = Gui.GuiService.GetScreen<GameLocationScreenExploration>();

            if (!guiConsoleScreen || !gameLocationScreenExploration)
            {
                yield break;
            }

            if (!gameLocationScreenExploration.Visible)
            {
                Gui.GuiService.ShowMessage(
                    MessageModal.Severity.Informative1,
                    "RestActivity/&RestActivityRespecTitle", "Message/&RespecMultiplayerAbortDescription",
                    "Message/&MessageOkTitle", string.Empty,
                    null, null);

                yield break;
            }

            IsRespecing = true;

            var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();
            var oldHero = functorParameters.RestingHero;
            var newHero = characterBuildingService.CreateNewCharacter().HeroCharacter;

            //Register generating new guid
            newHero.Register(true);

            OldHeroName = oldHero.Name;

            guiConsoleScreen.Hide(true);
            gameLocationScreenExploration.Hide(true);

            yield return StartRespec(newHero);

            if (IsRespecing)
            {
                FinalizeRespec(oldHero, newHero);
            }

            guiConsoleScreen.Show();
            gameLocationScreenExploration.Show();
        }

        private static IEnumerator StartRespec(RulesetCharacterHero hero)
        {
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();
            var characterCreationScreen = Gui.GuiService.GetScreen<CharacterCreationScreen>();

            restModalScreen.KeepCurrentState = true;
            restModalScreen.Hide(true);
            characterCreationScreen.OriginScreen = restModalScreen;
            characterCreationScreen.CurrentHero = hero;
            characterCreationScreen.Show();

            while (characterCreationScreen.currentHero != null)
            {
                yield return null;
            }

            characterCreationScreen.Hide();
            characterCreationScreen.RestoreOriginScreen();
            restModalScreen.Refresh();
            IsRespecing = !hero.TryGetHeroBuildingData(out _);
        }

        //TODO: prefer to use below code once I identify why RESPEC saves break on load
#if false
        private static void FinalizeRespec(
            [NotNull] RulesetCharacterHero oldHero,
            [NotNull] RulesetCharacterHero newHero)
        {
            //Init RESPEC
            var locationManager =
                ServiceRepository.GetService<IGameLocationService>() as GameLocationManager;
            var characterManager =
                ServiceRepository.GetService<IGameLocationCharacterService>() as GameLocationCharacterManager;
            var entityFactoryManager =
                ServiceRepository.GetService<IWorldLocationEntityFactoryService>() as
                    WorldLocationEntityFactoryManager;

            if (!characterManager || !entityFactoryManager)
            {
                IsRespecing = false;

                return;
            }

            var oldCharacter = GameLocationCharacter.GetFromActor(oldHero);
            var oldExperience = oldHero.GetAttribute(AttributeDefinitions.Experience);
            var oldGuid = oldHero.Guid;
            var newGuid = newHero.Guid;

            //Terminate all effects started by old character
            EffectHelpers.GetAllEffectsBySourceGuid(oldGuid).ForEach(e => e.DoTerminate(oldHero));

            //Replace source for all effects of new character
            EffectHelpers.GetAllEffectsBySourceGuid(newGuid).ForEach(e => e.SetGuid(oldGuid));

            //Replace source for all conditions of new character
            EffectHelpers.GetAllConditionsBySourceGuid(newGuid).ForEach(c => c.sourceGuid = oldGuid);

            //Unregister under new guid and assign older hero guid
            newHero.Unregister();

            //Create character will register new hero with oldGuid later on
            ServiceRepository.GetService<IRulesetEntityService>().SwapEntities(oldHero, newHero);

            //Copy tags and campaign stats
            newHero.Tags.AddRange(oldHero.Tags);
            newHero.Attributes[AttributeDefinitions.Experience] = oldExperience;
            newHero.criticalHits = oldHero.criticalHits;
            newHero.criticalFailures = oldHero.criticalFailures;
            newHero.inflictedDamage = oldHero.inflictedDamage;
            newHero.slainEnemies = oldHero.slainEnemies;
            newHero.sustainedInjuries = oldHero.sustainedInjuries;
            newHero.restoredHealth = oldHero.restoredHealth;
            newHero.usedMagicAndPowers = oldHero.usedMagicAndPowers;
            newHero.knockOuts = oldHero.knockOuts;

            //Handle conditions
            TransferConditionsOfCategory(oldHero, newHero, AttributeDefinitions.TagEffect);
            CleanupOldHeroConditions(oldHero, newHero);

            //Handle inventory
            //DropInventoryOnFloor(oldCharacter);

            //Create new character, spawn it, replace, and destroy old one
            var newCharacter = characterManager.CreateCharacter(oldCharacter.ControllerId, newHero, Side.Ally);

            TransferRelevantPersistentData(oldCharacter, newCharacter);
            entityFactoryManager.SpawnCharacter(newCharacter);
            entityFactoryManager.FinalizeSpawnCharacter(newCharacter);
            characterManager.ReplaceCharacter(oldCharacter, newCharacter);
            characterManager.RemoveCharacterFromTheGame(oldCharacter);
            characterManager.KillCharacter(oldCharacter, true, true, true, true);
            //entityFactoryManager.DestroyCharacter(oldCharacter);
            
            //Update game campaign party
            var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;

            gameCampaignCharacters.Find(x => x.RulesetCharacter == oldHero).RulesetCharacter = newHero;

            //Finalize RESPEC
            UpdateRestPanelUi(gameCampaignCharacters);

            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Informative1,
                "RestActivity/&RestActivityRespecTitle", "Message/&RespecSuccessfulDescription",
                "Message/&MessageOkTitle", string.Empty,
                null, null);

            IsRespecing = false;
        }
        
                
        private static void TransferRelevantPersistentData(
            GameLocationCharacter oldCharacter, GameLocationCharacter newCharacter)
        {
            newCharacter.ControllerId = oldCharacter.ControllerId;
            newCharacter.Orientation = oldCharacter.Orientation;
            newCharacter.LocationPosition = oldCharacter.LocationPosition;
            newCharacter.PerceptionState = oldCharacter.PerceptionState;
            newCharacter.ContextualFormation = oldCharacter.ContextualFormation;
        }
#endif

        private static void FinalizeRespec(
            [NotNull] RulesetCharacterHero oldHero,
            [NotNull] RulesetCharacterHero newHero)
        {
            var tags = oldHero.Tags;
            var experience = oldHero.GetAttribute(AttributeDefinitions.Experience);
            var gameCampaignCharacters = Gui.GameCampaign.Party.CharactersList;
            var characterManager =
                ServiceRepository.GetService<IGameLocationCharacterService>() as GameLocationCharacterManager;

            if (characterManager)
            {
                var gameLocationCharacter = GameLocationCharacter.GetFromActor(oldHero);

                var oldGuid = oldHero.Guid;
                var newGuid = newHero.Guid;

                //Terminate all effects started by old character
                EffectHelpers.GetAllEffectsBySourceGuid(oldGuid).ForEach(e => e.DoTerminate(oldHero));
                //Replace source for all effects of new character
                EffectHelpers.GetAllEffectsBySourceGuid(newGuid).ForEach(e => e.SetGuid(oldGuid));
                //Replace source for all conditions of new character
                EffectHelpers.GetAllConditionsBySourceGuid(newGuid).ForEach(c => c.sourceGuid = oldGuid);

                newHero.Unregister(); //unregister under new guid
                //Replace old character with new
                ServiceRepository.GetService<IRulesetEntityService>().SwapEntities(oldHero, newHero);
                newHero.Register(false); //register again under old guid

                newHero.Tags.AddRange(tags);
                newHero.Attributes[AttributeDefinitions.Experience] = experience;
                newHero.criticalHits = oldHero.criticalHits;
                newHero.criticalFailures = oldHero.criticalFailures;
                newHero.inflictedDamage = oldHero.inflictedDamage;
                newHero.slainEnemies = oldHero.slainEnemies;
                newHero.sustainedInjuries = oldHero.sustainedInjuries;
                newHero.restoredHealth = oldHero.restoredHealth;
                newHero.usedMagicAndPowers = oldHero.usedMagicAndPowers;
                newHero.knockOuts = oldHero.knockOuts;

                TransferConditionsOfCategory(oldHero, newHero, AttributeDefinitions.TagEffect);
                CleanupOldHeroConditions(oldHero, newHero);

                DropInventoryOnFloor(gameLocationCharacter);

                gameCampaignCharacters.Find(x => x.RulesetCharacter == oldHero).RulesetCharacter = newHero;

                UpdateRestPanelUi(gameCampaignCharacters);

                gameLocationCharacter.SetRuleset(newHero);

                var worldLocationEntityFactoryService =
                    ServiceRepository.GetService<IWorldLocationEntityFactoryService>();

                if (worldLocationEntityFactoryService.TryFindWorldCharacter(gameLocationCharacter,
                        out var worldLocationCharacter))
                {
                    worldLocationCharacter.GraphicsCharacter.RulesetCharacter = newHero;
                }

                characterManager.dirtyParty = true;
                characterManager.RefreshAllCharacters();
            }

            Gui.GuiService.ShowMessage(
                MessageModal.Severity.Informative1,
                "RestActivity/&RestActivityRespecTitle", "Message/&RespecSuccessfulDescription",
                "Message/&MessageOkTitle", string.Empty,
                null, null);

            IsRespecing = false;
        }

        private static void TransferConditionsOfCategory(RulesetActor oldHero, RulesetActor newHero, string category)
        {
            if (!oldHero.ConditionsByCategory.TryGetValue(category, out var conditions))
            {
                return;
            }

            newHero.AddConditionCategoryAsNeeded(category);
            newHero.AllConditions.AddRange(conditions);
            newHero.ConditionsByCategory[category].AddRange(conditions);
        }

        private static void CleanupOldHeroConditions(RulesetCharacterHero oldHero, RulesetCharacterHero newHero)
        {
            //Unregister all conditions that are not present in new hero
            oldHero.ConditionsByCategory
                .SelectMany(x => x.Value)
                .Where(c => !newHero.ConditionsByCategory.SelectMany(x => x.Value).Contains(c))
                .ToArray()
                .Do(c => c.Unregister());
            oldHero.AllConditions.Clear();
            oldHero.ConditionsByCategory.Clear();
        }

        private static void DropInventoryOnFloor([NotNull] GameLocationCharacter oldCharacter)
        {
            var oldHero = oldCharacter.RulesetCharacter;
            var position = oldCharacter.LocationPosition;
            var personalSlots = oldHero.CharacterInventory.PersonalContainer.InventorySlots;
            var slotsByName = oldHero.CharacterInventory.InventorySlotsByName;

            foreach (var equipedItem in personalSlots.Select(i => i.EquipedItem).Where(i => i != null))
            {
                DropItem(equipedItem, position);
            }

            foreach (var equipedItem in slotsByName.Select(s => s.Value.EquipedItem).Where(i => i != null))
            {
                DropItem(equipedItem, position);
            }
        }

        private static void DropItem(RulesetItem equipedItem, int3 position)
        {
            var inventoryCommandService = ServiceRepository.GetService<IInventoryCommandService>();

            equipedItem.AttunedToCharacter = string.Empty;

            if (equipedItem is RulesetItemSpellbook spellbook)
            {
                foreach (var scrollDefinition in spellbook.ScribedSpells
                             .Select(spellDefinition =>
                                 DatabaseRepository.GetDatabase<ItemDefinition>()
                                     .FirstOrDefault(item =>
                                         item.IsUsableDevice &&
                                         item.UsableDeviceDescription.UsableDeviceTags.Contains("Scroll") &&
                                         item.UsableDeviceDescription.DeviceFunctions.Any(function =>
                                             function.SpellDefinition == spellDefinition)))
                             .Where(scrollDefinition => scrollDefinition))
                {
                    var rulesetItem = new RulesetItem(scrollDefinition);

                    inventoryCommandService.CreateItemAtPosition(rulesetItem, position);
                }
            }
            else
            {
                inventoryCommandService.CreateItemAtPosition(equipedItem, position);
            }
        }

        private static void UpdateRestPanelUi(List<GameCampaignCharacter> gameCampaignCharacters)
        {
            var restModalScreen = Gui.GuiService.GetScreen<RestModal>();
            var restAfterPanel = restModalScreen.restAfterPanel;
            var characterPlatesTable = restAfterPanel.characterPlatesTable;

            for (var index = 0; index < characterPlatesTable.childCount; ++index)
            {
                var child = characterPlatesTable.GetChild(index);
                var component = child.GetComponent<CharacterPlateGame>();

                component.Unbind();

                if (index < gameCampaignCharacters.Count)
                {
                    component.Bind(gameCampaignCharacters[index].RulesetCharacter,
                        TooltipDefinitions.AnchorMode.BOTTOM_CENTER);
                    component.Refresh();
                }

                child.gameObject.SetActive(index < gameCampaignCharacters.Count);
            }
        }
    }
}
