namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IIncreaseSpellDc
{
    int GetSpellModifier(RulesetCharacter caster);
}

internal interface IIncreaseSpellAttackRoll
{
    RuleDefinitions.FeatureSourceType SourceType { get; set; }
    string SourceName { get; set; }
    int GetSpellAttackRollModifier(RulesetCharacter caster);
}
