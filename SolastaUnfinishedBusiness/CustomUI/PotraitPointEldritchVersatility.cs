using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.CustomBuilders;
using SolastaUnfinishedBusiness.CustomInterfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

class PotraitPointEldritchVersatility : ICustomPortraitPointPoolProvider
{

    public static ICustomPortraitPointPoolProvider Instance { get; } = new PotraitPointEldritchVersatility();
    public string Name => "EldritchVersatility";

    string ICustomPortraitPointPoolProvider.Tooltip(RulesetCharacter character)
    {
        var currentPoints = 0;
        var reserverPoints = 0;
        var maxPoints = 0;
        if (character.GetVersatilitySupportCondition(out var supportCondition))
        {
            currentPoints = supportCondition.CurrentPoints;
            reserverPoints = supportCondition.ReservedPoints;
            maxPoints = supportCondition.MaxPoints;
        }
        return "EldritchVersatilityPortraitPoolFormat".Formatted(Category.Tooltip, currentPoints, reserverPoints, maxPoints);
    }

    public AssetReferenceSprite Icon => Sprites.EldritchVersatilityResourceIcon;

    public string GetPoints(RulesetCharacter character)
    {
        var currentPoints = 0;
        if (character.GetVersatilitySupportCondition(out var supportCondition))
        {
            currentPoints = supportCondition.CurrentPoints;
        }
        return $"{currentPoints}";
    }
}
