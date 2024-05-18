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
        var metamagicOptionDefinition = item.ReactionRequest.ReactionParams.activeEffect.MetamagicOption;

        return metamagicOptionDefinition?.SorceryPointsCost.ToString() ?? "1";
    }

    public AssetReferenceSprite Icon => Sprites.SorceryPointsResourceIcon;

    public string GetUses(RulesetCharacter rulesetCharacter)
    {
        return rulesetCharacter.RemainingSorceryPoints.ToString();
    }
}
