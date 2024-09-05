namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestSpendPowerCustom(CharacterActionParams reactionParams, string name = "SpendPower")
    : ReactionRequestSpendPower(reactionParams, name)
{
    public override string FormatDescription()
    {
        return string.IsNullOrEmpty(reactionParams.StringParameter2)
            ? base.FormatDescription()
            : reactionParams.StringParameter2;
    }
}
