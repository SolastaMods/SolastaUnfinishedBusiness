using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Features;

namespace SolastaCommunityExpansion.Api.AdditionalExtensions;

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