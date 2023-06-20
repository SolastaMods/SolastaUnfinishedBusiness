using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IQualifySpellToRepertoireLine
{
    void QualifySpells(RulesetCharacter character, SpellRepertoireLine line, IEnumerable<SpellDefinition> spells);
}
