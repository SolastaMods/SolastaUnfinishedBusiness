using System.Linq;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

// this custom behavior allows an aura type power to start on rage and get terminated on rage stop
// it also enforce the condition to any other aura participant as soon as the barb enters rage
// finally it forces the condition to stop on barb turn start for any hero who had it but not in range anymore
public class CustomRagingAura :
    INotifyConditionRemoval, IOnAfterActionFeature, ICharacterTurnStartListener
{
    private readonly ConditionDefinition _conditionDefinition;
    private readonly FeatureDefinitionPower _powerDefinition;

    public CustomRagingAura(
        FeatureDefinitionPower powerDefinition,
        ConditionDefinition conditionDefinition)
    {
        _powerDefinition = powerDefinition;
        _conditionDefinition = conditionDefinition;
    }

    public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
    {
        var battle = Gui.Battle;

        if (battle == null)
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

        foreach (var targetLocationCharacter in battle.AllContenders
                     .Where(x =>
                         x.Side == locationCharacter.Side &&
                         x != locationCharacter &&
                         !gameLocationBattleService.IsWithinXCells(locationCharacter, x,
                             _powerDefinition.EffectDescription.targetParameter)))
        {
            var targetRulesetCharacter = targetLocationCharacter.RulesetCharacter;
            var rulesetCondition =
                targetRulesetCharacter.AllConditions.FirstOrDefault(x =>
                    x.ConditionDefinition == _conditionDefinition && x.SourceGuid == locationCharacter.Guid);

            if (rulesetCondition != null)
            {
                targetRulesetCharacter.RemoveCondition(rulesetCondition);
            }
        }
    }

    public void AfterConditionRemoved(RulesetActor removedFrom, RulesetCondition rulesetCondition)
    {
        RemoveCondition(removedFrom);
    }

    public void BeforeDyingWithCondition(RulesetActor rulesetActor, RulesetCondition rulesetCondition)
    {
        RemoveCondition(rulesetActor);
    }

    public void OnAfterAction(CharacterAction action)
    {
        if (action is CharacterActionSpendPower characterActionSpendPower &&
            characterActionSpendPower.activePower.PowerDefinition == _powerDefinition)
        {
            AddCondition(action.ActingCharacter);
        }
    }

    private void RemoveCondition(ISerializable rulesetActor)
    {
        if (rulesetActor is not RulesetCharacter sourceRulesetCharacter)
        {
            return;
        }

        var rulesetEffectPower =
            sourceRulesetCharacter.PowersUsedByMe.FirstOrDefault(x => x.PowerDefinition == _powerDefinition);

        if (rulesetEffectPower == null)
        {
            return;
        }

        sourceRulesetCharacter.TerminatePower(rulesetEffectPower);

        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        if (gameLocationCharacterService == null)
        {
            return;
        }

        foreach (var targetRulesetCharacter in gameLocationCharacterService.AllValidEntities
                     .Select(x => x.RulesetActor)
                     .OfType<RulesetCharacter>()
                     .Where(x => x.Side == sourceRulesetCharacter.Side && x != sourceRulesetCharacter))
        {
            var rulesetCondition =
                targetRulesetCharacter.AllConditions.FirstOrDefault(x =>
                    x.ConditionDefinition == _conditionDefinition && x.SourceGuid == sourceRulesetCharacter.Guid);

            if (rulesetCondition != null)
            {
                targetRulesetCharacter.RemoveCondition(rulesetCondition);
            }
        }
    }

    private void AddCondition(GameLocationCharacter sourceLocationCharacter)
    {
        var battle = Gui.Battle;

        if (battle == null)
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var factionName = sourceLocationCharacter.RulesetCharacter.CurrentFaction.Name;

        foreach (var targetLocationCharacter in battle.AllContenders
                     .Where(x =>
                         x.Side == sourceLocationCharacter.Side &&
                         x != sourceLocationCharacter &&
                         !x.RulesetCharacter.IsDeadOrDyingOrUnconscious &&
                         gameLocationBattleService.IsWithinXCells(sourceLocationCharacter, x, 2)))
        {
            var condition = RulesetCondition.CreateActiveCondition(
                targetLocationCharacter.Guid,
                _conditionDefinition,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                sourceLocationCharacter.Guid,
                factionName);

            targetLocationCharacter.RulesetCharacter.AddConditionOfCategory(AttributeDefinitions.TagEffect,
                condition);
        }
    }
}
