using System;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.ConditionDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses.Fighter;

internal sealed class MartialSpellShield : AbstractSubclass
{
    // ReSharper disable once InconsistentNaming
    private readonly CharacterSubclassDefinition Subclass;

    internal MartialSpellShield()
    {
        var magicAffinity = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellShieldConcentrationAdvantage", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetConcentrationModifiers(RuleDefinitions.ConcentrationAffinity.Advantage, 0)
            .SetHandsFullCastingModifiers(true, true, true)
            .SetCastingModifiers(0, RuleDefinitions.SpellParamsModifierType.None, 0,
                RuleDefinitions.SpellParamsModifierType.FlatValue, true, false, false)
            .AddToDB();

        var spellCasting = FeatureDefinitionCastSpellBuilder
            .Create("CastSpellSpellShield", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetSpellCastingOrigin(FeatureDefinitionCastSpell.CastingOrigin.Subclass)
            .SetSpellCastingAbility(AttributeDefinitions.Intelligence)
            .SetSpellList(SpellListDefinitions.SpellListWizard)
            .SetSpellKnowledge(RuleDefinitions.SpellKnowledge.Selection)
            .SetSpellReadyness(RuleDefinitions.SpellReadyness.AllKnown)
            .SetSlotsRecharge(RuleDefinitions.RechargeRate.LongRest)
            .SetReplacedSpells(SpellsSlotsContext.OneThirdCasterReplacedSpells)
            .SetKnownCantrips(3, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetKnownSpells(4, 3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER)
            .SetSlotsPerLevel(3, FeatureDefinitionCastSpellBuilder.CasterProgression.THIRD_CASTER);

        var conditionWarMagic = ConditionDefinitionBuilder
            .Create("ConditionSpellShieldWarMagic", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentationNoContent(true)
            .AddFeatures(FeatureDefinitionAttackModifiers.AttackModifierBerserkerFrenzy)
            .AddToDB();

        var effect = EffectDescriptionBuilder
            .Create()
            .SetTargetingData(RuleDefinitions.Side.Enemy, RuleDefinitions.RangeType.Self, 0,
                RuleDefinitions.TargetType.Self)
            .SetDurationData(RuleDefinitions.DurationType.Round, 0, false)
            .SetEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetConditionForm(conditionWarMagic, ConditionForm.ConditionOperation.Add)
                    .Build()
            )
            .Build();
        effect.canBePlacedOnCharacter = true;
        effect.targetExcludeCaster = false;

        var warMagicPower = FeatureDefinitionPowerBuilder
            .Create("PowerSpellShieldWarMagic", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
            .SetEffectDescription(effect)
            .SetActivationTime(RuleDefinitions.ActivationTime.OnSpellCast)
            .AddToDB();

        // replace attack with cantrip
        var replaceAttackWithCantrip = FeatureDefinitionReplaceAttackWithCantripBuilder
            .Create("ReplaceAttackWithCantripSpellShield", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        var vigor = FeatureDefinitionMagicAffinityBuilder
            .Create("MagicAffinitySpellShieldConcentrationAdvantageVigor", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature)
            .SetCustomSubFeatures(new VigorSpellDcModifier(),
                new VigorSpellAttackModifier
                {
                    sourceName = "VigorSpell", sourceType = RuleDefinitions.FeatureSourceType.ExplicitFeature
                })
            .AddToDB();

        var deflectionCondition = ConditionDefinitionBuilder
            .Create("ConditionSpellShieldArcaneDeflection", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Condition)
            .AddFeatures(FeatureDefinitionAttributeModifierBuilder
                .Create("AttributeModifierSpellShieldArcaneDeflection", DefinitionBuilder.CENamespaceGuid)
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
                .SetConditionForm(deflectionCondition, ConditionForm.ConditionOperation.Add, true, true)
                .Build())
            .Build();

        var arcaneDeflectionPower = FeatureDefinitionPowerBuilder
            .Create("PowerSpellShieldArcaneDeflection", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Feature, ConditionShielded.GuiPresentation.SpriteReference)
            .Configure(
                0, RuleDefinitions.UsesDetermination.AbilityBonusPlusFixed, AttributeDefinitions.Intelligence,
                RuleDefinitions.ActivationTime.Reaction, 0, RuleDefinitions.RechargeRate.AtWill,
                false, false, AttributeDefinitions.Intelligence, arcaneDeflection, false /* unique instance */)
            .AddToDB();

        var actionAffinitySpellShieldRangedDefense = FeatureDefinitionActionAffinityBuilder
            .Create(FeatureDefinitionActionAffinitys.ActionAffinityTraditionGreenMageLeafScales,
                "ActionAffinitySpellShieldRangedDefense", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation("PowerSpellShieldRangedDeflection", Category.Feature)
            .AddToDB();

        // Make Spell Shield subclass

        Subclass = CharacterSubclassDefinitionBuilder
            .Create("MartialSpellShield", DefinitionBuilder.CENamespaceGuid)
            .SetGuiPresentation(Category.Subclass, DomainBattle.GuiPresentation.SpriteReference)
            .AddFeatureAtLevel(magicAffinity, 3)
            .AddFeatureAtLevel(spellCasting.AddToDB(), 3)
            .AddFeatureAtLevel(warMagicPower, 7)
            .AddFeatureAtLevel(replaceAttackWithCantrip, 7)
            .AddFeatureAtLevel(vigor, 10)
            .AddFeatureAtLevel(arcaneDeflectionPower, 15)
            .AddFeatureAtLevel(actionAffinitySpellShieldRangedDefense, 18).AddToDB();
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
