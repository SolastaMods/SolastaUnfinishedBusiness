using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.Models;
using static FeatureDefinitionAttributeModifier;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.CharacterSubclassDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

public static class InnovationVitriolist
{
    private const string Name = "InnovationVitriolist";

    public static CharacterSubclassDefinition Build()
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
            .SetCustomSubFeatures(HasModifiedUses.Marker)
            .AddToDB();

        var powerUseModifierMixtureIntMod = FeatureDefinitionPowerUseModifierBuilder
            .Create($"PowerUseModifier{Name}MixtureIntMod")
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
            .AddFeatures(FeatureDefinitionAttributeModifierBuilder
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
                    .SetParticleEffectParameters(AcidSplash)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionCorroded))
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
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
                    .SetParticleEffectParameters(AcidArrow)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(conditionMiserable))
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
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
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
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
                    .SetParticleEffectParameters(PowerDragonBreath_Acid)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionConfused))
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        var mixturePowers = new FeatureDefinition[] { powerCorrosion, powerMisery, powerAffliction, powerViscosity };

        var featureSetMixture = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Mixture")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerMixture, powerUseModifierMixtureIntMod, powerUseModifierMixtureProficiencyBonus)
            .AddFeatureSet(mixturePowers)
            .AddToDB();

        // LEVEL 05

        // Vitriolic Infusion

        var additionalDamageInfusion = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}Infusion")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag("Infusion")
            .SetDamageValueDetermination(AdditionalDamageValueDetermination.ProficiencyBonus)
            .SetSpecificDamageType(DamageTypeAcid)
            .SetCustomSubFeatures(
                new RestrictedContextValidator((_, _, _, _, _, mode, effect) =>
                    (OperationType.Set, (mode != null && mode.EffectDescription.EffectForms.Any(x =>
                                            x.FormType == EffectForm.EffectFormType.Damage &&
                                            x.DamageForm.DamageType == DamageTypeAcid)) ||
                                        (effect != null && effect.EffectDescription.EffectForms.Any(x =>
                                            x.FormType == EffectForm.EffectFormType.Damage &&
                                            x.DamageForm.DamageType == DamageTypeAcid)))))
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
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddToDB();

        // determine power visibility based on mixture and spell slots remaining usages
        powerRefundMixture.SetCustomSubFeatures(new CustomBehaviorRefundMixture(powerMixture, powerRefundMixture));

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

        var additionalDamageArsenal = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}Arsenal")
            .SetGuiPresentationNoContent(true)
            .AddConditionOperation(ConditionOperationDescription.ConditionOperation.Add, conditionArsenal)
            .SetCustomSubFeatures(new RestrictedContextValidator((_, _, _, _, _, _, effect) =>
                (OperationType.Set,
                    effect != null &&
                    effect.SourceDefinition != null &&
                    mixturePowers.Contains(effect.SourceDefinition))))
            .AddToDB();

        // Vitriolic Arsenal - Bypass Resistance and Change Immunity to Resistance

        var featureArsenal = FeatureDefinitionBuilder
            .Create($"Feature{Name}Arsenal")
            .SetGuiPresentation(Category.Feature)
            .AddToDB();

        featureArsenal.SetCustomSubFeatures(new ModifyDamageAffinityArsenal(featureArsenal));

        // Vitriolic Arsenal

        var featureSetArsenal = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Arsenal")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerRefundMixture, additionalDamageArsenal, featureArsenal)
            .AddToDB();

        // LEVEL 15

        // Vitriolic Paragon

        var powerParagon = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}Paragon")
            .SetGuiPresentation(Category.Feature)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 6, TargetType.Individuals)
                    .SetDurationData(DurationType.Round, 5)
                    .SetParticleEffectParameters(HoldMonster)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.Negates, TurnOccurenceType.EndOfTurn, true)
                            .SetConditionForm(ConditionDefinitions.ConditionParalyzed,
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .SetCustomSubFeatures(PowerVisibilityModifier.Hidden)
            .AddToDB();

        // BEHAVIORS

        powerMixture.AddCustomSubFeatures(new SpendPowerFinishedByMeParagon(powerParagon, mixturePowers));
        PowerBundle.RegisterPowerBundle(powerMixture, true, mixturePowers.OfType<FeatureDefinitionPower>());

        // MAIN

        return CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, TraditionShockArcanist)
            .AddFeaturesAtLevel(3, autoPreparedSpells, featureSetMixture)
            .AddFeaturesAtLevel(5, featureSetVitriolicInfusion)
            .AddFeaturesAtLevel(9, featureSetArsenal)
            .AddFeaturesAtLevel(15, powerParagon)
            .AddToDB();
    }

    //
    // Refund Mixture
    //

    private class CustomBehaviorRefundMixture : IPowerUseValidity, IUsePowerFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerMixture;
        private readonly FeatureDefinitionPower _powerRefundMixture;

        public CustomBehaviorRefundMixture(
            FeatureDefinitionPower powerMixture,
            FeatureDefinitionPower powerRefundMixture)
        {
            _powerMixture = powerMixture;
            _powerRefundMixture = powerRefundMixture;
        }

        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            var spellRepertoire = character.GetClassSpellRepertoire(InventorClass.Class);

            if (spellRepertoire == null)
            {
                return false;
            }

            var canUsePowerMixture = character.CanUsePower(_powerMixture);
            var hasSpellSlotsAvailable = spellRepertoire.GetLowestAvailableSlotLevel() > 0;

            return !canUsePowerMixture && hasSpellSlotsAvailable;
        }

        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            if (power != _powerRefundMixture)
            {
                yield break;
            }

            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;
            var usablePower = UsablePowersProvider.Get(_powerMixture, rulesetCharacter);

            rulesetCharacter.RepayPowerUse(usablePower);

            var spellRepertoire = rulesetCharacter.GetClassSpellRepertoire(InventorClass.Class);

            if (spellRepertoire == null)
            {
                yield break;
            }

            var slotLevel = spellRepertoire.GetLowestAvailableSlotLevel();

            spellRepertoire.SpendSpellSlot(slotLevel);
        }
    }

    //
    // Arsenal
    //

    private sealed class ModifyDamageAffinityArsenal : IModifyDamageAffinity
    {
        private readonly FeatureDefinition _featureArsenal;

        public ModifyDamageAffinityArsenal(FeatureDefinition featureArsenal)
        {
            _featureArsenal = featureArsenal;
        }

        public void ModifyDamageAffinity(RulesetActor defender, RulesetActor attacker, List<FeatureDefinition> features)
        {
            var resistanceCount = features.RemoveAll(x =>
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

            if (attacker is RulesetCharacter rulesetCharacter && (resistanceCount > 0 || immunityCount > 0))
            {
                rulesetCharacter.LogCharacterUsedFeature(_featureArsenal);
            }
        }
    }

    //
    // Paragon
    //

    private sealed class SpendPowerFinishedByMeParagon : IUsePowerFinishedByMe
    {
        private readonly List<FeatureDefinition> _mixturePowers = new();
        private readonly FeatureDefinitionPower _powerParagon;

        public SpendPowerFinishedByMeParagon(
            FeatureDefinitionPower powerParagon,
            params FeatureDefinition[] mixturePowers)
        {
            _powerParagon = powerParagon;
            _mixturePowers.AddRange(mixturePowers);
        }

        public IEnumerator OnUsePowerFinishedByMe(CharacterActionUsePower action, FeatureDefinitionPower power)
        {
            var gameLocationDefender = action.actionParams.TargetCharacters[0];
            var rulesetDefender = gameLocationDefender.RulesetCharacter;

            if (rulesetDefender is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var rulesetAttacker = action.ActingCharacter.RulesetCharacter;

            if (!_mixturePowers.Contains(power) || rulesetAttacker.GetClassLevel(InventorClass.Class) < 15)
            {
                yield break;
            }

            var usablePower = UsablePowersProvider.Get(_powerParagon, rulesetAttacker);

            ServiceRepository.GetService<IRulesetImplementationService>()
                .InstantiateEffectPower(rulesetAttacker, usablePower, false)
                .AddAsActivePowerToSource()
                .ApplyEffectOnCharacter(rulesetDefender, true, gameLocationDefender.LocationPosition);
        }
    }
}
