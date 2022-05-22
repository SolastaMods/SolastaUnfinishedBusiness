namespace SolastaCommunityExpansion.CustomInterfaces
{
    public interface IConditionalPower
    {
        bool IsActive(RulesetCharacterHero character);
    }
}
