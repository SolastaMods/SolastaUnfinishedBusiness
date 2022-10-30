using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Builders;

internal static class DiceByRankBuilder
{
    internal static List<DiceByRank> BuildDiceByRankTable(
        int startDamage = 0, int incrementDamage = 1, int step = 1, int begin = 1)
    {
        var result = new List<DiceByRank>();

        for (var i = begin; i <= 20; i++)
        {
            result.Add(new DiceByRank { rank = i, diceNumber = startDamage + ((i - begin) / step * incrementDamage) });
        }

        return result;
    }
}
