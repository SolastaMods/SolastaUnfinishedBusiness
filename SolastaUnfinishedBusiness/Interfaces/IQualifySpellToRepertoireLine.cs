using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Interfaces;

internal interface IQualifySpellToRepertoireLine
{
    void QualifySpells(RulesetCharacter character, SpellRepertoireLine line, IEnumerable<SpellDefinition> spells);
}
