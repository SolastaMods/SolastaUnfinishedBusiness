using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourcePowerPool : ICustomReactionResource
{
    private readonly FeatureDefinitionPower _pool;

    public ReactionResourcePowerPool(FeatureDefinitionPower pool, AssetReferenceSprite icon)
    {
        _pool = pool;
        Icon = icon;
    }

    public AssetReferenceSprite Icon { get; }

    public string GetUses(RulesetCharacter character)
    {
        return character.GetRemainingPowerCharges(_pool).ToString();
    }
}
