using System;
using System.Collections.Generic;
using System.Linq;
using SolastaCommunityExpansion.Api.Extensions;
using SolastaCommunityExpansion.Api.Infrastructure;

namespace SolastaCommunityExpansion.Builders;

public abstract class SpellDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
    where TDefinition : SpellDefinition
    where TBuilder : SpellDefinitionBuilder<TDefinition, TBuilder>
{
    private void InitializeFields()
    {
        //
        // Should fix lots of official spells getting modified by spells on this mod
        //
        Definition.effectDescription = Definition.EffectDescription.Copy();
        Definition.implemented = true;
    }

    public TBuilder SetSpellLevel(int spellLevel)
    {
        Definition.spellLevel = spellLevel;
        return This();
    }

    public TBuilder SetRequiresConcentration(bool value)
    {
        Definition.requiresConcentration = value;
        return This();
    }

    public TBuilder SetSchoolOfMagic(SchoolOfMagicDefinition school)
    {
        Definition.schoolOfMagic = school.Name;
        return This();
    }

    public TBuilder SetSubSpells(params TDefinition[] subspells)
    {
        return SetSubSpells(subspells.AsEnumerable());
    }

    public TBuilder SetSubSpells(IEnumerable<TDefinition> subspells)
    {
        Definition.spellsBundle = true;
        Definition.SubspellsList.SetRange(subspells);
        return This();
    }

    public TBuilder SetCastingTime(RuleDefinitions.ActivationTime castingTime)
    {
        Definition.castingTime = castingTime;
        return This();
    }

    public TBuilder SetRitualCasting(RuleDefinitions.ActivationTime ritualCastingTime)
    {
        Definition.ritual = true;
        Definition.ritualCastingTime = ritualCastingTime;
        return This();
    }

    public TBuilder SetUniqueInstance(bool unique = true)
    {
        Definition.uniqueInstance = unique;
        return This();
    }

    public TBuilder SetVerboseComponent(bool verboseComponent)
    {
        Definition.verboseComponent = verboseComponent;
        return This();
    }

    public TBuilder SetSomaticComponent(bool somaticComponent)
    {
        Definition.somaticComponent = somaticComponent;
        return This();
    }

    public TBuilder SetMaterialComponent(RuleDefinitions.MaterialComponentType materialComponentType)
    {
        Definition.materialComponentType = materialComponentType;
        return This();
    }

    public TBuilder SetSpecificMaterialComponent(string specificMaterialComponentTag,
        int specificMaterialComponentCostGp, bool specificMaterialComponentConsumed)
    {
        Definition.materialComponentType = RuleDefinitions.MaterialComponentType.Specific;
        Definition.specificMaterialComponentTag = specificMaterialComponentTag;
        Definition.specificMaterialComponentCostGp = specificMaterialComponentCostGp;
        Definition.specificMaterialComponentConsumed = specificMaterialComponentConsumed;
        return This();
    }

    public TBuilder SetEffectDescription(EffectDescription effectDescription)
    {
        Definition.effectDescription = effectDescription;
        return This();
    }

    public TBuilder SetAiParameters(SpellAIParameters aiParameters)
    {
        Definition.aiParameters = aiParameters;
        return This();
    }

    public TBuilder SetConcentrationAction(ActionDefinitions.ActionParameter concentrationAction)
    {
        Definition.concentrationAction = concentrationAction;
        return This();
    }

    #region Constructors

    protected SpellDefinitionBuilder(string name, string guid) : base(name, guid)
    {
        InitializeFields();
    }

    protected SpellDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
    {
        InitializeFields();
    }

    protected SpellDefinitionBuilder(TDefinition original, string name, string guid) : base(original, name, guid)
    {
        InitializeFields();
    }

    protected SpellDefinitionBuilder(TDefinition original, string name, Guid guidNamespace)
        : base(original, name, guidNamespace)
    {
        InitializeFields();
    }

    #endregion
}

public class SpellDefinitionBuilder : SpellDefinitionBuilder<SpellDefinition, SpellDefinitionBuilder>
{
    #region Constructors

    public SpellDefinitionBuilder(string name, string guid) : base(name, guid)
    {
    }

    public SpellDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
    {
    }

    public SpellDefinitionBuilder(SpellDefinition original, string name, string guid) : base(original, name, guid)
    {
    }

    public SpellDefinitionBuilder(SpellDefinition original, string name, Guid guidNamespace) : base(original, name,
        guidNamespace)
    {
    }

    #endregion Constructors
}
