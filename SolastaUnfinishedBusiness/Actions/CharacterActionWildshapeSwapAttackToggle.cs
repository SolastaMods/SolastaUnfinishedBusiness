using System.Collections;
using JetBrains.Annotations;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionMonsterSwapAttackToggle(CharacterActionParams actionParams) : CharacterAction(actionParams)
#pragma warning restore CA1050
{
    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (rulesetCharacter is not RulesetCharacterMonster rulesetCharacterMonster)
        {
            yield break;
        }

        var monsterDefinition = rulesetCharacterMonster.MonsterDefinition;

        if (monsterDefinition.AttackIterations.Count < 2)
        {
            yield break;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter is not { HasAttackedSinceLastTurn: false })
        {
            yield break;
        }

        (monsterDefinition.AttackIterations[0], monsterDefinition.AttackIterations[1]) =
            (monsterDefinition.AttackIterations[1], monsterDefinition.AttackIterations[0]);

        rulesetCharacterMonster.RefreshAttackModes();
        rulesetCharacterMonster.CharacterRefreshed?.Invoke(rulesetCharacterMonster);
    }
}
