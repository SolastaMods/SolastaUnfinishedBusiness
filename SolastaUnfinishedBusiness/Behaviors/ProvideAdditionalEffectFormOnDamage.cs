using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.Behaviors;

public delegate IEnumerable<EffectForm> AdditionalEffectFormOnDamageHandler(
    GameLocationCharacter attacker, GameLocationCharacter defender, IAdditionalDamageProvider provider);
