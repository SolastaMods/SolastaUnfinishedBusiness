using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourceSorceryPoints : ICustomReactionResource, ICustomReactionCustomResourceUse
{
    private ReactionResourceSorceryPoints()
    {
    }

    public static ICustomReactionResource Instance { get; } = new ReactionResourceSorceryPoints();
    public AssetReferenceSprite Icon => Sprites.SorceryPointsResourceIcon;

    public string GetUses(RulesetCharacter character)
    {
        return character.RemainingSorceryPoints.ToString();
    }

    public string GetRequestPoints(RulesetCharacter rulesetCharacter)
    {
        return "2";
    }
}
