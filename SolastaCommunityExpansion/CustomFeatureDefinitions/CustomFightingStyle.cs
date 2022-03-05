using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public delegate bool IsActiveFightingStyleDelegate(RulesetCharacterHero character);

    public class CustomizableFightingStyleDefinition : FightingStyleDefinition, ICustomFightingStyle
    {
        private IsActiveFightingStyleDelegate isActive;

        internal void SetIsActiveDelegate(IsActiveFightingStyleDelegate del)
        {
            isActive = del;
        }

        public bool IsActive(RulesetCharacterHero character)
        {
            return isActive == null || isActive(character);
        }
    }

    public sealed class CustomizableFightingStyleBuilder : FightingStyleDefinitionBuilder<CustomizableFightingStyleDefinition, CustomizableFightingStyleBuilder>
    {
        private CustomizableFightingStyleBuilder(string name, string guid) : base(name, guid)
        {
        }

        public static CustomizableFightingStyleBuilder Create(string name, string guid)
        {
            return new CustomizableFightingStyleBuilder(name, guid);
        }

        public CustomizableFightingStyleBuilder SetIsActive(IsActiveFightingStyleDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }
    }
}
