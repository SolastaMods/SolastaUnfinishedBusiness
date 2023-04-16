using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomBehaviors;

internal sealed class AddConditionAmountToSpeedModifier : IModifyMovementSpeedAddition
{
    public int ModifySpeedAddition(RulesetCharacter character, IMovementAffinityProvider provider)
    {
        return character.FindFirstConditionHoldingFeature(provider as FeatureDefinition)?.Amount ?? 0;
    }
}
