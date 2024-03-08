namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestUsePowerCustom : ReactionRequestUsePower
{
    public ReactionRequestUsePowerCustom(
        CharacterActionParams reactionParams,
        string name = "UsePower")
        : base(reactionParams, name)
    {
        guiCharacter = new GuiCharacter(Character);
    }

    public ReactionRequestUsePowerCustom(
        CharacterActionParams reactionParams,
        GameLocationCharacter attacker = null)
        : base(reactionParams, "UsePower")
    {
        guiCharacter = new GuiCharacter(Character);
        guiAttacker = new GuiCharacter(attacker);
    }

    public ReactionRequestUsePowerCustom(
        CharacterActionParams reactionParams,
        string name = "UsePower",
        GameLocationCharacter attacker = null)
        : base(reactionParams, name)
    {
        guiCharacter = new GuiCharacter(Character);
        guiAttacker = new GuiCharacter(attacker);
    }

    public override string FormatDescription()
    {
        return string.IsNullOrEmpty(reactionParams.stringParameter2)
            ? base.FormatDescription()
            : reactionParams.stringParameter2;
    }
}
