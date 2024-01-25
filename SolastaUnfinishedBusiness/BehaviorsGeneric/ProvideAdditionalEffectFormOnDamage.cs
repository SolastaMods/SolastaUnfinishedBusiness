using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.BehaviorsGeneric;

public delegate IEnumerable<EffectForm> AdditionalEffectFormOnDamageHandler(
    GameLocationCharacter attacker, GameLocationCharacter defender, IAdditionalDamageProvider provider);
