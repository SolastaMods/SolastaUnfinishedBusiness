namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface IConditionalPower
    {
        bool IsActive(RulesetCharacterHero character);
    }
}
