using SolastaUnfinishedBusiness.Interfaces;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourceBardicInspiration : ICustomReactionResource
{
    private ReactionResourceBardicInspiration()
    {
    }

    public static ICustomReactionResource Instance { get; } = new ReactionResourceBardicInspiration();
    public AssetReferenceSprite Icon => Sprites.BardicDiceResourceIcon;

    public string GetUses(RulesetCharacter character)
    {
        return character.RemainingBardicInspirations.ToString();
    }
}
