using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class InvocationDefinitionBuilder :
    InvocationDefinitionBuilder<InvocationDefinition, InvocationDefinitionBuilder>
{
    #region Constructors

    protected InvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected InvocationDefinitionBuilder(InvocationDefinition original, string name, Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}

internal abstract class InvocationDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : InvocationDefinition
    where TBuilder : InvocationDefinitionBuilder<TDefinition, TBuilder>
{
    protected InvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected InvocationDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    internal TBuilder SetRequiredLevel(int level)
    {
        Definition.requiredLevel = level;
        return (TBuilder)this;
    }

    internal TBuilder SetGrantedFeature(FeatureDefinition featureDefinition)
    {
        Definition.grantedFeature = featureDefinition;
        return (TBuilder)this;
    }

#if false
    internal TBuilder SetRequiredSpell(SpellDefinition spell)
    {
        Definition.requiredKnownSpell = spell;
        return (TBuilder)this;
    }

    internal TBuilder SetRequiredPact(FeatureDefinition pact)
    {
        Definition.requiredPact = pact;
        return (TBuilder)this;
    }

    internal TBuilder SetGrantedSpell(SpellDefinition spell, bool consumeSlot = false, bool overrideMaterial = true)
    {
        Definition.grantedSpell = spell;
        Definition.consumesSpellSlot = consumeSlot;
        Definition.overrideMaterialComponent = overrideMaterial;
        return (TBuilder)this;
    }
#endif
}
