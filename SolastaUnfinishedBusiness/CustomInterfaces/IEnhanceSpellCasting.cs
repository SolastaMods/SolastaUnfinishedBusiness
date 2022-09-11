namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIncreaseSpellDC
{
    int GetSpellModifier(RulesetCharacter caster);
}

public interface IIncreaseSpellAttackRoll
{
    RuleDefinitions.FeatureSourceType sourceType { get; set; }
    string sourceName { get; set; }
    int GetSpellAttackRollModifier(RulesetCharacter caster);
}
