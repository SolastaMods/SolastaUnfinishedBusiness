namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    /// <summary>
    /// Implement on a FeatureDefinition to be able to set the minimum for an ability check total - that is, the total of the d20 + all modifiers. (Currently only supports Strength checks.)
    /// </summary>
    public interface IMinimumAbilityCheckTotal
    {
        int? MinimumStrengthAbilityCheckTotal(RulesetCharacter character, string proficiencyName);
    }
}
