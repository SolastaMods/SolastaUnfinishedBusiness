using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

public abstract class FeatDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatDefinition
    where TBuilder : FeatDefinitionBuilder<TDefinition, TBuilder>
{
    public TBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return This();
    }

    public TBuilder AddFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.AddRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return This();
    }

    public TBuilder SetAbilityScorePrerequisite(string abilityScore, int value)
    {
        Definition.minimalAbilityScorePrerequisite = true;
        Definition.minimalAbilityScoreName = abilityScore;
        Definition.minimalAbilityScoreValue = value;
        return This();
    }

    public TBuilder SetMustCastSpellsPrerequisite()
    {
        Definition.mustCastSpellsPrerequisite = true;
        return This();
    }

    public TBuilder SetFeatFamily(string family)
    {
        if (string.IsNullOrEmpty(family))
        {
            Definition.hasFamilyTag = false;
            Definition.familyTag = string.Empty;
        }
        else
        {
            Definition.hasFamilyTag = true;
            Definition.familyTag = family;
        }

        return This();
    }

    public TBuilder SetArmorProficiencyPrerequisite(ArmorCategoryDefinition category)
    {
        Definition.armorProficiencyPrerequisite = true;
        Definition.armorProficiencyCategory = category.Name;
        return This();
    }

    #region Constructors

    protected FeatDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    protected FeatDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original, name,
        definitionGuid)
    {
    }

    #endregion
}

[UsedImplicitly]
internal class FeatDefinitionBuilder : FeatDefinitionBuilder<FeatDefinition, FeatDefinitionBuilder>
{
    #region Constructors

    protected FeatDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatDefinitionBuilder(FeatDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    protected FeatDefinitionBuilder(FeatDefinition original, string name, string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    #endregion
}
