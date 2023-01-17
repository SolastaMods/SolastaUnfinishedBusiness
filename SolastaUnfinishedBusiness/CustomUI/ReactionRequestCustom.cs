using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomUI;

public class ReactionRequestCustom : ReactionRequest, IReactionRequestWithResource
{
    private readonly string type;

    internal ReactionRequestCustom(string type, CharacterActionParams reactionParams)
        : base(Name(type), reactionParams)
    {
        this.type = type;
    }

    public ICustomReactionResource Resource { get; set; }

    private static string Name(string type)
    {
        return $"CustomReaction{type}";
    }

    public override string FormatTitle()
    {
        return Gui.Localize($"Reaction/&CustomReaction{type}Title");
    }

    public override string FormatDescription()
    {
        return ReactionParams.StringParameter;
    }

    public override string FormatReactTitle()
    {
        return Gui.Localize($"Reaction/&CustomReaction{type}ReactTitle");
    }

    public override string FormatReactDescription()
    {
        return Gui.Localize($"Reaction/&CustomReaction{type}ReactDescription");
    }
}
