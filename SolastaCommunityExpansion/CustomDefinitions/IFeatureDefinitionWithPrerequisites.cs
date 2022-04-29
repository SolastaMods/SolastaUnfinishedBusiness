using System;
using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IFeatureDefinitionWithPrerequisites
    {
        public List<Func<bool>> Validators { get; set; }
    }
}
