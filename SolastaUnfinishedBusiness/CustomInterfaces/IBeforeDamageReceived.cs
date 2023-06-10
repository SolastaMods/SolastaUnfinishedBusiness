using System;
using System.Collections;

namespace SolastaUnfinishedBusiness.CustomInterfaces;
internal interface IBeforeDamageReceived
{
    IEnumerator OnBeforeReceivedDamage(GameLocationCharacter attacker, 
        GameLocationCharacter defender, 
        RulesetAttackMode attackMode,
        RulesetEffect rulesetEffect, 
        ActionModifier attackModifier, 
        bool rolledSavingThrow, 
        bool saveOutcomeSuccess);
}
