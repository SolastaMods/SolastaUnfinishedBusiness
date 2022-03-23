using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Builders;

namespace SolastaCommunityExpansion.CustomFeatureDefinitions
{
    public class FeatDefinitionWithPrerequisites : FeatDefinitionCustomCode
    {
        public new List<Func<FeatDefinition, RulesetCharacterHero, (bool result, string output)>> Validators { get; } = new();

        public override (bool result, string output) Validate(FeatDefinition feat, RulesetCharacterHero hero)
        {
            var results = Validators.Select(v => v(feat, hero));

            if (results.Any())
            {
                return (results.All(r => r.result), string.Join("\n", results.Select(r => r.output)));
            }

            return (true, string.Empty);
        }
    }
}
