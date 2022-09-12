using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.FightingStyles;

internal abstract class AbstractFightingStyle
{
    internal abstract FightingStyleDefinition GetStyle();

    internal abstract List<FeatureDefinitionFightingStyleChoice> GetChoiceLists();
}
