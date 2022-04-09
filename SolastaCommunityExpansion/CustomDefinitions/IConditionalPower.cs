namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface IConditionalPower
    {
        bool IsActive(RulesetCharacterHero character);
    }
}
