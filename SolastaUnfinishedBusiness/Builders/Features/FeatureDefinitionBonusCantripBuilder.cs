using System;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

public abstract class
    FeatureDefinitionBonusCantripsBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionBonusCantrips
    where TBuilder : FeatureDefinitionBonusCantripsBuilder<TDefinition, TBuilder>
{
    public TBuilder SetBonusCantrips(params SpellDefinition[] spellDefinitions)
    {
        Definition.BonusCantrips.SetRange(spellDefinitions);
        Definition.BonusCantrips.Sort(Sorting.Compare);
        return This();
    }

    #region Constructors

    protected FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionBonusCantripsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    protected FeatureDefinitionBonusCantripsBuilder(TDefinition original, string name, Guid namespaceGuid) : base(
        original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionBonusCantripsBuilder(TDefinition original, string name, string definitionGuid) :
        base(original, name, definitionGuid)
    {
    }

    #endregion
}

public class FeatureDefinitionBonusCantripsBuilder : FeatureDefinitionBonusCantripsBuilder<
    FeatureDefinitionBonusCantrips, FeatureDefinitionBonusCantripsBuilder>
{
    #region Constructors

    public FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    public FeatureDefinitionBonusCantripsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    public FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
