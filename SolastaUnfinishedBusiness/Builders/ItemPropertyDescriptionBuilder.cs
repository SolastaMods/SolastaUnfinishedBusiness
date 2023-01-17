using System;
using SolastaUnfinishedBusiness.Api;
using static EquipmentDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

internal class ItemPropertyDescriptionBuilder
{
    private readonly ItemPropertyDescription property;

    private ItemPropertyDescriptionBuilder()
    {
        property = new ItemPropertyDescription(DatabaseHelper.ItemDefinitions.GreataxePlus1.StaticProperties[0])
        {
            conditionDefinition = null,
            featureDefinition = null,
            type = ItemPropertyDescription.PropertyType.Feature,
            appliesOnItemOnly = true,
            knowledgeAffinity = KnowledgeAffinity.ActiveAndHidden
        };
    }

    internal static ItemPropertyDescriptionBuilder From(
        FeatureDefinition feature,
        bool appliesOnItemOnly = true,
        KnowledgeAffinity knowledgeAffinity = KnowledgeAffinity.ActiveAndHidden)
    {
        return new ItemPropertyDescriptionBuilder()
            .SetFeatureDefinition(feature)
            .SetAppliesOnItemOnly(appliesOnItemOnly)
            .SetKnowledgeAffinity(knowledgeAffinity);
    }

#if false
    internal ItemPropertyDescriptionBuilder SetConditionDefinition(ConditionDefinition condition)
    {
        property.type = ItemPropertyDescription.PropertyType.Condition;
        property.conditionDefinition = condition;
        return this;
    }
#endif

    private ItemPropertyDescriptionBuilder SetFeatureDefinition(FeatureDefinition feature)
    {
        property.type = ItemPropertyDescription.PropertyType.Feature;
        property.featureDefinition = feature;
        return this;
    }

    private ItemPropertyDescriptionBuilder SetAppliesOnItemOnly(bool value)
    {
        property.appliesOnItemOnly = value;
        return this;
    }

    private ItemPropertyDescriptionBuilder SetKnowledgeAffinity(KnowledgeAffinity value)
    {
        property.knowledgeAffinity = value;
        return this;
    }

    private void Validate()
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (property.Type)
        {
            case ItemPropertyDescription.PropertyType.Feature when property.FeatureDefinition == null:
                throw new ArgumentException("ItemPropertyDescriptionBuilder empty FeatureDefinition!");
            case ItemPropertyDescription.PropertyType.Condition when property.ConditionDefinition == null:
                throw new ArgumentException("ItemPropertyDescriptionBuilder empty ConditionDefinition!");
        }
    }

    internal ItemPropertyDescription Build()
    {
        Validate();
        return property;
    }
}
