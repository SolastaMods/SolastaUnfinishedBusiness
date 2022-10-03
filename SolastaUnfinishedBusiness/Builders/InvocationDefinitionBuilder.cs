using System;

namespace SolastaUnfinishedBusiness.Builders;

internal abstract class InvocationDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : InvocationDefinition
    where TBuilder : InvocationDefinitionBuilder<TDefinition, TBuilder>
{
    protected InvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected InvocationDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected InvocationDefinitionBuilder(TDefinition original, string name, Guid namespaceGuid) : base(original, name,
        namespaceGuid)
    {
    }

    protected InvocationDefinitionBuilder(TDefinition original, string name, string definitionGuid) : base(original,
        name, definitionGuid)
    {
    }

    internal TBuilder SetRequiredLevel(int level)
    {
        Definition.requiredLevel = level;
        return This();
    }

    internal TBuilder SetRequiredSpell(SpellDefinition spell)
    {
        Definition.requiredKnownSpell = spell;
        return This();
    }

    internal TBuilder SetRequiredPact(FeatureDefinition pact)
    {
        Definition.requiredPact = pact;
        return This();
    }

    internal TBuilder SetGrantedFeature(FeatureDefinition featureDefinition)
    {
        Definition.grantedFeature = featureDefinition;
        return This();
    }

    internal TBuilder SetGrantedSpell(SpellDefinition spell, bool consumeSlot = false, bool overrideMaterial = true)
    {
        Definition.grantedSpell = spell;
        Definition.consumesSpellSlot = consumeSlot;
        Definition.overrideMaterialComponent = overrideMaterial;
        return This();
    }
}

internal class InvocationDefinitionBuilder :
    InvocationDefinitionBuilder<InvocationDefinition, InvocationDefinitionBuilder>
{
    #region Constructors

    protected InvocationDefinitionBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected InvocationDefinitionBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected InvocationDefinitionBuilder(InvocationDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected InvocationDefinitionBuilder(InvocationDefinition original, string name, string definitionGuid) : base(
        original, name, definitionGuid)
    {
    }

    #endregion
}
