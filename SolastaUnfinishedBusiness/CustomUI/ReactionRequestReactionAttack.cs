using SolastaUnfinishedBusiness.Interfaces;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class ReactionRequestReactionAttack : ReactionRequest, IReactionRequestWithResource
{
    private readonly string _ally;
    private readonly GuiCharacter _target;
    private readonly string _type;

    internal ReactionRequestReactionAttack(string type, CharacterActionParams reactionParams)
        : base(Name(type), reactionParams)
    {
        _type = type;
        _target = new GuiCharacter(reactionParams.TargetCharacters[0]);
        _ally = reactionParams.StringParameter;
    }

    public override bool IsStillValid
    {
        get
        {
            var targetCharacter = ReactionParams.TargetCharacters[0];

            return targetCharacter.RulesetCharacter is { IsDeadOrDyingOrUnconscious: false } &&
                   ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters
                       .Contains(targetCharacter);
        }
    }

    public ICustomReactionResource Resource { get; set; }

    private static string Name(string type)
    {
        return $"ReactionAttack{type}";
    }

    public override string FormatTitle()
    {
        return Gui.Localize($"Reaction/&ReactionAttack{_type}Title");
    }

    public override string FormatDescription()
    {
        var format = $"Reaction/&ReactionAttack{_type}Description";

        return Gui.Format(format, _target.Name, _ally);
    }

    public override string FormatReactTitle()
    {
        var format = $"Reaction/&ReactionAttack{_type}ReactTitle";

        return Gui.Format(format, _target.Name, _ally);
    }

    public override string FormatReactDescription()
    {
        var format = $"Reaction/&ReactionAttack{_type}ReactDescription";

        return Gui.Format(format, _target.Name, _ally);
    }
}
