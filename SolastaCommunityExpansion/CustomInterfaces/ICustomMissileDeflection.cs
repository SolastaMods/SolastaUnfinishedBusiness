namespace SolastaCommunityExpansion.CustomInterfaces 
{
    public interface ICustomMissileDeflection
    {
        public int GetDamageReduction(RulesetCharacter target, RulesetCharacter attacker);
        public string FormatDescription(RulesetCharacter target, RulesetCharacter attacker, string def);
    }
}

