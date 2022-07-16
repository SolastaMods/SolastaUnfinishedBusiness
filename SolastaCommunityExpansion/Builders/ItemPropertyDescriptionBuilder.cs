using System;
using SolastaCommunityExpansion.Api;

namespace SolastaCommunityExpansion.Builders;

public class ItemPropertyDescriptionBuilder
{
    private readonly ItemPropertyDescription property;

    public ItemPropertyDescriptionBuilder()
    {
        property = new ItemPropertyDescription(DatabaseHelper.ItemDefinitions.GreataxePlus1.StaticProperties[0])
        {
            conditionDefinition = null,
            featureDefinition = null,
            type = ItemPropertyDescription.PropertyType.Feature,
            appliesOnItemOnly = true,
            knowledgeAffinity = EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden
        };
    }

    public static ItemPropertyDescriptionBuilder From(FeatureDefinition feature, bool appliesOnItemOnly = true,
        EquipmentDefinitions.KnowledgeAffinity knowledgeAffinity =
            EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden)
    {
        return new ItemPropertyDescriptionBuilder()
            .SetFeatureDefinition(feature)
            .SetAppliesOnItemOnly(appliesOnItemOnly)
            .SetKnowledgeAffinity(knowledgeAffinity);
    }

    public static ItemPropertyDescriptionBuilder From(ConditionDefinition conditione, bool appliesOnItemOnly = true,
        EquipmentDefinitions.KnowledgeAffinity knowledgeAffinity =
            EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden)
    {
        return new ItemPropertyDescriptionBuilder()
            .SetConditionDefinition(conditione)
            .SetAppliesOnItemOnly(appliesOnItemOnly)
            .SetKnowledgeAffinity(knowledgeAffinity);
    }

    public ItemPropertyDescriptionBuilder SetConditionDefinition(ConditionDefinition condition)
    {
        property.type = ItemPropertyDescription.PropertyType.Condition;
        property.conditionDefinition = condition;
        return this;
    }

    public ItemPropertyDescriptionBuilder SetFeatureDefinition(FeatureDefinition feature)
    {
        property.type = ItemPropertyDescription.PropertyType.Feature;
        property.featureDefinition = feature;
        return this;
    }

    public ItemPropertyDescriptionBuilder SetAppliesOnItemOnly(bool value)
    {
        property.appliesOnItemOnly = value;
        return this;
    }

    public ItemPropertyDescriptionBuilder SetKnowledgeAffinity(EquipmentDefinitions.KnowledgeAffinity value)
    {
        property.knowledgeAffinity = value;
        return this;
    }

    private void Validate()
    {
        switch (property.Type)
        {
            case ItemPropertyDescription.PropertyType.Feature when property.FeatureDefinition == null:
                throw new ArgumentException("ItemPropertyDescriptionBuilder empty FeatureDefinition!");
            case ItemPropertyDescription.PropertyType.Condition when property.ConditionDefinition == null:
                throw new ArgumentException("ItemPropertyDescriptionBuilder empty ConditionDefinition!");
        }
    }

    public ItemPropertyDescription Build()
    {
        Validate();
        return property;
    }
}
