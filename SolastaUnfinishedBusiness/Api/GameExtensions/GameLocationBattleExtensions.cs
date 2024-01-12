using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.CustomBehaviors;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class GameLocationBattleExtensions
{
    internal static List<GameLocationCharacter> GetContenders(this GameLocationBattle battle,
        GameLocationCharacter character = null,
        bool isOppositeSide = true,
        bool excludeSelf = true,
        bool hasToPerceiveTarget = false,
        int isWithinXCells = 0)
    {
        var aliveContenders = battle.AllContenders
            .Where(x =>
                x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } && (!excludeSelf || x != character));

        if (character == null)
        {
            return aliveContenders.ToList();
        }

        aliveContenders = isOppositeSide
            ? aliveContenders.Where(x => x.IsOppositeSide(character.Side))
            : aliveContenders.Where(x => x.Side == character.Side);

        if (isWithinXCells > 0)
        {
            aliveContenders = aliveContenders.Where(x =>
                DistanceCalculation.CalculateDistanceFromTwoCharacters(character, x) <= isWithinXCells);
        }

        if (hasToPerceiveTarget)
        {
            aliveContenders = isOppositeSide
                ? aliveContenders.Where(x => character.PerceivedFoes.Contains(x))
                : aliveContenders.Where(x => character.PerceivedAllies.Contains(x));
        }

        return aliveContenders.ToList();
    }
}
