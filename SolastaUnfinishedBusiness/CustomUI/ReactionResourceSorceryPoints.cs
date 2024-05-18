using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourceSorceryPoints : ICustomReactionResource, ICustomReactionCustomResourceUse
{
    private ReactionResourceSorceryPoints()
    {
    }

    public static ICustomReactionResource Instance { get; } = new ReactionResourceSorceryPoints();

    public string GetRequestPoints(CharacterReactionItem item)
    {
        return item.ReactionRequest.ReactionParams.StringParameter2 == string.Empty
            ? "1"
            : item.ReactionRequest.ReactionParams.StringParameter2;
    }

    public AssetReferenceSprite Icon => Sprites.SorceryPointsResourceIcon;

    public string GetUses(RulesetCharacter rulesetCharacter)
    {
        return rulesetCharacter.RemainingSorceryPoints.ToString();
    }
}
