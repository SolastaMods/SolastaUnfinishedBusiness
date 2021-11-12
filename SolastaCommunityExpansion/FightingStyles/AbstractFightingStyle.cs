using System.Collections.Generic;


namespace SolastaCommunityExpansion.FightingStyles
{
    abstract class AbstractFightingStyle
    {
        internal abstract FightingStyleDefinition GetStyle();

        internal abstract List<FeatureDefinitionFightingStyleChoice> GetChoiceLists();
    }
}
