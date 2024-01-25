using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourceChannelDivinity : ICustomReactionResource
{
    private ReactionResourceChannelDivinity()
    {
    }

    public static ReactionResourceChannelDivinity Instance { get; } = new();

    public AssetReferenceSprite Icon => Sprites.ChannelDivinityResourceIcon;

    public string GetUses(RulesetCharacter character)
    {
        var max = character.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);
        var used = character.UsedChannelDivinity;
        return $"{max - used}";
    }
}
