using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Features;

public interface ICharacterValidator
{
    bool IsValid(RulesetCharacter character);
}

public class CharacterValidator : ICharacterValidator
{
    private readonly Validator validator;

    public static readonly ICharacterValidator NoArmor =
        new CharacterValidator(character => !character.IsWearingArmor());
    
    public static readonly ICharacterValidator NoShield =
        new CharacterValidator(character => !character.IsWearingShield());

    public CharacterValidator(Validator validator)
    {
        this.validator = validator;
    }

    public bool IsValid(RulesetCharacter character)
    {
        return validator == null || validator(character);
    }

    public delegate bool Validator(RulesetCharacter character);
}

internal static class RulesetCharacterExension
{
    public static bool IsValid<T>(this RulesetCharacter instance, params T[] validators) where T : ICharacterValidator
    {
        return validators.All(v => v.IsValid(instance));
    }

    public static bool IsValid<T>(this RulesetCharacter instance, IEnumerable<T> validators) where T : ICharacterValidator
    {
        return validators == null || validators.All(v => v.IsValid(instance));
    }
}