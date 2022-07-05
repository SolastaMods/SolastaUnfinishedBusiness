using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Api.Infrastructure;
using SolastaCommunityExpansion.CustomDefinitions;

namespace SolastaCommunityExpansion.Builders;

public abstract class SpellDefinitionWithDependentEffectsBuilder<TDefinition, TBuilder> : SpellDefinitionBuilder<
    TDefinition,
    TBuilder>
    where TDefinition : SpellDefinitionWithDependentEffects
    where TBuilder : SpellDefinitionBuilder<TDefinition, TBuilder>
{
    public TBuilder AddFeatureEffects(params (List<FeatureDefinition>, EffectDescription)[] featureEffects)
    {
        Definition.FeaturesEffectList.AddRange(featureEffects);
        return This();
    }

    public TBuilder SetFeatureEffects(params (List<FeatureDefinition>, EffectDescription)[] featureEffects)
    {
        Definition.FeaturesEffectList.SetRange(featureEffects);
        return This();
    }

    #region Constructors

    protected SpellDefinitionWithDependentEffectsBuilder(string name, string guid) : base(name, guid)
    {
    }

    protected SpellDefinitionWithDependentEffectsBuilder(string name, Guid guidNamespace) : base(name,
        guidNamespace)
    {
    }

    protected SpellDefinitionWithDependentEffectsBuilder(TDefinition original, string name, string guid) : base(
        original, name, guid)
    {
    }

    protected SpellDefinitionWithDependentEffectsBuilder(TDefinition original, string name, Guid guidNamespace) :
        base(original, name, guidNamespace)
    {
    }

    #endregion Constructors
}

public class SpellDefinitionWithDependentEffectsBuilder : SpellDefinitionWithDependentEffectsBuilder<
    SpellDefinitionWithDependentEffects, SpellDefinitionWithDependentEffectsBuilder>
{
    #region Constructors

    public SpellDefinitionWithDependentEffectsBuilder(string name, string guid) : base(name, guid)
    {
    }

    public SpellDefinitionWithDependentEffectsBuilder(string name, Guid guidNamespace) : base(name,
        guidNamespace)
    {
    }

    public SpellDefinitionWithDependentEffectsBuilder(SpellDefinitionWithDependentEffects original,
        string name, string guid) : base(original, name, guid)
    {
    }

    public SpellDefinitionWithDependentEffectsBuilder(SpellDefinitionWithDependentEffects original,
        string name, Guid guidNamespace) : base(original, name, guidNamespace)
    {
    }

    #endregion Constructors
}
