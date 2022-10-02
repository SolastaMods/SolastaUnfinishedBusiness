namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface IIncreaseSpellDC
{
    int GetSpellModifier(RulesetCharacter caster);
}

internal interface IIncreaseSpellAttackRoll
{
    RuleDefinitions.FeatureSourceType sourceType { get; set; }
    string sourceName { get; set; }
    int GetSpellAttackRollModifier(RulesetCharacter caster);
}
