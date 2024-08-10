using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourceSorceryPoints : ICustomReactionResource, ICustomReactionCustomResourceUse
{
    private ReactionResourceSorceryPoints()
    {
    }

    public static ReactionResourceSorceryPoints Instance { get; } = new();

    public string GetRequestPoints(CharacterReactionItem item)
    {
        return item.ReactionRequest.ReactionParams.UsablePower?.PowerDefinition.CostPerUse.ToString() ?? "1";
    }

    public AssetReferenceSprite Icon => Sprites.SorceryPointsResourceIcon;

    public string GetUses(RulesetCharacter rulesetCharacter)
    {
        return rulesetCharacter.RemainingSorceryPoints.ToString();
    }
}
