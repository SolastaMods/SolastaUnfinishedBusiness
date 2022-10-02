namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIncreaseSpellDc
{
    int GetSpellModifier(RulesetCharacter caster);
}

public interface IIncreaseSpellAttackRoll
{
    RuleDefinitions.FeatureSourceType SourceType { get; set; }
    string SourceName { get; set; }
    int GetSpellAttackRollModifier(RulesetCharacter caster);
}
