using System;

namespace SolastaUnfinishedBusiness.Builders;

public abstract class InvocationDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
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
    
    public TBuilder SetRequierLevel(int level)
    {
        Definition.requiredLevel = level;
        return This();
    }
    
    public TBuilder SetGrantedFeature(FeatureDefinition featureDefinition)
    {
        Definition.grantedFeature = featureDefinition;
        return This();
    }
    
    
    public TBuilder SetGrantedSpell(SpellDefinition spell, bool consumeSlot = false, bool overrideMaterial = true)
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
