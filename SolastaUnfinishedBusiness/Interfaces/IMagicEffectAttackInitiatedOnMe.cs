using System.Collections;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IMagicEffectAttackInitiatedOnMe
{
    [UsedImplicitly]
    IEnumerator OnMagicEffectAttackInitiatedOnMe(
        CharacterActionMagicEffect action,
        RulesetEffect activeEffect,
        GameLocationCharacter attacker,
        GameLocationCharacter defender,
        ActionModifier attackModifier,
        bool firstTarget,
        bool checkMagicalAttackDamage);
}
