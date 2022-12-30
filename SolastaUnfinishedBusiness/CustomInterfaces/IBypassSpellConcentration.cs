using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IBypassSpellConcentration
{
    public IEnumerable<SpellDefinition> SpellDefinitions();
}
