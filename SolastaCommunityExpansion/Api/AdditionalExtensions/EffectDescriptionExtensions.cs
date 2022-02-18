using System;
using SolastaModApi.Diagnostics;

namespace SolastaModApi.Extensions
{
    public static partial class EffectDescriptionExtensions
    {
        public static T SetDuration<T>(this T entity, RuleDefinitions.DurationType type, int? duration = null)
                where T : EffectDescription
        {
            switch (type)
            {
                case RuleDefinitions.DurationType.Round:
                case RuleDefinitions.DurationType.Minute:
                case RuleDefinitions.DurationType.Hour:
                case RuleDefinitions.DurationType.Day:
                    if (duration == null)
                    {
                        throw new ArgumentNullException(nameof(duration), $"A duration value is required for duration type {type}.");
                    }
                    entity.SetDurationParameter(duration.Value);
                    break;
                default:
                    if (duration != null)
                    {
                        throw new SolastaModApiException($"A duration value is not expected for duration type {type}");
                    }
                    // TODO: is this sensible?
                    entity.SetDurationParameter(0);
                    break;
            }

            entity.SetDurationType(type);

            return entity;
        }

        public static T SetRange<T>(this T entity, RuleDefinitions.RangeType type, int? range = null)
                where T : EffectDescription
        {
            switch (type)
            {
                case RuleDefinitions.RangeType.RangeHit:
                case RuleDefinitions.RangeType.Distance:
                    if (range == null)
                    {
                        throw new ArgumentNullException(nameof(range), $"A range value is required for range type {type}.");
                    }
                    entity.SetRangeParameter(range.Value);
                    break;
                case RuleDefinitions.RangeType.Touch:
                    entity.SetRangeParameter(range ?? 0);
                    break;
                default: // Self, MeleeHit
                    if (range != null)
                    {
                        throw new SolastaModApiException($"A duration value is not expected for duration type {type}");
                    }
                    // TODO: is this sensible?
                    entity.SetRangeParameter(0);
                    break;
            }

            entity.SetRangeType(type);

            return entity;
        }

/*        // TODO
        public static T SetTarget<T>(this T entity, RuleDefinitions.Side side, RuleDefinitions.TargetType type, int targetParameter, int? targetParameter2 = null)
                where T : EffectDescription
        {
            return entity
                .SetTargetSide(side)
                .SetTarget(type, targetParameter, targetParameter2);
        }

        public static T SetTarget<T>(this T entity, RuleDefinitions.TargetType type, int targetParameter, int? targetParameter2 = null)
                where T : EffectDescription
        {
            switch (type)
            {
                case RuleDefinitions.TargetType.Self:
                    break;
                case RuleDefinitions.TargetType.Individuals:
                    break;
                case RuleDefinitions.TargetType.IndividualsUnique:
                    break;
                case RuleDefinitions.TargetType.Line:
                    break;
                case RuleDefinitions.TargetType.Cone:
                    break;
                case RuleDefinitions.TargetType.Cube:
                    // targetParameter = edgeSize
                    break;
                case RuleDefinitions.TargetType.Sphere:
                    break;
                case RuleDefinitions.TargetType.Item:
                    break;
                case RuleDefinitions.TargetType.PerceivingWithinDistance:
                    break;
                case RuleDefinitions.TargetType.SharedAmongIndividuals:
                    break;
                case RuleDefinitions.TargetType.Position:
                    break;
                case RuleDefinitions.TargetType.InLineOfSightWithinDistance:
                    break;
                case RuleDefinitions.TargetType.Cylinder:
                    // targetParameter = radius
                    // targetParameter2 = height/length
                    break;
                case RuleDefinitions.TargetType.WallLine:
                    break;
                case RuleDefinitions.TargetType.WallRing:
                    break;
                case RuleDefinitions.TargetType.CubeWithOffset:
                    break;
                case RuleDefinitions.TargetType.ArcFromIndividual:
                    break;
                case RuleDefinitions.TargetType.FreeSlot:
                    break;
                case RuleDefinitions.TargetType.CylinderWithDiameter:
                    // targetParameter = diameter
                    // targetParameter2 = height/length
                    break;
                default:
                    break;
            }

            entity.SetTargetType(type);

            return entity;
        }
*/
    }
}
