using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.CustomDefinitions
{
    public delegate bool IsActiveFightingStyleDelegate(RulesetCharacterHero character);

    public class FightingStyleDefinitionCustomizable : FightingStyleDefinition, ICustomFightingStyle
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

    public class CustomizableFightingStyleBuilder : FightingStyleDefinitionBuilder<FightingStyleDefinitionCustomizable, CustomizableFightingStyleBuilder>
    {
        protected CustomizableFightingStyleBuilder(string name, string guid) : base(name, guid)
        {
        }

        public CustomizableFightingStyleBuilder SetIsActive(IsActiveFightingStyleDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }
    }
}
