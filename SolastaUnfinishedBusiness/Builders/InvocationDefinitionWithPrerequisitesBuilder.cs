﻿using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class InvocationDefinitionWithPrerequisitesBuilder
    : InvocationDefinitionBuilder<InvocationDefinitionWithPrerequisites, InvocationDefinitionWithPrerequisitesBuilder>
{
    protected InvocationDefinitionWithPrerequisitesBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected InvocationDefinitionWithPrerequisitesBuilder(InvocationDefinitionWithPrerequisites original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal InvocationDefinitionWithPrerequisitesBuilder SetValidators(
        params Func<InvocationDefinitionWithPrerequisites, RulesetCharacterHero, (bool result, string output)>[]
            validators)
    {
        Definition.Validators.AddRange(validators);

        return this;
    }
}
