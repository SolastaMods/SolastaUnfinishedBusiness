namespace SolastaCommunityExpansion.CustomInterfaces;

public interface IIncreaseSpellDC
{
    int GetSpellModifier(RulesetCharacter caster);
}

public interface IIncreaseSpellAttackRoll
{
    int GetSpellAttackRollModifier(RulesetCharacter caster);
    RuleDefinitions.FeatureSourceType sourceType { get; set; }
    string sourceName { get; set; }
}


