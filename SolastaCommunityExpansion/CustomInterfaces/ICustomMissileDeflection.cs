namespace SolastaCommunityExpansion.CustomInterfaces;

public interface ICustomMissileDeflection
{
    public int GetDamageReduction(RulesetCharacter target, RulesetCharacter attacker);
    string FormatDescription(RulesetCharacter target, RulesetCharacter attacker, string def);
}
