using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.CustomUI;

public interface IReactionRequestWithResource
{
    ICustomReactionResource Resource { get; }
}

public class ReactionRequestCustom : ReactionRequest, IReactionRequestWithResource
{
    private readonly string _type;

    internal ReactionRequestCustom(string type, CharacterActionParams reactionParams)
        : base(Name(type), reactionParams)
    {
        _type = type;
    }

    public ICustomReactionResource Resource { get; set; }

    private static string Name(string type)
    {
        return $"CustomReaction{type}";
    }

    public override string FormatTitle()
    {
        return Gui.Localize($"Reaction/&CustomReaction{_type}Title");
    }

    public override string FormatDescription()
    {
        return ReactionParams.StringParameter;
    }

    public override string FormatReactTitle()
    {
        return Gui.Localize($"Reaction/&CustomReaction{_type}ReactTitle");
    }

    public override string FormatReactDescription()
    {
        return Gui.Localize($"Reaction/&CustomReaction{_type}ReactDescription");
    }
}
