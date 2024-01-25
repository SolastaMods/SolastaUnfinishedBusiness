using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomGenericBehaviors;

public delegate IEnumerable<EffectForm> AdditionalEffectFormOnDamageHandler(
    GameLocationCharacter attacker, GameLocationCharacter defender, IAdditionalDamageProvider provider);
