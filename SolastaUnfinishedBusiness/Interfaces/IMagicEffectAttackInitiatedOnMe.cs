using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMagicEffectAttackInitiatedOnMe
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectAttackInitiatedOnMe(
        CharacterActionMagicEffect action,
        RulesetEffect activeEffect,
        GameLocationCharacter target,
        ActionModifier attackModifier,
        List<EffectForm> actualEffectForms,
        bool firstTarget,
        bool checkMagicalAttackDamage);
}
