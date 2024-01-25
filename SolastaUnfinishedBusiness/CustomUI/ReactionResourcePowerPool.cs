using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomSpecificBehaviors;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourcePowerPool(FeatureDefinitionPower pool, AssetReferenceSprite icon) : ICustomReactionResource
{
    public AssetReferenceSprite Icon { get; } = icon;

    public string GetUses(RulesetCharacter character)
    {
        return character.GetRemainingPowerCharges(pool).ToString();
    }
}
