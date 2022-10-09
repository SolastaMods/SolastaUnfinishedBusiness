using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;

namespace SolastaUnfinishedBusiness.CustomDefinitions;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal class FeatDefinitionWithPrerequisitesBuilder : FeatDefinitionBuilder<FeatDefinitionWithPrerequisites,
    FeatDefinitionWithPrerequisitesBuilder>
{
    protected FeatDefinitionWithPrerequisitesBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal FeatDefinitionWithPrerequisitesBuilder SetValidators(
        [NotNull] params Func<FeatDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>[]
            validators)
    {
        Definition.Validators.AddRange(validators);

        return this;
    }
}

internal sealed class FeatDefinitionWithPrerequisites : FeatDefinition
{
    internal List<Func<FeatDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>>
        Validators { get; } = new();

    internal (bool result, string output) Validate(FeatDefinitionWithPrerequisites feat, RulesetCharacterHero hero)
    {
        var results = Validators.Select(v => v(feat, hero));

        var valueTuples = results as (bool result, string output)[] ?? results.ToArray();

        return valueTuples.Any()
            ? (valueTuples.All(r => r.result), string.Join("\n", valueTuples.Select(r => r.output)))
            : (true, string.Empty);
    }
}
