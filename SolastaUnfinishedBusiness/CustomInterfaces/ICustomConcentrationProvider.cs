using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICustomConcentrationProvider
{
    string Name { get; }
    string Tooltip { get; }
    AssetReferenceSprite Icon { get; }
    void Stop(RulesetCharacter character);
}
