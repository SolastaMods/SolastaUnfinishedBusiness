using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomInterfaces;

public interface ICustomReactionResource
{
    AssetReferenceSprite Icon { get; }
    string GetUses(RulesetCharacter rulesetCharacter);

}
