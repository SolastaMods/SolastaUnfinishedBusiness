using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomPortraitPointPoolProvider
{
    public string Name { get; }
    public string Tooltip(RulesetCharacter character);
    public AssetReferenceSprite Icon { get; }
    public string GetPoints(RulesetCharacter character);
}
