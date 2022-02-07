using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Builders.Features
{
    public sealed class FeatureDefinitionAttributeModifierBuilder : BaseDefinitionBuilder<FeatureDefinitionAttributeModifier>
    {
/*        private FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, string guid)
            : base(original, name, guid)
        {
        }
*/
        private FeatureDefinitionAttributeModifierBuilder(string name, string guid)
            : base(name, guid)
        {
        }

        private FeatureDefinitionAttributeModifierBuilder(string name, Guid namespaceGuid, Category category = Category.None)
            : base(name, namespaceGuid, category)
        {
        }

        private FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name, Guid namespaceGuid, Category category = Category.None)
            : base(original, name, namespaceGuid, category)
        {
        }

        // Add other standard Create methods and constructors as required.

        public static FeatureDefinitionAttributeModifierBuilder Create(string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAttributeModifierBuilder(name, namespaceGuid);
        }

        public static FeatureDefinitionAttributeModifierBuilder Create(string name, string guid)
        {
            return new FeatureDefinitionAttributeModifierBuilder(name, guid);
        }

        public static FeatureDefinitionAttributeModifierBuilder Create(FeatureDefinitionAttributeModifier original, string name, Guid namespaceGuid)
        {
            return new FeatureDefinitionAttributeModifierBuilder(original, name, namespaceGuid);
        }

        public FeatureDefinitionAttributeModifierBuilder SetModifier(AttributeModifierOperation modifierType, string attribute, int amount)
        {
            Definition.SetModifierType2(modifierType);
            Definition.SetModifiedAttribute(attribute);
            Definition.SetModifierValue(amount);
            return this;
        }

        public FeatureDefinitionAttributeModifierBuilder SetModifierAbilityScore(string abilityScore)
        {
            Definition.SetModifierAbilityScore(abilityScore);
            return this;
        }
    }
}
