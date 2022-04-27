using System;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IFeatureDefinitionWithPrerequisites
    {
        public Func<bool> Validator { get; set; }
    }
}
