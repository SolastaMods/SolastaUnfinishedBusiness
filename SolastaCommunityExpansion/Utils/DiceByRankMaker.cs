using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.Utils
{
    public static class DiceByRankMaker
    {
        public static List<DiceByRank> Make(params (int, int)[] ranks)
        {
            return ranks
                .Select((rank) => new DiceByRank().SetRank(rank.Item1).SetDices(rank.Item2))
                .ToList();
        }

        public static List<DiceByRank> MakeBySteps(int start = 0, int increment = 1, int step = 0)
        {
            var result = new List<DiceByRank>();
            for (int i = 0; i < 20; i++)
            {
                result.Add(new DiceByRank().SetRank(i).SetDices(start + ((i + 1) / (step + 1)) * increment));
            }

            return result;
        }

        public static DiceByRank SetRank(this DiceByRank instance, int rank)
        {
            instance.SetField("rank", rank);
            return instance;
        }

        public static DiceByRank SetDices(this DiceByRank instance, int dices)
        {
            instance.SetField("diceNumber", dices);
            return instance;
        }
    }
}
