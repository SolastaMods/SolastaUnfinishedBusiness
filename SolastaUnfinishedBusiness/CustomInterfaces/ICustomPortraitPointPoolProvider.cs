using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

internal interface ICustomPortraitPointPoolProvider
{
    public string Name { get; }
    public string Tooltip { get; }
    public AssetReferenceSprite Icon { get; }
    public int GetPoints(RulesetCharacter character);
}
