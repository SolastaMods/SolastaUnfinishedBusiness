namespace SolastaMulticlass.Models
{
    internal static class WildshapeContext
    {
        internal static RulesetCharacter GetHero(RulesetCharacter rulesetCharacter)
        {
            if (rulesetCharacter is RulesetCharacterHero hero)
            {
                return hero;
            }
            else if (rulesetCharacter is RulesetCharacterMonster monster && monster.IsSubstitute)
            {
                return monster.OriginalFormCharacter;
            }
            else
            {
                return rulesetCharacter;
            }
        }
    }
}
