using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

internal sealed class StopPowerConcentrationProvider : CustomConcentrationControl.ICustomConcentrationProvider
{
    internal FeatureDefinitionPower StopPower;

    internal StopPowerConcentrationProvider(string name, string tooltip, AssetReferenceSprite icon)
    {
        Name = name;
        Tooltip = tooltip;
        Icon = icon;
    }

    public string Name { get; }
    public string Tooltip { get; }
    public AssetReferenceSprite Icon { get; }

    public void Stop(RulesetCharacter character)
    {
        if (!StopPower)
        {
            return;
        }

        var locationCharacter = GameLocationCharacter.GetFromActor(character);
        var usablePower = PowerProvider.Get(StopPower, character);

        locationCharacter.MyExecuteActionSpendPower(usablePower, locationCharacter);
    }
}
