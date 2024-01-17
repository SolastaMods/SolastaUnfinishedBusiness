using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomValidators;
using TA;
using UnityEngine;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

public static class GameLocationCharacterExtensions
{
    public static bool IsMyTurn(this GameLocationCharacter character)
    {
        return Gui.Battle != null && Gui.Battle.ActiveContenderIgnoringLegendary == character;
    }

    public static bool IsWithinRange(this GameLocationCharacter source, GameLocationCharacter target, int range)
    {
        if (Main.Settings.UseOfficialDistanceCalculation)
        {
            return DistanceCalculation.CalculateDistanceFromTwoCharacters(source, target) <= range;
        }

        return int3.Distance(source.LocationPosition, target.LocationPosition) <= range;
    }

    public static bool IsMagicEffectValidUnderObscurementOrMagicalDarkness(
        this GameLocationCharacter source,
        IMagicEffect magicEffect,
        GameLocationCharacter target)
    {
        if (target == null)
        {
            return true;
        }

        if (Main.Settings.EffectsThatTargetDistantIndividualsAndDontRequireSight.Contains(magicEffect.Name))
        {
            return true;
        }

        var rulesetSource = source.RulesetActor;
        var rulesetTarget = target.RulesetActor;

        if (!rulesetSource.IsUnderHeavyObscurementOrMagicalDarkness() &&
            !rulesetTarget.IsUnderHeavyObscurementOrMagicalDarkness())
        {
            return true;
        }

        var effectDescription = magicEffect.EffectDescription;
        var shouldTrigger = effectDescription is
        {
            RangeType: RuleDefinitions.RangeType.Distance,
            TargetType: RuleDefinitions.TargetType.Individuals or RuleDefinitions.TargetType.IndividualsUnique
        };

        return !shouldTrigger || source.CanPerceiveTarget(target);
    }

    // consolidate all checks if a character can perceive another
    public static bool CanPerceiveTarget(
        this GameLocationCharacter __instance,
        GameLocationCharacter target)
    {
        var canPerceiveVanilla =
            (__instance.Side == target.Side && __instance.PerceivedAllies.Contains(target)) ||
            (__instance.Side != target.Side && __instance.PerceivedFoes.Contains(target));

        if (!Main.Settings.UseAlternateLightingAndObscurementRules || !canPerceiveVanilla)
        {
            return canPerceiveVanilla;
        }

        var visibilityService =
            ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

        return visibilityService.MyIsCellPerceivedByCharacter(target.LocationPosition, __instance, target);
    }

    internal static LocationDefinitions.LightingState ComputeLightingStateOnTargetPosition(
        this GameLocationCharacter instance,
        int3 targetPosition)
    {
        var savePosition = new int3(
            instance.LocationPosition.x,
            instance.LocationPosition.y,
            instance.LocationPosition.z);

        instance.LocationPosition = targetPosition;

        var illumination = ComputeIllumination(instance);

        instance.LocationPosition = savePosition;

        return illumination;

        // this is copy-and-paste from vanilla code GameLocationVisibilityManager.ComputeIllumination
        // except for Darkness determination in patch block and some clean up for not required scenarios
        static LocationDefinitions.LightingState ComputeIllumination(IIlluminable illuminable)
        {
            const LocationDefinitions.LightingState UNLIT = LocationDefinitions.LightingState.Unlit;

            if (!illuminable.Valid)
            {
                return UNLIT;
            }

            var visibilityManager =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            visibilityManager!.positionCache.Clear();
            illuminable.GetAllPositionsToCheck(visibilityManager.positionCache);

            if (visibilityManager.positionCache == null || visibilityManager.positionCache.Empty())
            {
                return UNLIT;
            }

            if (illuminable is GameLocationCharacter { RulesetCharacter: not null } locationCharacter1 &&
                locationCharacter1.RulesetCharacter.HasConditionOfType("ConditionDarkness"))
            {
                return LocationDefinitions.LightingState.Darkness;
            }

            var gridAccessor = new GridAccessor(visibilityManager.positionCache[0]);

            if (visibilityManager.positionCache.Any(position =>
                    (gridAccessor.RuntimeFlags(position) & CellFlags.Runtime.DynamicSightImpaired) !=
                    CellFlags.Runtime.None))
            {
                return LocationDefinitions.LightingState.Darkness;
            }

            var lightingState1 = LocationDefinitions.LightingState.Unlit;

            foreach (var position in visibilityManager.positionCache)
            {
                gridAccessor.FetchSector(position);
                if (gridAccessor.sector == null ||
                    gridAccessor.sector.GlobalLightingState == LocationDefinitions.LightingState.Unlit)
                {
                    continue;
                }

                lightingState1 = gridAccessor.sector.GlobalLightingState;

                if (lightingState1 != LocationDefinitions.LightingState.Bright)
                {
                    continue;
                }

                return LocationDefinitions.LightingState.Bright;
            }

            visibilityManager.lightsByDistance.Clear();

            foreach (var lightSources in visibilityManager.lightSourcesMap)
            {
                var key = lightSources.Value;
                var locationPosition = key.LocationPosition;

                if (key.RulesetLightSource.DayCycleType != RuleDefinitions.LightSourceDayCycleType.Always &&
                    !key.RulesetLightSource.IsDayCycleActive)
                {
                    continue;
                }

                var dimRange = key.RulesetLightSource.DimRange;
                var magnitude = (locationPosition - illuminable.Position).magnitude;

                if (magnitude <= dimRange + (double)illuminable.DetectionRange &&
                    (!visibilityManager.charactersByLight.TryGetValue(key.RulesetLightSource,
                         out var locationCharacter3) ||
                     locationCharacter3 is { IsValidForVisibility: true }))
                {
                    visibilityManager.lightsByDistance.Add(
                        new KeyValuePair<GameLocationLightSource, float>(key, magnitude));
                }
            }

            visibilityManager.lightsByDistance.Sort(visibilityManager.lightSortMethod);

            var lightingState2 = LocationDefinitions.LightingState.Unlit;

            foreach (var int3 in visibilityManager.positionCache)
            {
                foreach (var keyValuePair in visibilityManager.lightsByDistance)
                {
                    var key = keyValuePair.Key;
                    var dimRange = key.RulesetLightSource.DimRange;
                    var locationPosition = key.LocationPosition;
                    var magnitudeSqr = (locationPosition - int3).magnitudeSqr;

                    if (!magnitudeSqr.IsInferiorOrNearlyEqual(dimRange * dimRange))
                    {
                        continue;
                    }

                    var fromGridPosition1 =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(
                            key.LocationPosition);
                    var fromGridPosition2 =
                        visibilityManager.gameLocationPositioningService.GetWorldPositionFromGridPosition(int3);
                    visibilityManager.AdaptRayForVerticalityAndDiagonals(key.LocationPosition, int3,
                        ref fromGridPosition1,
                        true);
                    var flag = true;

                    if (key.RulesetLightSource.IsSpot)
                    {
                        var to = fromGridPosition2 - fromGridPosition1;

                        to.Normalize();
                        flag = Vector3.Angle(key.RulesetLightSource.SpotDirection, to)
                            .IsInferiorOrNearlyEqual(key.RulesetLightSource.SpotAngle * 0.5f);
                    }

                    if (!flag ||
                        visibilityManager.gameLocationPositioningService.RaycastGridSightBlocker(fromGridPosition1,
                            fromGridPosition2, visibilityManager.GameLocationService) ||
                        visibilityManager.gameLocationPositioningService.IsSightImpaired(key.LocationPosition, int3))
                    {
                        continue;
                    }

                    lightingState2 =
                        key.RulesetLightSource.BrightRange <= 0.0 || magnitudeSqr >
                        key.RulesetLightSource.BrightRange * (double)key.RulesetLightSource.BrightRange
                            ? LocationDefinitions.LightingState.Dim
                            : LocationDefinitions.LightingState.Bright;
                    if (lightingState2 == LocationDefinitions.LightingState.Bright)
                    {
                        break;
                    }
                }

                if (lightingState2 != LocationDefinitions.LightingState.Unlit)
                {
                    break;
                }
            }

            var illumination2 =
                lightingState1 != LocationDefinitions.LightingState.Dim ||
                lightingState2 != LocationDefinitions.LightingState.Unlit
                    ? lightingState2
                    : LocationDefinitions.LightingState.Dim;

            return illumination2;
        }
    }

    internal static (RulesetAttackMode mode, ActionModifier modifier) GetFirstMeleeModeThatCanAttack(
        this GameLocationCharacter instance,
        GameLocationCharacter target,
        IGameLocationBattleService service = null)
    {
        service ??= ServiceRepository.GetService<IGameLocationBattleService>();

        foreach (var mode in instance.RulesetCharacter.AttackModes)
        {
            if (!ValidatorsWeapon.IsMelee(mode))
            {
                continue;
            }

            // Prepare attack evaluation params
            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var modifier = new ActionModifier();

            attackParams.FillForPhysicalReachAttack(instance, instance.LocationPosition, mode,
                target, target.LocationPosition, modifier);

            // Check if the attack is possible and collect the attack modifier inside the attackParams
            if (service.CanAttack(attackParams))
            {
                return (mode, modifier);
            }
        }

        return (null, null);
    }

    internal static (RulesetAttackMode mode, ActionModifier modifier) GetFirstRangedModeThatCanAttack(
        this GameLocationCharacter instance,
        GameLocationCharacter target,
        IGameLocationBattleService service = null)
    {
        service ??= ServiceRepository.GetService<IGameLocationBattleService>();

        foreach (var mode in instance.RulesetCharacter.AttackModes)
        {
            if (mode.Reach)
            {
                continue;
            }

            // Prepare attack evaluation params
            var attackParams = new BattleDefinitions.AttackEvaluationParams();
            var modifier = new ActionModifier();

            attackParams.FillForPhysicalRangeAttack(instance, instance.LocationPosition, mode,
                target, target.LocationPosition, modifier);

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

    internal static bool CanAct(this GameLocationCharacter instance)
    {
        var character = instance.RulesetCharacter;

        return character is { IsDeadOrDyingOrUnconscious: false }
               && !character.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionProne)
               && !character.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionIncapacitated)
               && !character.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionStunned)
               && !character.HasConditionOfTypeOrSubType(RuleDefinitions.ConditionParalyzed);
    }

    internal static bool IsReactionAvailable(this GameLocationCharacter instance, bool ignoreReactionUses = false)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

        if (actionService == null)
        {
            return false;
        }

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
#endif

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

    internal static bool CanCastAnyInvocationOfActionId(
        this GameLocationCharacter instance,
        Id actionId,
        ActionScope scope,
        bool canCastSpells,
        bool canOnlyUseCantrips)
    {
        var character = instance.RulesetCharacter;

        if (character.Invocations.Empty())
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

            if (definition.HasSubFeatureOfType<HiddenInvocation>() || !isValid)
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
            if (isValid && grantedSpell != null)
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

    internal static void BurnOneMainAttack(this GameLocationCharacter instance)
    {
        if (Gui.Battle == null)
        {
            return;
        }

        var rulesetCharacter = instance.RulesetCharacter;

        if (!Main.Settings.EnableMonkDoNotRequireAttackActionForBonusUnarmoredAttack &&
            rulesetCharacter.GetClassLevel(CharacterClassDefinitions.Monk) > 0)
        {
            var usablePower = UsablePowersProvider.Get(FeatureDefinitionPowers.PowerMonkMartialArts, rulesetCharacter);
            var actionParams = new CharacterActionParams(instance, Id.SpendPower)
            {
                ActionDefinition = DatabaseHelper.ActionDefinitions.SpendPower,
                RulesetEffect = ServiceRepository.GetService<IRulesetImplementationService>()
                    //CHECK: no need for AddAsActivePowerToSource
                    .InstantiateEffectPower(rulesetCharacter, usablePower, false),
                targetCharacters = { instance }
            };

            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(actionParams, null, true);
        }

        // burn one main attack
        instance.UsedMainAttacks++;
        rulesetCharacter.ExecutedAttacks++;
        rulesetCharacter.RefreshAttackModes();

        var maxAttacksNumber = rulesetCharacter.AttackModes
            .Where(x => x.ActionType == ActionType.Main)
            .Max(x => x.AttacksNumber);

        if (maxAttacksNumber - instance.UsedMainAttacks > 0)
        {
            return;
        }

        instance.CurrentActionRankByType[ActionType.Main]++;
        instance.UsedMainAttacks = 0;
    }
}
