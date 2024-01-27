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
        switch (Type)
        {
            case PowerPoolBonusCalculationType.Fixed:
                return Value;
            case PowerPoolBonusCalculationType.CharacterLevel:
                return Value * character.TryGetAttributeValue(AttributeDefinitions.CharacterLevel);
            case PowerPoolBonusCalculationType.ClassLevel:
                var classLevel = character.GetClassLevel(Attribute);
                return Value * classLevel;
            case PowerPoolBonusCalculationType.Attribute:
                return Value * character.TryGetAttributeValue(Attribute);
            case PowerPoolBonusCalculationType.AttributeMod:
                var attribute = character.TryGetAttributeValue(Attribute);
                return Value * AttributeDefinitions.ComputeAbilityScoreModifier(attribute);
        }

        return Value;
    }
}

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
    AttributeMod
}
