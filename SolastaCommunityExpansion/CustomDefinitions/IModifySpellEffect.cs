using System.Collections.Generic;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IModifySpellEffect
    {
        EffectDescription ModifyEffect(RulesetEffectSpell spell, EffectDescription effect);
    }
}
