namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestSpendPowerCustom : ReactionRequestSpendPower
{
    public ReactionRequestSpendPowerCustom(CharacterActionParams reactionParams, string name = "SpendPower")
        : base(reactionParams, name)
    {
    }

    public override string FormatDescription()
    {
        return string.IsNullOrEmpty(reactionParams.stringParameter2)
            ? base.FormatDescription()
            : reactionParams.stringParameter2;
    }
}
