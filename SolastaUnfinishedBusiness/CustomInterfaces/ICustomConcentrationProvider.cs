using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICustomConcentrationProvider
{
    public string Name { get; }
    public string Tooltip { get; }
    public AssetReferenceSprite Icon { get; }
    public void Stop(RulesetCharacter character);
}
