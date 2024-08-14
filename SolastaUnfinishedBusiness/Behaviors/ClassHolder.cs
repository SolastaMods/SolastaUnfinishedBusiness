namespace SolastaUnfinishedBusiness.Behaviors;

public record ClassHolder(CharacterClassDefinition Class)
{
    public CharacterClassDefinition Class { get; } = Class;
}
