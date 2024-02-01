using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPreventRemoveConcentrationOnDamage
{
    public HashSet<SpellDefinition> SpellsThatShouldNotCheckConcentrationOnDamage(RulesetCharacter rulesetCharacter);
}
