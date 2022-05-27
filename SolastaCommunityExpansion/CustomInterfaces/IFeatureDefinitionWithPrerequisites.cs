using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaCommunityExpansion.CustomInterfaces
{
    public interface IFeatureDefinitionWithPrerequisites
    {
        [CanBeNull]
        public delegate string Validate();

        public List<Validate> Validators { get; set; }
    }
}
