using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Builders;

internal static class DiceByRankBuilder
{
    internal static List<DiceByRank> BuildDiceByRankTable(
        int startDamage = 0, int incrementDamage = 1, int step = 1, int begin = 1)
    {
        var result = new List<DiceByRank>();

        for (var i = 1; i <= 20; i++)
        {
            var multiplier = i < begin ? 0 : 1;

            result.Add(new DiceByRank
            {
                rank = i, diceNumber = multiplier * (startDamage + ((i - begin) / step * incrementDamage))
            });
        }

        return result;
    }
}
