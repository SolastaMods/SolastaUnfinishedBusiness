using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IValidateDieRollModifier
{
    public bool CanModifyRoll(
        // ReSharper disable once UnusedParameter.Global
        RulesetCharacter character,
        // ReSharper disable once UnusedParameter.Global
        List<FeatureDefinition> features,
        List<string> damageTypes);
}
