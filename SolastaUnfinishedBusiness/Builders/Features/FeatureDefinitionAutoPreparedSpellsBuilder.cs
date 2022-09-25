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
    public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(
        params AutoPreparedSpellsGroup[] autoSpellLists)
    {
        return SetPreparedSpellGroups(autoSpellLists.AsEnumerable());
    }

    public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(
        IEnumerable<AutoPreparedSpellsGroup> autoSpellLists)
    {
        Definition.AutoPreparedSpellsGroups.SetRange(autoSpellLists);
        return this;
    }

    public FeatureDefinitionAutoPreparedSpellsBuilder SetCastingClass(
        CharacterClassDefinition castingClass)
    {
        Definition.spellcastingClass = castingClass;
        return this;
    }

    public FeatureDefinitionAutoPreparedSpellsBuilder SetAutoTag(string tag)
    {
        Definition.autopreparedTag = tag;
        return this;
    }

    public FeatureDefinitionAutoPreparedSpellsBuilder SetSpellcastingClass(CharacterClassDefinition characterClass)
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

public static class AutoPreparedSpellsGroupBuilder
{
    public static AutoPreparedSpellsGroup BuildSpellGroup(int classLevel, params SpellDefinition[] spellNames)
    {
        return BuildSpellGroup(classLevel, spellNames.AsEnumerable());
    }

    public static AutoPreparedSpellsGroup BuildSpellGroup(int classLevel, IEnumerable<SpellDefinition> spellNames)
    {
        return new AutoPreparedSpellsGroup { ClassLevel = classLevel, SpellsList = spellNames.ToList() };
    }
}
