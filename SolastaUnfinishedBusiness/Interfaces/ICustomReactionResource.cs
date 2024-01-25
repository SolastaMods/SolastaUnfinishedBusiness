using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.Interfaces;

public interface ICustomReactionResource
{
    AssetReferenceSprite Icon { get; }
    string GetUses(RulesetCharacter rulesetCharacter);
}
