using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomConcentrationProvider
{
    public string Name { get; }
    public string Tooltip { get; }
    public AssetReferenceSprite Icon { get; }
    public void Stop(RulesetCharacter character);
}
