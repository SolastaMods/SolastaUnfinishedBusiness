using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// allows to bypass spell concentration if original spell on this list
// for each spell on this list you need another one set without concentration post-fixed with "NonConcentration"
public interface IBypassSpellConcentration
{
    public IEnumerable<SpellDefinition> SpellDefinitions();
    
    // allow to bypass only if a certain level upcast was triggered. set to zero to bypass this check
    public int OnlyWithUpcastGreaterThan();
}
