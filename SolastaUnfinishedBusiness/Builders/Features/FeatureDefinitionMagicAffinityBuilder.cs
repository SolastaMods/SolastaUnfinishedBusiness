using System;
using System.Linq;
using JetBrains.Annotations;

namespace SolastaUnfinishedBusiness.Builders.Features;

[UsedImplicitly]
internal class FeatureDefinitionMagicAffinityBuilder
    : DefinitionBuilder<FeatureDefinitionMagicAffinity, FeatureDefinitionMagicAffinityBuilder>
{
    internal FeatureDefinitionMagicAffinityBuilder SetConcentrationModifiers(
        RuleDefinitions.ConcentrationAffinity concentrationAffinity,
        int threshold = -1)
    {
        Definition.concentrationAffinity = concentrationAffinity;

        if (threshold > 0)
        {
            Definition.overConcentrationThreshold = threshold;
        }

        return this;
    }

    internal FeatureDefinitionMagicAffinityBuilder SetHandsFullCastingModifiers(
        bool weapon,
        bool weaponOrShield,
        bool weaponAsFocus)
    {
        Definition.somaticWithWeaponOrShield = weaponOrShield;
        Definition.somaticWithWeapon = weapon;
        Definition.canUseProficientWeaponAsFocus = weaponAsFocus;

        return this;
    }

    internal FeatureDefinitionMagicAffinityBuilder SetRitualCasting(RuleDefinitions.RitualCasting ritual)
    {
        Definition.ritualCasting = ritual;
        return this;
    }

    internal FeatureDefinitionMagicAffinityBuilder SetCastingModifiers(
        int attackModifier = 0,
        RuleDefinitions.SpellParamsModifierType attackModifierType = RuleDefinitions.SpellParamsModifierType.FlatValue,
        int dcModifier = 0,
        RuleDefinitions.SpellParamsModifierType dcModifierType = RuleDefinitions.SpellParamsModifierType.FlatValue,
        bool noProximityPenalty = false,
        bool cantripRetribution = false,
        bool halfDamageCantrips = false)
    {
        Definition.spellAttackModifierType = attackModifierType;
        Definition.spellAttackModifier = attackModifier;
        Definition.rangeSpellNoProximityPenalty = noProximityPenalty;
        Definition.saveDCModifierType = dcModifierType;
        Definition.saveDCModifier = dcModifier;
        Definition.cantripRetribution = cantripRetribution;
        Definition.forceHalfDamageOnCantrips = halfDamageCantrips;

        return this;
    }

    internal FeatureDefinitionMagicAffinityBuilder SetWarList(
        int levelBonus,
        params SpellDefinition[] spells)
    {
        Definition.usesWarList = true;
        Definition.warListSlotBonus = levelBonus;
        Definition.WarListSpells.AddRange(spells.Select(s => s.Name));
        Definition.WarListSpells.Sort();

        return this;
    }

    internal FeatureDefinitionMagicAffinityBuilder SetSpellWithModifiedSaveDc(
        SpellDefinition spellDefinition,
        int bonus)
    {
        Definition.spellWithModifiedSaveDC = spellDefinition;
        Definition.bonusToEffectSaveDC = bonus;
        Definition.addBonusToEffectSaveDC = SpellAndPowersDefinitions.RulesetEffectSaveDCBonusType.Spell;
        return this;
    }

    internal FeatureDefinitionMagicAffinityBuilder SetSpellLearnAndPrepModifiers(
        float scribeDurationMultiplier,
        float scribeCostMultiplier,
        int additionalScribedSpells,
        RuleDefinitions.AdvantageType scribeAdvantage,
        RuleDefinitions.PreparedSpellsModifier preparedModifier)
    {
        Definition.scribeCostMultiplier = scribeCostMultiplier;
        Definition.scribeDurationMultiplier = scribeDurationMultiplier;
        Definition.additionalScribedSpells = additionalScribedSpells;
        Definition.scribeAdvantageType = scribeAdvantage;
        Definition.preparedSpellModifier = preparedModifier;

        return this;
    }


    internal FeatureDefinitionMagicAffinityBuilder SetExtendedSpellList(SpellListDefinition spellListDefinition)
    {
        Definition.extendedSpellList = spellListDefinition;

        return this;
    }

    internal FeatureDefinitionMagicAffinityBuilder IgnoreClassRestrictionsOnMagicalItems()
    {
        Definition.ignoreClassRestrictionsOnMagicalItems = true;

        return this;
    }

    #region Constructors

    protected FeatureDefinitionMagicAffinityBuilder(string name, Guid namespaceGuid)
        : base(name, namespaceGuid)
    {
    }

    protected FeatureDefinitionMagicAffinityBuilder(FeatureDefinitionMagicAffinity original, string name,
        Guid namespaceGuid)
        : base(original, name, namespaceGuid)
    {
    }

    #endregion
}
