using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionCraftingAffinityBuilder : FeatureDefinitionBuilder<FeatureDefinitionCraftingAffinity,
    FeatureDefinitionCraftingAffinityBuilder>
{
    public FeatureDefinitionCraftingAffinityBuilder SetAffinityGroups(
        float durationMultiplier,
        bool doubleProficiency,
        params ToolTypeDefinition[] tools)
    {
        Definition.affinityGroups.SetRange(tools.Select(t =>
            new FeatureDefinitionCraftingAffinity.CraftingAffinityGroup
            {
                tooltype = t, durationMultiplier = durationMultiplier, doubleProficiencyBonus = doubleProficiency
            }));
        return this;
    }

    #region Constructors

    public FeatureDefinitionCraftingAffinityBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    public FeatureDefinitionCraftingAffinityBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    public FeatureDefinitionCraftingAffinityBuilder(FeatureDefinitionCraftingAffinity original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    public FeatureDefinitionCraftingAffinityBuilder(FeatureDefinitionCraftingAffinity original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
