using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.Behaviors;

public class ModifyPowerPoolAmount : IModifyPowerPoolAmount
{
    public int Value { get; set; } = 1;
    public PowerPoolBonusCalculationType Type { get; set; } = PowerPoolBonusCalculationType.Fixed;
    public string Attribute { get; set; }
    public FeatureDefinitionPower PowerPool { get; set; }

    public int PoolChangeAmount(RulesetCharacter character)
    {
        return Type switch
        {
            PowerPoolBonusCalculationType.Fixed => Value,
            PowerPoolBonusCalculationType.CharacterLevel =>
                Value * character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel),
            PowerPoolBonusCalculationType.ClassLevel =>
                Value * character.GetClassLevel(Attribute),
            PowerPoolBonusCalculationType.Attribute =>
                Value * character.TryGetAttributeValue(Attribute),
            PowerPoolBonusCalculationType.AttributeModifier =>
                Value * AttributeDefinitions.ComputeAbilityScoreModifier(character.TryGetAttributeValue(Attribute)),
            PowerPoolBonusCalculationType.ConditionAmount =>
                Value * (character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, Attribute, out var activeCondition)
                    ? activeCondition.Amount
                    : 0),
            _ => Value
        };
    }
}

// required for short/long rest integration
internal class HasModifiedUses
{
    private HasModifiedUses()
    {
    }

    public static HasModifiedUses Marker { get; } = new();
}

public enum PowerPoolBonusCalculationType
{
    Fixed,
    CharacterLevel,
    ClassLevel,
    Attribute,
    AttributeModifier,
    ConditionAmount
}
