#if false
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.RecipeDefinitions;

namespace SolastaUnfinishedBusiness.Models;

internal static class DelegatesContext
{
    internal static void LateLoad()
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
        Main.Info("Game Created");

        var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

        gameLocationService.LocationReady += LocationReady;
        gameLocationService.LocationUnloading += LocationUnloading;
    }

    private static void GameDestroying()
    {
        Main.Info("Game Destroying");

        var gameLocationService = ServiceRepository.GetService<IGameLocationService>();

        gameLocationService.LocationReady -= LocationReady;
        gameLocationService.LocationUnloading -= LocationUnloading;
    }

    //
    // IGameLocationService
    //

    private static void LocationReady(string locationDefinitionName, string userLocationTitle)
    {
        Main.Info("Location Ready");

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        // gameLocationCharacterService.CharacterCreated += CharacterCreated;
        // gameLocationCharacterService.CharacterRevealed += CharacterRevealed;
        // gameLocationCharacterService.CharacterKilled += CharacterKilled;
        // gameLocationCharacterService.CharacterDestroying += CharacterDestroying;

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

        foreach (var gameLocationCharacter in gameLocationCharacterService.ValidCharacters)
        {
            // gameLocationCharacter.Placed += Placed;
            // gameLocationCharacter.MoveStarted += MoveStarted;
            // gameLocationCharacter.PrepareChargeStarted += PrepareChargeStarted;
            // gameLocationCharacter.ChargeStarted += ChargeStarted;
            // gameLocationCharacter.ChargeEnded += ChargeEnded;
            // gameLocationCharacter.ChargeAborted += ChargeAborted;
            // gameLocationCharacter.UpdateMotions += UpdateMotions;
            // gameLocationCharacter.TeleportStarted += TeleportStarted;
            // gameLocationCharacter.Moved += Moved;
            // gameLocationCharacter.Rotated += Rotated;
            // gameLocationCharacter.Stopped += Stopped;
            // gameLocationCharacter.FallStarted += FallStarted;
            // gameLocationCharacter.FallStopped += FallStopped;
            // gameLocationCharacter.CrawlStarted += CrawlStarted;
            // gameLocationCharacter.BurrowStarted += BurrowStarted;
            // gameLocationCharacter.BurrowEnded += BurrowEnded;
            // gameLocationCharacter.JumpStarted += JumpStarted;
            // gameLocationCharacter.JumpFinished += JumpFinished;
            // gameLocationCharacter.VaultStarted += VaultStarted;
            // gameLocationCharacter.VaultFinished += VaultFinished;
            // gameLocationCharacter.ClimbStarted += ClimbStarted;
            // gameLocationCharacter.ClimbFinished += ClimbFinished;
            // gameLocationCharacter.ChangeSurfaceStarted += ChangeSurfaceStarted;
            // gameLocationCharacter.AttackStart += AttackStart;
            // gameLocationCharacter.AttackImpactStart += AttackImpactStart;
            // gameLocationCharacter.DeflectAttackStart += DeflectAttackStart;
            // gameLocationCharacter.ManipulateStart += ManipulateStart;
            // gameLocationCharacter.ManipulateEnd += ManipulateEnd;
            // gameLocationCharacter.ShoveStart += ShoveStart;
            // gameLocationCharacter.CastingStart += CastingStart;
            // gameLocationCharacter.HitStart += HitStart;
            // gameLocationCharacter.PathFailed += PathFailed;
            // gameLocationCharacter.DialogStarted += DialogStarted;
            // gameLocationCharacter.DialogEnded += DialogEnded;
            // gameLocationCharacter.DialogChoiceStarted += DialogChoiceStarted;
            // gameLocationCharacter.DialogChoiceEnded += DialogChoiceEnded;
            // gameLocationCharacter.SpeechStarted += SpeechStarted;
            // gameLocationCharacter.SpeechEnded += SpeechEnded;
            // gameLocationCharacter.ListenStarted += ListenStarted;
            // gameLocationCharacter.ListenEnded += ListenEnded;
            // gameLocationCharacter.AdditionalAnimationStarted += AdditionalAnimationStarted;
            // gameLocationCharacter.TextFeedbackRequested += TextFeedbackRequested;
            // gameLocationCharacter.InGameDialogLineRequested += InGameDialogLineRequested;
            // gameLocationCharacter.AlreadySuccessful += AlreadySuccessful;
            // gameLocationCharacter.AlreadyFailed += AlreadyFailed;
            // gameLocationCharacter.ProneStatusChanged += ProneStatusChanged;
            // gameLocationCharacter.IsAngryStatusChanged += IsAngryStatusChanged;
            // gameLocationCharacter.UsedTacticalMovesChanged += UsedTacticalMovesChanged;
            // gameLocationCharacter.CurrentMonsterAttackChanged += CurrentMonsterAttackChanged;
            // gameLocationCharacter.DisolveStarted += DissolveStarted;

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            // rulesetCharacter.ConditionAdded += ConditionAdded;
            // rulesetCharacter.ConditionRemoved += ConditionRemoved;
            // rulesetCharacter.ConditionRemovedForVisual += ConditionRemovedForVisual;
            // rulesetCharacter.ConditionOccurenceReached += ConditionOccurenceReached;
            // rulesetCharacter.ConditionSaveRerollRequested += ConditionSaveRerollRequested;
            // rulesetCharacter.ImmuneToSpell += ImmuneToSpell;
            // rulesetCharacter.ImmuneToSpellLevel += ImmuneToSpellLevel;
            // rulesetCharacter.ImmuneToDamage += ImmuneToDamage;
            // rulesetCharacter.DamageAltered += DamageAltered;
            // rulesetCharacter.ImmuneToCondition += ImmuneToCondition;
            // rulesetCharacter.SaveRolled += SaveRolled;
            // rulesetCharacter.DieRerolled += DieRerolled;
            // rulesetCharacter.AttackInitiated += AttackInitiated;
            // rulesetCharacter.AttackRolled += AttackRolled;
            // rulesetCharacter.IncomingAttackRolled += IncomingAttackRolled;
            // rulesetCharacter.AttackAutomaticHit += AttackAutomaticHit;
            // rulesetCharacter.AttackAutomaticCritical += AttackAutomaticCritical;
            // rulesetCharacter.DamageFormsTriggered += DamageFormsTriggered;
            // rulesetCharacter.HealingFormsTriggered += HealingFormsTriggered;
            // rulesetCharacter.IncomingDamageNotified += IncomingDamageNotified;
            // rulesetCharacter.AbilityScoreIncreased += AbilityScoreIncreased;
            // rulesetCharacter.DamageHalved += DamageHalved;
            // rulesetCharacter.DamageReduced += DamageReduced;
            // rulesetCharacter.ReplacedAbilityScoreForSave += ReplacedAbilityScoreForSave;
            // rulesetCharacter.AdditionalSaveDieRolled += AdditionalSaveDieRolled;
            // rulesetCharacter.DamageReceived += DamageReceived;
            // rulesetCharacter.AlterationInflicted += AlterationInflicted;
            // rulesetCharacter.SpellDissipated += SpellDissipated;
            // rulesetCharacter.TagRevealed += TagRevealed;
            // rulesetCharacter.ActorReplaced += ActorReplaced;

            if (rulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
            {
                return;
            }

            // rulesetCharacterHero.ItemEquipedCallback += ItemEquipped;

            // rulesetCharacterHero.CharacterInventory.ItemEquiped += ItemEquiped;
            // rulesetCharacterHero.CharacterInventory.ItemAltered += ItemAltered;
            // rulesetCharacterHero.CharacterInventory.ItemUnequiped += ItemUnequiped;
            // rulesetCharacterHero.CharacterInventory.ItemReleased += ItemReleased;      
        }
    }

    private static void LocationUnloading(string locationDefinitionName, string userLocationTitle)
    {
        Main.Info("Location Unloading");

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        // gameLocationCharacterService.CharacterCreated -= CharacterCreated;
        // gameLocationCharacterService.CharacterRevealed -= CharacterRevealed;
        // gameLocationCharacterService.CharacterKilled -= CharacterKilled;
        // gameLocationCharacterService.CharacterDestroying -= CharacterDestroying;

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

        foreach (var gameLocationCharacter in gameLocationCharacterService.ValidCharacters)
        {
            // gameLocationCharacter.Placed -= Placed;
            // gameLocationCharacter.MoveStarted -= MoveStarted;
            // gameLocationCharacter.PrepareChargeStarted -= PrepareChargeStarted;
            // gameLocationCharacter.ChargeStarted -= ChargeStarted;
            // gameLocationCharacter.ChargeEnded -= ChargeEnded;
            // gameLocationCharacter.ChargeAborted -= ChargeAborted;
            // gameLocationCharacter.UpdateMotions -= UpdateMotions;
            // gameLocationCharacter.TeleportStarted -= TeleportStarted;
            // gameLocationCharacter.Moved -= Moved;
            // gameLocationCharacter.Rotated -= Rotated;
            // gameLocationCharacter.Stopped -= Stopped;
            // gameLocationCharacter.FallStarted -= FallStarted;
            // gameLocationCharacter.FallStopped -= FallStopped;
            // gameLocationCharacter.CrawlStarted -= CrawlStarted;
            // gameLocationCharacter.BurrowStarted -= BurrowStarted;
            // gameLocationCharacter.BurrowEnded -= BurrowEnded;
            // gameLocationCharacter.JumpStarted -= JumpStarted;
            // gameLocationCharacter.JumpFinished -= JumpFinished;
            // gameLocationCharacter.VaultStarted -= VaultStarted;
            // gameLocationCharacter.VaultFinished -= VaultFinished;
            // gameLocationCharacter.ClimbStarted -= ClimbStarted;
            // gameLocationCharacter.ClimbFinished -= ClimbFinished;
            // gameLocationCharacter.ChangeSurfaceStarted -= ChangeSurfaceStarted;
            // gameLocationCharacter.AttackStart -= AttackStart;
            // gameLocationCharacter.AttackImpactStart -= AttackImpactStart;
            // gameLocationCharacter.DeflectAttackStart -= DeflectAttackStart;
            // gameLocationCharacter.ManipulateStart -= ManipulateStart;
            // gameLocationCharacter.ManipulateEnd -= ManipulateEnd;
            // gameLocationCharacter.ShoveStart -= ShoveStart;
            // gameLocationCharacter.CastingStart -= CastingStart;
            // gameLocationCharacter.HitStart -= HitStart;
            // gameLocationCharacter.PathFailed -= PathFailed;
            // gameLocationCharacter.DialogStarted -= DialogStarted;
            // gameLocationCharacter.DialogEnded -= DialogEnded;
            // gameLocationCharacter.DialogChoiceStarted -= DialogChoiceStarted;
            // gameLocationCharacter.DialogChoiceEnded -= DialogChoiceEnded;
            // gameLocationCharacter.SpeechStarted -= SpeechStarted;
            // gameLocationCharacter.SpeechEnded -= SpeechEnded;
            // gameLocationCharacter.ListenStarted -= ListenStarted;
            // gameLocationCharacter.ListenEnded -= ListenEnded;
            // gameLocationCharacter.AdditionalAnimationStarted -= AdditionalAnimationStarted;
            // gameLocationCharacter.TextFeedbackRequested -= TextFeedbackRequested;
            // gameLocationCharacter.InGameDialogLineRequested -= InGameDialogLineRequested;
            // gameLocationCharacter.AlreadySuccessful -= AlreadySuccessful;
            // gameLocationCharacter.AlreadyFailed -= AlreadyFailed;
            // gameLocationCharacter.ProneStatusChanged -= ProneStatusChanged;
            // gameLocationCharacter.IsAngryStatusChanged -= IsAngryStatusChanged;
            // gameLocationCharacter.UsedTacticalMovesChanged -= UsedTacticalMovesChanged;
            // gameLocationCharacter.CurrentMonsterAttackChanged -= CurrentMonsterAttackChanged;
            // gameLocationCharacter.DisolveStarted -= DissolveStarted;

            var rulesetCharacter = gameLocationCharacter.RulesetCharacter;

            // rulesetCharacter.ConditionAdded -= ConditionAdded;
            // rulesetCharacter.ConditionRemoved -= ConditionRemoved;
            // rulesetCharacter.ConditionRemovedForVisual -= ConditionRemovedForVisual;
            // rulesetCharacter.ConditionOccurenceReached -= ConditionOccurenceReached;
            // rulesetCharacter.ConditionSaveRerollRequested -= ConditionSaveRerollRequested;
            // rulesetCharacter.ImmuneToSpell -= ImmuneToSpell;
            // rulesetCharacter.ImmuneToSpellLevel -= ImmuneToSpellLevel;
            // rulesetCharacter.ImmuneToDamage -= ImmuneToDamage;
            // rulesetCharacter.DamageAltered -= DamageAltered;
            // rulesetCharacter.ImmuneToCondition -= ImmuneToCondition;
            // rulesetCharacter.SaveRolled -= SaveRolled;
            // rulesetCharacter.DieRerolled -= DieRerolled;
            // rulesetCharacter.AttackInitiated -= AttackInitiated;
            // rulesetCharacter.AttackRolled -= AttackRolled;
            // rulesetCharacter.IncomingAttackRolled -= IncomingAttackRolled;
            // rulesetCharacter.AttackAutomaticHit -= AttackAutomaticHit;
            // rulesetCharacter.AttackAutomaticCritical -= AttackAutomaticCritical;
            // rulesetCharacter.DamageFormsTriggered -= DamageFormsTriggered;
            // rulesetCharacter.HealingFormsTriggered -= HealingFormsTriggered;
            // rulesetCharacter.IncomingDamageNotified -= IncomingDamageNotified;
            // rulesetCharacter.AbilityScoreIncreased -= AbilityScoreIncreased;
            // rulesetCharacter.DamageHalved -= DamageHalved;
            // rulesetCharacter.DamageReduced -= DamageReduced;
            // rulesetCharacter.ReplacedAbilityScoreForSave -= ReplacedAbilityScoreForSave;
            // rulesetCharacter.AdditionalSaveDieRolled -= AdditionalSaveDieRolled;
            // rulesetCharacter.DamageReceived -= DamageReceived;
            // rulesetCharacter.AlterationInflicted -= AlterationInflicted;
            // rulesetCharacter.SpellDissipated -= SpellDissipated;
            // rulesetCharacter.TagRevealed -= TagRevealed;
            // rulesetCharacter.ActorReplaced -= ActorReplaced;

            if (rulesetCharacter is not RulesetCharacterHero rulesetCharacterHero)
            {
                return;
            }

            // rulesetCharacterHero.ItemEquipedCallback -= ItemEquipped;
            // rulesetCharacterHero.CharacterInventory.ItemEquiped -= ItemEquiped;
            // rulesetCharacterHero.CharacterInventory.ItemAltered -= ItemAltered;
            // rulesetCharacterHero.CharacterInventory.ItemUnequiped -= ItemUnequiped;
            // rulesetCharacterHero.CharacterInventory.ItemReleased -= ItemReleased;        
        }
    }

    //
    // IGameLocationCharacterService
    //

    private static void CharacterCreated(GameLocationCharacter gameLocationCharacter)
    {
        Main.Info($"{gameLocationCharacter.Name} Character Created");
    }

    private static void CharacterRevealed(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Character Revealed");
    }

    private static void CharacterDestroying(GameLocationCharacter gameLocationCharacter)
    {
        Main.Info($"{gameLocationCharacter.Name} Character Destroying");
    }

    private static void CharacterKilled(GameLocationCharacter character, bool considerDead)
    {
        Main.Info($"{character.Name} Character Killed");
    }

    //
    // RulesetCharacterHero
    //

    private static void ItemEquipped(RulesetCharacterHero hero, RulesetItem item)
    {
        Main.Info($"{hero.Name} Item Equipped Hero");
    
        WizardBladeDancer.OnItemEquipped(hero);
        CollegeOfWarDancer.OnItemEquipped(hero);
    }

    //
    // CharacterInventory
    //

    private static void ItemEquiped(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Info("Item Equipped");
    }
    
    private static void ItemAltered(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Info("Item Altered");
    }
    
    private static void ItemUnequiped(
        RulesetInventory characterInventory,
        RulesetInventorySlot slot,
        RulesetItem item)
    {
        Main.Info("Item Unequipped");
    }
    
    private static void ItemReleased(RulesetItem item, bool canKeep)
    {
        Main.Info("Item Released");
    }

    //
    // IGameLocationActionService
    //

    private static void ActionStarted(CharacterAction characterAction)
    {
        Main.Info($"{characterAction.ActingCharacter?.Name} -> {characterAction.ActionDefinition.Name} STARTED");

        Global.ActionStarted(characterAction);
    }

    private static void ActionChainStarted(CharacterActionChainParams characterActionChainParams)
    {
        Main.Info($"{characterActionChainParams.ActingCharacter.Name} Action Chain Started");
    }

    private static void ActionChainFinished(
        CharacterActionChainParams characterActionChainParams,
        bool aborted)
    {
        Main.Info($"{characterActionChainParams.ActingCharacter.Name} Action Chain Finished");
    }

    private static void MagicEffectPreparing(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Info($"{data.Caster.Name} Magic Effect Preparing");
    }

    private static void MagicEffectPreparingOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Info($"{data.Caster.Name} Magic Effect Preparing On Target");
    }

    private static void MagicEffectLaunch(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Info($"{data.Caster.Name} Magic Effect Launch");
    }

    private static void MagicEffectCastOnTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Info($"{data.Caster.Name} Magic Effect Cast On Target");
    }

    private static void MagicEffectCastOnZone(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Info($"{data.Caster.Name} Magic Effect Cast On Zone");
    }

    private static void MagicEffectBeforeHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Info($"{data.Caster.Name} Magic Effect Before Hit Target");
    }

    private static void MagicEffectHitTarget(ref ActionDefinitions.MagicEffectCastData data)
    {
        Main.Info($"{data.Caster.Name} Magic Effect Hit Target");
    }

    private static void SpellCast(string spellDefinitionName, int fromDevice)
    {
        Main.Info($"{spellDefinitionName} Spell Cast");
    }

    private static void ActionUsed(
        GameLocationCharacter actingCharacter,
        CharacterActionParams actionParams,
        ActionDefinition actionDefinition)
    {
        Main.Info($"{actingCharacter.Name} -> {actionDefinition.Name} FINISHED");
    }

    private static void ShoveActionUsed(
        GameLocationCharacter actingCharacter,
        GameLocationCharacter attackingCharacter,
        ActionDefinition actionDefinition,
        bool success)
    {
        Main.Info($"{actingCharacter.Name} Shove Action Used");
    }

    private static void ItemUsed(string itemDefinitionName)
    {
        Main.Info("Item Used");
    }

    //
    // GameLocationCharacter
    //

    private static void Placed(GameLocationCharacter character, int3 location)
    {
        Main.Info($"{character.Name} Placed");
    }

    private static void MoveStarted(ref GameLocationCharacterDefinitions.MoveStartedParameters parameters)
    {
        Main.Info("Move Started");
    }

    private static void PrepareChargeStarted(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Info($"{character.Name} Prepare Charge Started");
    }

    private static void ChargeStarted(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Info($"{character.Name} Charge Started");
    }

    private static void ChargeEnded(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Info($"{character.Name} Charge Ended");
    }

    private static void ChargeAborted(
        GameLocationCharacter character,
        CharacterActionParams actionParams)
    {
        Main.Info($"{character.Name} Charge Aborted");
    }

    private static void TeleportStarted(
        GameLocationCharacter character,
        int3 destination,
        LocationDefinitions.Orientation destinationOrientation,
        bool sendEvent)
    {
        Main.Info($"{character.Name} Teleport Started");
    }

    private static void Moved(ref GameLocationCharacterDefinitions.MovedParameters parameters)
    {
        Main.Info("Moved");
    }

    private static void Stopped(CharacterAction.InterruptionType interruption)
    {
        Main.Info("Stopped");
    }

    private static void FallStarted(
        GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool featherFall)
    {
        Main.Info($"{character.Name} Fall Started");
    }

    private static void FallStopped(
        GameLocationCharacter character,
        int3 source,
        int3 destination)
    {
        Main.Info($"{character.Name} Fall Stopped");
    }

    private static void CrawlStarted(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Crawl Started");
    }

    private static void BurrowStarted(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Burrow Started");
    }

    private static void BurrowEnded(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Burrow Ended");
    }

    private static void JumpStarted(
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        bool useMomentum,
        bool dragonJump,
        bool buletteLeap)
    {
        Main.Info("Jump Started");
    }

    private static void JumpFinished(
        GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool jumpFailed)
    {
        Main.Info($"{character.Name} Jump Finished");
    }

    private static void VaultStarted(
        int3 source,
        int3 destination,
        LocationDefinitions.Orientation orientation,
        float distance,
        float duration)
    {
        Main.Info("Vault Started");
    }

    private static void VaultFinished(int3 source, int3 destination)
    {
        Main.Info("Vault Finished");
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
        Main.Info($"{character.Name} Climb Started");
    }

    private static void ClimbFinished(
        GameLocationCharacter character,
        int3 source,
        int3 destination,
        bool easyClimb)
    {
        Main.Info($"{character.Name} Climb Finished");
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
        Main.Info($"{character.Name} Change Surface Started");
    }

    private static void AttackStart(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        Main.Info($"{attacker.Name},{defender.Name} Attack Start");

        //PATCH: support for `IOnAttackHitEffect` - calls before attack handlers
        var character = attacker.RulesetCharacter;

        if (character == null)
        {
            return;
        }

        var features = character.GetSubFeaturesByType<IBeforeAttackEffect>();

        foreach (var effect in features)
        {
            effect.BeforeOnAttackHit(attacker, defender, outcome, actionParams, attackMode, attackModifier);
        }
    }

    private static void AttackImpactStart(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        RuleDefinitions.RollOutcome outcome,
        CharacterActionParams actionParams,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier)
    {
        Main.Info($"{attacker.Name},{defender.Name}Attack Impact Start");

        //PATCH: support for `IOnAttackHitEffect` - calls after attack handlers
        var character = attacker.RulesetCharacter;

        if (character == null)
        {
            return;
        }

        var features = character.GetSubFeaturesByType<IAfterAttackEffect>();

        foreach (var effect in features)
        {
            effect.AfterOnAttackHit(attacker, defender, outcome, actionParams, attackMode, attackModifier);
        }
    }

    private static void DeflectAttackStart(
        GameLocationCharacter blocker,
        GameLocationCharacter attacker,
        bool isMonkDeflectMissile)
    {
        Main.Info($"{blocker.Name},{attacker.Name} Deflect Attack Start");
    }

    private static void ManipulateStart(
        GameLocationCharacter character,
        AnimationDefinitions.ManipulationType animation)
    {
        Main.Info($"{character.Name} Manipulate Start");
    }

    private static void ManipulateEnd(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Manipulate End");
    }

    private static void ShoveStart(
        GameLocationCharacter attacker,
        GameLocationCharacter target,
        bool success)
    {
        Main.Info($"{attacker.Name},{target.Name} Shove Start");
    }

    private static void CastingStart(
        ref ActionDefinitions.MagicEffectCastData magicEffectCastData)
    {
        Main.Info("Casting Start");
    }

    private static void HitStart(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Hit Start");
    }

    private static void PathFailed(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Path Failed");
    }
 
    private static void DialogStarted(
        GameLocationCharacter character,
        NarrativeSequence narrativeSequence)
    {
        Main.Info($"{character.Name} Dialog Started");
    }

    private static void DialogEnded(
        GameLocationCharacter character,
        NarrativeSequence narrativeSequence)
    {
        Main.Info($"{character.Name} Dialog Ended");
    }

    private static void DialogChoiceStarted(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Dialog Choice Started");
    }

    private static void DialogChoiceEnded(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Dialog Choice Ended");
    }

    private static void SpeechStarted(
        ref GameLocationCharacterDefinitions.SpeechStartedParameters parameters)
    {
        Main.Info("Speech Started");
    }

    private static void SpeechEnded(GameLocationCharacter character, bool forceStop)
    {
        Main.Info($"{character.Name} Speech Ended");
    }

    private static void ListenStarted(
        GameLocationCharacter character,
        GameLocationCharacter speaker,
        bool lookAtOverriden,
        bool facialExpressionOverriden)
    {
        Main.Info($"{character.Name} Listen Started");
    }

    private static void ListenEnded(
        GameLocationCharacter character,
        GameLocationCharacter speaker)
    {
        Main.Info($"{character.Name} Listen Ended");
    }

    private static void TextFeedbackRequested(
        GameLocationCharacter character,
        string colorStyle,
        string text)
    {
        Main.Info($"{character.Name} Text Feedback Requested");
    }

    private static void InGameDialogLineRequested(
        GameLocationCharacter character,
        string line,
        float duration,
        bool banterPoolElement,
        bool spoken)
    {
        Main.Info($"{character.Name} In Game Dialog Line Requested");
    }

    private static void AlreadySuccessful(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Already Successful");
    }

    private static void AlreadyFailed(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Already Failed");
    }

    private static void ProneStatusChanged(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Prone Status Changed");
    }

    private static void IsAngryStatusChanged(GameLocationCharacter character, bool status)
    {
        Main.Info($"{character.Name} IsAngry Status Changed");
    }

    private static void UsedTacticalMovesChanged(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Used Tactical Moves Changed");
    }

    private static void CurrentMonsterAttackChanged(
        GameLocationCharacter character,
        MonsterAttackDefinition monsterAttackDefinition,
        Action graphicsRefreshedCallback)
    {
        Main.Info($"{character.Name} Current Monster Attack Changed");
    }

    private static void DissolveStarted(GameLocationCharacter character)
    {
        Main.Info($"{character.Name} Dissolve Started");
    }

    //
    // RulesetCharacter
    //

    private static void ConditionAdded(RulesetActor character, RulesetCondition addedActiveCondition)
    {
        Main.Info("ConditionAdded");
    }
    
    private static void ConditionRemoved(RulesetActor character, RulesetCondition removedActiveCondition)
    {
        Main.Info("ConditionRemoved");
    }
    
    private static void ConditionRemovedForVisual(
        RulesetActor character,
        RulesetCondition removedActiveCondition,
        bool showGraphics = true)
    {
        Main.Info("ConditionRemovedForVisual");
    }
    
    private static void ConditionOccurenceReached(RulesetActor character, RulesetCondition removedActiveCondition)
    {
        Main.Info("ConditionOccurenceReached");
    }
    
    private static void ConditionSaveRerollRequested(RulesetActor character, RulesetCondition activeCondition,
        RuleDefinitions.AdvantageType advantageType, bool removeOnSuccess, out bool success)
    {
        Main.Info("ConditionSaveRerollRequested");
    }
    
    private static void ImmuneToSpell(RulesetActor character, SpellDefinition spellDefinition)
    {
        Main.Info("ImmuneToSpell");
    }
    
    private static void ImmuneToSpellLevel(RulesetActor character, SpellDefinition spellDefinition, int maxSpellLevel)
    {
        Main.Info("ImmuneToSpellLevel");
    }
    
    private static void ImmuneToDamage(RulesetActor character, string damageType, bool silent)
    {
        Main.Info("ImmuneToDamage");
    }
    
    private static void DamageAltered(
        RulesetActor character,
        string damageType,
        RuleDefinitions.DamageAffinityType damageAffinityType,
        int flatDamageReduction,
        bool silent)
    {
        Main.Info("DamageAltered");
    }
    
    private static void ImmuneToCondition(RulesetActor character, string conditionName, ulong sourceGuid)
    {
        Main.Info("ImmuneToCondition");
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
        Main.Info("SaveRolled");
    }
    
    private static void DieRerolled(
        RulesetActor character,
        RuleDefinitions.DieType dieType,
        int previousValue,
        int newValue,
        string localizationKey)
    {
        Main.Info("DieRerolled");
    }
    
    private static void AttackInitiated(
        RulesetActor character,
        int firstRoll,
        int secondRoll,
        int modifier,
        RuleDefinitions.AdvantageType advantageType)
    {
        Main.Info("AttackInitiated");
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
        bool opportunity = false,
        ActionDefinitions.ReactionCounterAttackType reactionCounterAttackType =
            ActionDefinitions.ReactionCounterAttackType.None)
    {
        Main.Info("AttackRolled");
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
        Main.Info("IncomingAttackRolled");
    }
    
    private static void AttackAutomaticHit(RulesetActor character, RulesetActor target, BaseDefinition attackMethod)
    {
        Main.Info("AttackAutomaticHit");
    }
    
    private static void AttackAutomaticCritical(RulesetActor target)
    {
        Main.Info("AttackAutomaticCritical");
    }
    
    private static void DamageFormsTriggered(RulesetActor character, List<EffectGroupInfo> damageInfos)
    {
        Main.Info("DamageFormsTriggered");
    }
    
    private static void HealingFormsTriggered(RulesetActor character, List<EffectGroupInfo> healingInfos)
    {
        Main.Info("HealingFormsTriggered");
    }
    
    private static void IncomingDamageNotified(
        RulesetActor character,
        RuleDefinitions.EffectSourceType damageType,
        BaseDefinition attackVectorDefinition)
    {
        Main.Info("IncomingDamageNotified");
    }
    
    private static void AbilityScoreIncreased(
        RulesetActor character,
        string abilityScore,
        int valueIncrease,
        int maxIncrease)
    {
        Main.Info("AbilityScoreIncreased");
    }
    
    private static void DamageHalved(RulesetActor character, FeatureDefinition feature)
    {
        Main.Info("DamageHalved");
    }
    
    private static void DamageReduced(RulesetActor character, FeatureDefinition feature, int reductionAmount)
    {
        Main.Info("DamageReduced");
    }
    
    private static void ReplacedAbilityScoreForSave(
        RulesetActor saver,
        FeatureDefinition feature,
        string originalAbilityScore,
        string replacedAbilityScore)
    {
        Main.Info("ReplacedAbilityScoreForSave");
    }
    
    private static void AdditionalSaveDieRolled(
        RulesetActor character,
        RuleDefinitions.TrendInfo trendInfo)
    {
        Main.Info("AdditionalSaveDieRolled");
    }
    
    private static void DamageReceived(
        RulesetActor target,
        int damage,
        string damageType,
        ulong sourceGuid,
        RollInfo rollInfo)
    {
        Main.Info("DamageReceived");
    }
    
    private static void AlterationInflicted(
        RulesetActor source,
        RulesetActor target,
        AlterationForm.Type alterationType)
    {
        Main.Info("AlterationInflicted");
    }
    
    private static void SpellDissipated(
        RulesetActor source,
        RulesetActor target,
        SpellDefinition spellDefinition,
        bool success)
    {
        Main.Info("SpellDissipated");
    }
    
    private static void TagRevealed(RulesetActor actor, string revealedTag)
    {
        Main.Info("TagRevealed");
    }
    
    private static void ActorReplaced(RulesetActor originalActor, RulesetActor newActor)
    {
        Main.Info("ActorReplaced");
    }
}
#endif
