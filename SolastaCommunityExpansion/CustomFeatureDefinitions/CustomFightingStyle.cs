using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using System.Collections.Generic;

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

    public class CustomizableFightingStyleBuilder : BaseDefinitionBuilder<CustomizableFightingStyle>
    {
        public CustomizableFightingStyleBuilder(string name, string guid,
            List<FeatureDefinition> features, GuiPresentation guiPresentation) : base(name, guid)
        {
            // The features field is not automatically initialized.
            Definition.SetField("features", new List<FeatureDefinition>(features));
            Definition.SetGuiPresentation(guiPresentation);
        }

        public CustomizableFightingStyleBuilder SetIsActive(IsActiveFightingStyleDelegate del)
        {
            Definition.SetIsActiveDelegate(del);
            return this;
        }
    }
}
