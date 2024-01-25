using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyDamageAffinity
{
    public void ModifyDamageAffinity(
        [UsedImplicitly] RulesetActor defender,
        RulesetActor attacker,
        List<FeatureDefinition> features);
}
