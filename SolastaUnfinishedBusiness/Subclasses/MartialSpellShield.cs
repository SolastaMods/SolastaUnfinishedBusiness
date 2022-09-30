using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialSpellShield : AbstractSubclass
{
    public const string Name = "MartialSpellShield";

    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal MartialSpellShield()
    {
        var magicAffinitySpellShieldConcentrationAdvantage = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellShieldConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.None, 0,
                RuleDefinitions.SpellParamsModifierType.FlatValue, true)
            .AddToDB();

        var castSpellSpellShield = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellSpellShield")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .SetKnownSpells(4, FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster);

        var conditionSpellShieldWarMagic = ConditionDefinitionBuilder
            .Create("ConditionSpellShieldWarMagic")
            .SetGuiPresentationNoContent(true)
            .AddFeatures(FeatureDefinitionAttackModifiers.AttackModifierBerserkerFrenzy)
            .AddToDB();

        var effect = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(
                RuleDefinitions.Side.Enemy,
                RuleDefinitions.RangeType.Self,
                0,
                RuleDefinitions.TargetType.Self)
            .SetDurationData(RuleDefinitions.DurationType.Round, 0, false)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionSpellShieldWarMagic, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();

        effect.canBePlacedOnCharacter = true;

        var powerSpellShieldWarMagic = FeatureDefinitionPowerBuilder
            .Create("PowerSpellShieldWarMagic")
            .SetGuiPresentation(Category.Feature)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(effect)
            .SetActivationTime(RuleDefinitions.ActivationTime.OnSpellCast)
            .AddToDB();

        // replace attack with cantrip
        var replaceAttackWithCantripSpellShield = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripSpellShield")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var magicAffinitySpellShieldConcentrationAdvantageVigor = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellShieldConcentrationAdvantageVigor")
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new VigorSpellDcModifier(),
                new VigorSpellAttackModifier
                {
                    sourceName = "VigorSpell", sourceType = RuleDefinitions.FeatureSourceType.ExplicitFeature
                })
            .AddToDB();

        var conditionSpellShieldArcaneDeflection = ConditionDefinitionBuilder
            .Create("ConditionSpellShieldArcaneDeflection")
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierSpellShieldArcaneDeflection")
                .SetModifier(FeatureDefinitionAttributeModifier.AttributeModifierOperation.Additive,
                    AttributeDefinitions.ArmorClass, 3)
                .SetGuiPresentation("ConditionSpellShieldArcaneDeflection", Category.Condition,
                    ConditionShielded.GuiPresentation.SpriteReference)
                .AddToDB())
            .SetConditionType(RuleDefinitions.ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(RuleDefinitions.DurationType.Round, 1)
            .AddToDB();

        var arcaneDeflection = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Ally, RuleDefinitions.RangeType.Self, 1,
                RuleDefinitions.TargetType.Self, 1, 0)
            .AddEffectForm(EffectFormBuilder
                .Create()
                .CreatedByCharacter()
                .SetConditionForm(conditionSpellShieldArcaneDeflection, ConditionForm.ConditionOperation.Add, true,
                    true)
                .Build())
            .Build();

        var powerSpellShieldArcaneDeflection = FeatureDefinitionPowerBuilder
            .Create("PowerSpellShieldArcaneDeflection")
            .SetGuiPresentation(Category.Feature, ConditionShielded.GuiPresentation.SpriteReference)
            .Configure(
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.Reaction, 0, RuleDefinitions.RechargeRate.AtWill,
                false, false, AttributeDefinitions.Intelligence, arcaneDeflection /* unique instance */)
            .AddToDB();

        var actionAffinitySpellShieldRangedDefense = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinityTraditionGreenMageLeafScales,
                "ActionAffinitySpellShieldRangedDefense")
            .SetGuiPresentation("PowerSpellShieldRangedDeflection", Category.Feature)
            .AddToDB();

        // Make Spell Shield subclass

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, DomainBattle.GuiPresentation.SpriteReference)
            .AddFeaturesAtLevel(3, magicAffinitySpellShieldConcentrationAdvantage)
            .AddFeaturesAtLevel(3, castSpellSpellShield.AddToDB())
            .AddFeaturesAtLevel(7, powerSpellShieldWarMagic)
            .AddFeaturesAtLevel(7, replaceAttackWithCantripSpellShield)
            .AddFeaturesAtLevel(10, magicAffinitySpellShieldConcentrationAdvantageVigor)
            .AddFeaturesAtLevel(15, powerSpellShieldArcaneDeflection)
            .AddFeaturesAtLevel(18, actionAffinitySpellShieldRangedDefense)
            .AddToDB();
    }

    internal override FeatureDefinitionSubclassChoice GetSubclassChoiceList()
    {
        return FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;
    }

    internal override CharacterSubclassDefinition GetSubclass()
    {
        return Subclass;
    }

    private static int CalculateModifier([NotNull] RulesetCharacter myself)
    {
        if (myself == null)
        {
            throw new ArgumentNullException(nameof(myself));
        }

        var strModifier =
            AttributeDefinitions.ComputeAbilityScoreModifier(myself.GetAttribute(AttributeDefinitions.Strength)
                .CurrentValue);
        var dexModifier =
            AttributeDefinitions.ComputeAbilityScoreModifier(myself.GetAttribute(AttributeDefinitions.Dexterity)
                .CurrentValue);
        return Math.Max(strModifier, dexModifier);
    }

    private sealed class VigorSpellDcModifier : IIncreaseSpellDC
    {
        public int GetSpellModifier(RulesetCharacter caster)
        {
            return CalculateModifier(caster);
        }
    }

    private sealed class VigorSpellAttackModifier : IIncreaseSpellAttackRoll
    {
        public int GetSpellAttackRollModifier(RulesetCharacter caster)
        {
            return CalculateModifier(caster);
        }

        public RuleDefinitions.FeatureSourceType sourceType { get; set; }
        public string sourceName { get; set; }
    }
}
