using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.CustomDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class CustomizableFightingStyleBuilder
    : DefinitionBuilder<CustomFightingStyleDefinition, CustomizableFightingStyleBuilder>
{
    protected CustomizableFightingStyleBuilder(string name, Guid guid) : base(name, guid)
    {
    }

    [NotNull]
    internal CustomizableFightingStyleBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features.OrderBy(f => f.Name));
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }
}
