using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

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

    /**
     * Builds dice by rank table from rank/dice pairs
     */
    [UsedImplicitly]
    internal static List<DiceByRank> Build(params (int rank, int dice)[] steps)
    {
        return steps.Select(s => new DiceByRank { rank = s.rank, diceNumber = s.dice }).ToList();
    }

    /**
     * Builds dice by rank table by specifying points of change of dice number, filling spaces between with values of previous point
     */
    [UsedImplicitly]
    internal static List<DiceByRank> InterpolateDiceByRankTable(int minRank = 1, int maxRank = 20,
        params (int level, int dice)[] steps)
    {
        var result = new List<DiceByRank>();
        if (steps == null || steps.Length == 0) { return result; }

        var sorted = steps.ToList();
        sorted.Sort((a, b) => a.level - b.level);

        var k = -1;

        for (var i = minRank; i <= maxRank; i++)
        {
            while (k < sorted.Count - 1 && i >= sorted[k + 1].level) { k++; }

            result.Add(new DiceByRank { rank = i, diceNumber = k < 0 ? 0 : sorted[k].dice });
        }

        return result;
    }
}
