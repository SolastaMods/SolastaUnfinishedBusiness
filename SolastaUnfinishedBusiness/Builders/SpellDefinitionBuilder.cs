using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Builders;

[UsedImplicitly]
internal class SpellDefinitionBuilder : DefinitionBuilder<SpellDefinition, SpellDefinitionBuilder>
{
    private void InitializeFields()
    {
        // should fix lots of official spells getting modified by spells on this mod
        var copy = new EffectDescription();

        copy.Copy(Definition.effectDescription);
        Definition.effectDescription = copy;
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
        Definition.spellsBundle = subspells.Length != 0;
        return this;
    }

    internal SpellDefinitionBuilder SetCastingTime(ActivationTime castingTime)
    {
        Definition.castingTime = castingTime;
        return this;
    }

    internal SpellDefinitionBuilder SetRitualCasting(ActivationTime ritualCastingTime)
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

    internal SpellDefinitionBuilder SetVocalSpellSameType(VocalSpellSemeType type)
    {
        Definition.vocalSpellSemeType = type;
        return this;
    }

    internal SpellDefinitionBuilder SetSomaticComponent(bool somaticComponent)
    {
        Definition.somaticComponent = somaticComponent;
        return this;
    }

    internal SpellDefinitionBuilder SetMaterialComponent(MaterialComponentType materialComponentType)
    {
        Definition.materialComponentType = materialComponentType;
        return this;
    }

    internal SpellDefinitionBuilder SetSpecificMaterialComponent(
        string specificMaterialComponentTag,
        int specificMaterialComponentCostGp,
        bool specificMaterialComponentConsumed)
    {
        Definition.materialComponentType = MaterialComponentType.Specific;
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

    internal SpellDefinitionBuilder SetUniqueInstance()
    {
        Definition.uniqueInstance = true;
        return this;
    }

    #region Constructors

    protected SpellDefinitionBuilder(string name, Guid guidNamespace) : base(name, guidNamespace)
    {
        InitializeFields();
    }

    protected SpellDefinitionBuilder(SpellDefinition original, string name, Guid guidNamespace)
        : base(original, name, guidNamespace)
    {
        InitializeFields();
    }

    #endregion Constructors
}
