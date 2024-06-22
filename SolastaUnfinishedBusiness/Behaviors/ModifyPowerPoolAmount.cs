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
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        // ReSharper disable once ConvertSwitchStatementToSwitchExpression
        switch (Type)
        {
            case PowerPoolBonusCalculationType.Fixed:
                return Value;
            case PowerPoolBonusCalculationType.CharacterLevel:
                return Value * character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            case PowerPoolBonusCalculationType.ClassLevel:
                return Value * character.GetClassLevel(Attribute);
            case PowerPoolBonusCalculationType.Attribute:
                return Value * character.TryGetAttributeValue(Attribute);
            case PowerPoolBonusCalculationType.AttributeModifier:
                return
                    Value * AttributeDefinitions.ComputeAbilityScoreModifier(character.TryGetAttributeValue(Attribute));
            case PowerPoolBonusCalculationType.ConditionAmount:
                return Value * (character.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, Attribute, out var activeCondition)
                    ? activeCondition.Amount
                    : 0);
        }

        return Value;
    }
}

internal class HasModifiedUses
{
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
