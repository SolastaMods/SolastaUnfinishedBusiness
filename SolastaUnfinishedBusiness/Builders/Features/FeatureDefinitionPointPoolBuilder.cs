using System;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionPointPoolBuilder
    : DefinitionBuilder<FeatureDefinitionPointPool, FeatureDefinitionPointPoolBuilder>
{
    internal FeatureDefinitionPointPoolBuilder SetPool(HeroDefinitions.PointsPoolType poolType, int poolAmount)
    {
        Definition.poolType = poolType;
        Definition.poolAmount = poolAmount;
        return this;
    }

    internal FeatureDefinitionPointPoolBuilder SetSpellOrCantripPool(
        HeroDefinitions.PointsPoolType poolType,
        int poolAmount,
        SpellListDefinition spellListOverride = null,
        string extraSpellsTag = "",
        int minSpellLevel = 0,
        int maxSpellLevel = 0,
        bool ritualsOnly = false)
    {
        Definition.poolType = poolType;
        Definition.poolAmount = poolAmount;
        Definition.spellListOverride = spellListOverride;
        Definition.extraSpellsTag = extraSpellsTag;
        Definition.minSpellLevel = minSpellLevel;
        Definition.maxSpellLevel = maxSpellLevel;
        Definition.ritualOnly = ritualsOnly;
        return this;
    }

    internal FeatureDefinitionPointPoolBuilder RestrictChoices(params string[] choices)
    {
        Definition.RestrictedChoices.AddRange(choices);
        Definition.RestrictedChoices.Sort();
        return this;
    }

    internal FeatureDefinitionPointPoolBuilder OnlyUniqueChoices()
    {
        Definition.uniqueChoices = true;
        return this;
    }

    #region Constructors

    internal FeatureDefinitionPointPoolBuilder(string name, Guid namespaceGuid) : base(name, namespaceGuid)
    {
    }

    internal FeatureDefinitionPointPoolBuilder(FeatureDefinitionPointPool original, string name, Guid namespaceGuid) :
        base(original, name, namespaceGuid)
    {
    }

    #endregion
}
