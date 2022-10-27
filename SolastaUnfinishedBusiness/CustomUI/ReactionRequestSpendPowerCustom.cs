namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestSpendPowerCustom : ReactionRequestSpendPower
{
    public ReactionRequestSpendPowerCustom(CharacterActionParams reactionParams, string name = "SpendPower") : base(
        reactionParams, name)
    {
    }

    public override string FormatDescription()
    {
        if (string.IsNullOrEmpty(reactionParams.stringParameter2))
        {
            return base.FormatDescription();
        }

        return reactionParams.stringParameter2;
    }
}
