namespace SolastaUnfinishedBusiness.Interfaces;

public interface IIgnoreAoOOnMe
{
    public bool CanIgnoreAoOOnSelf(
        // ReSharper disable once UnusedParameter.Global
        RulesetCharacter defender,
        // ReSharper disable once UnusedParameter.Global
        RulesetCharacter attacker);
}
