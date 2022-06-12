using System;
using SolastaModApi.Diagnostics;

namespace SolastaModApi.Extensions;

/// <summary>
///     TODO: remove this extension and replace usage with EffectDescriptionBuilder
/// </summary>
public static partial class EffectDescriptionExtensions
{
    
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
