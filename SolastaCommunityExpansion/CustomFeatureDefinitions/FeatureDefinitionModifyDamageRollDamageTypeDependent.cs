using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public class FeatureDefinitionModifyDamageRollDamageTypeDependent : FeatureDefinitionDieRollModifier
    {
        public List<string> DamageTypes = new();
    }
}
