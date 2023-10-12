using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class FeatDefinitionBuilder : FeatDefinitionBuilder<FeatDefinition, FeatDefinitionBuilder>
{
    #region Constructors

    protected FeatDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatDefinitionBuilder(FeatDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}

internal abstract class FeatDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatDefinition
    where TBuilder : FeatDefinitionBuilder<TDefinition, TBuilder>
{
    internal TBuilder SetFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.SetRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return (TBuilder)this;
    }

    internal TBuilder AddFeatures(params FeatureDefinition[] features)
    {
        Definition.Features.AddRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return (TBuilder)this;
    }

    internal TBuilder SetAbilityScorePrerequisite(string abilityScore, int value)
    {
        Definition.minimalAbilityScorePrerequisite = true;
        Definition.minimalAbilityScoreName = abilityScore;
        Definition.minimalAbilityScoreValue = value;
        return (TBuilder)this;
    }

    internal TBuilder SetMustCastSpellsPrerequisite()
    {
        Definition.mustCastSpellsPrerequisite = !Main.Settings.DisableCastSpellPreRequisitesOnModFeats;
        return (TBuilder)this;
    }

    internal TBuilder SetKnownFeatsPrerequisite(params string[] list)
    {
        Definition.knownFeatsPrerequisite.SetRange(list);
        return (TBuilder)this;
    }

    internal TBuilder SetFeatFamily(string family)
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

        return (TBuilder)this;
    }

    internal TBuilder SetArmorProficiencyPrerequisite(string category = null)
    {
        Definition.armorProficiencyPrerequisite = category != null;
        Definition.armorProficiencyCategory = category ?? String.Empty;
        return (TBuilder)this;
    }

    #region Constructors

    protected FeatDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
