using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Gambits;
using static ActionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Models;

public static class CustomActionIdContext
{
    internal static FeatureDefinitionPower FarStep { get; private set; }

    internal static void Load()
    {
        BuildCustomInvocationActions();
        BuildCustomPushedAction();
        BuildFarStepAction();
        BuildDoNothingActions();
    }

    private static void BuildCustomInvocationActions()
    {
        if (!DatabaseHelper.TryGetDefinition<ActionDefinition>("CastInvocation", out var baseAction))
        {
            return;
        }

        ActionDefinitionBuilder
            .Create(baseAction, "CastInvocationBonus")
            .SetActionId(ExtraActionId.CastInvocationBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CastInvocationNoCost")
            .SetActionId(ExtraActionId.CastInvocationNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CastPlaneMagicMain")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, 10)
            .SetActionId(ExtraActionId.CastPlaneMagicMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "CastPlaneMagicBonus")
            .SetGuiPresentation("CastPlaneMagic", Category.Action, Sprites.ActionPlaneMagic, 41)
            .SetActionId(ExtraActionId.CastPlaneMagicBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "InventorInfusion")
            .SetGuiPresentation(Category.Action, Sprites.ActionInfuse, 20)
            .SetActionId(ExtraActionId.InventorInfusion)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.Exploration)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "TacticianGambitMain")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .SetCustomSubFeatures(GambitsBuilders.GambitActionDiceBox.Instance)
            .SetActionId(ExtraActionId.TacticianGambitMain)
            .SetActionType(ActionType.Main)
            .SetActionScope(ActionScope.All)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "TacticianGambitBonus")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .SetCustomSubFeatures(GambitsBuilders.GambitActionDiceBox.Instance)
            .SetActionId(ExtraActionId.TacticianGambitBonus)
            .SetActionType(ActionType.Bonus)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "TacticianGambitNoCost")
            .SetGuiPresentation("TacticianGambit", Category.Action, Sprites.ActionGambit, 20)
            .SetCustomSubFeatures(GambitsBuilders.GambitActionDiceBox.Instance)
            .SetActionId(ExtraActionId.TacticianGambitNoCost)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.Battle)
            .AddToDB();
    }

    private static void BuildCustomPushedAction()
    {
        if (!DatabaseHelper.TryGetDefinition<ActionDefinition>("Pushed", out var baseAction))
        {
            return;
        }

        ActionDefinitionBuilder
            .Create(baseAction, "PushedCustom")
            .SetActionId(ExtraActionId.PushedCustom)
            .AddToDB();
    }

    private static void BuildFarStepAction()
    {
        const string NAME = "FarStep";

        FarStep = FeatureDefinitionPowerBuilder
            .Create($"Power{NAME}")
            .SetGuiPresentation(NAME, Category.Action, Sprites.PowerFarStep)
            .SetUsesFixed(ActivationTime.BonusAction)
            .DelegatedToAction()
            .SetEffectDescription(EffectDescriptionBuilder.Create()
                .SetDurationData(DurationType.Instantaneous)
                .SetTargetingData(Side.Ally, RangeType.Self, 12, TargetType.Position)
                .SetEffectForms(EffectFormBuilder.Create()
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
        if (!DatabaseHelper.TryGetDefinition<ActionDefinition>("UseBardicInspiration", out var baseAction))
        {
            return;
        }

        ActionDefinitionBuilder
            .Create(baseAction, "DoNothingFree")
            .SetGuiPresentationNoContent()
            .SetActionId(ExtraActionId.DoNothingFree)
            .SetActionType(ActionType.NoCost)
            .SetActionScope(ActionScope.All)
            .OverrideClassName("DoNothing")
            .AddToDB();

        ActionDefinitionBuilder
            .Create(baseAction, "DoNothingReaction")
            .SetGuiPresentationNoContent()
            .SetActionId(ExtraActionId.DoNothingReaction)
            .SetActionType(ActionType.Reaction)
            .SetActionScope(ActionScope.All)
            .OverrideClassName("DoNothing")
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

        if (actionId == (Id)ExtraActionId.CombatWildShape)
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
            result = CanUseInvocationAction(actionId, scope, character, canCastSpells, canOnlyUseCantrips);
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

    private static ActionStatus CanUseInvocationAction(Id actionId, ActionScope scope,
        RulesetCharacter character, bool canCastSpells, bool canOnlyUseCantrips)
    {
        if (IsGambitActionId(actionId)
            && character.HasPower(GambitsBuilders.GambitPool)
            && character.KnowsAnyInvocationOfActionId(actionId, scope)
            && character.GetRemainingPowerCharges(GambitsBuilders.GambitPool) <= 0)
        {
            return ActionStatus.OutOfUses;
        }

        return character.CanCastAnyInvocationOfActionId(actionId, scope, canCastSpells, canOnlyUseCantrips)
            ? ActionStatus.Available
            : ActionStatus.Unavailable;
    }

    private static bool IsInvocationActionId(Id id)
    {
        var extra = (ExtraActionId)id;

        //TODO: consider adding all invocation actions to a list and check it here
        return id is Id.CastInvocation
               || extra is ExtraActionId.CastInvocationBonus
                   or ExtraActionId.CastInvocationNoCost
                   or ExtraActionId.InventorInfusion
                   or ExtraActionId.CastPlaneMagicMain
                   or ExtraActionId.CastPlaneMagicBonus
               || IsGambitActionId(id);
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
