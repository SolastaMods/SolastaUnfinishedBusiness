using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;
using static RuleDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

internal sealed class MartialSpellShield : AbstractSubclass
{
    public const string Name = "MartialSpellShield";

    internal MartialSpellShield()
    {
        var magicAffinitySpellShieldConcentrationAdvantage = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellShieldConcentrationAdvantage")
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, SpellParamsModifierType.None, 0,
                SpellParamsModifierType.FlatValue, true)
            .AddToDB();

        var castSpellSpellShield = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellSpellShield")
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .SetSpellKnowledge(SpellKnowledge.Selection)
            .SetSpellReadyness(SpellReadyness.AllKnown)
            .SetSlotsRecharge(RechargeRate.LongRest)
            .SetReplacedSpells(4, 1)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .SetKnownSpells(4, FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .SetSlotsPerLevel(FeatureDefinitionCastSpellBuilder.CasterProgression.ThirdCaster)
            .AddToDB();

        var conditionSpellShieldWarMagic = ConditionDefinitionBuilder
            .Create("ConditionSpellShieldWarMagic")
            .SetGuiPresentationNoContent(true)
            .AddFeatures(FeatureDefinitionAttackModifiers.AttackModifierBerserkerFrenzy)
            .AddToDB();

        var effect = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(
                Side.Enemy,
                RangeType.Self,
                0,
                TargetType.Self)
            .SetDurationData(DurationType.Round, 0, false)
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
            .SetRechargeRate(RechargeRate.AtWill)
            .SetEffectDescription(effect)
            .SetActivationTime(ActivationTime.OnSpellCast)
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
                    sourceName = "VigorSpell", sourceType = FeatureSourceType.ExplicitFeature
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
            .SetConditionType(ConditionType.Beneficial)
            .SetAllowMultipleInstances(false)
            .SetDuration(DurationType.Round, 1)
            .AddToDB();

        var arcaneDeflection = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(Side.Ally, RangeType.Self, 1,
                TargetType.Self, 1, 0)
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
                0, UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                ActivationTime.Reaction, 0, RechargeRate.AtWill,
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
            .AddFeaturesAtLevel(3,
                magicAffinitySpellShieldConcentrationAdvantage,
                castSpellSpellShield)
            .AddFeaturesAtLevel(7,
                powerSpellShieldWarMagic,
                replaceAttackWithCantripSpellShield)
            .AddFeaturesAtLevel(10,
                magicAffinitySpellShieldConcentrationAdvantageVigor)
            .AddFeaturesAtLevel(15,
                powerSpellShieldArcaneDeflection)
            .AddFeaturesAtLevel(18,
                actionAffinitySpellShieldRangedDefense)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }

    internal override FeatureDefinitionSubclassChoice SubclassChoice =>
        FeatureDefinitionSubclassChoices.SubclassChoiceFighterMartialArchetypes;

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

        public FeatureSourceType sourceType { get; set; }
        public string sourceName { get; set; }
    }
}
