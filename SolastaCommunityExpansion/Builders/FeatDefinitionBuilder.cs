using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Builders;

public abstract class FeatDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatDefinition
    where TBuilder : FeatDefinitionBuilder<TDefinition, TBuilder>
{
    public TBuilder SetFeatures(params FeatureDefinition[] features)
    {
        return SetFeatures(features.AsEnumerable());
    }

    public TBuilder SetFeatures(IEnumerable<FeatureDefinition> features)
    {
        Definition.Features.SetRange(features);
        Definition.Features.Sort(Sorting.Compare);
        return This();
    }

    public TBuilder AddFeatures(params FeatureDefinition[] features)
    {
        return AddFeatures(features.AsEnumerable());
    }

    public TBuilder AddFeatures(IEnumerable<FeatureDefinition> features)
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

#if false
    public TBuilder SetClassPrerequisite(params string[] classes)
    {
        return SetClassPrerequisite(classes.AsEnumerable());
    }

    public TBuilder SetClassPrerequisite(IEnumerable<string> classes)
    {
        Definition.CompatibleClassesPrerequisite.SetRange(classes.OrderBy(c => c));
        return This();
    }

    public TBuilder SetRacePrerequisite(params string[] races)
    {
        return SetRacePrerequisite(races.AsEnumerable());
    }

    public TBuilder SetRacePrerequisite(IEnumerable<string> races)
    {
        Definition.CompatibleRacesPrerequisite.SetRange(races.OrderBy(r => r));
        return This();
    }
    
    public TBuilder SetFeatPrerequisite(params string[] feats)
    {
        return SetFeatPrerequisite(feats.AsEnumerable());
    }

    public TBuilder SetFeatPrerequisite(IEnumerable<string> feats)
    {
        Definition.KnownFeatsPrerequisite.SetRange(feats.OrderBy(f => f));
        return This();
    }
#endif

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

public class FeatDefinitionBuilder : FeatDefinitionBuilder<FeatDefinition, FeatDefinitionBuilder>
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
