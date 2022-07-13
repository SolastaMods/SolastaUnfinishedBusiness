using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.CustomDefinitions;

internal class FeatDefinitionWithPrerequisitesBuilder : FeatDefinitionBuilder<FeatDefinitionWithPrerequisites,
    FeatDefinitionWithPrerequisitesBuilder>
{
    public FeatDefinitionWithPrerequisitesBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    public FeatDefinitionWithPrerequisitesBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    public FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    public FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    public FeatDefinitionWithPrerequisitesBuilder SetValidators(
        params Func<FeatDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>[]
            validators)
    {
        Definition.Validators.AddRange(validators);

        return This();
    }
}

internal class FeatDefinitionWithPrerequisites : FeatDefinition
{
    public List<Func<FeatDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>>
        Validators { get; } = new();

    public (bool result, string output) Validate(FeatDefinitionWithPrerequisites feat, RulesetCharacterHero hero)
    {
        var results = Validators.Select(v => v(feat, hero));

        var valueTuples = results as (bool result, string output)[] ?? results.ToArray();
        
        return valueTuples.Any() ? (valueTuples.All(r => r.result), string.Join("\n", valueTuples.Select(r => r.output))) : (true, string.Empty);
    }
}
