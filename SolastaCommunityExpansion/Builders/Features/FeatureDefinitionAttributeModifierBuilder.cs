using System;
using SolastaModApi.Extensions;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionAttributeModifierBuilder : FeatureDefinitionBuilder<FeatureDefinitionAttributeModifier
    , FeatureDefinitionAttributeModifierBuilder>
{
    public FeatureDefinitionAttributeModifierBuilder SetModifier(AttributeModifierOperation modifierType,
        string attribute, int amount)
    {
        Definition.modifierType2 = modifierType;
        Definition.modifiedAttribute = attribute;
        Definition.modifierValue = amount;
        return this;
    }

    public FeatureDefinitionAttributeModifierBuilder SetModifierAbilityScore(string abilityScore)
    {
        Definition.modifierAbilityScore = abilityScore;
        return this;
    }

    public FeatureDefinitionAttributeModifierBuilder SetModifierValue(int amount)
    {
        Definition.modifierValue = amount;
        return this;
    }

    public FeatureDefinitionAttributeModifierBuilder SetModifiedAttribute(string attribute)
    {
        Definition.modifiedAttribute = attribute;
        return this;
    }

    public FeatureDefinitionAttributeModifierBuilder SetModifierType2(AttributeModifierOperation modifierType)
    {
        Definition.modifierType2 = modifierType;
        return this;
    }

    public FeatureDefinitionAttributeModifierBuilder SetSituationalContext(
        RuleDefinitions.SituationalContext situationalContext)
    {
        Definition.situationalContext = situationalContext;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionAttributeModifierBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAttributeModifierBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }


    protected FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAttributeModifierBuilder(FeatureDefinitionAttributeModifier original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
