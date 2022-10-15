namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIncreaseSpellAttackRoll
{
    RuleDefinitions.FeatureSourceType SourceType { get; set; }
    string SourceName { get; set; }
    int GetSpellAttackRollModifier(RulesetCharacter caster);
}
