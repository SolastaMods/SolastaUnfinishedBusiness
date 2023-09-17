using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICastSpellFinishedByMe
{
    [UsedImplicitly]
    public IEnumerator OnCastSpellFinishedByMe(CharacterActionCastSpell action, SpellDefinition spell);
}
