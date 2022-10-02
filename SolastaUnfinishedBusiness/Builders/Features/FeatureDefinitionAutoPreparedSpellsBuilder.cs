using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using static FeatureDefinitionAutoPreparedSpells;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAutoPreparedSpellsBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionAutoPreparedSpells, FeatureDefinitionAutoPreparedSpellsBuilder>
{
    internal FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(
        params AutoPreparedSpellsGroup[] autoSpellLists)
    {
        Definition.AutoPreparedSpellsGroups.SetRange(autoSpellLists);
        return this;
    }

    internal FeatureDefinitionAutoPreparedSpellsBuilder SetCastingClass(
        CharacterClassDefinition castingClass)
    {
        Definition.spellcastingClass = castingClass;
        return this;
    }

    internal FeatureDefinitionAutoPreparedSpellsBuilder SetAutoTag(string tag)
    {
        Definition.autopreparedTag = tag;
        return this;
    }

    internal FeatureDefinitionAutoPreparedSpellsBuilder SetSpellcastingClass(CharacterClassDefinition characterClass)
    {
        Definition.spellcastingClass = characterClass;
        return this;
    }

    #region Constructors

    protected FeatureDefinitionAutoPreparedSpellsBuilder(string name, Guid namespaceGuid) : base(name,
        namespaceGuid)
    {
    }

    protected FeatureDefinitionAutoPreparedSpellsBuilder(string name, string definitionGuid) : base(name,
        definitionGuid)
    {
    }

    protected FeatureDefinitionAutoPreparedSpellsBuilder(FeatureDefinitionAutoPreparedSpells original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    protected FeatureDefinitionAutoPreparedSpellsBuilder(FeatureDefinitionAutoPreparedSpells original, string name,
        string definitionGuid) : base(original, name, definitionGuid)
    {
    }

    #endregion
}

internal static class AutoPreparedSpellsGroupBuilder
{
    internal static AutoPreparedSpellsGroup BuildSpellGroup(int classLevel, params SpellDefinition[] spellNames)
    {
        return BuildSpellGroup(classLevel, spellNames.AsEnumerable());
    }

    internal static AutoPreparedSpellsGroup BuildSpellGroup(int classLevel, IEnumerable<SpellDefinition> spellNames)
    {
        return new AutoPreparedSpellsGroup { ClassLevel = classLevel, SpellsList = spellNames.ToList() };
    }
}
