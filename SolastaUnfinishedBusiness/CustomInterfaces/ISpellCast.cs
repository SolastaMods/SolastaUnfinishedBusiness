using System.Collections;
using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

// On spell being cast
internal interface ISpellCast
{
    IEnumerator OnSpellCast(
            RulesetCharacter featureOwner,
            GameLocationCharacter caster,
            CharacterActionCastSpell castAction,
            RulesetEffectSpell selectEffectSpell,
            RulesetSpellRepertoire selectedRepertoire,
            SpellDefinition selectedSpellDefinition);
}
