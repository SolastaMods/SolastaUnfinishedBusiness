using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaCommunityExpansion.CustomInterfaces;
using SolastaCommunityExpansion.Subclasses.Wizard;
using TA;

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
        Main.Log("Game Created");

        var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

        gameLocationService.LocationReady += LocationReady;
        gameLocationService.LocationUnloading += LocationUnloading;

        var characterBuildingService = ServiceRepository.GetService<ICharacterBuildingService>();

        characterBuildingService.CharacterLevelUpStarted += CharacterLevelUpStarted;
    }

    private static void GameDestroying()
    {
        Main.Log("Game Destroying");

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
        Main.Log("Location Ready");

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
            //gameLocationCharacter.UpdateMotions += UpdateMotions;
            gameLocationCharacter.TeleportStarted += TeleportStarted;
            gameLocationCharacter.Moved += Moved;
            //gameLocationCharacter.Rotated += Rotated;
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
            //gameLocationCharacter.AdditionalAnimationStarted += AdditionalAnimationStarted;
            gameLocationCharacter.TextFeedbackRequested += TextFeedbackRequested;
            gameLocationCharacter.InGameDialogLineRequested += InGameDialogLineRequested;
            gameLocationCharacter.AlreadySuccessful += AlreadySuccessful;
            gameLocationCharacter.AlreadyFailed += AlreadyFailed;
            gameLocationCharacter.ProneStatusChanged += ProneStatusChanged;
            gameLocationCharacter.IsAngryStatusChanged += IsAngryStatusChanged;
            gameLocationCharacter.UsedTacticalMovesChanged += UsedTacticalMovesChanged;
            gameLocationCharacter.CurrentMonsterAttackChanged += CurrentMonsterAttackChanged;
            gameLocationCharacter.DisolveStarted += DissolveStarted;

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            rulesetCharacter.ConditionAdded += ConditionAdded;
            rulesetCharacter.ConditionRemoved += ConditionRemoved;
            rulesetCharacter.ConditionRemovedForVisual += ConditionRemovedForVisual;
            rulesetCharacter.ConditionOccurenceReached += ConditionOccurenceReached;
            //rulesetCharacter.ConditionSaveRerollRequested += ConditionSaveRerollRequested;
            rulesetCharacter.ImmuneToSpell += ImmuneToSpell;
            rulesetCharacter.ImmuneToSpellLevel += ImmuneToSpellLevel;
            rulesetCharacter.ImmuneToDamage += ImmuneToDamage;
            rulesetCharacter.DamageAltered += DamageAltered;
            rulesetCharacter.ImmuneToCondition += ImmuneToCondition;
            rulesetCharacter.SaveRolled += SaveRolled;
            rulesetCharacter.DieRerolled += DieRerolled;
            rulesetCharacter.AttackInitiated += AttackInitiated;
            rulesetCharacter.AttackRolled += AttackRolled;
            rulesetCharacter.IncomingAttackRolled += IncomingAttackRolled;
            rulesetCharacter.AttackAutomaticHit += AttackAutomaticHit;
            rulesetCharacter.AttackAutomaticCritical += AttackAutomaticCritical;
            rulesetCharacter.DamageFormsTriggered += DamageFormsTriggered;
            rulesetCharacter.HealingFormsTriggered += HealingFormsTriggered;
            rulesetCharacter.IncomingDamageNotified += IncomingDamageNotified;
            rulesetCharacter.AbilityScoreIncreased += AbilityScoreIncreased;
            rulesetCharacter.DamageHalved += DamageHalved;
            rulesetCharacter.DamageReduced += DamageReduced;
            rulesetCharacter.ReplacedAbilityScoreForSave += ReplacedAbilityScoreForSave;
            rulesetCharacter.AdditionalSaveDieRolled += AdditionalSaveDieRolled;
            rulesetCharacter.DamageReceived += DamageReceived;
            rulesetCharacter.AlterationInflicted += AlterationInflicted;
            rulesetCharacter.SpellDissipated += SpellDissipated;
            rulesetCharacter.TagRevealed += TagRevealed;
            rulesetCharacter.ActorReplaced += ActorReplaced;

            if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
            {
                continue;
            }

            rulesetCharacterHero.ItemEquipedCallback += ItemEquipped;
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
        Main.Log("Location Unloading");

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
            //gameLocationCharacter.UpdateMotions -= UpdateMotions;
            gameLocationCharacter.TeleportStarted -= TeleportStarted;
            gameLocationCharacter.Moved -= Moved;
            //gameLocationCharacter.Rotated -= Rotated;
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
            //gameLocationCharacter.AdditionalAnimationStarted -= AdditionalAnimationStarted;
            gameLocationCharacter.TextFeedbackRequested -= TextFeedbackRequested;
            gameLocationCharacter.InGameDialogLineRequested -= InGameDialogLineRequested;
            gameLocationCharacter.AlreadySuccessful -= AlreadySuccessful;
            gameLocationCharacter.AlreadyFailed -= AlreadyFailed;
            gameLocationCharacter.ProneStatusChanged -= ProneStatusChanged;
            gameLocationCharacter.IsAngryStatusChanged -= IsAngryStatusChanged;
            gameLocationCharacter.UsedTacticalMovesChanged -= UsedTacticalMovesChanged;
            gameLocationCharacter.CurrentMonsterAttackChanged -= CurrentMonsterAttackChanged;
            gameLocationCharacter.DisolveStarted -= DissolveStarted;

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            rulesetCharacter.ConditionAdded -= ConditionAdded;
            rulesetCharacter.ConditionRemoved -= ConditionRemoved;
            rulesetCharacter.ConditionRemovedForVisual -= ConditionRemovedForVisual;
            rulesetCharacter.ConditionOccurenceReached -= ConditionOccurenceReached;
            //rulesetCharacter.ConditionSaveRerollRequested -= ConditionSaveRerollRequested;
            rulesetCharacter.ImmuneToSpell -= ImmuneToSpell;
            rulesetCharacter.ImmuneToSpellLevel -= ImmuneToSpellLevel;
            rulesetCharacter.ImmuneToDamage -= ImmuneToDamage;
            rulesetCharacter.DamageAltered -= DamageAltered;
            rulesetCharacter.ImmuneToCondition -= ImmuneToCondition;
            rulesetCharacter.SaveRolled -= SaveRolled;
            rulesetCharacter.DieRerolled -= DieRerolled;
            rulesetCharacter.AttackInitiated -= AttackInitiated;
            rulesetCharacter.AttackRolled -= AttackRolled;
            rulesetCharacter.IncomingAttackRolled -= IncomingAttackRolled;
            rulesetCharacter.AttackAutomaticHit -= AttackAutomaticHit;
            rulesetCharacter.AttackAutomaticCritical -= AttackAutomaticCritical;
            rulesetCharacter.DamageFormsTriggered -= DamageFormsTriggered;
            rulesetCharacter.HealingFormsTriggered -= HealingFormsTriggered;
            rulesetCharacter.IncomingDamageNotified -= IncomingDamageNotified;
            rulesetCharacter.AbilityScoreIncreased -= AbilityScoreIncreased;
            rulesetCharacter.DamageHalved -= DamageHalved;
            rulesetCharacter.DamageReduced -= DamageReduced;
            rulesetCharacter.ReplacedAbilityScoreForSave -= ReplacedAbilityScoreForSave;
            rulesetCharacter.AdditionalSaveDieRolled -= AdditionalSaveDieRolled;
            rulesetCharacter.DamageReceived -= DamageReceived;
            rulesetCharacter.AlterationInflicted -= AlterationInflicted;
            rulesetCharacter.SpellDissipated -= SpellDissipated;
            rulesetCharacter.TagRevealed -= TagRevealed;
            rulesetCharacter.ActorReplaced -= ActorReplaced;

            if (gameLocationCharacter.RulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
            {
                continue;
            }

            rulesetCharacterHero.ItemEquipedCallback -= ItemEquipped;
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

    private static void CharacterLevelUpStarted([NotNull] RulesetCharacterHero hero)
    {
        Main.Log($"{hero.Name} Character Level Up Started");
    }

    //
    // IGameLocationCharacterService
    //

    private static void CharacterCreated([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Character Created");
    }

    private static void CharacterRevealed([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Character Revealed");
    }

    private static void CharacterDestroying([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Character Destroying");
    }

    private static void CharacterKilled([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Character Killed");

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

    private static void ItemEquipped([NotNull] RulesetCharacterHero hero, [NotNull] RulesetItem item)
    {
        Main.Log($"{hero.Name} Item Equipped Hero");

        BladeDancer.OnItemEquipped(hero, item);
    }

    //
    // CharacterInventory
    //

    private static void ItemEquiped(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Log("Item Equipped");
    }

    private static void ItemAltered(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Log("Item Altered");
    }

    private static void ItemUnequiped(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Log("Item Unequipped");
    }

    private static void ItemReleased(RulesetItem item, bool canKeep)
    {
        Main.Log("Item Released");
    }

    //
    // IGameLocationActionService
    //

    private static void ActionStarted([NotNull] CharacterAction characterAction)
    {
        Main.Log($"{characterAction.ActingCharacter.Name} {characterAction.ActionId} Action Started");

        Global.ActionStarted(characterAction);
    }

    private static void ActionChainStarted([NotNull] CharacterActionChainParams characterActionChainParams)
    {
        Main.Log($"{characterActionChainParams.ActingCharacter.Name} Action Chain Started");
    }

    private static void ActionChainFinished(
        [NotNull] CharacterActionChainParams characterActionChainParams,
        bool aborted)
    {
        Main.Log($"{characterActionChainParams.ActingCharacter.Name} Action Chain Finished");
    }

    private static void MagicEffectPreparing(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Log($"{data.Caster?.Name} Magic Effect Preparing");
    }

    private static void MagicEffectPreparingOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Log($"{data.Caster?.Name} Magic Effect Preparing On Target");
    }

    private static void MagicEffectLaunch(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Log($"{data.Caster?.Name} Magic Effect Launch");
    }

    private static void MagicEffectCastOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Log($"{data.Caster?.Name} Magic Effect Cast On Target");
    }

    private static void MagicEffectCastOnZone(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Log($"{data.Caster?.Name} Magic Effect Cast On Zone");
    }

    private static void MagicEffectBeforeHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Log($"{data.Caster?.Name} Magic Effect Before Hit Target");
    }

    private static void MagicEffectHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Log($"{data.Caster?.Name} Magic Effect Hit Target");
    }

    private static void SpellCast(string spellDefinitionName, int fromDevice)
    {
        Main.Log($"{spellDefinitionName} Spell Cast");
    }

    private static void ActionUsed(
        [CanBeNull] GameLocationCharacter actingCharacter,
        CharacterActionParams actionParams,
        ActionDefinition actionDefinition)
    {
        Main.Log($"{actingCharacter?.Name} Action Used");
    }

    private static void ShoveActionUsed(
        [CanBeNull] GameLocationCharacter actingCharacter,
        GameLocationCharacter attackingCharacter,
        ActionDefinition actionDefinition,
        bool success)
    {
        Main.Log($"{actingCharacter?.Name} Shove Action Used");
    }

    private static void ItemUsed(string itemDefinitionName)
    {
        Main.Log("Item Used");
    }

    //
    // GameLocationCharacter
    //

    private static void Placed([CanBeNull] GameLocationCharacter character, int3 location)
    {
        Main.Log($"{character?.Name} Placed");
    }

    private static void MoveStarted(ref GameLocationCharacterDefinitions.MoveStartedParameters parameters)
    {
        Main.Log("Move Started");
    }

    private static void PrepareChargeStarted(
        [NotNull] GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Log($"{character.Name} Prepare Charge Started");
    }

    private static void ChargeStarted(
        [NotNull] GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Log($"{character.Name} Charge Started");
    }

    private static void ChargeEnded(
        [NotNull] GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Log($"{character.Name} Charge Ended");
    }

    private static void ChargeAborted(
        [NotNull] GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Log($"{character.Name} Charge Aborted");
    }

    private static void TeleportStarted(
        [NotNull] GameLocationCharacter character,
        int3 destination,
        LocationDefinitions.Orientation destinationOrientation,
        bool sendEvent)
    {
        Main.Log($"{character.Name} Teleport Started");
    }

    private static void Moved(ref GameLocationCharacterDefinitions.MovedParameters parameters)
    {
        Main.Log("Moved");
    }

    private static void Stopped(CharacterAction.InterruptionType interruption)
    {
        Main.Log("Stopped");
    }

    private static void FallStarted(
        [NotNull] GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool featherFall)
    {
        Main.Log($"{character.Name} Fall Started");
    }

    private static void FallStopped(
        [NotNull] GameLocationCharacter character,
        int3 source,
        int3 destination)
    {
        Main.Log($"{character.Name} Fall Stopped");
    }

    private static void CrawlStarted([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Crawl Started");
    }

    private static void BurrowStarted([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Burrow Started");
    }

    private static void BurrowEnded([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Burrow Ended");
    }

    private static void JumpStarted(
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        bool useMomentum,
        bool dragonJump,
        bool buletteLeap)
    {
        Main.Log("Jump Started");
    }

    private static void JumpFinished(
        [NotNull] GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool jumpFailed)
    {
        Main.Log($"{character.Name} Jump Finished");
    }

    private static void VaultStarted(
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        float distance,
        float duration)
    {
        Main.Log("Vault Started");
    }

    private static void VaultFinished(int3 source, int3 destination)
    {
        Main.Log("Vault Finished");
    }

    private static void ClimbStarted(
        [NotNull] GameLocationCharacter character,
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        float climbingDirection,
        bool lastMove,
        bool fastClimb)
    {
        Main.Log($"{character.Name} Climb Started");
    }

    private static void ClimbFinished(
        [NotNull] GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool easyClimb)
    {
        Main.Log($"{character.Name} Climb Finished");
    }

    private static void ChangeSurfaceStarted(
        [NotNull] GameLocationCharacter character,
        int3 destinationPosition,
        LocationDefinitions.Orientation destinationOrientation,
        CellFlags.Side destinationSide,
        RuleDefinitions.MoveMode moveMode,
        ActionDefinitions.MoveStance moveStance,
        bool difficultTerrain,
        bool lastMove)
    {
        Main.Log($"{character.Name} Change Surface Started");
    }

    private static void AttackStart(
        [NotNull] GameLocationCharacter attacker,
        [NotNull] GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        Main.Log($"{attacker.Name},{defender.Name} Attack Start");
    }

    private static void AttackImpactStart(
        [NotNull] GameLocationCharacter attacker,
        [NotNull] GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        Main.Log($"{attacker.Name},{defender.Name}Attack Impact Start");
    }

    private static void DeflectAttackStart(
        [NotNull] GameLocationCharacter blocker,
        [NotNull] GameLocationCharacter attacker)
    {
        Main.Log($"{blocker.Name},{attacker.Name} Deflect Attack Start");
    }

    private static void ManipulateStart(
        [NotNull] GameLocationCharacter character,
        AnimationDefinitions.ManipulationType animation)
    {
        Main.Log($"{character.Name} Manipulate Start");
    }

    private static void ManipulateEnd([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Manipulate End");
    }

    private static void ShoveStart(
        [NotNull] GameLocationCharacter attacker,
        [NotNull] GameLocationCharacter target,
        bool success)
    {
        Main.Log($"{attacker.Name},{target.Name} Shove Start");
    }

    private static void CastingStart(
        ref ActionDefinitions.MagicEffectCastData magicEffectCastData)
    {
        Main.Log("Casting Start");
    }

    private static void HitStart([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Hit Start");
    }

    private static void PathFailed([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Path Failed");
    }

    private static void DialogStarted(
        [NotNull] GameLocationCharacter character,
        NarrativeSequence narrativeSequence)
    {
        Main.Log($"{character.Name} Dialog Started");
    }

    private static void DialogEnded(
        [NotNull] GameLocationCharacter character,
        NarrativeSequence narrativeSequence)
    {
        Main.Log($"{character.Name} Dialog Ended");
    }

    private static void DialogChoiceStarted([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Dialog Choice Started");
    }

    private static void DialogChoiceEnded([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Dialog Choice Ended");
    }

    private static void SpeechStarted(
        ref GameLocationCharacterDefinitions.SpeechStartedParameters parameters)
    {
        Main.Log("Speech Started");
    }

    private static void SpeechEnded([NotNull] GameLocationCharacter character, bool forceStop)
    {
        Main.Log($"{character.Name} Speech Ended");
    }

    private static void ListenStarted(
        [NotNull] GameLocationCharacter character,
        GameLocationCharacter speaker,
        bool lookAtOverriden,
        bool facialExpressionOverriden)
    {
        Main.Log($"{character.Name} Listen Started");
    }

    private static void ListenEnded(
        [NotNull] GameLocationCharacter character,
        GameLocationCharacter speaker)
    {
        Main.Log($"{character.Name} Listen Ended");
    }

    private static void TextFeedbackRequested(
        [NotNull] GameLocationCharacter character,
        string colorStyle,
        string text)
    {
        Main.Log($"{character.Name} Text Feedback Requested");
    }

    private static void InGameDialogLineRequested(
        [NotNull] GameLocationCharacter character,
        string line,
        float duration,
        bool banterPoolElement)
    {
        Main.Log($"{character.Name} In Game Dialog Line Requested");
    }

    private static void AlreadySuccessful([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Already Successful");
    }

    private static void AlreadyFailed([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Already Failed");
    }

    private static void ProneStatusChanged([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Prone Status Changed");
    }

    private static void IsAngryStatusChanged([NotNull] GameLocationCharacter character, bool status)
    {
        Main.Log($"{character.Name} IsAngry Status Changed");
    }

    private static void UsedTacticalMovesChanged([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Used Tactical Moves Changed");
    }

    private static void CurrentMonsterAttackChanged(
        [NotNull] GameLocationCharacter character,
        MonsterAttackDefinition monsterAttackDefinition,
        Action graphicsRefreshedCallback)
    {
        Main.Log($"{character.Name} Current Monster Attack Changed");
    }

    private static void DissolveStarted([NotNull] GameLocationCharacter character)
    {
        Main.Log($"{character.Name} Dissolve Started");
    }

    //
    // RulesetCharacter
    //

    private static void ConditionAdded(RulesetActor character, RulesetCondition addedActiveCondition)
    {
        Main.Log("ConditionAdded");
    }

    private static void ConditionRemoved(RulesetActor character, RulesetCondition removedActiveCondition)
    {
        Main.Log("ConditionRemoved");
    }

    private static void ConditionRemovedForVisual(
        RulesetActor character,
        RulesetCondition removedActiveCondition,
        bool showGraphics = true)
    {
        Main.Log("ConditionRemovedForVisual");
    }

    private static void ConditionOccurenceReached(RulesetActor character, RulesetCondition removedActiveCondition)
    {
        Main.Log("ConditionOccurenceReached");
    }

    // private static void ConditionSaveRerollRequested(RulesetActor character, RulesetCondition activeCondition,
    //     RuleDefinitions.AdvantageType advantageType, bool removeOnSuccess, out bool success)
    // {
    //     Main.Log("ConditionSaveRerollRequested");
    // }

    private static void ImmuneToSpell(RulesetActor character, SpellDefinition spellDefinition)
    {
        Main.Log("ImmuneToSpell");
    }

    private static void ImmuneToSpellLevel(RulesetActor character, SpellDefinition spellDefinition, int maxSpellLevel)
    {
        Main.Log("ImmuneToSpellLevel");
    }

    private static void ImmuneToDamage(RulesetActor character, string damageType, bool silent)
    {
        Main.Log("ImmuneToDamage");
    }

    private static void DamageAltered(
        RulesetActor character,
        string damageType,
        RuleDefinitions.DamageAffinityType damageAffinityType,
        bool silent)
    {
        Main.Log("DamageAltered");
    }

    private static void ImmuneToCondition(RulesetActor character, string conditionName, ulong sourceGuid)
    {
        Main.Log("ImmuneToCondition");
    }

    private static void SaveRolled(
        RulesetActor character,
        string abilityScoreName,
        BaseDefinition sourceDefinition,
        RuleDefinitions.RollOutcome outcome,
        int saveDc,
        int totalRoll,
        int saveRoll,
        int firstRoll,
        int secondRoll,
        int rollModifier,
        List<RuleDefinitions.TrendInfo> modifierTrends,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        bool hasHitVisual)
    {
        Main.Log("SaveRolled");
    }

    private static void DieRerolled(
        RulesetActor character,
        RuleDefinitions.DieType dieType,
        int previousValue,
        int newValue,
        string localizationKey)
    {
        Main.Log("DieRerolled");
    }

    private static void AttackInitiated(
        RulesetActor character,
        int firstRoll,
        int secondRoll,
        int modifier,
        RuleDefinitions.AdvantageType advantageType)
    {
        Main.Log("AttackInitiated");
    }

    private static void AttackRolled(
        RulesetActor character,
        RulesetActor target,
        BaseDefinition attackMethod,
        RuleDefinitions.RollOutcome outcome,
        int attackRoll,
        int rawRoll,
        int modifier,
        List<RuleDefinitions.TrendInfo> toHitTrends,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        bool opportunity = false)
    {
        Main.Log("AttackRolled");
    }

    private static void IncomingAttackRolled(
        RulesetActor character,
        RulesetActor target,
        BaseDefinition attackMethod,
        bool rangeAttack,
        RuleDefinitions.RollOutcome outcome,
        int attackRoll,
        int rawRoll,
        int modifier,
        List<RuleDefinitions.TrendInfo> toHitTrends,
        List<RuleDefinitions.TrendInfo> advantageTrends,
        bool opportunity = false)
    {
        Main.Log("IncomingAttackRolled");
    }

    private static void AttackAutomaticHit(RulesetActor character, RulesetActor target, BaseDefinition attackMethod)
    {
        Main.Log("AttackAutomaticHit");
    }

    private static void AttackAutomaticCritical(RulesetActor target)
    {
        Main.Log("AttackAutomaticCritical");
    }

    private static void DamageFormsTriggered(RulesetActor character, List<EffectGroupInfo> damageInfos)
    {
        Main.Log("DamageFormsTriggered");
    }

    private static void HealingFormsTriggered(RulesetActor character, List<EffectGroupInfo> healingInfos)
    {
        Main.Log("HealingFormsTriggered");
    }

    private static void IncomingDamageNotified(
        RulesetActor character,
        RuleDefinitions.EffectSourceType damageType,
        BaseDefinition attackVectorDefinition)
    {
        Main.Log("IncomingDamageNotified");
    }

    private static void AbilityScoreIncreased(
        RulesetActor character,
        string abilityScore,
        int valueIncrease,
        int maxIncrease)
    {
        Main.Log("AbilityScoreIncreased");
    }

    private static void DamageHalved(RulesetActor character, FeatureDefinition feature)
    {
        Main.Log("DamageHalved");
    }

    private static void DamageReduced(RulesetActor character, FeatureDefinition feature, int reductionAmount)
    {
        Main.Log("DamageReduced");
    }

    private static void ReplacedAbilityScoreForSave(
        RulesetActor saver,
        FeatureDefinition feature,
        string originalAbilityScore,
        string replacedAbilityScore)
    {
        Main.Log("ReplacedAbilityScoreForSave");
    }

    private static void AdditionalSaveDieRolled(
        RulesetActor character,
        RuleDefinitions.TrendInfo trendInfo)
    {
        Main.Log("AdditionalSaveDieRolled");
    }

    private static void DamageReceived(
        RulesetActor target,
        int damage,
        string damageType,
        ulong sourceGuid,
        RollInfo rollInfo)
    {
        Main.Log("DamageReceived");
    }

    private static void AlterationInflicted(
        RulesetActor source,
        RulesetActor target,
        AlterationForm.Type alterationType)
    {
        Main.Log("AlterationInflicted");
    }

    private static void SpellDissipated(
        RulesetActor source,
        RulesetActor target,
        SpellDefinition spellDefinition,
        bool success)
    {
        Main.Log("SpellDissipated");
    }

    private static void TagRevealed(RulesetActor actor, string revealedTag)
    {
        Main.Log("TagRevealed");
    }

    private static void ActorReplaced(RulesetActor originalActor, RulesetActor newActor)
    {
        Main.Log("ActorReplaced");
    }
}
