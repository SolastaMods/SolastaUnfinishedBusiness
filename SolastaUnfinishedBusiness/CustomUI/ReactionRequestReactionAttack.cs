using SolastaUnfinishedBusiness.CustomInterfaces;

namespace SolastaUnfinishedBusiness.CustomUI;

internal class ReactionRequestReactionAttack : ReactionRequest, IReactionRequestWithResource
{
    private readonly string ally;
    private readonly GuiCharacter target;
    private readonly string type;

    internal ReactionRequestReactionAttack(string type, CharacterActionParams reactionParams)
        : base(Name(type), reactionParams)
    {
        this.type = type;
        target = new GuiCharacter(reactionParams.TargetCharacters[0]);
        ally = reactionParams.StringParameter;
    }

    public override bool IsStillValid
    {
        get
        {
            var targetCharacter = ReactionParams.TargetCharacters[0];

            return ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters
                .Contains(targetCharacter) && !targetCharacter.RulesetCharacter.IsDeadOrDyingOrUnconscious;
        }
    }

    public ICustomReactionResource Resource { get; set; }

    private static string Name(string type)
    {
        return $"ReactionAttack{type}";
    }

    public override string FormatTitle()
    {
        return Gui.Localize($"Reaction/&ReactionAttack{type}Title");
    }

    public override string FormatDescription()
    {
        var format = $"Reaction/&ReactionAttack{type}Description";

        return Gui.Format(format, target.Name, ally);
    }

    public override string FormatReactTitle()
    {
        var format = $"Reaction/&ReactionAttack{type}ReactTitle";

        return Gui.Format(format, target.Name, ally);
    }

    public override string FormatReactDescription()
    {
        var format = $"Reaction/&ReactionAttack{type}ReactDescription";

        return Gui.Format(format, target.Name, ally);
    }
}
