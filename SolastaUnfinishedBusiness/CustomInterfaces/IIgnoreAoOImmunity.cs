namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIgnoreAoOImmunity
{
    bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker, float distance);
}
