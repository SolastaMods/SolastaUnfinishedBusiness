using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
internal sealed class InvocationDefinitionWithPrerequisites : InvocationDefinition
{
    internal List<Func<InvocationDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>>
        Validators { get; } = [];

    internal (bool result, string output) Validate(InvocationDefinitionWithPrerequisites invocation,
        RulesetCharacterHero hero)
    {
        var results = Validators.Select(v => v(invocation, hero));
        var valueTuples = results as (bool result, string output)[] ?? results.ToArray();

        return valueTuples.Length != 0
            ? (valueTuples.All(r => r.result), string.Join("\n", valueTuples.Select(r => r.output)))
            : (true, string.Empty);
    }
}
