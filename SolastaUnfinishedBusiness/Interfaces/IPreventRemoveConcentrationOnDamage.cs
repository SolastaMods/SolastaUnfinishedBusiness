using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPreventRemoveConcentrationOnDamage
{
    public HashSet<SpellDefinition> SpellsThatShouldNotRollConcentrationCheckFromDamage(
        RulesetCharacter rulesetCharacter);
}
