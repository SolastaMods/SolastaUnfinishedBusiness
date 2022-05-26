using SolastaCommunityExpansion.CustomInterfaces;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public delegate bool IsActiveConditionalPowerDelegate(RulesetCharacterHero character);

    public class FeatureDefinitionConditionalPower : FeatureDefinitionPower, IConditionalPower
    {
        private IsActiveConditionalPowerDelegate isActive;

        public bool IsActive(RulesetCharacterHero character)
        {
            return isActive == null || isActive(character);
        }

        internal void SetIsActiveDelegate(IsActiveConditionalPowerDelegate del)
        {
            isActive = del;
        }
    }
}
