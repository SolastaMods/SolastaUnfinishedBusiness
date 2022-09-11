using System;
using static FeatureDefinitionAttributeModifier;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionAttributeModifierBuilder : FeatureDefinitionBuilder<FeatureDefinitionAttributeModifier
    , FeatureDefinitionAttributeModifierBuilder>
{
    public FeatureDefinitionAttributeModifierBuilder SetModifier(AttributeModifierOperation modifierType,
        string attribute, int amount)
    {
        Definition.modifierOperation = modifierType;
        Definition.modifiedAttribute = attribute;
        Definition.modifierValue = amount;
        return this;
    }

    public FeatureDefinitionAttributeModifierBuilder SetModifierAbilityScore(string abilityScore, bool minimum1 = false)
    {
        Definition.modifierAbilityScore = abilityScore;
        Definition.modifierOperation = AttributeModifierOperation.AddAbilityScoreBonus;
        Definition.minimum1 = minimum1;
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
