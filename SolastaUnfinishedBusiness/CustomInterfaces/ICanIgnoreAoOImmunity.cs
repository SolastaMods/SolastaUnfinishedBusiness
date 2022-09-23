namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICanIgnoreAoOImmunity
{
    bool CanIgnoreAoOImmunity(RulesetCharacter character, RulesetCharacter attacker, float distance);
}
