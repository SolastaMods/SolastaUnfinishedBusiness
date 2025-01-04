using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Races;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Subclasses.Builders;
using static ActionDefinitions;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Models;

public static class CustomActionIdContext
{
    private static readonly List<Id> ExtraActionIdToggles =
    [
        (Id)ExtraActionId.AmazingDisplayToggle,
        (Id)ExtraActionId.ArcaneArcherToggle,
        (Id)ExtraActionId.AudaciousWhirlToggle,
        (Id)ExtraActionId.BalefulScionToggle,
        (Id)ExtraActionId.BlessedStrikesToggle,
        (Id)ExtraActionId.BrutalStrikeToggle,
        (Id)ExtraActionId.CleavingAttackToggle,
        (Id)ExtraActionId.CompellingStrikeToggle,
        (Id)ExtraActionId.CoordinatedAssaultToggle,
        (Id)ExtraActionId.CunningStrikeToggle,
        (Id)ExtraActionId.DeadEyeToggle,
        (Id)ExtraActionId.DestructiveWrathToggle,
        (Id)ExtraActionId.DragonHideToggle,
        (Id)ExtraActionId.DyingLightToggle,
        (Id)ExtraActionId.ElementalFuryToggle,
        (Id)ExtraActionId.FeatCrusherToggle,
        (Id)ExtraActionId.ForcePoweredStrikeToggle,
        (Id)ExtraActionId.GloomBladeToggle,
        (Id)ExtraActionId.GrappleOnUnarmedToggle,
        (Id)ExtraActionId.GravityWellToggle,
        (Id)ExtraActionId.HailOfBladesToggle,
        (Id)ExtraActionId.ImpishWrathToggle,
        (Id)ExtraActionId.MasterfulWhirlToggle,
        (Id)ExtraActionId.MindSculptToggle,
        (Id)ExtraActionId.NatureStrikesToggle,
        (Id)ExtraActionId.OrcishFuryToggle,
        (Id)ExtraActionId.OverChannelToggle,
        (Id)ExtraActionId.PaladinSmiteToggle,
        (Id)ExtraActionId.PowerAttackToggle,
        (Id)ExtraActionId.PowerSurgeToggle,
        (Id)ExtraActionId.PressTheAdvantageToggle,
        (Id)ExtraActionId.QuiveringPalmToggle,
        (Id)ExtraActionId.SupremeWillToggle,
        (Id)ExtraActionId.ThunderousStrikeToggle,
        (Id)ExtraActionId.WeaponMasteryToggle,
        (Id)ExtraActionId.ZenShotToggle
    ];

    private static readonly List<Id> ExtraActionIdPowers =
    [
        (Id)ExtraActionId.AmazingDisplayToggle,
        (Id)ExtraActionId.ArcaneArcherToggle,
        (Id)ExtraActionId.AudaciousWhirlToggle,
        (Id)ExtraActionId.BalefulScionToggle,
        (Id)ExtraActionId.BondOfTheTalismanTeleport,
        (Id)ExtraActionId.CoordinatedAssaultToggle,
        (Id)ExtraActionId.DestructiveWrathToggle,
        (Id)ExtraActionId.FarStep,
        (Id)ExtraActionId.ForcePoweredStrikeToggle,
        (Id)ExtraActionId.ImpishWrathToggle,
        (Id)ExtraActionId.OrcishFuryToggle,
        (Id)ExtraActionId.PowerSurgeToggle,
        (Id)ExtraActionId.QuiveringPalmToggle,
        (Id)ExtraActionId.ZenShotToggle
    ];

    internal static readonly List<Id> ExtraActionIdProxies =
    [
        (Id)ExtraActionId.ProxyDarkness,
        (Id)ExtraActionId.ProxyDawn,
        (Id)ExtraActionId.ProxyHoundWeapon,
        (Id)ExtraActionId.ProxyPactWeapon,
        (Id)ExtraActionId.ProxyPetalStorm
    ];

    internal static FeatureDefinitionPower FarStep { get; private set; }

    internal static void Load()
    {
        BuildCustomInvocationActions();
        BuildCustomPushedAction();
        BuildCustomRageStartAction();
        BuildCustomToggleActions();
        BuildDoNothingActions();
        BuildFarStepAction();
        BuildPrioritizeAction();
        BuildProxyActions();
    }

    private static void BuildProxyActions()
    {
        ActionDefinitionBuilder
            .Create(ProxySpiritualWeapon, "ActionProxyPactWeapon")
            .SetActionId(ExtraActionId.ProxyPactWeapon)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(ProxySpiritualWeaponFree, "ActionProxyPactWeaponFree")
            .SetActionId(ExtraActionId.ProxyPactWeaponFree)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(ProxyFlamingSphere, "ActionProxyPetalStorm")
            .SetActionId(ExtraActionId.ProxyPetalStorm)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(ProxySpiritualWeapon, "ActionProxyFaithfulHound")
            .SetActionId(ExtraActionId.ProxyHoundWeapon)
            .SetActionType(ActionType.NoCost)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(ProxyFlamingSphere, "ActionProxyDawn")
            .SetActionId(ExtraActionId.ProxyDawn)
            .SetActionType(ActionType.Bonus)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(ProxyFlamingSphere, "ActionProxyDarkness")
            .SetActionId(ExtraActionId.ProxyDarkness)
            .SetActionType(ActionType.NoCost)
            .AddToDB();
    }

    private static void BuildCustomInvocationActions()
    {
        ActionDefinitionBuilder
            .Create(CastInvocation, "CastInvocationBonus")
            .SetSortOrder(CastBonus.GuiPresentation.sortOrder + 2)
            .SetActionId(ExtraActionId.CastInvocationBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "CastInvocationNoCost")
            .SetSortOrder(CastNoCost.GuiPresentation.sortOrder + 2)
            .SetActionId(ExtraActionId.CastInvocationNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "CastPlaneMagicMain")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic)
            .SetSortOrder(CastMain.GuiPresentation.sortOrder + 1)
            .SetActionId(ExtraActionId.CastPlaneMagicMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "CastPlaneMagicBonus")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic)
            .SetSortOrder(CastBonus.GuiPresentation.sortOrder + 1)
            .SetActionId(ExtraActionId.CastPlaneMagicBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "InventorInfusion")
            .SetGuiPresentation(Category.Action, Sprites.ActionInfuse, 20)
            .SetActionId(ExtraActionId.InventorInfusion)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.Exploration)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "TacticianGambitMain")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .AddCustomSubFeatures(GambitsBuilders.GambitActionDiceBox.Instance)
            .SetActionId(ExtraActionId.TacticianGambitMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "TacticianGambitBonus")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .AddCustomSubFeatures(GambitsBuilders.GambitActionDiceBox.Instance)
            .SetActionId(ExtraActionId.TacticianGambitBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "TacticianGambitNoCost")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .AddCustomSubFeatures(GambitsBuilders.GambitActionDiceBox.Instance)
            .SetActionId(ExtraActionId.TacticianGambitNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "EldritchVersatilityMain")
            .SetGuiPresentation("EldritchVersatility", Category.Action, Sprites.ActionEldritchVersatility, 20)
            .SetActionId(ExtraActionId.EldritchVersatilityMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "EldritchVersatilityBonus")
            .SetGuiPresentation("EldritchVersatility", Category.Action, Sprites.ActionEldritchVersatility, 20)
            .SetActionId(ExtraActionId.EldritchVersatilityBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "EldritchVersatilityNoCost")
            .SetGuiPresentation("EldritchVersatility", Category.Action, Sprites.ActionEldritchVersatility, 20)
            .SetActionId(ExtraActionId.EldritchVersatilityNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();
    }

    private static void BuildCustomPushedAction()
    {
        ActionDefinitionBuilder
            .Create(Pushed, "PushedCustom")
            .SetActionId(ExtraActionId.PushedCustom)
            .AddToDB();
    }

    private static void BuildCustomRageStartAction()
    {
        ActionDefinitionBuilder
            .Create(RageStart, "CombatRageStart")
            .SetActionId(ExtraActionId.CombatRageStart)
            .SetActionType(ActionType.NoCost)
            .SetActivatedPower(PathOfTheSavagery.PowerPrimalInstinct)
            .AddToDB();
    }

    internal static void ReorderToggles(List<Id> actions)
    {
        var powerNdx = actions.FindIndex(x => x == Id.Cautious);

        if (powerNdx < 0)
        {
            return;
        }

        foreach (var id in ExtraActionIdToggles.Where(actions.Contains))
        {
            DoReorder(id);
        }

        return;

        void DoReorder(Id actionId, int overrideIndex = -1)
        {
            actions.Remove(actionId);
            actions.Insert(overrideIndex < 0 ? powerNdx : overrideIndex, actionId);
        }
    }

    private static void BuildCustomToggleActions()
    {
        ActionDefinitionBuilder
            .Create(MetamagicToggle, "BlessedStrikesToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.BlessedStrikesToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "BrutalStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.BrutalStrikeToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "CompellingStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CompellingStrikeToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "CunningStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CunningStrikeToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "DragonHideToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.DragonHideToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "DyingLightToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.DyingLightToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "ElementalFuryToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ElementalFuryToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "FeatCrusherToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.FeatCrusherToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "GloomBladeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.GloomBladeToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "HailOfBladesToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.HailOfBladesToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "MasterfulWhirlToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.MasterfulWhirlToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "MindSculptToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.MindSculptToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "NatureStrikesToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.NatureStrikesToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "PaladinSmiteToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.PaladinSmiteToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "ThunderousStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ThunderousStrikeToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "PressTheAdvantageToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.PressTheAdvantageToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "SupremeWillToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.SupremeWillToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "GrappleOnUnarmedToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.GrappleOnUnarmedToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "CleavingAttackToggle")
            .SetGuiPresentation(Category.Action, WhirlwindAttack)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CleavingAttackToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "PowerAttackToggle")
            .SetGuiPresentation(Category.Action, WhirlwindAttack)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.PowerAttackToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "DeadEyeToggle")
            .SetGuiPresentation(Category.Action, Volley)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.DeadEyeToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "OverChannelToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.OverChannelToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "GravityWellToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.GravityWellToggle)
            .OverrideClassName("Toggle")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "WeaponMasteryToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.WeaponMasteryToggle)
            .OverrideClassName("Toggle")
            .AddToDB();
    }

    private static void BuildFarStepAction()
    {
        const string NAME = "FarStep";

        FarStep = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Action,
                Sprites.GetSprite("PowerFarStep", Resources.PowerFarStep, 256, 128))
            .SetUsesFixed(ActivationTime.BonusAction)
            .DelegatedToAction()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.TeleportToDestination)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.MistyStep)
                    .UseQuickAnimations()
                    .Build())
            .AddToDB();

        ActionDefinitionBuilder
            .Create($"Action{NAME}")
            .SetGuiPresentation(NAME, Category.Action, Sprites.FarStep, 71)
            .SetActionId(ExtraActionId.FarStep)
            .OverrideClassName("UsePower")
            .SetActionScope(ActionScope.All)
            .SetActionType(ActionType.Bonus)
            .SetFormType(ActionFormType.Small)
            .SetActivatedPower(FarStep)
            .AddToDB();
    }

    private static void BuildDoNothingActions()
    {
        ActionDefinitionBuilder
            .Create(UseBardicInspiration, "DoNothingFree")
            .SetGuiPresentationNoContent()
            .SetActionId(ExtraActionId.DoNothingFree)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.All)
            .OverrideClassName("DoNothing")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(UseBardicInspiration, "DoNothingReaction")
            .SetGuiPresentationNoContent()
            .SetActionId(ExtraActionId.DoNothingReaction)
            .SetActionType(ActionType.Reaction)
            .SetActionScope(ActionScope.All)
            .OverrideClassName("DoNothing")
            .AddToDB();
    }

    private static void BuildPrioritizeAction()
    {
        ActionDefinitionBuilder
            .Create(UseBardicInspiration, "PrioritizeAction")
            .SetGuiPresentationNoContent()
            .SetActionId(ExtraActionId.PrioritizeAction)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.All)
            .OverrideClassName("PrioritizeAction")
            .AddToDB();
    }

    public static void ProcessCustomActionIds(
        GameLocationCharacter locationCharacter,
        ref ActionStatus result,
        Id actionId,
        ActionScope scope,
        ActionStatus actionTypeStatus,
        bool ignoreMovePoints)
    {
        var action = ServiceRepository.GetService<IGameLocationActionService>().AllActionDefinitions[actionId];
        var actionType = action.actionType;
        var character = locationCharacter.RulesetCharacter;

        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (actionId)
        {
            case (Id)ExtraActionId.CombatWildShape:
            {
                var power = character.GetPowerFromDefinition(action.ActivatedPower);

                if (power is not { RemainingUses: > 0 } ||
                    (character is RulesetCharacterMonster monster &&
                     monster.MonsterDefinition.CreatureTags.Contains(TagsDefinitions.CreatureTagWildShape)))
                {
                    result = ActionStatus.Unavailable;
                }

                return;
            }
            case (Id)ExtraActionId.CombatRageStart:
            {
                if (character.HasConditionOfTypeOrSubType(ConditionRaging))
                {
                    result = ActionStatus.Unavailable;
                }

                return;
            }
            case (Id)ExtraActionId.FlightSuspend:
            {
                if (Main.Settings.AllowFlightSuspend && character.IsTemporarilyFlying())
                {
                    result = ActionStatus.Available;
                }
                else
                {
                    result = ActionStatus.Unavailable;
                }

                return;
            }
            case (Id)ExtraActionId.FlightResume:
            {
                if (Main.Settings.AllowFlightSuspend &&
                    character.HasConditionOfTypeOrSubType(CustomConditionsContext.FlightSuspended.Name))
                {
                    result = ActionStatus.Available;
                }
                else
                {
                    result = ActionStatus.Unavailable;
                }

                return;
            }
            case (Id)ExtraActionId.CrystalDefenseOff:
            {
                result = character.HasConditionOfType(RaceWyrmkinBuilder.ConditionCrystalDefenseName)
                    ? ActionStatus.Available
                    : ActionStatus.Unavailable;
                return;
            }
            case (Id)ExtraActionId.ProxyHoundWeapon:
            {
                result = character.ControlledEffectProxies.Any(x =>
                    x.EffectProxyDefinition.Name == "ProxyFaithfulHound" && x.ExecutedAttacks == 0)
                    ? ActionStatus.Available
                    : ActionStatus.Unavailable;
                return;
            }
            case (Id)ExtraActionId.CastQuickened:
            {
                result = CanUseActionQuickened(locationCharacter, scope);
                return;
            }
        }

        var isInvocationAction = IsInvocationActionId(actionId);
        var isPowerUse = IsPowerUseActionId(actionId);

        if (!isInvocationAction && !isPowerUse)
        {
            return;
        }

        if (actionTypeStatus == ActionStatus.Irrelevant)
        {
            actionTypeStatus = locationCharacter.GetActionTypeStatus(action.ActionType, scope, ignoreMovePoints);
        }

        if (action.ActionScope != ActionScope.All && action.ActionScope != scope)
        {
            result = ActionStatus.Unavailable;

            return;
        }

        if (action.UsesPerTurn > 0)
        {
            var name = action.Name;

            if (locationCharacter.UsedSpecialFeatures.TryGetValue(name, out var value) && value >= action.UsesPerTurn)
            {
                result = ActionStatus.Unavailable;

                return;
            }
        }

        var index = locationCharacter.currentActionRankByType[actionType];
        var actionPerformanceFilters = locationCharacter.actionPerformancesByType[actionType];

        if (action.RequiresAuthorization)
        {
            if (index >= actionPerformanceFilters.Count
                || !actionPerformanceFilters[index].AuthorizedActions.Contains(actionId))
            {
                result = ActionStatus.Unavailable;

                return;
            }
        }
        else if (index >= actionPerformanceFilters.Count)
        {
            result = ActionStatus.Unavailable;

            return;
        }

        var canCastSpells = character.CanCastSpells();
        var canOnlyUseCantrips = scope == ActionScope.Battle && locationCharacter.CanOnlyUseCantrips;

        if (isInvocationAction)
        {
            result = CanUseInvocationAction(actionId, scope, locationCharacter, canCastSpells, canOnlyUseCantrips);
        }

        if (isPowerUse)
        {
            result = character.CanUsePower(action.ActivatedPower, considerHaving: true)
                ? ActionStatus.Available
                : ActionStatus.Unavailable;
        }

        if (result == ActionStatus.Available)
        {
            if ((actionType == ActionType.Bonus && (
                    locationCharacter.UsedBonusAttacks > 0
                    || character.HasConditionOfType(ConditionFlurryOfBlows)
                    || character.HasConditionOfType(
                        ConditionTraditionFreedomFlurryOfBlowsUnarmedStrikeBonusUnendingStrikes)))
                || (actionType == ActionType.Main && locationCharacter.UsedMainAttacks > 0))
            {
                result = ActionStatus.NoLongerAvailable;
            }
        }

        if (result == ActionStatus.Available && actionTypeStatus != ActionStatus.Available)
        {
            result = actionTypeStatus == ActionStatus.Spent ? ActionStatus.Unavailable : actionTypeStatus;
        }
    }

    private static ActionStatus CanUseInvocationAction(
        Id actionId,
        ActionScope scope,
        GameLocationCharacter locationCharacter,
        bool canCastSpells,
        bool canOnlyUseCantrips)
    {
        var character = locationCharacter.RulesetCharacter;

        if (IsGambitActionId(actionId)
            && character.HasPower(GambitsBuilders.GambitPool)
            && character.KnowsAnyInvocationOfActionId(actionId, scope)
            && character.GetRemainingPowerCharges(GambitsBuilders.GambitPool) <= 0)
        {
            return ActionStatus.OutOfUses;
        }

        return locationCharacter.CanCastAnyInvocationOfActionId(actionId, scope, canCastSpells, canOnlyUseCantrips)
            ? ActionStatus.Available
            : ActionStatus.Unavailable;
    }

    private static bool IsEldritchVersatilityId(Id id)
    {
        var extra = (ExtraActionId)id;

        return extra
            is ExtraActionId.EldritchVersatilityMain
            or ExtraActionId.EldritchVersatilityBonus
            or ExtraActionId.EldritchVersatilityNoCost;
    }

    private static bool IsGambitActionId(Id id)
    {
        var extra = (ExtraActionId)id;

        return extra
            is ExtraActionId.TacticianGambitMain
            or ExtraActionId.TacticianGambitBonus
            or ExtraActionId.TacticianGambitNoCost;
    }

    internal static bool IsInvocationActionId(Id id)
    {
        var extra = (ExtraActionId)id;

        return id is Id.CastInvocation ||
               extra
                   is ExtraActionId.CastInvocationBonus
                   or ExtraActionId.CastInvocationNoCost
                   or ExtraActionId.InventorInfusion
                   or ExtraActionId.CastPlaneMagicMain
                   or ExtraActionId.CastPlaneMagicBonus ||
               IsGambitActionId(id) ||
               IsEldritchVersatilityId(id) ||
               IsEldritchVersatilityId(id);
    }

    private static bool IsPowerUseActionId(Id id)
    {
        return ExtraActionIdPowers.Contains(id);
    }

    internal static bool IsToggleId(Id id)
    {
        return ExtraActionIdToggles.Contains(id);
    }

    private static ActionStatus CanUseActionQuickened(GameLocationCharacter glc, ActionScope scope)
    {
        if (!Main.Settings.EnableSorcererQuickenedAction || scope != ActionScope.Battle)
        {
            return ActionStatus.Unavailable;
        }

        var hero = glc.RulesetCharacter.GetOriginalHero();
        var quickenedSpell = MetamagicOptionDefinitions.MetamagicQuickenedSpell;

        // more or less in order of cost
        if (hero == null ||
            !hero.TrainedMetamagicOptions.Contains(quickenedSpell) ||
            (Main.Settings.HideQuickenedActionWhenMetamagicOff && !glc.IsActionOnGoing(Id.MetamagicToggle)) ||
            glc.GetActionTypeStatus(ActionType.Bonus) != ActionStatus.Available ||
            !glc.RulesetCharacter.CanCastSpellOfActionType(ActionType.Main, glc.CanOnlyUseCantrips))
        {
            return ActionStatus.Unavailable;
        }

        return hero.RemainingSorceryPoints < quickenedSpell.SorceryPointsCost
            ? ActionStatus.OutOfUses
            : ActionStatus.Available;
    }

    internal static void CheckQuickenedStatus(GuiCharacterAction action, ActionStatus status, GuiTooltip tooltip,
        ref string fail)
    {
        if (action.ActionId != (Id)ExtraActionId.CastQuickened) { return; }

        if (status != ActionStatus.OutOfUses) { return; }

        tooltip.Content = tooltip.Content.Substring(0, tooltip.Content.Length - fail.Length);
        fail = "\n" + Gui.Colorize(Gui.Format(FailureFlagInsufficientSorceryPoints), Gui.ColorFailure);
        tooltip.Content += fail;
    }

    internal static void UpdateCastActionForm(GuiCharacterAction action, List<Id> actions)
    {
        if (action.ActionId == Id.CastBonus)
        {
            action.actionDefinition.formType = actions.Contains((Id)ExtraActionId.CastQuickened)
                ? ActionFormType.Small
                : ActionFormType.Large;
        }
        else if (action.actionId == (Id)ExtraActionId.CastQuickened)
        {
            action.actionDefinition.formType = actions.Contains(Id.CastBonus)
                ? ActionFormType.Small
                : ActionFormType.Large;
        }
    }

    internal static bool IsCastSpellAction(this ActionDefinition action)
    {
        return action.Id is Id.CastMain or Id.CastBonus or (Id)ExtraActionId.CastQuickened;
    }
}
