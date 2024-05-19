using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static AttributeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionAdditionalDamages;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class MartialSpellShield : AbstractSubclass
{
    private const string Name = "SpellShield";
    internal const string FullName = $"Martial{Name}";

    public MartialSpellShield()
    {
        // Spell Casting

        var castSpell = FeatureDefinitionCastSpellBuilder
            .Create($"CastSpell{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(Intelligence)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetKnownSpells(4, FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.OneThird)
            .AddToDB();

        // LEVEL 10

        // Blade Weaving

        var conditionBladeWeaving = ConditionDefinitionBuilder
            .Create($"Condition{Name}BladeWeaving")
            .SetGuiPresentation(Category.Condition, ConditionDazzled)
            .SetConditionType(ConditionType.Detrimental)
            .SetPossessive()
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
            .SetSpecialInterruptions(ConditionInterruption.SavingThrow)
            .SetFeatures(
                FeatureDefinitionSavingThrowAffinityBuilder
                    .Create($"SavingThrowAffinity{Name}BladeWeaving")
                    .SetGuiPresentation($"Condition{Name}BladeWeaving", Category.Condition, Gui.NoLocalization)
                    .SetAffinities(CharacterSavingThrowAffinity.Disadvantage, false,
                        Strength,
                        Dexterity,
                        Constitution,
                        Intelligence,
                        Wisdom,
                        Charisma)
                    .AddToDB())
            .AddToDB();

        // kept name for backward compatibility
        var additionalDamageBladeWeaving = FeatureDefinitionAdditionalDamageBuilder
            .Create($"MagicAffinity{Name}CombatMagicVigor")
            .SetGuiPresentation(Category.Feature)
            .SetRequiredProperty(RestrictedContextRequiredProperty.Weapon)
            .SetAttackModeOnly()
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionBladeWeaving)
            .SetImpactParticleReference(AdditionalDamageHalfOrcSavageAttacks.impactParticleReference)
            .AddToDB();

        // LEVEL 07

        // Cantrip Attack

        // War Magic

        // LEVEL 15

        // Arcane Deflection

        var conditionArcaneDeflection = ConditionDefinitionBuilder
            .Create($"Condition{Name}ArcaneDeflection")
            .SetGuiPresentation($"Power{Name}ArcaneDeflection", Category.Feature, ConditionShielded)
            .AddFeatures(
                MagicAffinityConditionShielded,
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}ArcaneDeflection")
                    .SetGuiPresentation($"Power{Name}ArcaneDeflection", Category.Feature)
                    .SetModifier(
                        AttributeModifierOperation.Additive,
                        ArmorClass,
                        3)
                    .AddToDB())
            .AddToDB();

        var powerArcaneDeflection = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}ArcaneDeflection")
            .SetGuiPresentation(Category.Feature, ConditionShielded)
            .SetUsesFixed(ActivationTime.Reaction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Round, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionArcaneDeflection))
                    .SetParticleEffectParameters(Shield)
                    .Build())
            .AddToDB();

        // LEVEL 18

        // Protective Barrier

        // kept name for backward compatibility
        var powerProtectiveBarrier = FeatureDefinitionPowerBuilder
            .Create($"ActionAffinity{Name}RangedDefense")
            .SetGuiPresentation($"Power{Name}RangedDeflection", Category.Feature, PowerTraditionCourtMageSpellShield)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique, 4)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{Name}ProtectiveBarrier")
                                .SetGuiPresentation(Category.Condition, MageArmor)
                                .SetPossessive()
                                .SetFeatures(
                                    FeatureDefinitionAttributeModifierBuilder
                                        .Create($"AttributeModifier{Name}ProtectiveBarrier")
                                        .SetGuiPresentation($"Condition{Name}ProtectiveBarrier", Category.Condition)
                                        .SetModifier(
                                            AttributeModifierOperation.Additive,
                                            ArmorClass, 2)
                                        .AddToDB())
                                .AddToDB()))
                    .SetParticleEffectParameters(PowerTraditionCourtMageSpellShield)
                    .Build())
            .AddToDB();

        Subclass = CharacterSubclassDefinitionBuilder
            .Create($"Martial{Name}")
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.MartialSpellShield, 256))
            .AddFeaturesAtLevel(3, MagicAffinityCasterFightingCombatMagicImproved, castSpell)
            .AddFeaturesAtLevel(7, AttackReplaceWithCantripCasterFighting, PowerCasterFightingWarMagic)
            .AddFeaturesAtLevel(10, additionalDamageBladeWeaving)
            .AddFeaturesAtLevel(15, powerArcaneDeflection)
            .AddFeaturesAtLevel(18, powerProtectiveBarrier)
            .AddToDB();
    }

    internal override CharacterClassDefinition Klass => CharacterClassDefinitions.Fighter;

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }
}
