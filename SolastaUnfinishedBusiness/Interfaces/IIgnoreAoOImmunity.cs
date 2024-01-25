namespace SolastaUnfinishedBusiness.Interfaces;

public interface IIgnoreAoOImmunity
{
    bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker, float distance);
}
