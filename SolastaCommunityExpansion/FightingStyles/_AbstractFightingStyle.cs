using System.Collections.Generic;

namespace SolastaCommunityExpansion.FightingStyles;

internal abstract class AbstractFightingStyle
{
    internal abstract FightingStyleDefinition GetStyle();

    internal abstract List<FeatureDefinitionFightingStyleChoice> GetChoiceLists();
}
