using System.Collections.Generic;
using System.Linq;

namespace SolastaCommunityExpansion.Features;

public delegate bool CharacterValidator(RulesetCharacter character);

public static class CharacterValidators
{
    public static readonly CharacterValidator NoArmor = character => !character.IsWearingArmor();

    public static readonly CharacterValidator NoShield = character => !character.IsWearingShield();
}