using System.Collections;
using System.Collections.Generic;

namespace SolastaUnfinishedBusiness.CustomInterfaces;
internal interface IDamageReceived
{
    IEnumerator OnDamageReceived(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            int damageAmount,
            RulesetEffect rulesetEffect,
            List<string> effectiveDamageTypes);
}
