using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;
using TA;
using UnityEngine;

namespace SolastaCommunityExpansion.Models;

internal static class DelegatesContext
{
    internal static void Load()
    {
        var gameService = ServiceRepository.GetService<IGameService>();

        gameService.GameCreated += GameCreated;
        gameService.GameDestroying += GameDestroying;
    }

    //
    // IGameService
    //

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

        var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

        characterBuildingService.CharacterLevelUpStarted -= CharacterLevelUpStarted;
    }

    //
    // IGameLocationService
    //

    private static void LocationReady(string locationDefinitionName, string userLocationTitle)
    {
        Main.Logger.Log("Location Ready");

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        gameLocationCharacterService.CharacterCreated += CharacterCreated;
        gameLocationCharacterService.CharacterRevealed += CharacterRevealed;
        gameLocationCharacterService.CharacterKilled += CharacterKilled;
        gameLocationCharacterService.CharacterDestroying += CharacterDestroying;

        foreach (var gameLocationCharacter in gameLocationCharacterService.ValidCharacters)
        {
            gameLocationCharacter.Placed += Placed;
            gameLocationCharacter.MoveStarted += MoveStarted;
            gameLocationCharacter.PrepareChargeStarted += PrepareChargeStarted;
            gameLocationCharacter.ChargeStarted += ChargeStarted;
            gameLocationCharacter.ChargeEnded += ChargeEnded;
            gameLocationCharacter.ChargeAborted += ChargeAborted;
            gameLocationCharacter.UpdateMotions += UpdateMotions;
            gameLocationCharacter.TeleportStarted += TeleportStarted;
            gameLocationCharacter.Moved += Moved;
            gameLocationCharacter.Rotated += Rotated;
            gameLocationCharacter.Stopped += Stopped;
            gameLocationCharacter.FallStarted += FallStarted;
            gameLocationCharacter.FallStopped += FallStopped;
            gameLocationCharacter.CrawlStarted += CrawlStarted;
            gameLocationCharacter.BurrowStarted += BurrowStarted;
            gameLocationCharacter.BurrowEnded += BurrowEnded;
            gameLocationCharacter.JumpStarted += JumpStarted;
            gameLocationCharacter.JumpFinished += JumpFinished;
            gameLocationCharacter.VaultStarted += VaultStarted;
            gameLocationCharacter.VaultFinished += VaultFinished;
            gameLocationCharacter.ClimbStarted += ClimbStarted;
            gameLocationCharacter.ClimbFinished += ClimbFinished;
            gameLocationCharacter.ChangeSurfaceStarted += ChangeSurfaceStarted;
            gameLocationCharacter.AttackStart += AttackStart;
            gameLocationCharacter.AttackImpactStart += AttackImpactStart;
            gameLocationCharacter.DeflectAttackStart += DeflectAttackStart;
            gameLocationCharacter.ManipulateStart += ManipulateStart;
            gameLocationCharacter.ManipulateEnd += ManipulateEnd;
            gameLocationCharacter.ShoveStart += ShoveStart;
            gameLocationCharacter.CastingStart += CastingStart;
            gameLocationCharacter.HitStart += HitStart;
            gameLocationCharacter.PathFailed += PathFailed;
            gameLocationCharacter.DialogStarted += DialogStarted;
            gameLocationCharacter.DialogEnded += DialogEnded;
            gameLocationCharacter.DialogChoiceStarted += DialogChoiceStarted;
            gameLocationCharacter.DialogChoiceEnded += DialogChoiceEnded;
            gameLocationCharacter.SpeechStarted += SpeechStarted;
            gameLocationCharacter.SpeechEnded += SpeechEnded;
            gameLocationCharacter.ListenStarted += ListenStarted;
            gameLocationCharacter.ListenEnded += ListenEnded;
            gameLocationCharacter.AdditionalAnimationStarted += AdditionalAnimationStarted;
            gameLocationCharacter.TextFeedbackRequested += TextFeedbackRequested;
            gameLocationCharacter.InGameDialogLineRequested += InGameDialogLineRequested;
            gameLocationCharacter.AlreadySuccessful += AlreadySuccessful;
            gameLocationCharacter.AlreadyFailed += AlreadyFailed;
            gameLocationCharacter.ProneStatusChanged += ProneStatusChanged;
            gameLocationCharacter.IsAngryStatusChanged += IsAngryStatusChanged;
            gameLocationCharacter.UsedTacticalMovesChanged += UsedTacticalMovesChanged;
            gameLocationCharacter.CurrentMonsterAttackChanged += CurrentMonsterAttackChanged;
            gameLocationCharacter.DisolveStarted += DisolveStarted;

            if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
            {
                continue;
            }

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
        gameLocationCharacterService.CharacterRevealed -= CharacterRevealed;
        gameLocationCharacterService.CharacterKilled -= CharacterKilled;
        gameLocationCharacterService.CharacterDestroying -= CharacterDestroying;

        foreach (var gameLocationCharacter in gameLocationCharacterService.ValidCharacters)
        {
            gameLocationCharacter.Placed -= Placed;
            gameLocationCharacter.MoveStarted -= MoveStarted;
            gameLocationCharacter.PrepareChargeStarted -= PrepareChargeStarted;
            gameLocationCharacter.ChargeStarted -= ChargeStarted;
            gameLocationCharacter.ChargeEnded -= ChargeEnded;
            gameLocationCharacter.ChargeAborted -= ChargeAborted;
            gameLocationCharacter.UpdateMotions -= UpdateMotions;
            gameLocationCharacter.TeleportStarted -= TeleportStarted;
            gameLocationCharacter.Moved -= Moved;
            gameLocationCharacter.Rotated -= Rotated;
            gameLocationCharacter.Stopped -= Stopped;
            gameLocationCharacter.FallStarted -= FallStarted;
            gameLocationCharacter.FallStopped -= FallStopped;
            gameLocationCharacter.CrawlStarted -= CrawlStarted;
            gameLocationCharacter.BurrowStarted -= BurrowStarted;
            gameLocationCharacter.BurrowEnded -= BurrowEnded;
            gameLocationCharacter.JumpStarted -= JumpStarted;
            gameLocationCharacter.JumpFinished -= JumpFinished;
            gameLocationCharacter.VaultStarted -= VaultStarted;
            gameLocationCharacter.VaultFinished -= VaultFinished;
            gameLocationCharacter.ClimbStarted -= ClimbStarted;
            gameLocationCharacter.ClimbFinished -= ClimbFinished;
            gameLocationCharacter.ChangeSurfaceStarted -= ChangeSurfaceStarted;
            gameLocationCharacter.AttackStart -= AttackStart;
            gameLocationCharacter.AttackImpactStart -= AttackImpactStart;
            gameLocationCharacter.DeflectAttackStart -= DeflectAttackStart;
            gameLocationCharacter.ManipulateStart -= ManipulateStart;
            gameLocationCharacter.ManipulateEnd -= ManipulateEnd;
            gameLocationCharacter.ShoveStart -= ShoveStart;
            gameLocationCharacter.CastingStart -= CastingStart;
            gameLocationCharacter.HitStart -= HitStart;
            gameLocationCharacter.PathFailed -= PathFailed;
            gameLocationCharacter.DialogStarted -= DialogStarted;
            gameLocationCharacter.DialogEnded -= DialogEnded;
            gameLocationCharacter.DialogChoiceStarted -= DialogChoiceStarted;
            gameLocationCharacter.DialogChoiceEnded -= DialogChoiceEnded;
            gameLocationCharacter.SpeechStarted -= SpeechStarted;
            gameLocationCharacter.SpeechEnded -= SpeechEnded;
            gameLocationCharacter.ListenStarted -= ListenStarted;
            gameLocationCharacter.ListenEnded -= ListenEnded;
            gameLocationCharacter.AdditionalAnimationStarted -= AdditionalAnimationStarted;
            gameLocationCharacter.TextFeedbackRequested -= TextFeedbackRequested;
            gameLocationCharacter.InGameDialogLineRequested -= InGameDialogLineRequested;
            gameLocationCharacter.AlreadySuccessful -= AlreadySuccessful;
            gameLocationCharacter.AlreadyFailed -= AlreadyFailed;
            gameLocationCharacter.ProneStatusChanged -= ProneStatusChanged;
            gameLocationCharacter.IsAngryStatusChanged -= IsAngryStatusChanged;
            gameLocationCharacter.UsedTacticalMovesChanged -= UsedTacticalMovesChanged;
            gameLocationCharacter.CurrentMonsterAttackChanged -= CurrentMonsterAttackChanged;
            gameLocationCharacter.DisolveStarted -= DisolveStarted;

            if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
            {
                continue;
            }

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

    //
    // ICharacterBuildingService
    //

    private static void CharacterLevelUpStarted(RulesetCharacterHero hero)
    {
        Main.Logger.Log($"{hero.Name} Character Level Up Started");
    }

    //
    // IGameLocationCharacterService
    //

    private static void CharacterCreated(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character.Name} Character Created");
    }

    private static void CharacterRevealed(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character.Name} Character Revealed");
    }

    private static void CharacterDestroying(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character.Name} Character Destroying");
    }

    private static void CharacterKilled(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character.Name} Character Killed");

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

    //
    // RulesetCharacterHero
    //

    private static void ItemEquiped(RulesetCharacterHero hero, RulesetItem item)
    {
        Main.Logger.Log($"{hero.Name} Item Equipped Hero");
    }

    //
    // CharacterInventory
    //

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

    //
    // IGameLocationActionService
    //

    private static void ActionStarted([NotNull] CharacterAction characterAction)
    {
        Main.Logger.Log($"{characterAction.ActingCharacter.Name} {characterAction.ActionId} Action Started");

        Global.ActionStarted(characterAction);
    }

    private static void ActionChainStarted(CharacterActionChainParams characterActionChainParams)
    {
        Main.Logger.Log($"{characterActionChainParams.ActingCharacter.Name} Action Chain Started");
    }

    private static void ActionChainFinished(
        CharacterActionChainParams characterActionChainParams,
        bool aborted)
    {
        Main.Logger.Log($"{characterActionChainParams.ActingCharacter.Name} Action Chain Finished");
    }

    private static void MagicEffectPreparing(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log($"{data.Caster?.Name} Magic Effect Preparing");
    }

    private static void MagicEffectPreparingOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log($"{data.Caster?.Name} Magic Effect Preparing On Target");
    }

    private static void MagicEffectLaunch(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log($"{data.Caster?.Name} Magic Effect Launch");
    }

    private static void MagicEffectCastOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log($"{data.Caster?.Name} Magic Effect Cast On Target");
    }

    private static void MagicEffectCastOnZone(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log($"{data.Caster?.Name} Magic Effect Cast On Zone");
    }

    private static void MagicEffectBeforeHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log($"{data.Caster?.Name} Magic Effect Before Hit Target");
    }

    private static void MagicEffectHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Logger.Log($"{data.Caster?.Name} Magic Effect Hit Target");
    }

    private static void SpellCast(string spellDefinitionName, int fromDevice)
    {
        Main.Logger.Log($"{spellDefinitionName} Spell Cast");
    }

    private static void ActionUsed(
        GameLocationCharacter actingCharacter,
        CharacterActionParams actionParams,
        ActionDefinition actionDefinition)
    {
        Main.Logger.Log($"{actingCharacter?.Name} Action Used");
    }

    private static void ShoveActionUsed(
        GameLocationCharacter actingCharacter,
        GameLocationCharacter attackingCharacter,
        ActionDefinition actionDefinition,
        bool success)
    {
        Main.Logger.Log($"{actingCharacter?.Name} Shove Action Used");
    }

    private static void ItemUsed(string itemDefinitionName)
    {
        Main.Logger.Log("Item Used");
    }

    //
    // GameLocationCharacter
    //

    private static void Placed(GameLocationCharacter character, int3 location)
    {
        Main.Logger.Log($"{character?.Name} Placed");
    }

    private static void MoveStarted(ref GameLocationCharacterDefinitions.MoveStartedParameters parameters)
    {
        Main.Logger.Log("Move Started");
    }

    private static void PrepareChargeStarted(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Logger.Log($"{character?.Name} Prepare Charge Started");
    }

    private static void ChargeStarted(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Logger.Log($"{character?.Name} Charge Started");
    }

    private static void ChargeEnded(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Logger.Log($"{character?.Name} Charge Ended");
    }

    private static void ChargeAborted(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Logger.Log($"{character?.Name} Charge Aborted");
    }

    private static void UpdateMotions()
    {
        Main.Logger.Log("Update Motions");
    }

    private static void TeleportStarted(
        GameLocationCharacter character,
        int3 destination,
        LocationDefinitions.Orientation destinationOrientation,
        bool sendEvent)
    {
        Main.Logger.Log($"{character?.Name} Teleport Started");
    }

    private static void Moved(ref GameLocationCharacterDefinitions.MovedParameters parameters)
    {
        Main.Logger.Log("Moved");
    }

    private static void Rotated(
        GameLocationCharacter character,
        LocationDefinitions.Orientation sourceOrientation,
        LocationDefinitions.Orientation destinationOrientation,
        Vector3 targetPosition,
        bool refreshVisual = true,
        bool refreshPerception = true)
    {
        Main.Logger.Log($"{character?.Name} Rotated");
    }

    private static void Stopped(CharacterAction.InterruptionType interruption)
    {
        Main.Logger.Log("Stopped");
    }

    private static void FallStarted(
        GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool featherFall)
    {
        Main.Logger.Log($"{character?.Name} Fall Started");
    }

    private static void FallStopped(
        GameLocationCharacter character,
        int3 source,
        int3 destination)
    {
        Main.Logger.Log($"{character?.Name} Fall Stopped");
    }

    private static void CrawlStarted(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Crawl Started");
    }

    private static void BurrowStarted(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Burrow Started");
    }

    private static void BurrowEnded(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Burrow Ended");
    }

    private static void JumpStarted(
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        bool useMomentum,
        bool dragonJump,
        bool buletteLeap)
    {
        Main.Logger.Log("Jump Started");
    }

    private static void JumpFinished(
        GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool jumpFailed)
    {
        Main.Logger.Log($"{character?.Name} Jump Finished");
    }

    private static void VaultStarted(
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        float distance,
        float duration)
    {
        Main.Logger.Log("Vault Started");
    }

    private static void VaultFinished(int3 source, int3 destination)
    {
        Main.Logger.Log("Vault Finished");
    }

    private static void ClimbStarted(
        GameLocationCharacter character,
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        float climbingDirection,
        bool lastMove,
        bool fastClimb)
    {
        Main.Logger.Log($"{character?.Name} Climb Started");
    }

    private static void ClimbFinished(
        GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool easyClimb)
    {
        Main.Logger.Log($"{character?.Name} Climb Finished");
    }

    private static void ChangeSurfaceStarted(
        GameLocationCharacter character,
        int3 destinationPosition,
        LocationDefinitions.Orientation destinationOrientation,
        CellFlags.Side destinationSide,
        RuleDefinitions.MoveMode moveMode,
        ActionDefinitions.MoveStance moveStance,
        bool difficultTerrain,
        bool lastMove)
    {
        Main.Logger.Log($"{character?.Name} Change Surface Started");
    }

    private static void AttackStart(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        Main.Logger.Log($"{attacker?.Name},{defender?.Name} Attack Start");
    }

    private static void AttackImpactStart(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        Main.Logger.Log($"{attacker?.Name},{defender?.Name}Attack Impact Start");
    }

    private static void DeflectAttackStart(
        GameLocationCharacter blocker,
        GameLocationCharacter attacker)
    {
        Main.Logger.Log($"{blocker?.Name},{attacker?.Name} Deflect Attack Start");
    }

    private static void ManipulateStart(
        GameLocationCharacter character,
        AnimationDefinitions.ManipulationType animation)
    {
        Main.Logger.Log($"{character?.Name} Manipulate Start");
    }

    private static void ManipulateEnd(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Manipulate End");
    }

    private static void ShoveStart(
        GameLocationCharacter attacker,
        GameLocationCharacter target,
        bool success)
    {
        Main.Logger.Log($"{attacker?.Name},{target?.Name} Shove Start");
    }

    private static void CastingStart(
        ref ActionDefinitions.MagicEffectCastData magicEffectCastData)
    {
        Main.Logger.Log("Casting Start");
    }

    private static void HitStart(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Hit Start");
    }

    private static void PathFailed(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Path Failed");
    }

    private static void DialogStarted(
        GameLocationCharacter character,
        NarrativeSequence narrativeSequence)
    {
        Main.Logger.Log($"{character?.Name} Dialog Started");
    }

    private static void DialogEnded(
        GameLocationCharacter character,
        NarrativeSequence narrativeSequence)
    {
        Main.Logger.Log($"{character?.Name} Dialog Ended");
    }

    private static void DialogChoiceStarted(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Dialog Choice Started");
    }

    private static void DialogChoiceEnded(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Dialog Choice Ended");
    }

    private static void SpeechStarted(
        ref GameLocationCharacterDefinitions.SpeechStartedParameters parameters)
    {
        Main.Logger.Log("Speech Started");
    }

    private static void SpeechEnded(GameLocationCharacter character, bool forceStop)
    {
        Main.Logger.Log($"{character?.Name} Speech Ended");
    }

    private static void ListenStarted(
        GameLocationCharacter character,
        GameLocationCharacter speaker,
        bool lookAtOverriden,
        bool facialExpressionOverriden)
    {
        Main.Logger.Log($"{character?.Name} Listen Started");
    }

    private static void ListenEnded(
        GameLocationCharacter character,
        GameLocationCharacter speaker)
    {
        Main.Logger.Log($"{character?.Name} Listen Ended");
    }

    private static void AdditionalAnimationStarted(
        ref GameLocationCharacterDefinitions.AdditionalAnimationParameters parameters)
    {
        Main.Logger.Log("AdditionalAnimationStarted");
    }

    private static void TextFeedbackRequested(
        GameLocationCharacter character,
        string colorStyle,
        string text)
    {
        Main.Logger.Log($"{character?.Name} Text Feedback Requested");
    }

    private static void InGameDialogLineRequested(
        GameLocationCharacter character,
        string line,
        float duration,
        bool banterPoolElement)
    {
        Main.Logger.Log($"{character?.Name} In Game Dialog Line Requested");
    }

    private static void AlreadySuccessful(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Already Successful");
    }

    private static void AlreadyFailed(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Already Failed");
    }

    private static void ProneStatusChanged(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Prone Status Changed");
    }

    private static void IsAngryStatusChanged(GameLocationCharacter character, bool status)
    {
        Main.Logger.Log("IsAngry Status Changed");
    }

    private static void UsedTacticalMovesChanged(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Used Tactical Moves Changed");
    }

    private static void CurrentMonsterAttackChanged(
        GameLocationCharacter character,
        MonsterAttackDefinition monsterAttackDefinition,
        Action graphicsRefreshedCallback)
    {
        Main.Logger.Log($"{character?.Name} Current Monster Attack Changed");
    }

    private static void DisolveStarted(GameLocationCharacter character)
    {
        Main.Logger.Log($"{character?.Name} Dissolve Started");
    }
}
