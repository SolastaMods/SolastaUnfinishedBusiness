﻿using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Infrastructure;
using static FeatureDefinitionAutoPreparedSpells;

namespace SolastaCommunityExpansion.Builders.Features;

public class FeatureDefinitionAutoPreparedSpellsBuilder
    : FeatureDefinitionBuilder<FeatureDefinitionAutoPreparedSpells, FeatureDefinitionAutoPreparedSpellsBuilder>
{
    public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(
        params AutoPreparedSpellsGroup[] autospelllists)
    {
        return SetPreparedSpellGroups(autospelllists.AsEnumerable());
    }

    public FeatureDefinitionAutoPreparedSpellsBuilder SetPreparedSpellGroups(
        IEnumerable<AutoPreparedSpellsGroup> autospelllists)
    {
        Definition.AutoPreparedSpellsGroups.SetRange(autospelllists);
        return this;
    }

    public FeatureDefinitionAutoPreparedSpellsBuilder SetCastingClass(CharacterClassDefinition castingClass)
    {
        Definition.spellcastingClass = castingClass;
        return this;
    }

    public FeatureDefinitionAutoPreparedSpellsBuilder SetAffinityRace(CharacterRaceDefinition castingRace)
    {
        Definition.affinityRace = castingRace;
        return this;
    }

    /**
         * * This tag is used to create a tooltip:
         * * this.autoPreparedTitle.Text = string.Format("Screen/&{0}SpellTitle", autoPreparedTag);
         * * this.autoPreparedTooltip.Content = string.Format("Screen/&{0}SpellDescription", autoPreparedTag);
         */
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
    public static AutoPreparedSpellsGroup BuildSpellGroup(int classLevel, params SpellDefinition[] spellnames)
    {
        return BuildSpellGroup(classLevel, spellnames.AsEnumerable());
    }

    public static AutoPreparedSpellsGroup BuildSpellGroup(int classLevel, IEnumerable<SpellDefinition> spellnames)
    {
        return new AutoPreparedSpellsGroup { ClassLevel = classLevel, SpellsList = spellnames.ToList() };
    }
}
