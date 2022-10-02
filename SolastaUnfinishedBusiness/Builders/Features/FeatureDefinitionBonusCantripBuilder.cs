using System;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders.Features;

internal abstract class
    FeatureDefinitionBonusCantripsBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : FeatureDefinitionBonusCantrips
    where TBuilder : FeatureDefinitionBonusCantripsBuilder<TDefinition, TBuilder>
{
    internal TBuilder SetBonusCantrips(params SpellDefinition[] spellDefinitions)
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

internal class FeatureDefinitionBonusCantripsBuilder : FeatureDefinitionBonusCantripsBuilder<
    FeatureDefinitionBonusCantrips, FeatureDefinitionBonusCantripsBuilder>
{
    #region Constructors

    internal FeatureDefinitionBonusCantripsBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionBonusCantripsBuilder(string name, string definitionGuid) : base(name, definitionGuid)
    {
    }

    internal FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    internal FeatureDefinitionBonusCantripsBuilder(FeatureDefinitionBonusCantrips original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}
