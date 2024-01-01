using System;
using System.Collections.Generic;
using System.Linq;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

internal sealed class FeatDefinitionWithPrerequisites : FeatDefinition
{
    internal List<Func<FeatDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>>
        Validators { get; } = [];

    internal (bool result, string output) Validate(FeatDefinitionWithPrerequisites feat, RulesetCharacterHero hero)
    {
        var results = Validators.Select(v => v(feat, hero));
        var valueTuples = results as (bool result, string output)[] ?? results.ToArray();

        return valueTuples.Length != 0
            ? (valueTuples.All(r => r.result), string.Join("\n", valueTuples.Select(r => r.output)))
            : (true, string.Empty);
    }
}
