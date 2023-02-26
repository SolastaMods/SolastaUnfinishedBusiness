using SolastaUnfinishedBusiness.CustomInterfaces;
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
        return Gui.Format("Tooltip/&ChannelDivinityPortraitPoolFormat", (max - used).ToString(), max.ToString());
    }

    public AssetReferenceSprite Icon => Sprites.ChannelDivinityResourceIcon;

    public string GetPoints(RulesetCharacter character)
    {
        var max = character.TryGetAttributeValue(AttributeDefinitions.ChannelDivinityNumber);
        var used = character.UsedChannelDivinity;
        return $"{max - used}";
    }
}
