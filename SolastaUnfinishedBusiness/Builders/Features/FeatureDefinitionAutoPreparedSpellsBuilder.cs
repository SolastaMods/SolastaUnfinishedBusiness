using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static FeatureDefinitionAutoPreparedSpells;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionAutoPreparedSpellsBuilder
    : DefinitionBuilder<FeatureDefinitionAutoPreparedSpells, FeatureDefinitionAutoPreparedSpellsBuilder>
{
    internal FeatureDefinitionAutoPreparedSpellsBuilder AddPreparedSpellGroup(
        int classLevel,
        params SpellDefinition[] spells)
    {
        Definition.AutoPreparedSpellsGroups.Add(new AutoPreparedSpellsGroup
        {
            ClassLevel = classLevel, spellsList = spells.ToList()
        });
        return this;
    }

    internal FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(
        params AutoPreparedSpellsGroup[] autoSpellLists)
    {
        Definition.AutoPreparedSpellsGroups.SetRange(autoSpellLists);
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

    protected FeatureDefinitionAutoPreparedSpellsBuilder(FeatureDefinitionAutoPreparedSpells original, string name,
        Guid namespaceGuid) : base(original, name, namespaceGuid)
    {
    }

    #endregion
}

internal static class AutoPreparedSpellsGroupBuilder
{
    internal static AutoPreparedSpellsGroup BuildSpellGroup(int classLevel, params SpellDefinition[] spells)
    {
        return new AutoPreparedSpellsGroup { ClassLevel = classLevel, SpellsList = spells.ToList() };
    }
}
