using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IDefinitionWithCustomFeatures
    {
        List<object> CustomFeatures { get; }
    }
}
