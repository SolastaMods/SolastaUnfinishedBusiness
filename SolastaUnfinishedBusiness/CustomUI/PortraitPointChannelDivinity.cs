using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class PortraitPointChannelDivinity : ICustomPortraitPointPoolProvider
{
    private PortraitPointChannelDivinity()
    {
    }

    public static ICustomPortraitPointPoolProvider Instance { get; } = new PortraitPointChannelDivinity();
    public string Name => "ChannelDivinity";

    string ICustomPortraitPointPoolProvider.Tooltip(RulesetCharacter character)
    {
        var max = character.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);
        var used = character.UsedChannelDivinity;
        return "ChannelDivinityPortraitPoolFormat".Formatted(Category.Tooltip, max - used, max);
    }

    public AssetReferenceSprite Icon => Sprites.ChannelDivinityResourceIcon;

    public string GetPoints(RulesetCharacter character)
    {
        var max = character.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);
        var used = character.UsedChannelDivinity;
        return $"{max - used}";
    }
}
