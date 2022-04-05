namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface ICustomFightingStyle
    {
        bool IsActive(RulesetCharacterHero character);
    }
}
