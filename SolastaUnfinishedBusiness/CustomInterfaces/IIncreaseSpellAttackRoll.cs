namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIncreaseSpellAttackRoll
{
    RuleDefinitions.FeatureSourceType SourceType { get; }
    string SourceName { get; set; }
    int GetSpellAttackRollModifier(RulesetCharacter caster);
}
