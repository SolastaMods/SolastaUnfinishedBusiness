using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using TA;

namespace SolastaUnfinishedBusiness.Behaviors.Specific;

internal static class FlankingAndHigherGround
{
    private static readonly Dictionary<(ulong, ulong), bool> FlankingDeterminationCache = new();

    internal static void ClearFlankingDeterminationCache()
    {
        FlankingDeterminationCache.Clear();
    }

    private static IEnumerable<CellFlags.Side> GetEachSide(CellFlags.Side side)
    {
        if ((side & CellFlags.Side.North) > 0)
        {
            yield return CellFlags.Side.North;
        }

        if ((side & CellFlags.Side.South) > 0)
        {
            yield return CellFlags.Side.South;
        }

        if ((side & CellFlags.Side.East) > 0)
        {
            yield return CellFlags.Side.East;
        }

        if ((side & CellFlags.Side.West) > 0)
        {
            yield return CellFlags.Side.West;
        }

        if ((side & CellFlags.Side.Top) > 0)
        {
            yield return CellFlags.Side.Top;
        }

        if ((side & CellFlags.Side.Bottom) > 0)
        {
            yield return CellFlags.Side.Bottom;
        }
    }

    private static IEnumerable<int3> GetPositions(GameLocationCharacter gameLocationCharacter)
    {
        // collect all positions in the character cube surface
        var basePosition = gameLocationCharacter.LocationPosition;
        var maxExtents = gameLocationCharacter.SizeParameters.maxExtent;

        // traverse by horizontal planes as most common use case in battles
        for (var x = 0; x <= maxExtents.x; x++)
        {
            for (var z = 0; z <= maxExtents.z; z++)
            {
                for (var y = 0; y <= maxExtents.y; y++)
                {
                    yield return basePosition + new int3(x, y, z);
                }
            }
        }
    }

    private static bool GetAllies(
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        out List<GameLocationCharacter> allies)
    {
        var locationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        allies = locationCharacterService.AllValidEntities
            .Where(x =>
                x.Side == attacker.Side &&
                x.IsWithinRange(defender, 1) &&
                x.CanAct())
            .ToList();

        return allies.Count > 0;
    }

    private static bool IsFlanking(GameLocationCharacter attacker, GameLocationCharacter defender)
    {
        if (FlankingDeterminationCache.TryGetValue((attacker.Guid, defender.Guid), out var result))
        {
            return result;
        }

        FlankingDeterminationCache.Add((attacker.Guid, defender.Guid), false);

        if (!GetAllies(attacker, defender, out var alliesInRange))
        {
            return false;
        }

        // collect all possible flanking sides from all attacker cells against all enemy cells

        var attackerFlankingSides = new HashSet<CellFlags.Side>();

        foreach (var attackerPosition in GetPositions(attacker))
        {
            foreach (var defenderPosition in GetPositions(defender))
            {
                var attackerDirection = defenderPosition - attackerPosition;
                var attackerSide = CellFlags.DirectionToAllSurfaceSides(attackerDirection);
                var flankingSide = GetEachSide(attackerSide)
                    .Aggregate(CellFlags.Side.None, (current, side) => current | CellFlags.InvertSide(side));

                attackerFlankingSides.Add(flankingSide);
            }
        }

        result = alliesInRange
            .Any(ally => GetPositions(ally)
                .Any(allyPosition => GetPositions(defender)
                    .Any(defenderPosition =>
                        attackerFlankingSides.Contains(
                            CellFlags.DirectionToAllSurfaceSides(defenderPosition - allyPosition)))));

        FlankingDeterminationCache[(attacker.Guid, defender.Guid)] = result;

        return result;
    }

    //
    // FLANKING IMPLEMENTATION WITH MATH
    // Uses classes in FlankingMathExtensions
    private static bool IsFlankingWithMath(GameLocationCharacter attacker, GameLocationCharacter defender)
    {
        if (FlankingDeterminationCache.TryGetValue((attacker.Guid, defender.Guid), out var result))
        {
            return result;
        }

        FlankingDeterminationCache.Add((attacker.Guid, defender.Guid), false);

        if (!GetAllies(attacker, defender, out var alliesInRange))
        {
            return false;
        }

        var attackerCenter = new FlankingMathExtensions.Point3D(attacker.LocationBattleBoundingBox.Center);
        var defenderCube = new FlankingMathExtensions.Cube(
            new FlankingMathExtensions.Point3D(defender.LocationBattleBoundingBox.Min),
            new FlankingMathExtensions.Point3D(defender.LocationBattleBoundingBox.Max + 1));

        foreach (var allyCenter in alliesInRange
                     .Select(ally => new FlankingMathExtensions.Point3D(ally.LocationBattleBoundingBox.Center)))
        {
            result = FlankingMathExtensions.LineIntersectsCubeOppositeSides(attackerCenter, allyCenter, defenderCube);

            if (result)
            {
                break;
            }
        }

        FlankingDeterminationCache[(attacker.Guid, defender.Guid)] = result;

        return result;
    }

    internal static void HandleFlanking(BattleDefinitions.AttackEvaluationParams evaluationParams)
    {
        if (!Main.Settings.UseOfficialFlankingRules)
        {
            return;
        }

        if (!Main.Settings.UseOfficialFlankingRulesAlsoForRanged &&
            evaluationParams.attackProximity
                is BattleDefinitions.AttackProximity.MagicRange
                or BattleDefinitions.AttackProximity.MagicReach
                or BattleDefinitions.AttackProximity.MagicDistance
                or BattleDefinitions.AttackProximity.PhysicalRange
                or BattleDefinitions.AttackProximity.SimpleRange)
        {
            return;
        }

        var attacker = evaluationParams.attacker;
        var defender = evaluationParams.defender;

        if (!Main.Settings.UseOfficialFlankingRulesAlsoForReach && !attacker.IsWithinRange(defender, 1))
        {
            return;
        }

        if (Main.Settings.UseMathFlankingRules)
        {
            if (!IsFlankingWithMath(attacker, defender))
            {
                return;
            }
        }
        else
        {
            if (!IsFlanking(attacker, defender))
            {
                return;
            }
        }

        var actionModifier = evaluationParams.attackModifier;

        if (Main.Settings.UseOfficialFlankingRulesButAddAttackModifier)
        {
            actionModifier.attackRollModifier += 1;
            actionModifier.attackToHitTrends.Add(
                new RuleDefinitions.TrendInfo(1, RuleDefinitions.FeatureSourceType.Unknown, "Feedback/&FlankingAttack",
                    null));
        }
        else
        {
            actionModifier.AttackAdvantageTrends.Add(
                new RuleDefinitions.TrendInfo(1, RuleDefinitions.FeatureSourceType.Unknown, "Feedback/&FlankingAttack",
                    null));
        }
    }

    internal static void HandleHigherGround(BattleDefinitions.AttackEvaluationParams evaluationParams)
    {
        if (!Main.Settings.EnableHigherGroundRules)
        {
            return;
        }

        var attacker = evaluationParams.attacker;
        var defender = evaluationParams.defender;

        if (attacker.LocationPosition.y <= defender.LocationPosition.y)
        {
            return;
        }

        var actionModifier = evaluationParams.attackModifier;

        actionModifier.attackRollModifier += 1;
        actionModifier.attackToHitTrends.Add(
            new RuleDefinitions.TrendInfo(1, RuleDefinitions.FeatureSourceType.Unknown, "Feedback/&HigherGroundAttack",
                null));
    }
}
