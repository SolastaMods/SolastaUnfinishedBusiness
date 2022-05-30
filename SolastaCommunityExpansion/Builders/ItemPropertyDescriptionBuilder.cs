using System;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Builders;

public class ItemPropertyDescriptionBuilder
{
    private readonly ItemPropertyDescription property;

    public ItemPropertyDescriptionBuilder()
    {
        property = new ItemPropertyDescription(DatabaseHelper.ItemDefinitions.GreataxePlus1.StaticProperties[0]);
        property.SetConditionDefinition(null);
        property.SetFeatureDefinition(null);
        property.SetType(ItemPropertyDescription.PropertyType.Feature);
        property.SetAppliesOnItemOnly(true);
        property.SetKnowledgeAffinity(EquipmentDefinitions.KnowledgeAffinity.ActiveAndHidden);
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
        property.SetType(ItemPropertyDescription.PropertyType.Condition);
        property.SetConditionDefinition(condition);
        return this;
    }

    public ItemPropertyDescriptionBuilder SetFeatureDefinition(FeatureDefinition feature)
    {
        property.SetType(ItemPropertyDescription.PropertyType.Feature);
        property.SetFeatureDefinition(feature);
        return this;
    }

    public ItemPropertyDescriptionBuilder SetAppliesOnItemOnly(bool value)
    {
        property.SetAppliesOnItemOnly(value);
        return this;
    }

    public ItemPropertyDescriptionBuilder SetKnowledgeAffinity(EquipmentDefinitions.KnowledgeAffinity value)
    {
        property.SetKnowledgeAffinity(value);
        return this;
    }

    private void Validate()
    {
        if (property.Type == ItemPropertyDescription.PropertyType.Feature && property.FeatureDefinition == null)
        {
            throw new ArgumentException("ItemPropertyDescriptionBuilder empty FeatureDefinition!");
        }

        if (property.Type == ItemPropertyDescription.PropertyType.Condition && property.ConditionDefinition == null)
        {
            throw new ArgumentException("ItemPropertyDescriptionBuilder empty ConditionDefinition!");
        }
    }

    public ItemPropertyDescription Build()
    {
        Validate();
        return property;
    }
}
