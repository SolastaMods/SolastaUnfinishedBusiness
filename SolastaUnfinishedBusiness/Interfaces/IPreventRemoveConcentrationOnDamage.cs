namespace SolastaUnfinishedBusiness.Interfaces;

public interface IPreventRemoveConcentrationOnDamage
{
    public SpellDefinition[] SpellsThatShouldNotRollConcentrationCheckFromDamage(RulesetCharacter rulesetCharacter);
}
