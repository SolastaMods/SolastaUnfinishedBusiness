using System;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    /**
     * Note this is based on FeatureDefinitionPower so that you can take advantage of power usage calculations
     * like proficiency or ability score usage. However in order to do that the game needs to add a power to
     * the hero and only one power for a given name+guid is added. Which means if you want to add a +1 modifier
     * at 4 different character levels you need to create 4 different FeatureDefinitionPowerPoolModifier.
     */
    public class FeatureDefinitionPowerPoolModifierBuilder : FeatureDefinitionPowerBuilder<FeatureDefinitionPowerPoolModifier, FeatureDefinitionPowerPoolModifierBuilder>
    {
        public FeatureDefinitionPowerPoolModifierBuilder(string name, string guid,
            int powerPoolModifier, RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName, FeatureDefinitionPower poolPower, GuiPresentation guiPresentation) : base(name, guid)
        {
            Configure(powerPoolModifier, usesDetermination, usesAbilityScoreName, poolPower);

            Definition.SetGuiPresentation(guiPresentation);
        }

        public FeatureDefinitionPowerPoolModifierBuilder Configure(
            int powerPoolModifier, RuleDefinitions.UsesDetermination usesDetermination,
            string usesAbilityScoreName, FeatureDefinitionPower poolPower)
        {
            Definition.SetFixedUsesPerRecharge(powerPoolModifier);
            Definition.SetUsesDetermination(usesDetermination);
            Definition.SetUsesAbilityScoreName(usesAbilityScoreName);
            // This is just an activation time that should not be shown in the UI.
            Definition.SetActivationTime(RuleDefinitions.ActivationTime.Permanent);
            // Math for usage gets weird if this isn't 1.
            Definition.SetCostPerUse(1);
            // The game throws an exception if there is no effect description.
            Definition.SetEffectDescription(new EffectDescription());
            Definition.SetOverriddenPower(Definition);

            Definition.PoolPower = poolPower;

            return This();
        }

        #region Constructors
        public FeatureDefinitionPowerPoolModifierBuilder(FeatureDefinitionPowerPoolModifier original) : base(original)
        {
        }

        public FeatureDefinitionPowerPoolModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionPowerPoolModifierBuilder(string name, string definitionGuid) : base(name, definitionGuid)
        {
        }

        public FeatureDefinitionPowerPoolModifierBuilder(string name, bool createGuiPresentation = true) : base(name, createGuiPresentation)
        {
        }

        public FeatureDefinitionPowerPoolModifierBuilder(FeatureDefinitionPowerPoolModifier original, string name, bool createGuiPresentation = true) : base(original, name, createGuiPresentation)
        {
        }

        public FeatureDefinitionPowerPoolModifierBuilder(FeatureDefinitionPowerPoolModifier original, string name, Guid namespaceGuid) : base(original, name, namespaceGuid)
        {
        }

        public FeatureDefinitionPowerPoolModifierBuilder(FeatureDefinitionPowerPoolModifier original, string name, string definitionGuid) : base(original, name, definitionGuid)
        {
        }
        #endregion
    }
}
