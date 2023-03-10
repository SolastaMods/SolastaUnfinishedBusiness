using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class FightingStyleBuilder : DefinitionBuilder<FightingStyleDefinition, FightingStyleBuilder>
{
    protected FightingStyleBuilder(string name, Guid guid) : base(name, guid)
    {
    }

    [NotNull]
    internal FightingStyleBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features.OrderBy(f => f.Name));
        Definition.Features.Sort(Sorting.Compare);
        return this;
    }
}
