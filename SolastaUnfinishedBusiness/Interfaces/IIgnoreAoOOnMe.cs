namespace SolastaUnfinishedBusiness.Interfaces;

public interface IIgnoreAoOOnMe
{
    public bool CanIgnoreAoOOnSelf(RulesetCharacter defender, RulesetCharacter attacker);
}
