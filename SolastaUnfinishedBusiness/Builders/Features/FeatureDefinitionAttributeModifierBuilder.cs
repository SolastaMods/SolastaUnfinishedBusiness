using System;
using JetBrains.Annotations;
using static FeatureDefinitionAttributeModifier;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAttributeModifierBuilder : FeatureDefinitionBuilder<FeatureDefinitionAttributeModifier,
    FeatureDefinitionAttributeModifierBuilder>
{
    internal FeatureDefinitionAttributeModifierBuilder SetModifier(
        AttributeModifierOperation modifierType,
        string attribute,
        int amount)
    {
        Definition.modifierOperation = modifierType;
        Definition.modifiedAttribute = attribute;
        Definition.modifierValue = amount;
        return this;
    }

    internal FeatureDefinitionAttributeModifierBuilder SetModifierAbilityScore(
        string abilityScore,
        bool minimum1 = false)
    {
        Definition.modifierAbilityScore = abilityScore;
        Definition.modifierOperation = AttributeModifierOperation.AddAbilityScoreBonus;
        Definition.minimum1 = minimum1;
        return this;
    }

    internal FeatureDefinitionAttributeModifierBuilder SetModifiedAttribute(string attribute)
    {
        Definition.modifiedAttribute = attribute;
        return this;
    }

    internal FeatureDefinitionAttributeModifierBuilder SetSituationalContext(
        RuleDefinitions.SituationalContext situationalContext)
    {
        Definition.situationalContext = situationalContext;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionAttributeModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
