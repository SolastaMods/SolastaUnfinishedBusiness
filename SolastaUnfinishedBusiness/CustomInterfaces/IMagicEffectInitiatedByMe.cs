using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IMagicEffectInitiatedByMe
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition);
}
