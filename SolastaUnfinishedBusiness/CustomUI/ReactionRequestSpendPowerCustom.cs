namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestSpendPowerCustom(CharacterActionParams reactionParams, string name = "SpendPower")
    : ReactionRequestSpendPower(reactionParams, name)
{
    public override string FormatDescription()
    {
        return string.IsNullOrEmpty(reactionParams.stringParameter2)
            ? base.FormatDescription()
            : reactionParams.stringParameter2;
    }
}
