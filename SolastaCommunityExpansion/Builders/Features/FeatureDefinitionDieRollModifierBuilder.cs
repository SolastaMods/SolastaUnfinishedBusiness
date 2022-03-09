using System;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders.Features
{
    public class FeatureDefinitionDieRollModifierBuilder
        : FeatureDefinitionAffinityBuilder<FeatureDefinitionDieRollModifier, FeatureDefinitionDieRollModifierBuilder>
    {
        protected FeatureDefinitionDieRollModifierBuilder(string name, Guid namespaceGuid)
            : base(name, namespaceGuid)
        {
        }

        public FeatureDefinitionDieRollModifierBuilder SetModifiers(
            RuleDefinitions.RollContext context, int rerollCount, int minRerollValue, string consoleLocalizationKey)
        {
            Definition.SetValidityContext(context);
            Definition.SetRerollLocalizationKey(consoleLocalizationKey);
            Definition.SetRerollCount(rerollCount);
            Definition.SetMinRerollValue(minRerollValue);
            Definition.SetMinRollValue(minRerollValue);
            return this;
        }
    }
}
