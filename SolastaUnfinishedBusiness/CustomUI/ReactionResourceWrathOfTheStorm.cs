using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine.AddressableAssets;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionResourceWrathOfTheStorm : ICustomReactionResource
{
    private ReactionResourceWrathOfTheStorm()
    {
    }

    public static ICustomReactionResource Instance { get; } = new ReactionResourceWrathOfTheStorm();
    public AssetReferenceSprite Icon => DatabaseHelper.ActionDefinitions.ActionSurge.GuiPresentation.SpriteReference;

    public string GetUses(RulesetCharacter character)
    {
        var usablePower = PowerProvider.Get(DomainTempest.PowerWrathOfTheStorm, character);

        return character.GetRemainingUsesOfPower(usablePower).ToString();
    }
}
