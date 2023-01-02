namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface IIncreaseSpellAttackRoll
{
    RuleDefinitions.FeatureSourceType SourceType { get; }
    string SourceName { get; }
    int GetSpellAttackRollModifier(RulesetCharacter caster);
}
