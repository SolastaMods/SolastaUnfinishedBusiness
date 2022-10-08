using System;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class SpellDefinitionBuilder : DefinitionBuilder<SpellDefinition, SpellDefinitionBuilder>
{
    private void InitializeFields()
    {
        // should fix lots of official spells getting modified by spells on this mod
        Definition.effectDescription = Definition.EffectDescription.Copy();
        Definition.implemented = true;
    }

    internal SpellDefinitionBuilder SetSpellLevel(int spellLevel)
    {
        Definition.spellLevel = spellLevel;
        return this;
    }

    internal SpellDefinitionBuilder SetRequiresConcentration(bool value)
    {
        Definition.requiresConcentration = value;
        return this;
    }

    internal SpellDefinitionBuilder SetSchoolOfMagic(SchoolOfMagicDefinition school)
    {
        Definition.schoolOfMagic = school.Name;
        return this;
    }

    internal SpellDefinitionBuilder SetSubSpells(params SpellDefinition[] subspells)
    {
        Definition.spellsBundle = true;
        Definition.SubspellsList.SetRange(subspells);
        Definition.spellsBundle = subspells.Any();
        return this;
    }

    internal SpellDefinitionBuilder SetCastingTime(RuleDefinitions.ActivationTime castingTime)
    {
        Definition.castingTime = castingTime;
        return this;
    }

    internal SpellDefinitionBuilder SetRitualCasting(RuleDefinitions.ActivationTime ritualCastingTime)
    {
        Definition.ritual = true;
        Definition.ritualCastingTime = ritualCastingTime;
        return this;
    }

    internal SpellDefinitionBuilder SetVerboseComponent(bool verboseComponent)
    {
        Definition.verboseComponent = verboseComponent;
        return this;
    }

    internal SpellDefinitionBuilder SetSomaticComponent(bool somaticComponent)
    {
        Definition.somaticComponent = somaticComponent;
        return this;
    }

    internal SpellDefinitionBuilder SetMaterialComponent(RuleDefinitions.MaterialComponentType materialComponentType)
    {
        Definition.materialComponentType = materialComponentType;
        return this;
    }

    internal SpellDefinitionBuilder SetSpecificMaterialComponent(
        string specificMaterialComponentTag,
        int specificMaterialComponentCostGp,
        bool specificMaterialComponentConsumed)
    {
        Definition.materialComponentType = RuleDefinitions.MaterialComponentType.Specific;
        Definition.specificMaterialComponentTag = specificMaterialComponentTag;
        Definition.specificMaterialComponentCostGp = specificMaterialComponentCostGp;
        Definition.specificMaterialComponentConsumed = specificMaterialComponentConsumed;
        return this;
    }

    internal SpellDefinitionBuilder SetEffectDescription(EffectDescription effectDescription)
    {
        Definition.effectDescription = effectDescription;
        return this;
    }

    internal SpellDefinitionBuilder SetAiParameters(SpellAIParameters aiParameters)
    {
        Definition.aiParameters = aiParameters;
        return this;
    }

    internal SpellDefinitionBuilder SetUniqueInstance(bool uniqueInstance)
    {
        Definition.uniqueInstance = uniqueInstance;
        return this;
    }

    internal SpellDefinitionBuilder SetConcentrationAction(ActionDefinitions.ActionParameter concentrationAction)
    {
        Definition.concentrationAction = concentrationAction;
        return this;
    }
    
    #region Constructors

    internal SpellDefinitionBuilder(string name, string guid) : base(name, guid)
    {
        InitializeFields();
    }

    internal SpellDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
    {
        InitializeFields();
    }

    internal SpellDefinitionBuilder(SpellDefinition original, string name, string guid) : base(original, name, guid)
    {
        InitializeFields();
    }

    internal SpellDefinitionBuilder(SpellDefinition original, string name, Guid guidNamespace) : base(original, name,
        guidNamespace)
    {
        InitializeFields();
    }

    #endregion Constructors
}
