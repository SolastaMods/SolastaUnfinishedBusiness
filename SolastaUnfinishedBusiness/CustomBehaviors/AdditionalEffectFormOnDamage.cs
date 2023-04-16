using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

public delegate IEnumerable<EffectForm> AdditionalEffectFormOnDamageHandler(
    GameLocationCharacter attacker, GameLocationCharacter defender, IAdditionalDamageProvider provider);
