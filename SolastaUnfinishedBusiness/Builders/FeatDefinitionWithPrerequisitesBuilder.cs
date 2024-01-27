using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class FeatDefinitionWithPrerequisitesBuilder
    : FeatDefinitionBuilder<FeatDefinitionWithPrerequisites, FeatDefinitionWithPrerequisitesBuilder>
{
    protected FeatDefinitionWithPrerequisitesBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatDefinitionWithPrerequisitesBuilder(FeatDefinitionWithPrerequisites original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal FeatDefinitionWithPrerequisitesBuilder SetValidators(
        params Func<FeatDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>[] validators)
    {
        Definition.Validators.AddRange(validators);

        return this;
    }
}
