using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api;
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

namespace SolastaUnfinishedBusiness.Models;

public static class CustomActionIdContext
{
    private static readonly List<Id> ExtraActionIdToggles =
    [
        (Id)ExtraActionId.ArcaneArcherToggle,
        (Id)ExtraActionId.AudaciousWhirlToggle,
        (Id)ExtraActionId.CompellingStrikeToggle,
        (Id)ExtraActionId.CoordinatedAssaultToggle,
        (Id)ExtraActionId.CunningStrikeToggle,
        (Id)ExtraActionId.DyingLightToggle,
        (Id)ExtraActionId.FeatCrusherToggle,
        (Id)ExtraActionId.HailOfBladesToggle,
        (Id)ExtraActionId.MasterfulWhirlToggle,
        (Id)ExtraActionId.MindSculptToggle,
        (Id)ExtraActionId.PaladinSmiteToggle,
        (Id)ExtraActionId.PressTheAdvantageToggle,
        (Id)ExtraActionId.SupremeWillToggle,
        (Id)ExtraActionId.ImpishWrathToggle, // defined in sub race
        (Id)ExtraActionId.QuiveringPalmToggle
    ];

    internal static FeatureDefinitionPower FarStep { get; private set; }

    internal static void Load()
    {
        BuildCustomInvocationActions();
        BuildCustomPushedAction();
        BuildCustomRageStartAction();
        BuildCustomToggleActions();
        BuildDoNothingActions();
        BuildPrioritizeAction();
        BuildFarStepAction();
    }

    private static void BuildCustomInvocationActions()
    {
        ActionDefinitionBuilder
            .Create(CastInvocation, "CastInvocationBonus")
            .SetActionId(ExtraActionId.CastInvocationBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "CastInvocationNoCost")
            .SetActionId(ExtraActionId.CastInvocationNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "CastPlaneMagicMain")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, 10)
            .SetActionId(ExtraActionId.CastPlaneMagicMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "CastPlaneMagicBonus")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, 41)
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
            .Create(CastInvocation, "CastSpellMasteryMain")
            .SetGuiPresentation("CastSpellMastery", Category.Action, Sprites.ActionPlaneMagic, 10)
            .SetActionId(ExtraActionId.CastSpellMasteryMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(CastInvocation, "CastSignatureSpellsMain")
            .SetGuiPresentation("CastSignatureSpells", Category.Action, Sprites.ActionPlaneMagic, 10)
            .SetActionId(ExtraActionId.CastSignatureSpellsMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
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

    internal static bool IsCustomActionIdToggle(Id action)
    {
        return ExtraActionIdToggles.Contains(action);
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
            .Create(MetamagicToggle, "HailOfBladesToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.HailOfBladesToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "ArcaneArcherToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.ArcaneArcherToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "CoordinatedAssaultToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CoordinatedAssaultToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "DyingLightToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.DyingLightToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "PressTheAdvantageToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.PressTheAdvantageToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "AudaciousWhirlToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.AudaciousWhirlToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "CunningStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CunningStrikeToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "MasterfulWhirlToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.MasterfulWhirlToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "FeatCrusherToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.FeatCrusherToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "PaladinSmiteToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.PaladinSmiteToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "MindSculptToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.MindSculptToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "SupremeWillToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.SupremeWillToggle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(MetamagicToggle, "CompellingStrikeToggle")
            .SetOrUpdateGuiPresentation(Category.Action)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CompellingStrikeToggle)
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
                    .SetParticleEffectParameters(DatabaseHelper.SpellDefinitions.MistyStep)
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
                if (character.HasAnyConditionOfType(ConditionRaging))
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

    internal static bool IsInvocationActionId(Id id)
    {
        var extra = (ExtraActionId)id;

        return id is Id.CastInvocation
               || extra is ExtraActionId.CastInvocationBonus
                   or ExtraActionId.CastInvocationNoCost
                   or ExtraActionId.InventorInfusion
                   or ExtraActionId.CastPlaneMagicMain
                   or ExtraActionId.CastPlaneMagicBonus
                   or ExtraActionId.CastSpellMasteryMain
                   or ExtraActionId.CastSignatureSpellsMain
               || IsGambitActionId(id)
               || IsEldritchVersatilityId(id)
               || IsEldritchVersatilityId(id);
    }

    private static bool IsEldritchVersatilityId(Id id)
    {
        var extra = (ExtraActionId)id;

        return extra is ExtraActionId.EldritchVersatilityMain
            or ExtraActionId.EldritchVersatilityBonus
            or ExtraActionId.EldritchVersatilityNoCost;
    }

    private static bool IsGambitActionId(Id id)
    {
        var extra = (ExtraActionId)id;

        return extra is ExtraActionId.TacticianGambitMain
            or ExtraActionId.TacticianGambitBonus
            or ExtraActionId.TacticianGambitNoCost;
    }

    private static bool IsPowerUseActionId(Id id)
    {
        var extra = (ExtraActionId)id;

        return extra is ExtraActionId.FarStep
            or ExtraActionId.BondOfTheTalismanTeleport;
    }
}
