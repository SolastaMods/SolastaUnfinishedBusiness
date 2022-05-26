using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.CustomInterfaces
{
    public interface IFeatureDefinitionWithPrerequisites
    {
        public List<Validate> Validators { get; set; }

        [CanBeNull]
        public delegate string Validate();
    }
}
