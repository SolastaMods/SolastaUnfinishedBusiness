using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomPortraitPointPoolProvider
{
    string Name { get; }
    string Tooltip { get; }
    AssetReferenceSprite Icon { get; }
    int GetPoints(RulesetCharacter character);
}
