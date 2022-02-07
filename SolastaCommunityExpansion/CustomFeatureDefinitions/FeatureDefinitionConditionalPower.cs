namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface IConditionalPower
    {
        bool IsActive(RulesetCharacterHero character);
    }

    public delegate bool IsActiveConditionalPowerDelegate(RulesetCharacterHero character);

    public class FeatureDefinitionConditionalPower : FeatureDefinitionPower, IConditionalPower
    {
        private IsActiveConditionalPowerDelegate isActive;

        internal void SetIsActiveDelegate(IsActiveConditionalPowerDelegate del)
        {
            isActive = del;
        }

        public bool IsActive(RulesetCharacterHero character)
        {
            return isActive == null || isActive(character);
        }
    }
}
