using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Builders;

internal static class DiceByRankBuilder
{
    internal static DiceByRank BuildDiceByRank(int rank, int dice)
    {
        var diceByRank = new DiceByRank { rank = rank, diceNumber = dice };

        return diceByRank;
    }

    internal static List<DiceByRank> BuildDiceByRankTable(int start = 0, int increment = 1, int step = 0)
    {
        var result = new List<DiceByRank>();

        for (var i = 1; i <= 20; i++)
        {
            result.Add(new DiceByRank { rank = i, diceNumber = start + ((i + 1) / (step + 1) * increment) });
        }

        return result;
    }
}
