using JetBrains.Annotations;
using TA;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface IModifyCoverType
{
    public void ModifyCoverType(
        [UsedImplicitly] GameLocationCharacter attacker,
        int3 attackerPosition,
        GameLocationCharacter defender,
        int3 defenderPosition,
        ActionModifier attackModifier,
        ref RuleDefinitions.CoverType bestCoverType,
        bool ignoreCoverFromCharacters);
}
