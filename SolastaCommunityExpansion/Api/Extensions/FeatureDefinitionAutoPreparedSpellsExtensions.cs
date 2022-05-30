using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;

namespace SolastaModApi.Extensions;

/// <summary>
///     This helper extensions class was automatically generated.
///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
/// </summary>
[TargetType(typeof(FeatureDefinitionAutoPreparedSpells))]
[GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
public static class FeatureDefinitionAutoPreparedSpellsExtensions
{
    public static T AddAutoPreparedSpellsGroups<T>(this T entity,
        params FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup[] value)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        AddAutoPreparedSpellsGroups(entity, value.AsEnumerable());
        return entity;
    }

    public static T AddAutoPreparedSpellsGroups<T>(this T entity,
        IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> value)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        entity.AutoPreparedSpellsGroups.AddRange(value);
        return entity;
    }

    public static T ClearAutoPreparedSpellsGroups<T>(this T entity)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        entity.AutoPreparedSpellsGroups.Clear();
        return entity;
    }

    public static T SetAffinityRace<T>(this T entity, CharacterRaceDefinition value)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        entity.SetField("affinityRace", value);
        return entity;
    }

    public static T SetAutoPreparedSpellsGroups<T>(this T entity,
        params FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup[] value)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        SetAutoPreparedSpellsGroups(entity, value.AsEnumerable());
        return entity;
    }

    public static T SetAutoPreparedSpellsGroups<T>(this T entity,
        IEnumerable<FeatureDefinitionAutoPreparedSpells.AutoPreparedSpellsGroup> value)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        entity.AutoPreparedSpellsGroups.SetRange(value);
        return entity;
    }

    public static T SetAutopreparedTag<T>(this T entity, String value)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        entity.SetField("autopreparedTag", value);
        return entity;
    }

    public static T SetSpellcastingClass<T>(this T entity, CharacterClassDefinition value)
        where T : FeatureDefinitionAutoPreparedSpells
    {
        entity.SetField("spellcastingClass", value);
        return entity;
    }
}
