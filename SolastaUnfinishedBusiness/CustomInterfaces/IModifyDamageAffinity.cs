using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IModifyDamageAffinity
{
    public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features);
}
