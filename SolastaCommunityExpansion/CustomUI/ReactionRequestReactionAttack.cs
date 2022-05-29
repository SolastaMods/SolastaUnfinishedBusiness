namespace SolastaCommunityExpansion.CustomUI;

public class ReactionRequestReactionAttack : ReactionRequest
{
    public const string Name = "ReactionAttack";
    private readonly GuiCharacter target;
    public ReactionRequestReactionAttack(CharacterActionParams reactionParams)
        : base(Name, reactionParams)
    {
        target = new GuiCharacter(reactionParams.TargetCharacters[0]);
    }
    
    public override bool IsStillValid
    {
        get
        {
            var targetCharacter = ReactionParams.TargetCharacters[0];
            return ServiceRepository.GetService<IGameLocationCharacterService>().ValidCharacters.Contains(targetCharacter) && !targetCharacter.RulesetCharacter.IsDeadOrDyingOrUnconscious;
        }
    }

    public override string FormatTitle()
    {
        return Gui.Localize($"Reaction/&ReactionAttack{ReactionParams.StringParameter}Title");
    }

    public override string FormatDescription()
    {
        var format = $"Reaction/&ReactionAttack{ReactionParams.StringParameter}Description";
        return Gui.Format(format, target.Name);
    }

    public override string FormatReactTitle()
    {
        var format = $"Reaction/&ReactionAttack{ReactionParams.StringParameter}ReactTitle";
        return Gui.Format(format, target.Name);
    }

    public override string FormatReactDescription()
    {
        var format = $"Reaction/&ReactionAttack{ReactionParams.StringParameter}ReactDescription";
        return Gui.Format(format, target.Name);
    }
    
    
}