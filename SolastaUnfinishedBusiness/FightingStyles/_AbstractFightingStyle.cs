using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal abstract class AbstractFightingStyle
{
    internal abstract FightingStyleDefinition FightingStyle { get; }

    internal abstract List<FeatureDefinitionFightingStyleChoice> FightingStyleChoice { get; }
}
