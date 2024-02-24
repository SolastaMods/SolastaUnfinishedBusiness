using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class PortraitPointForcePoints : ICustomPortraitPointPoolProvider
{
    public static ICustomPortraitPointPoolProvider Instance { get; } = new PortraitPointForcePoints();
    public string Name => "ForcePoints";

    string ICustomPortraitPointPoolProvider.Tooltip(RulesetCharacter character)
    {
        var usablePower = PowerProvider.Get(MartialForceKnight.PowerPsionicInitiate, character);
        var max = character.GetMaxUsesOfPower(usablePower);
        var remaining = character.GetRemainingUsesOfPower(usablePower);

        return $"{Name}PortraitPoolFormat".Formatted(Category.Tooltip, remaining, max);
    }

    public AssetReferenceSprite Icon => Sprites.ForcePointsResourceIcon;

    public string GetPoints(RulesetCharacter character)
    {
        var usablePower = PowerProvider.Get(MartialForceKnight.PowerPsionicInitiate, character);
        var remaining = character.GetRemainingUsesOfPower(usablePower);

        return $"{remaining}";
    }
}
