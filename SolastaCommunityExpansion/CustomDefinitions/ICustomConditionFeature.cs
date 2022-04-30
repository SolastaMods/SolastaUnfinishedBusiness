namespace SolastaCommunityExpansion.CustomDefinitions
{
    public interface ICustomConditionFeature
    {
        public void ApplyFeature(RulesetCharacter hero);
        public void RemoveFeature(RulesetCharacter hero);
    }
}
