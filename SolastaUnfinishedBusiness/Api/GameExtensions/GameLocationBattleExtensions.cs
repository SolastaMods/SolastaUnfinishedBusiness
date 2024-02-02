using System.Collections.Generic;
using System.Linq;

namespace SolastaUnfinishedBusiness.Api.GameExtensions;

internal static class GameLocationBattleExtensions
{
    internal static List<GameLocationCharacter> GetContenders(this GameLocationBattle battle,
        GameLocationCharacter character,
        GameLocationCharacter perceiver = null,
        bool hasToPerceivePerceiver = false,
        bool isOppositeSide = true,
        bool excludeSelf = true,
        bool hasToPerceiveTarget = false,
        int withinRange = 0)
    {
        var aliveContenders = battle.AllContenders
            .Where(x =>
                x.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                !x.IsCharging &&
                !x.MoveStepInProgress &
                (!excludeSelf || x != character));

        if (character == null)
        {
            return aliveContenders.ToList();
        }

        aliveContenders = isOppositeSide
            ? aliveContenders.Where(x => x.IsOppositeSide(character.Side))
            : aliveContenders.Where(x => x.Side == character.Side);

        if (withinRange > 0)
        {
            aliveContenders = aliveContenders.Where(x => character.IsWithinRange(x, withinRange));
        }

        var finalPerceiver = perceiver ?? character;

        if (hasToPerceiveTarget)
        {
            aliveContenders = aliveContenders.Where(finalPerceiver.CanPerceiveTarget);
        }

        if (hasToPerceivePerceiver)
        {
            aliveContenders = aliveContenders.Where(x => x.CanPerceiveTarget(finalPerceiver));
        }

        return aliveContenders.ToList();
    }
}
