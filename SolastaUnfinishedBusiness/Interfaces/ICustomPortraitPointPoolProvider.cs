using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ICustomPortraitPointPoolProvider
{
    public string Name { get; }
    public AssetReferenceSprite Icon { get; }
    public string Tooltip(RulesetCharacter character);
    public string GetPoints(RulesetCharacter character);
}
