using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.ModKit.Utility;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Validators;
using TA;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class GameLocationCharacterExtensions
{
    internal static List<ActionModifier> GetActionModifiers(int count)
    {
        var actionModifiers = new List<ActionModifier>();

        for (var i = 0; i < count; i++)
        {
            actionModifiers.Add(new ActionModifier());
        }

        return actionModifiers;
    }

    #region Actions

    internal static void MyExecuteActionAttack(
        this GameLocationCharacter attacker,
        Id actionId,
        GameLocationCharacter defender,
        RulesetAttackMode attackMode,
        ActionModifier actionModifier)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var actionParams = new CharacterActionParams(
            attacker,
            actionId,
            attackMode,
            defender,
            actionModifier);

        actionService.ExecuteAction(actionParams, null, true);
    }

    internal static void MyExecuteActionCastNoCost(
        this GameLocationCharacter caster,
        SpellDefinition spell,
        int slotLevel,
        CharacterActionParams originalActionParams,
        RulesetSpellRepertoire spellRepertoire = null)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCaster = caster.RulesetCharacter;
        var effectSpell = ServiceRepository.GetService<IRulesetImplementationService>()
            .InstantiateEffectSpell(rulesetCaster, spellRepertoire, spell, slotLevel, false);
        var actionParams = originalActionParams.Clone();

        actionParams.ActionDefinition = actionService.AllActionDefinitions[Id.CastNoCost];
        actionParams.RulesetEffect = effectSpell;
        actionService.ExecuteAction(actionParams, null, true);
    }

    internal static void MyExecuteActionDodge(this GameLocationCharacter character)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

        actionService.ExecuteAction(new CharacterActionParams(character, Id.Dodge), null, true);
    }

    internal static void MyExecuteActionPowerNoCost(
        this GameLocationCharacter character, RulesetUsablePower usablePower, params GameLocationCharacter[] targets)
    {
        var actionModifiers = GetActionModifiers(targets.Length);
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
        var rulesetCharacter = character.RulesetCharacter;
        var actionParams = new CharacterActionParams(character, Id.PowerNoCost)
        {
            ActionModifiers = actionModifiers,
            RulesetEffect = implementationService.InstantiateEffectPower(rulesetCharacter, usablePower, false),
            UsablePower = usablePower,
            targetCharacters = [.. targets]
        };

        actionService.ExecuteAction(actionParams, null, true);
    }

    internal static void MyExecuteActionSpendPower(
        this GameLocationCharacter character, RulesetUsablePower usablePower, params GameLocationCharacter[] targets)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
        var rulesetCharacter = character.RulesetCharacter;
        var actionParams = new CharacterActionParams(character, Id.SpendPower)
        {
            StringParameter = usablePower.PowerDefinition.Name,
            RulesetEffect = implementationService.InstantiateEffectPower(rulesetCharacter, usablePower, false),
            UsablePower = usablePower,
            targetCharacters = [.. targets]
        };

        actionService.ExecuteInstantSingleAction(actionParams);
    }

    internal static void MyExecuteActionStabilizeAndStandUp(this GameLocationCharacter character, int hitPoints)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var rulesetCharacter = character.RulesetCharacter;

        rulesetCharacter.StabilizeAndGainHitPoints(hitPoints);

        actionService.ExecuteInstantSingleAction(new CharacterActionParams(character, Id.StandUp));
    }

    internal static void MyExecuteActionTacticalMove(this GameLocationCharacter character, int3 position)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        if (!actionManager)
        {
            return;
        }

        var actionParams = new CharacterActionParams(
            character, Id.TacticalMove, MoveStance.Run, position, LocationDefinitions.Orientation.North)
        {
            BoolParameter3 = false, BoolParameter5 = false
        };

        actionManager!.actionChainByCharacter.TryGetValue(character, out var actionChainSlot);

        var collection = actionChainSlot?.actionQueue;

        if (collection is { Count: > 0 } &&
            collection[0].action is CharacterActionMoveStepWalk)
        {
            actionParams.BoolParameter2 = true;
        }

        actionManager.ExecuteActionChain(
            new CharacterActionChainParams(actionParams.ActingCharacter, actionParams), null, false);
    }

    #endregion

    #region Reactions

    internal static IEnumerator MyReactForOpportunityAttack(
        this GameLocationCharacter attacker,
        GameLocationCharacter defender,
        GameLocationCharacter waiter,
        RulesetAttackMode attackMode,
        ActionModifier actionModifier,
        string stringParameter2,
        Action reactionValidated = null,
        GameLocationBattleManager battleManager = null,
        ReactionResourcePowerPool resource = null)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        battleManager ??= ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!actionManager || !battleManager)
        {
            yield break;
        }

        var reactionParams = new CharacterActionParams(
            attacker,
            Id.AttackOpportunity,
            attackMode,
            defender,
            actionModifier) { StringParameter2 = stringParameter2 };
        var reactionRequest =
            new ReactionRequestReactionAttack(stringParameter2, reactionParams) { Resource = resource };
        var count = actionManager.PendingReactionRequestGroups.Count;

        actionManager.AddInterruptRequest(reactionRequest);

        yield return battleManager.WaitForReactions(waiter, actionManager, count);

        if (reactionParams.ReactionValidated)
        {
            reactionValidated?.Invoke();
        }
    }

    internal static IEnumerator MyReactToCastSpell(
        this GameLocationCharacter caster,
        SpellDefinition spell,
        GameLocationCharacter target,
        GameLocationCharacter waiter,
        Action<CharacterActionParams> reactionValidated = null,
        GameLocationBattleManager battleManager = null)
    {
        battleManager ??= ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!battleManager)
        {
            yield break;
        }

        var ruleCaster = caster.RulesetCharacter;
        var slotLevel = ruleCaster.GetLowestSlotLevelAndRepertoireToCastSpell(spell, out var repertoire);

        if (slotLevel < spell.SpellLevel || repertoire == null)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var reactionParams = new CharacterActionParams(caster, Id.CastReaction)
        {
            ActionModifiers = { new ActionModifier() },
            IntParameter = slotLevel,
            StringParameter = spell.Name,
            RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectSpell(ruleCaster, repertoire, spell, slotLevel, false),
            SpellRepertoire = repertoire,
            TargetCharacters = { target },
            IsReactionEffect = true
        };
        var count = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToSpendSpellSlot(reactionParams);

        yield return battleManager.WaitForReactions(waiter, actionService, count);

        if (reactionParams.ReactionValidated)
        {
            reactionValidated?.Invoke(reactionParams);
        }
    }

    internal static IEnumerator MyReactToDoNothing(
        this GameLocationCharacter character,
        ExtraActionId actionId,
        GameLocationCharacter waiter,
        string type,
        string stringParameter,
        Action reactionValidated = null,
        Action reactionNotValidated = null,
        GameLocationBattleManager battleManager = null,
        ICustomReactionResource resource = null)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        battleManager ??= ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!actionManager || !battleManager)
        {
            yield break;
        }

        var reactionParams = new CharacterActionParams(character, (Id)actionId)
        {
            StringParameter = stringParameter, IsReactionEffect = actionId == ExtraActionId.DoNothingReaction
        };
        var reactionRequest = new ReactionRequestCustom(type, reactionParams) { Resource = resource };
        var count = actionManager.PendingReactionRequestGroups.Count;

        actionManager.AddInterruptRequest(reactionRequest);

        yield return battleManager.WaitForReactions(waiter, actionManager, count);

        if (reactionParams.ReactionValidated)
        {
            reactionValidated?.Invoke();
        }
        else
        {
            reactionNotValidated?.Invoke();
        }
    }

    internal static IEnumerator MyReactToSpendPower(
        this GameLocationCharacter character,
        RulesetUsablePower usablePower,
        GameLocationCharacter waiter,
        string stringParameter,
        string stringParameter2 = "",
        Action reactionValidated = null,
        GameLocationBattleManager battleManager = null)
    {
        battleManager ??= ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!battleManager)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
        var reactionParams = new CharacterActionParams(character, Id.SpendPower)
        {
            StringParameter = stringParameter,
            StringParameter2 = stringParameter2,
            RulesetEffect =
                implementationService.InstantiateEffectPower(character.RulesetCharacter, usablePower, false),
            UsablePower = usablePower
        };
        var count = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToSpendPower(reactionParams);

        yield return battleManager.WaitForReactions(waiter, actionService, count);

        if (reactionParams.ReactionValidated)
        {
            reactionValidated?.Invoke();
        }
    }

    internal static IEnumerator MyReactToSpendPowerBundle(
        this GameLocationCharacter character,
        RulesetUsablePower usablePower,
        List<GameLocationCharacter> targets,
        GameLocationCharacter waiter,
        string stringParameter,
        string stringParameter2 = "",
        Action<ReactionRequestSpendBundlePower> reactionValidated = null,
        Action<ReactionRequestSpendBundlePower> reactionNotValidated = null,
        GameLocationBattleManager battleManager = null)
    {
        var actionManager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

        battleManager ??= ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!actionManager || !battleManager)
        {
            yield break;
        }

        var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
        var reactionParams = new CharacterActionParams(character, Id.SpendPower)
        {
            StringParameter = stringParameter,
            StringParameter2 = stringParameter2,
            ActionModifiers = GetActionModifiers(targets.Count),
            RulesetEffect =
                implementationService.InstantiateEffectPower(character.RulesetCharacter, usablePower, false),
            UsablePower = usablePower,
            targetCharacters = targets
        };
        var reactionRequest =
            new ReactionRequestSpendBundlePower(reactionParams, reactionValidated, reactionNotValidated);
        var count = actionManager.PendingReactionRequestGroups.Count;

        actionManager.AddInterruptRequest(reactionRequest);

        yield return battleManager.WaitForReactions(waiter, actionManager, count);
    }

    internal static IEnumerator MyReactToUsePower(
        this GameLocationCharacter character,
        Id actionId,
        RulesetUsablePower usablePower,
        List<GameLocationCharacter> targets,
        GameLocationCharacter waiter,
        string stringParameter,
        string stringParameter2 = "",
        Action reactionValidated = null,
        GameLocationBattleManager battleManager = null)
    {
        battleManager ??= ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

        if (!battleManager)
        {
            yield break;
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var implementationService = ServiceRepository.GetService<IRulesetImplementationService>();
        var actionParams = new CharacterActionParams(character, actionId)
        {
            StringParameter = stringParameter,
            StringParameter2 = stringParameter2,
            ActionModifiers = GetActionModifiers(targets.Count),
            RulesetEffect =
                implementationService.InstantiateEffectPower(character.RulesetCharacter, usablePower, false),
            UsablePower = usablePower,
            targetCharacters = targets,
            IsReactionEffect = actionId == Id.PowerReaction
        };
        var count = actionService.PendingReactionRequestGroups.Count;

        actionService.ReactToUsePower(actionParams, "UsePower", character);

        yield return battleManager.WaitForReactions(waiter, actionService, count);

        if (actionParams.ReactionValidated)
        {
            reactionValidated?.Invoke();
        }
    }

    #endregion

    #region Shift Key Status

    private const string ShiftKeyState = "ShiftKeyState";

    internal static bool GetAndClearShiftState(this GameLocationCharacter character)
    {
        var result = character.GetSpecialFeatureUses(ShiftKeyState) == 1;

        character.SetSpecialFeatureUses(ShiftKeyState, 0);

        return result;
    }

    internal static void RegisterShiftState(this GameLocationCharacter character)
    {
        character.SetSpecialFeatureUses(ShiftKeyState, Global.IsShiftPressed ? 1 : 0);
    }

    #endregion

    internal static GameLocationCharacter GetEffectControllerOrSelf(this GameLocationCharacter character)
    {
        if (character.RulesetCharacter is not RulesetCharacterEffectProxy effectProxy)
        {
            return character;
        }

        var controllerCharacter = EffectHelpers.GetCharacterByGuid(effectProxy.ControllerGuid);

        if (controllerCharacter == null)
        {
            return character;
        }

        var locationController = GameLocationCharacter.GetFromActor(controllerCharacter);

        return locationController ?? character;
    }

    public static bool IsMyTurn(this GameLocationCharacter character)
    {
        return Gui.Battle != null && Gui.Battle.ActiveContenderIgnoringLegendary == character;
    }

    public static bool IsWithinRange(
        this GameLocationCharacter source, GameLocationCharacter target, int range)
    {
        //PATCH: use better distance calculation algorithm
        return DistanceCalculation.GetDistanceFromCharacters(source, target) <= range;
    }

    // consolidate all checks if a character can perceive another
    public static bool CanPerceiveTarget(
        this GameLocationCharacter __instance, GameLocationCharacter target)
    {
        if (__instance == target)
        {
            return true;
        }

        var vanillaCanPerceive =
            (__instance.Side == target.Side && __instance.PerceivedAllies.Contains(target)) ||
            (__instance.Side != target.Side && __instance.PerceivedFoes.Contains(target));

        if (!Main.Settings.UseOfficialLightingObscurementAndVisionRules) // || !vanillaCanPerceive)
        {
            return vanillaCanPerceive;
        }

        // can only perceive targets on cells that can be perceived
        var visibilityService = ServiceRepository.GetService<IGameLocationVisibilityService>();

        return visibilityService.MyIsCellPerceivedByCharacter(target.LocationPosition, __instance, target);
    }

    internal static (RulesetAttackMode mode, ActionModifier modifier) GetFirstMeleeModeThatCanAttack(
        this GameLocationCharacter instance,
        GameLocationCharacter target,
        IGameLocationBattleService service,
        bool allowUnarmed = false)
    {
        foreach (var mode in instance.RulesetCharacter.AttackModes)
        {
            // don't use IsMelee(attackMode) here
            var isValid = (allowUnarmed && mode.SourceObject is null) ||
                          (mode.SourceObject is RulesetItem rulesetItem &&
                           ValidatorsWeapon.IsMelee(null, rulesetItem, instance.RulesetCharacter));

            if (!isValid)
            {
                continue;
            }

            // Prepare attack evaluation params
            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var modifier = new ActionModifier();

            attackParams.FillForPhysicalReachAttack(
                instance, instance.LocationPosition, mode, target, target.LocationPosition, modifier);

            // Check if the attack is possible and collect the attack modifier inside the attackParams
            if (service.CanAttack(attackParams))
            {
                return (mode, modifier);
            }
        }

        return (null, null);
    }

    internal static (RulesetAttackMode mode, ActionModifier modifier) GetFirstRangedModeThatCanAttack(
        this GameLocationCharacter instance, GameLocationCharacter target, IGameLocationBattleService service)
    {
        foreach (var mode in instance.RulesetCharacter.AttackModes)
        {
            if (mode.Reach)
            {
                continue;
            }

            // Prepare attack evaluation params
            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var modifier = new ActionModifier();

            attackParams.FillForPhysicalRangeAttack(
                instance, instance.LocationPosition, mode, target, target.LocationPosition, modifier);

            // Check if the attack is possible and collect the attack modifier inside the attackParams
            if (service.CanAttack(attackParams))
            {
                return (mode, modifier);
            }
        }

        return (null, null);
    }

    internal static RulesetAttackMode GetFirstRangedModeThatCanBeReadied(this GameLocationCharacter instance)
    {
        return instance.RulesetCharacter.AttackModes
            .Where(mode => mode.ActionType == ActionType.Main)
            .FirstOrDefault(mode => mode.Ranged || mode.Thrown);
    }

    /**
     * Finds first attack mode that can attack target on positionBefore, but can't on positionAfter
     */
    internal static bool CanPerformOpportunityAttackOnCharacter(
        this GameLocationCharacter instance,
        GameLocationCharacter target,
        int3? positionBefore,
        int3? positionAfter,
        out RulesetAttackMode attackMode,
        out ActionModifier attackModifier,
        bool allowRange = false,
        IGameLocationBattleService service = null,
        bool accountAoOImmunity = false,
        IsWeaponValidHandler weaponValidator = null)
    {
        service ??= ServiceRepository.GetService<IGameLocationBattleService>();
        attackMode = null;
        attackModifier = null;

        if (accountAoOImmunity && !service.IsValidAttackerForOpportunityAttackOnCharacter(instance, target))
        {
            return false;
        }

        foreach (var mode in instance.RulesetCharacter.AttackModes)
        {
            if (mode.Ranged && !allowRange)
            {
                continue;
            }

            if (!(weaponValidator?.Invoke(mode, null, instance.RulesetCharacter) ?? true))
            {
                continue;
            }

            // Prepare attack evaluation params
            var paramsBefore = new BattleDefinitions.AttackEvaluationParams();

            if (mode.Ranged)
            {
                paramsBefore.FillForPhysicalRangeAttack(instance, instance.LocationPosition, mode,
                    target, positionBefore ?? target.LocationPosition, new ActionModifier());
            }
            else
            {
                paramsBefore.FillForPhysicalReachAttack(instance, instance.LocationPosition, mode,
                    target, positionBefore ?? target.LocationPosition, new ActionModifier());
            }

            // Check if the attack is possible and collect the attack modifier inside the attackParams
            if (!service.CanAttack(paramsBefore))
            {
                continue;
            }

            if (positionAfter != null)
            {
                var paramsAfter = new BattleDefinitions.AttackEvaluationParams();

                paramsAfter.FillForPhysicalReachAttack(instance, instance.LocationPosition, mode,
                    target, positionAfter.Value, new ActionModifier());

                // skip if attack is still possible after move - target hasn't left reach yet
                if (service.CanAttack(paramsAfter))
                {
                    continue;
                }
            }

            attackMode = mode;
            attackModifier = paramsBefore.attackModifier;

            return true;
        }

        return false;
    }

    internal static bool CanAct(this GameLocationCharacter character)
    {
        var rulesetCharacter = character.RulesetCharacter;

        return rulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
               !character.IsCharging &&
               !character.MoveStepInProgress &&
               !rulesetCharacter.IsIncapacitated &&
               !rulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionCharmed) &&
               !rulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionFrightened) &&
               !rulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionProne) &&
               !rulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionStunned) &&
               !rulesetCharacter.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionParalyzed);
    }

    internal static bool IsReactionAvailable(this GameLocationCharacter instance, bool ignoreReactionUses = false)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

        var hasReactionInQueue = actionService.PendingReactionRequestGroups
            .SelectMany(x => x.Requests)
            .Any(x => x.Character == instance);

        if (hasReactionInQueue)
        {
            return false;
        }

        if (!ignoreReactionUses)
        {
            return instance.GetActionTypeStatus(ActionType.Reaction) == ActionStatus.Available;
        }

        var wasUsed = instance.currentActionRankByType[ActionType.Reaction] > 0;

        if (wasUsed)
        {
            instance.currentActionRankByType[ActionType.Reaction]--;
        }

        var canReact = instance.GetActionTypeStatus(ActionType.Reaction) == ActionStatus.Available;

        if (wasUsed)
        {
            instance.currentActionRankByType[ActionType.Reaction]++;
        }

        return canReact;
    }

    internal static bool CanReact(this GameLocationCharacter instance, bool ignoreReactionUses = false)
    {
        return instance.CanAct() && IsReactionAvailable(instance, ignoreReactionUses);
    }

    internal static bool OnceInMyTurnIsValid(this GameLocationCharacter instance, string key)
    {
        return instance.OncePerTurnIsValid(key) &&
               Gui.Battle != null && Gui.Battle.ActiveContender == instance;
    }

    internal static void IncrementSpecialFeatureUses(this GameLocationCharacter instance, string key)
    {
        instance.UsedSpecialFeatures.AddOrReplace(key, instance.UsedSpecialFeatures.GetValueOrDefault(key) + 1);
    }

    internal static void SetSpecialFeatureUses(this GameLocationCharacter instance, string key, int value)
    {
        instance.UsedSpecialFeatures.AddOrReplace(key, value);
    }

    internal static int GetSpecialFeatureUses(this GameLocationCharacter instance, string key, int def = -1)
    {
        return instance.UsedSpecialFeatures.TryGetValue(key, out var value)
            ? value
            : def;
    }

    internal static bool OncePerTurnIsValid(this GameLocationCharacter instance, string key)
    {
        return !instance.UsedSpecialFeatures.ContainsKey(key);
    }

#if false
    internal static int GetActionTypeRank(this GameLocationCharacter instance, ActionType type)
    {
        var ranks = instance.currentActionRankByType;
        return ranks.TryGetValue(type, out var value) ? value : 0;
    }

    internal static FeatureDefinition GetCurrentAdditionalActionFeature(
        this GameLocationCharacter instance,
        ActionType type)
    {
        if (!instance.currentActionRankByType.TryGetValue(type, out var rank))
        {
            return null;
        }

        var filters = instance.ActionPerformancesByType[type];
        return rank >= filters.Count ? null : PerformanceFilterExtraData.GetData(filters[rank])?.Feature;
    }
#endif

    internal static bool CanCastAnyInvocationOfActionId(
        this GameLocationCharacter instance,
        Id actionId,
        ActionScope scope,
        bool canCastSpells,
        bool canOnlyUseCantrips)
    {
        var character = instance.RulesetCharacter;

        if (character.Invocations.Count == 0)
        {
            return false;
        }

        ActionStatus? mainSpell = null;
        ActionStatus? bonusSpell = null;

        foreach (var invocation in character.Invocations)
        {
            var definition = invocation.InvocationDefinition;
            var isValid = definition
                .GetAllSubFeaturesOfType<IsInvocationValidHandler>()
                .All(v => v(character, definition));

            if (definition.HasSubFeatureOfType<ModifyInvocationVisibility>() || !isValid)
            {
                continue;
            }

            if (scope == ActionScope.Battle)
            {
                isValid = definition.GetActionId() == actionId;
            }
            else
            {
                isValid = definition.GetMainActionId() == actionId;
            }

            var grantedSpell = definition.GrantedSpell;
            if (isValid && grantedSpell)
            {
                if (!canCastSpells)
                {
                    isValid = false;
                }
                else if (canOnlyUseCantrips && grantedSpell.SpellLevel > 0)
                {
                    isValid = false;
                }
                else
                {
                    var spellActionId = grantedSpell.BattleActionId;

                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (spellActionId)
                    {
                        case Id.CastMain:
                            mainSpell ??= scope == ActionScope.Battle
                                ? instance.GetActionStatus(spellActionId, scope)
                                : ActionStatus.Available;
                            if (mainSpell != ActionStatus.Available)
                            {
                                isValid = false;
                            }

                            break;
                        case Id.CastBonus:
                            bonusSpell ??= scope == ActionScope.Battle
                                ? instance.GetActionStatus(spellActionId, scope)
                                : ActionStatus.Available;
                            if (bonusSpell != ActionStatus.Available)
                            {
                                isValid = false;
                            }

                            break;
                    }
                }
            }

            if (isValid)
            {
                return true;
            }
        }

        return false;
    }

    internal static void HandleMonkMartialArts(this GameLocationCharacter instance)
    {
        var rulesetCharacter = instance.RulesetCharacter;

        if (Main.Settings.EnableMonkMartialArts2024 ||
            rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Monk) == 0)
        {
            return;
        }

        var usablePower = PowerProvider.Get(FeatureDefinitionPowers.PowerMonkMartialArts, rulesetCharacter);

        instance.MyExecuteActionSpendPower(usablePower, instance);
    }

    internal static int GetAllowedMainAttacks(this GameLocationCharacter instance)
    {
        var performanceFilters = instance.actionPerformancesByType[ActionType.Main];
        var index = instance.currentActionRankByType[ActionType.Main];

        var maxAttacks = instance.RulesetCharacter.AttackModes
            .Where(mode => mode.ActionType == ActionType.Main)
            .Max(mode => mode.AttacksNumber);

        var maxAllowedAttacks = index >= performanceFilters.Count ? -1 : performanceFilters[index].MaxAttacksNumber;

        return maxAllowedAttacks < 0 || maxAllowedAttacks > maxAttacks ? maxAttacks : maxAllowedAttacks;
    }


    internal static void BurnOneMainAttack(this GameLocationCharacter instance)
    {
        if (Gui.Battle == null)
        {
            return;
        }

        instance.HandleMonkMartialArts();

        var rulesetCharacter = instance.RulesetCharacter;

        // burn one main attack
        instance.HasAttackedSinceLastTurn = true;
        instance.UsedMainAttacks++;
        rulesetCharacter.ExecutedAttacks++;
        rulesetCharacter.RefreshAttackModes();

        var allowedMainAttacks = instance.GetAllowedMainAttacks();

        if (instance.UsedMainAttacks < allowedMainAttacks)
        {
            return;
        }

        instance.CurrentActionRankByType[ActionType.Main]++;
        instance.UsedMainAttacks = 0;
    }

    internal static void BurnOneBonusAttack(this GameLocationCharacter instance)
    {
        if (Gui.Battle == null)
        {
            return;
        }

        var rulesetCharacter = instance.RulesetCharacter;

        // burn one bonus attack
        instance.HasAttackedSinceLastTurn = true;
        instance.UsedBonusAttacks++;
        rulesetCharacter.ExecutedBonusAttacks++;
        rulesetCharacter.RefreshAttackModes();

        var maxAttacks = rulesetCharacter.AttackModes
            .FirstOrDefault(attackMode => attackMode.ActionType == ActionType.Bonus)?.AttacksNumber ?? 0;

        if (instance.UsedMainAttacks < maxAttacks)
        {
            return;
        }

        instance.CurrentActionRankByType[ActionType.Bonus]++;
        instance.UsedBonusAttacks = 0;
    }
}
