using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.BehaviorsGeneric;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class PortraitPointCoordinatedAssault : ICustomPortraitPointPoolProvider
{
    public static ICustomPortraitPointPoolProvider Instance { get; } = new PortraitPointCoordinatedAssault();
    public string Name => "CoordinatedAssault";

    string ICustomPortraitPointPoolProvider.Tooltip(RulesetCharacter character)
    {
        var usablePower = PowerProvider.Get(MartialWarlord.PowerCoordinatedAssault, character);
        var max = character.GetMaxUsesOfPower(usablePower);
        var remaining = character.GetRemainingUsesOfPower(usablePower);

        return "CoordinatedAssaultPortraitPoolFormat".Formatted(Category.Tooltip, remaining, max);
    }

    public AssetReferenceSprite Icon => Sprites.EldritchVersatilityResourceIcon;

    public string GetPoints(RulesetCharacter character)
    {
        var usablePower = PowerProvider.Get(MartialWarlord.PowerCoordinatedAssault, character);
        var remaining = character.GetRemainingUsesOfPower(usablePower);

        return $"{remaining}";
    }
}
