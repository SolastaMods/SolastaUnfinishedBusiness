using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IForceMaxDamageTypeDependent
{
    BaseDefinition FeatureDefinition { get; }
    List<string> DamageTypes { get; }
}
