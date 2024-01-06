using System.Collections;
using System.Linq;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

// this custom behavior allows an aura type power to start on rage and get terminated on rage stop
// it also enforce the condition to any other aura participant as soon as the barb enters rage
// finally it forces the condition to stop on barb turn start for any hero who had it but not in range anymore
public class CustomRagingAura :
    IOnConditionAddedOrRemoved, IActionFinishedByMe, ICharacterTurnStartListener
{
    private readonly ConditionDefinition _conditionDefinition;
    private readonly bool _friendlyAura;
    private readonly FeatureDefinitionPower _powerDefinition;

    public CustomRagingAura(
        FeatureDefinitionPower powerDefinition,
        ConditionDefinition conditionDefinition,
        bool friendlyAura)
    {
        _powerDefinition = powerDefinition;
        _conditionDefinition = conditionDefinition;
        _friendlyAura = friendlyAura;
    }

    public IEnumerator OnActionFinishedByMe(CharacterAction characterAction)
    {
        if (characterAction is not CharacterActionSpendPower action
            || action.activePower.PowerDefinition != _powerDefinition)
        {
            yield break;
        }

        if (_friendlyAura)
        {
            AddCondition(action.ActingCharacter);
        }
        else
        {
            action.ActingCharacter.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                AttributeDefinitions.TagEffect,
                _conditionDefinition.Name);
        }
    }

    public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
    {
        var battle = Gui.Battle;

        if (battle == null)
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

        if (_friendlyAura)
        {
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
        else
        {
            foreach (var targetLocationCharacter in battle.AllContenders
                         .Where(x =>
                             x.IsOppositeSide(locationCharacter.Side) &&
                             (!gameLocationBattleService.IsWithinXCells(locationCharacter, x,
                                  _powerDefinition.EffectDescription.targetParameter) ||
                              !locationCharacter.RulesetCharacter.HasConditionOfType(ConditionRaging))))
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

        if (!locationCharacter.RulesetCharacter.HasConditionOfType(ConditionRaging))
        {
            RemoveCondition(locationCharacter.RulesetActor);
        }
    }

    public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        // empty
    }

    public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
    {
        RemoveCondition(target);
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

        if (_friendlyAura)
        {
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
        else
        {
            foreach (var targetRulesetCharacter in gameLocationCharacterService.AllValidEntities
                         .Select(x => x.RulesetActor)
                         .OfType<RulesetCharacter>()
                         .Where(x => x.Side != sourceRulesetCharacter.Side))
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
    }

    private void AddCondition(GameLocationCharacter sourceLocationCharacter)
    {
        var battle = Gui.Battle;

        if (battle == null)
        {
            return;
        }

        var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();
        var sourceCharacter = sourceLocationCharacter.RulesetCharacter;

        if (_friendlyAura)
        {
            foreach (var allyCharacter in battle.AllContenders
                         .Where(x =>
                             x != sourceLocationCharacter &&
                             x.Side == sourceLocationCharacter.Side &&
                             x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                             gameLocationBattleService.IsWithinXCells(sourceLocationCharacter, x, 2))
                         .Select(allyLocationCharacter => allyLocationCharacter.RulesetCharacter)
                         .ToList()) // avoid changing enumerator
            {
                allyCharacter.InflictCondition(
                    _conditionDefinition.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    sourceCharacter.guid,
                    sourceCharacter.CurrentFaction.Name,
                    1,
                    _conditionDefinition.Name,
                    0,
                    0,
                    0);
            }
        }
        else
        {
            foreach (var defenderCharacter in battle.AllContenders
                         .Where(x =>
                             x != sourceLocationCharacter &&
                             x.IsOppositeSide(sourceLocationCharacter.Side) &&
                             x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                             gameLocationBattleService.IsWithinXCells(sourceLocationCharacter, x, 2) &&
                             sourceLocationCharacter.RulesetCharacter.HasConditionOfType(ConditionRaging))
                         .Select(defenderLocationCharacter => defenderLocationCharacter.RulesetCharacter)
                         .ToList()) // avoid changing enumerator
            {
                defenderCharacter.InflictCondition(
                    _conditionDefinition.Name,
                    DurationType.Round,
                    1,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    sourceCharacter.guid,
                    sourceCharacter.CurrentFaction.Name,
                    1,
                    _conditionDefinition.Name,
                    0,
                    0,
                    0);
            }
        }
    }
}
