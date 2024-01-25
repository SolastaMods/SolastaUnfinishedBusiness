using SolastaUnfinishedBusiness.CustomGenericBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatureDefinitionPowerUseModifier : FeatureDefinition, IModifyPowerPoolAmount
{
    internal ModifyPowerPoolAmount Modifier { get; } = new();

    public FeatureDefinitionPower PowerPool => Modifier.PowerPool;

    public int PoolChangeAmount(RulesetCharacter character)
    {
        return Modifier.PoolChangeAmount(character);
    }
}
