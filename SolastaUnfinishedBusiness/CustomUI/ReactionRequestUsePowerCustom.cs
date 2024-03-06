namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestUsePowerCustom(CharacterActionParams reactionParams, string name = "UsePower")
    : ReactionRequestSpendPower(reactionParams, name)
{
    public override string FormatDescription()
    {
        return string.IsNullOrEmpty(reactionParams.stringParameter2)
            ? base.FormatDescription()
            : reactionParams.stringParameter2;
    }
}
