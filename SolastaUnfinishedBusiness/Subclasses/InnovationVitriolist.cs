using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class InnovationVitriolist : AbstractSubclass
{
    private const string Name = "InnovationVitriolist";

    public InnovationVitriolist()
    {
        // LEVEL 03

        // Auto Prepared Spells

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InnovationVitriolist")
            .AddPreparedSpellGroup(3, SpellsContext.CausticZap, Shield)
            .AddPreparedSpellGroup(5, AcidArrow, Blindness)
            .AddPreparedSpellGroup(9, ProtectionFromEnergy, StinkingCloud)
            .AddPreparedSpellGroup(13, Blight, Stoneskin)
            .AddPreparedSpellGroup(17, CloudKill, Contagion)
            .AddToDB();

        // Vitriolic Mixtures

        var powerMixture = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Mixture")
            .SetGuiPresentation(Category.Feature, PowerPactChainSprite)
            .SetUsesFixed(ActivationTime.BonusAction, RechargeRate.LongRest, 1, 0)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .Build())
            .AddCustomSubFeatures(HasModifiedUses.Marker)
            .AddToDB();

        var powerUseModifierMixtureIntelligenceModifier = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}MixtureIntelligenceModifier")
            .SetGuiPresentationNoContent(true)
            .SetModifier(powerMixture, PowerPoolBonusCalculationType.AttributeMod, AttributeDefinitions.Intelligence)
            .AddToDB();

        var powerUseModifierMixtureProficiencyBonus = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}MixtureProficiencyBonus")
            .SetGuiPresentationNoContent(true)
            .SetModifier(powerMixture, PowerPoolBonusCalculationType.Attribute, AttributeDefinitions.ProficiencyBonus)
            .AddToDB();

        // Corrosion

        var conditionCorroded = ConditionDefinitionBuilder
            .Create($"Condition{Name}Corroded")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionHeavilyEncumbered)
            .SetConditionType(ConditionType.Detrimental)
            .AddFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{Name}Corroded")
                    .SetGuiPresentation($"Condition{Name}Corroded", Category.Condition)
                    .SetModifier(AttributeModifierOperation.Additive, AttributeDefinitions.ArmorClass, -2)
                    .AddToDB())
            .AddToDB();

        var powerCorrosion = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}Corrosion")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerMixture)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .RollSaveOnlyIfRelevantForms()
                    .SetParticleEffectParameters(AcidSplash)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionCorroded))
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Misery

        var conditionMiserable = ConditionDefinitionBuilder
            .Create($"Condition{Name}Miserable")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionAcidArrowed)
            .SetConditionType(ConditionType.Detrimental)
            .SetRecurrentEffectForms(
                EffectFormBuilder
                    .Create()
                    .SetDamageForm(DamageTypeAcid, 2, DieType.D4)
                    .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                    .SetCreatedBy()
                    .Build())
            .AddToDB();

        var powerMisery = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}Misery")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerMixture)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .RollSaveOnlyIfRelevantForms()
                    .SetParticleEffectParameters(AcidArrow)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionMiserable))
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Affliction

        var powerAffliction = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}Affliction")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerMixture)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .RollSaveOnlyIfRelevantForms()
                    .SetParticleEffectParameters(AcidSplash)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D4)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypePoison, 2, DieType.D4)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionPoisoned))
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Viscosity

        var powerViscosity = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}Viscosity")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.NoCost, powerMixture)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 1)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .RollSaveOnlyIfRelevantForms()
                    .SetParticleEffectParameters(PowerDragonBreath_Acid)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionConfused))
                    .Build())
            .AddCustomSubFeatures(ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Mixture

        var mixturePowers = new FeatureDefinition[] { powerCorrosion, powerMisery, powerAffliction, powerViscosity };

        var featureSetMixture = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Mixture")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerMixture, powerUseModifierMixtureIntelligenceModifier, powerUseModifierMixtureProficiencyBonus)
            .AddFeatureSet(mixturePowers)
            .AddToDB();

        // LEVEL 05

        // Vitriolic Infusion

        var additionalDamageInfusion = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}Infusion")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("Infusion")
            .SetAttackModeOnly()
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetSpecificDamageType(DamageTypeAcid)
            .AddToDB();

        var featureSetVitriolicInfusion = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Infusion")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(additionalDamageInfusion, DamageAffinityAcidResistance)
            .AddToDB();

        // LEVEL 09

        // Vitriolic Arsenal - Refund Mixture

        var powerRefundMixture = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RefundMixture")
            .SetGuiPresentation(Category.Feature, PowerDomainInsightForeknowledge)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorRefundMixture(powerMixture))
            .AddToDB();

        // Vitriolic Arsenal - Prevent Reactions

        var conditionArsenal = ConditionDefinitionBuilder
            .Create(ConditionDefinitions.ConditionShocked, $"Condition{Name}Arsenal")
            .SetGuiPresentation(Category.Condition)
            .SetFeatures(
                FeatureDefinitionActionAffinityBuilder
                    .Create($"ActionAffinity{Name}Arsenal")
                    .SetGuiPresentationNoContent(true)
                    .SetAllowedActionTypes(reaction: false)
                    .AddToDB())
            .AddToDB();

        // Vitriolic Arsenal - Bypass Resistance and Change Immunity to Resistance

        var featureArsenal = FeatureDefinitionBuilder
            .Create($"Feature{Name}Arsenal")
            .SetGuiPresentation($"FeatureSet{Name}Arsenal", Category.Feature)
            .AddToDB();

        featureArsenal.AddCustomSubFeatures(new ModifyDamageAffinityArsenal());

        // Vitriolic Arsenal

        var featureSetArsenal = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Arsenal")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerRefundMixture, featureArsenal)
            .AddToDB();

        // LEVEL 15

        // Vitriolic Paragon

        var featureParagon = FeatureDefinitionBuilder
            .Create($"Feature{Name}Paragon")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        // Vitriolic Mixtures - Behavior

        powerMixture.AddCustomSubFeatures(new ModifyMagicEffectAnyOnTargetMixture(conditionArsenal, mixturePowers));
        PowerBundle.RegisterPowerBundle(powerMixture, true, mixturePowers.OfType<FeatureDefinitionPower>());

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, TraditionShockArcanist)
            .AddFeaturesAtLevel(3, autoPreparedSpells, featureSetMixture)
            .AddFeaturesAtLevel(5, featureSetVitriolicInfusion)
            .AddFeaturesAtLevel(9, featureSetArsenal)
            .AddFeaturesAtLevel(15, featureParagon)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override CharacterClassDefinition Klass => InventorClass.Class;
    internal override FeatureDefinitionSubclassChoice SubclassChoice => InventorClass.SubclassChoice;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    //
    // Mixtures - Add additional PB damage to any acid damage / Add additional conditions at 9 and 15
    //

    private sealed class ModifyMagicEffectAnyOnTargetMixture : IModifyEffectDescription
    {
        private readonly ConditionDefinition _conditionArsenal;
        private readonly List<FeatureDefinition> _mixturePowers = [];

        public ModifyMagicEffectAnyOnTargetMixture(
            ConditionDefinition conditionArsenal,
            params FeatureDefinition[] mixturePowers)
        {
            _conditionArsenal = conditionArsenal;
            _mixturePowers.AddRange(mixturePowers);
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return character.GetClassLevel(InventorClass.Class) >= 5;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var levels = character.GetClassLevel(InventorClass.Class);

            // Infusion - add additional PB damage to any acid damage
            if (levels >= 5)
            {
                var pb = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);

                foreach (var effectForm in effectDescription.EffectForms
                             .Where(x => x.FormType == EffectForm.EffectFormType.Damage &&
                                         x.DamageForm.DamageType == DamageTypeAcid))
                {
                    effectForm.DamageForm.bonusDamage += pb;
                }
            }

            // Arsenal - add shocked at 9
            if (levels >= 9 && _mixturePowers.Contains(definition))
            {
                effectDescription.EffectForms.Add(EffectFormBuilder.ConditionForm(_conditionArsenal));
            }

            // Paragon - add paralyzed at 15
            if (levels >= 15 && _mixturePowers.Contains(definition))
            {
                effectDescription.EffectForms.Add(
                    EffectFormBuilder
                        .Create()
                        .HasSavingThrow(EffectSavingThrowType.Negates)
                        .SetConditionForm(ConditionDefinitions.ConditionParalyzed, ConditionForm.ConditionOperation.Add)
                        .Build());
            }

            return effectDescription;
        }
    }

    //
    // Refund Mixture
    //

    private sealed class CustomBehaviorRefundMixture(FeatureDefinitionPower powerMixture)
        : IValidatePowerUse, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerMixture, rulesetCharacter);

            rulesetCharacter.RepayPowerUse(usablePower);

            var spellRepertoire = rulesetCharacter.GetClassSpellRepertoire(InventorClass.Class);

            if (spellRepertoire == null)
            {
                yield break;
            }

            var slotLevel = spellRepertoire.GetLowestAvailableSlotLevel();

            spellRepertoire.SpendSpellSlot(slotLevel);
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var spellRepertoire = character.GetClassSpellRepertoire(InventorClass.Class);

            if (spellRepertoire == null)
            {
                return false;
            }

            var canUsePowerMixture = character.CanUsePower(powerMixture);
            var hasSpellSlotsAvailable = spellRepertoire.GetLowestAvailableSlotLevel() > 0;

            return !canUsePowerMixture && hasSpellSlotsAvailable;
        }
    }

    //
    // Arsenal - Bypass Acid Resistance / Change Acid Immunity to Acid Resistance
    //

    private sealed class ModifyDamageAffinityArsenal : IModifyDamageAffinity
    {
        public void ModifyDamageAffinity(
            RulesetActor defender,
            RulesetActor attacker,
            List<FeatureDefinition> features)
        {
            features.RemoveAll(x =>
                x is IDamageAffinityProvider
                {
                    DamageAffinityType: DamageAffinityType.Resistance, DamageType: DamageTypeAcid
                });

            var immunityCount = features.RemoveAll(x =>
                x is IDamageAffinityProvider
                {
                    DamageAffinityType: DamageAffinityType.Immunity, DamageType: DamageTypeAcid
                });

            if (immunityCount > 0)
            {
                features.Add(DamageAffinityAcidResistance);
            }
        }
    }
}
