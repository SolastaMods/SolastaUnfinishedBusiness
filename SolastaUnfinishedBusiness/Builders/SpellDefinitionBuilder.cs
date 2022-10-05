using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

internal abstract class SpellDefinitionBuilder<TDefinition, TBuilder> : DefinitionBuilder<TDefinition, TBuilder>
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

    internal TBuilder SetSpellLevel(int spellLevel)
    {
        Definition.spellLevel = spellLevel;
        return This();
    }

    internal TBuilder SetRequiresConcentration(bool value)
    {
        Definition.requiresConcentration = value;
        return This();
    }

    internal TBuilder SetSchoolOfMagic(SchoolOfMagicDefinition school)
    {
        Definition.schoolOfMagic = school.Name;
        return This();
    }

    internal TBuilder SetSubSpells(params SpellDefinition[] subspells)
    {
        Definition.spellsBundle = true;
        Definition.SubspellsList.SetRange(subspells);
        Definition.spellsBundle = subspells.Any();
        return This();
    }

    internal TBuilder SetCastingTime(RuleDefinitions.ActivationTime castingTime)
    {
        Definition.castingTime = castingTime;
        return This();
    }

    internal TBuilder SetRitualCasting(RuleDefinitions.ActivationTime ritualCastingTime)
    {
        Definition.ritual = true;
        Definition.ritualCastingTime = ritualCastingTime;
        return This();
    }

    internal TBuilder SetVerboseComponent(bool verboseComponent)
    {
        Definition.verboseComponent = verboseComponent;
        return This();
    }

    internal TBuilder SetSomaticComponent(bool somaticComponent)
    {
        Definition.somaticComponent = somaticComponent;
        return This();
    }

    internal TBuilder SetMaterialComponent(RuleDefinitions.MaterialComponentType materialComponentType)
    {
        Definition.materialComponentType = materialComponentType;
        return This();
    }

    internal TBuilder SetSpecificMaterialComponent(string specificMaterialComponentTag,
        int specificMaterialComponentCostGp, bool specificMaterialComponentConsumed)
    {
        Definition.materialComponentType = RuleDefinitions.MaterialComponentType.Specific;
        Definition.specificMaterialComponentTag = specificMaterialComponentTag;
        Definition.specificMaterialComponentCostGp = specificMaterialComponentCostGp;
        Definition.specificMaterialComponentConsumed = specificMaterialComponentConsumed;
        return This();
    }

    internal TBuilder SetEffectDescription(EffectDescription effectDescription)
    {
        Definition.effectDescription = effectDescription;
        return This();
    }

    internal TBuilder SetAiParameters(SpellAIParameters aiParameters)
    {
        Definition.aiParameters = aiParameters;
        return This();
    }

    internal TBuilder SetUniqueInstance(bool uniqueInstance)
    {
        Definition.uniqueInstance = uniqueInstance;
        return This();
    }

    internal TBuilder SetConcentrationAction(ActionDefinitions.ActionParameter concentrationAction)
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

[UsedImplicitly]
internal class SpellDefinitionBuilder : SpellDefinitionBuilder<SpellDefinition, SpellDefinitionBuilder>
{
    #region Constructors

    internal SpellDefinitionBuilder(string name, string guid) : base(name, guid)
    {
    }

    internal SpellDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
    {
    }

    internal SpellDefinitionBuilder(SpellDefinition original, string name, string guid) : base(original, name, guid)
    {
    }

    internal SpellDefinitionBuilder(SpellDefinition original, string name, Guid guidNamespace) : base(original, name,
        guidNamespace)
    {
    }

    #endregion Constructors
}
