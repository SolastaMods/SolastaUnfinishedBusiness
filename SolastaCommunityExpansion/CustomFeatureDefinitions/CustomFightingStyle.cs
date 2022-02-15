using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public interface ICustomFightingStyle
    {
        bool IsActive(RulesetCharacterHero character);
    }

    public delegate bool IsActiveFightingStyleDelegate(RulesetCharacterHero character);

    public class CustomizableFightingStyle : FightingStyleDefinition, ICustomFightingStyle
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

    public class CustomizableFightingStyleBuilder : DefinitionBuilder<CustomizableFightingStyle>
    {
        public CustomizableFightingStyleBuilder(string name, string guid,
            IEnumerable<FeatureDefinition> features, GuiPresentation guiPresentation) : base(name, guid)
        {
            Definition.Features.SetRange(features);
            Definition.SetGuiPresentation(guiPresentation);
        }

        public CustomizableFightingStyleBuilder SetIsActive(IsActiveFightingStyleDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }
    }
}
