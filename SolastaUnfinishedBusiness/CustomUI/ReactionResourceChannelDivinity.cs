using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourceChannelDivinity : ICustomReactionResource
{
    public static ReactionResourceChannelDivinity Instance { get; } = new();

    private ReactionResourceChannelDivinity()
    {
    }

    public AssetReferenceSprite Icon => Sprites.ChannelDivinityResourceIcon;

    public string GetUses(RulesetCharacter character)
    {
        var max = character.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);
        var used = character.UsedChannelDivinity;
        return $"{max - used}/{max}";
    }
}
