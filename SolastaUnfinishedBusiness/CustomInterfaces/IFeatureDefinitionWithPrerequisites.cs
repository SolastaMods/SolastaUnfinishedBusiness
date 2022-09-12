using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IFeatureDefinitionWithPrerequisites
{
    [CanBeNull]
    public delegate string Validate();

    public List<Validate> Validators { get; }
}
