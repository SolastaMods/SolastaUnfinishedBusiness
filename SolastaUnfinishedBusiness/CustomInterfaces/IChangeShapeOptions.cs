using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IChangeShapeOptions
{
    ConditionDefinition SpecialSubstituteCondition { get; }
    List<ShapeOptionDescription> ShapeOptions { get; }
}
