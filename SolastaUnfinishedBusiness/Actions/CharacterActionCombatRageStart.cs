using System.Collections;
using JetBrains.Annotations;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionCombatRageStart(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        var actingCharacter = ActingCharacter;

        if (actingCharacter.Stealthy)
        {
            actingCharacter.SetStealthy(false);
        }

        var actionService = ServiceRepository.GetService<IGameLocationActionService>();

        if (actionService == null)
        {
            yield break;
        }

        var newParams = ActionParams.Clone();

        newParams.ActionDefinition = actionService.AllActionDefinitions[ActionDefinitions.Id.PowerNoCost];
        actionService.ExecuteAction(newParams, null, true);
        actingCharacter.RulesetCharacter.SpendRagePoint();

        yield return ServiceRepository.GetService<IGameLocationBattleService>()?
            .HandleReactionToRageStart(actingCharacter);
    }
}
