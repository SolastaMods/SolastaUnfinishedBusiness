using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Features;

public delegate bool CharacterValidator(RulesetCharacter character);

public static class CharacterValidators
{
    public static readonly CharacterValidator NoArmor = character => !character.IsWearingArmor();

    public static readonly CharacterValidator NoShield = character => !character.IsWearingShield();
}

internal static class RulesetCharacterExension
{
    public static bool IsValid(this RulesetCharacter instance, params CharacterValidator[] validators)
    {
        return validators.All(v => v(instance));
    }

    public static bool IsValid(this RulesetCharacter instance, IEnumerable<CharacterValidator> validators)
    {
        return validators == null || validators.All(v => v(instance));
    }
}