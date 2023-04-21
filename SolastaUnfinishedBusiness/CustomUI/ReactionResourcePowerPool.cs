using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourcePowerPool : ICustomReactionResource
{
    private readonly FeatureDefinitionPower pool;

    public ReactionResourcePowerPool(FeatureDefinitionPower pool, AssetReferenceSprite icon)
    {
        this.pool = pool;
        Icon = icon;
    }

    public AssetReferenceSprite Icon { get; }

    public string GetUses(RulesetCharacter character)
    {
        return character.GetRemainingPowerCharges(pool).ToString();
    }
}

public class ReactionResourcePower : ICustomReactionResource
{
    private readonly FeatureDefinitionPower power;

    public ReactionResourcePower(FeatureDefinitionPower pool, AssetReferenceSprite icon)
    {
        power = pool;
        Icon = icon;
    }

    public AssetReferenceSprite Icon { get; }

    public string GetUses(RulesetCharacter character)
    {
        return character.GetRemainingUsesOfPower(UsablePowersProvider.Get(power, character)).ToString();
    }
}
