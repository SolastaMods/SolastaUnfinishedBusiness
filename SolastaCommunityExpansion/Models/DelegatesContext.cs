using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.Models;

//
// WORK IN PROGRESS: GOAL IS TO REUSE AS MANY AS IN GAME DELEGATES AS POSSIBLE INSTEAD OF PATCHING CODE
//
internal static class DelegatesContext
{
    internal static void Load()
    {
        var gameService = ServiceRepository.GetService<IGameService>();

        gameService.GameCreated += GameCreated;
        gameService.GameDestroying += GameDestroying;
    }

    private static void GameCreated()
    {
        Main.Logger.Log("Game Created");

        var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

        gameLocationService.LocationReady += LocationReady;
        gameLocationService.LocationUnloading += LocationUnloading;

        var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

        characterBuildingService.CharacterLevelUpStarted += CharacterLevelUpStarted;
    }

    private static void GameDestroying()
    {
        Main.Logger.Log("Game Destroying");

        var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

        gameLocationService.LocationReady -= LocationReady;
        gameLocationService.LocationUnloading -= LocationUnloading;
    }

    private static void LocationReady(string locationDefinitionName, string userLocationTitle)
    {
        Main.Logger.Log("Location Loaded");

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        gameLocationCharacterService.CharacterCreated += CharacterCreated;
        gameLocationCharacterService.CharacterKilled += CharacterKilled;
        gameLocationCharacterService.CharacterDestroying += CharacterDestroying;

        foreach (var rulesetCharacterHero in gameLocationCharacterService.PartyCharacters
            .Select(gameLocationCharacter =>
                gameLocationCharacter.RulesetCharacter as RulesetCharacterHero
                ?? gameLocationCharacter.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero)
            .Where(rulesetCharacterHero => rulesetCharacterHero != null))
        {
            rulesetCharacterHero.ItemEquipedCallback += ItemEquiped;
            rulesetCharacterHero.CharacterInventory.ItemEquiped += ItemEquiped;
            rulesetCharacterHero.CharacterInventory.ItemAltered += ItemAltered;
            rulesetCharacterHero.CharacterInventory.ItemUnequiped += ItemUnequiped;
            rulesetCharacterHero.CharacterInventory.ItemReleased += ItemReleased;
        }

        var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();

        gameLocationActionService.ActionStarted += ActionStarted;
        gameLocationActionService.ActionChainStarted += ActionChainStarted;
        gameLocationActionService.ActionChainFinished += ActionChainFinished;
        gameLocationActionService.MagicEffectPreparing += MagicEffectPreparing;
        gameLocationActionService.MagicEffectPreparingOnTarget += MagicEffectPreparingOnTarget;
        gameLocationActionService.MagicEffectLaunch += MagicEffectLaunch;
        gameLocationActionService.MagicEffectCastOnTarget += MagicEffectCastOnTarget;
        gameLocationActionService.MagicEffectCastOnZone += MagicEffectCastOnZone;
        gameLocationActionService.MagicEffectBeforeHitTarget += MagicEffectBeforeHitTarget;
        gameLocationActionService.MagicEffectHitTarget += MagicEffectHitTarget;
        gameLocationActionService.SpellCast += SpellCast;
        gameLocationActionService.ActionUsed += ActionUsed;
        gameLocationActionService.ShoveActionUsed += ShoveActionUsed;
        gameLocationActionService.ItemUsed += ItemUsed;
    }

    private static void LocationUnloading(string locationDefinitionName, string userLocationTitle)
    {
        Main.Logger.Log("Location Unloading");

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        gameLocationCharacterService.CharacterCreated -= CharacterCreated;
        gameLocationCharacterService.CharacterKilled -= CharacterKilled;
        gameLocationCharacterService.CharacterDestroying -= CharacterDestroying;

        foreach (var rulesetCharacterHero in gameLocationCharacterService.PartyCharacters
            .Select(gameLocationCharacter =>
             gameLocationCharacter.RulesetCharacter as RulesetCharacterHero
             ?? gameLocationCharacter.RulesetCharacter.OriginalFormCharacter as RulesetCharacterHero)
            .Where(rulesetCharacterHero => rulesetCharacterHero != null))
        {
            rulesetCharacterHero.ItemEquipedCallback -= ItemEquiped;
            rulesetCharacterHero.CharacterInventory.ItemEquiped -= ItemEquiped;
            rulesetCharacterHero.CharacterInventory.ItemAltered -= ItemAltered;
            rulesetCharacterHero.CharacterInventory.ItemUnequiped -= ItemUnequiped;
            rulesetCharacterHero.CharacterInventory.ItemReleased -= ItemReleased;
        }

        var gameLocationActionService = ServiceRepository.GetService<IGameLocationActionService>();

        gameLocationActionService.ActionStarted -= ActionStarted;
        gameLocationActionService.ActionChainStarted -= ActionChainStarted;
        gameLocationActionService.ActionChainFinished -= ActionChainFinished;
        gameLocationActionService.MagicEffectPreparing -= MagicEffectPreparing;
        gameLocationActionService.MagicEffectPreparingOnTarget -= MagicEffectPreparingOnTarget;
        gameLocationActionService.MagicEffectLaunch -= MagicEffectLaunch;
        gameLocationActionService.MagicEffectCastOnTarget -= MagicEffectCastOnTarget;
        gameLocationActionService.MagicEffectCastOnZone -= MagicEffectCastOnZone;
        gameLocationActionService.MagicEffectBeforeHitTarget -= MagicEffectBeforeHitTarget;
        gameLocationActionService.MagicEffectHitTarget -= MagicEffectHitTarget;
        gameLocationActionService.SpellCast -= SpellCast;
        gameLocationActionService.ActionUsed -= ActionUsed;
        gameLocationActionService.ShoveActionUsed -= ShoveActionUsed;
        gameLocationActionService.ItemUsed -= ItemUsed;
    }

    private static void CharacterLevelUpStarted(RulesetCharacterHero hero)
    {
        Main.Logger.Log("Character Level Up Started");
    }

    private static void CharacterCreated(GameLocationCharacter character)
    {
        Main.Logger.Log("Character Created");
    }

    private static void CharacterDestroying(GameLocationCharacter character)
    {
        Main.Logger.Log("Character Destroying");
    }

    private static void CharacterKilled(GameLocationCharacter character)
    {
        Main.Logger.Log("Character Killed");

        var attacker = Global.ActivePlayerCharacter?.RulesetCharacter;

        if (attacker == null)
        {
            return;
        }

        var features = new List<FeatureDefinition>();

        attacker.EnumerateFeaturesToBrowse<IOnCharacterKill>(features);

        // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
        foreach (IOnCharacterKill characterKill in features)
        {
            characterKill.OnCharacterKill(character);
        }
    }

    private static void ItemEquiped(RulesetCharacterHero hero, RulesetItem item)
    {
        Main.Logger.Log("Item Equipped Hero");
    }

    private static void ItemEquiped(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Logger.Log("Item Equipped");
    }

    private static void ItemAltered(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Logger.Log("Item Altered");
    }

    private static void ItemUnequiped(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Logger.Log("Item Unequipped");
    }

    private static void ItemReleased(RulesetItem item, bool canKeep)
    {
        Main.Logger.Log("Item Released");
    }

    private static void ActionStarted([NotNull] CharacterAction characterAction)
    {
        Main.Logger.Log("Action Started");

        Global.ActionStarted(characterAction);
    }

    private static void ActionChainStarted(CharacterActionChainParams characterActionChainParams)
    {
        Main.Logger.Log("Action Chain Started");
    }

    private static void ActionChainFinished(
        CharacterActionChainParams characterActionChainParams,
        bool aborted)
    {
        Main.Logger.Log("Action Chain Finished");
    }

    private static void MagicEffectPreparing(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log("Magic Effect Preparing");
    }

    private static void MagicEffectPreparingOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log("Magic Effect Preparing On Target");
    }

    private static void MagicEffectLaunch(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log("Magic Effect Launch");
    }

    private static void MagicEffectCastOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log("Magic Effect Cast On Target");
    }

    private static void MagicEffectCastOnZone(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log("Magic Effect Cast On Zone");
    }

    private static void MagicEffectBeforeHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log("Magic Effect Before Hit Target");
    }

    private static void MagicEffectHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log("Magic Effect Hit Target");
    }

    private static void SpellCast(string spellDefinitionName, int fromDevice)
    {
        Main.Logger.Log("Spell Cast");
    }

    private static void ActionUsed(
        GameLocationCharacter actingCharacter,
        CharacterActionParams actionParams,
        ActionDefinition actionDefinition)
    {
        Main.Logger.Log("Action Used");
    }

    private static void ShoveActionUsed(
        GameLocationCharacter actingCharacter,
        GameLocationCharacter attackingCharacter,
        ActionDefinition actionDefinition,
        bool success)
    {
        Main.Logger.Log("Shove Action Used");
    }

    private static void ItemUsed(string itemDefinitionName)
    {
        Main.Logger.Log("Item Used");
    }
}
