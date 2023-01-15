using System.Collections;
using JetBrains.Annotations;

//This should have default namespace so that it can be properly created by `CharacterActionPatcher`
// ReSharper disable once CheckNamespace
[UsedImplicitly]
#pragma warning disable CA1050
public class CharacterActionWildshapeSwapAttackToggle : CharacterAction
#pragma warning restore CA1050
{
    public CharacterActionWildshapeSwapAttackToggle(CharacterActionParams actionParams) : base(actionParams)
    {
    }

    public override IEnumerator ExecuteImpl()
    {
        var rulesetCharacter = ActingCharacter.RulesetCharacter;

        if (rulesetCharacter is not RulesetCharacterMonster monster)
        {
            yield break;
        }

        var monsterDef = monster.MonsterDefinition;

        if (monsterDef.AttackIterations.Count < 2)
        {
            yield break;
        }

        var gameLocationCharacter = GameLocationCharacter.GetFromActor(rulesetCharacter);

        if (gameLocationCharacter.HasAttackedSinceLastTurn)
        {
            yield break;
        }

        (monsterDef.AttackIterations[0], monsterDef.AttackIterations[1]) =
            (monsterDef.AttackIterations[1], monsterDef.AttackIterations[0]);

        monster.RefreshAttackModes();

        yield return null;
    }
}
