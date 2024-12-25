using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.Classes;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static ActionDefinitions;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
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
            .AddPreparedSpellGroup(13, Blight, SpellsContext.VitriolicSphere)
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
            .AddToDB();

        powerMixture.AddCustomSubFeatures(
            HasModifiedUses.Marker,
            new ModifyPowerPoolAmount
            {
                PowerPool = powerMixture,
                Type = PowerPoolBonusCalculationType.AttributeModifier,
                Attribute = AttributeDefinitions.Intelligence
            },
            new ModifyPowerPoolAmount
            {
                PowerPool = powerMixture,
                Type = PowerPoolBonusCalculationType.Attribute,
                Attribute = AttributeDefinitions.ProficiencyBonus
            });

        // Corrosion

        var conditionCorroded = ConditionDefinitionBuilder
            .Create($"Condition{Name}Corroded")
            .SetGuiPresentation(Category.Condition, Gui.EmptyContent, ConditionDefinitions.ConditionHeatMetal)
            .SetConditionType(ConditionType.Detrimental)
            // need special duration because of SetUseSpellAttack
            .SetSpecialDuration(DurationType.Round, 1)
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
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
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
            // required as in a feature set
            .AddCustomSubFeatures(ClassHolder.Inventor, ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Misery

        var conditionMiserable = ConditionDefinitionBuilder
            .Create($"Condition{Name}Miserable")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionAcidArrowed)
            .SetConditionType(ConditionType.Detrimental)
            // need special duration because of SetUseSpellAttack
            .SetSpecialDuration(DurationType.Round, 1)
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
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
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
            // required as in a feature set
            .AddCustomSubFeatures(ClassHolder.Inventor, ModifyPowerVisibility.Hidden)
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
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
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
            // required as in a feature set
            .AddCustomSubFeatures(ClassHolder.Inventor, ModifyPowerVisibility.Hidden)
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
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.SpellCastingFeature)
                    .RollSaveOnlyIfRelevantForms()
                    .SetParticleEffectParameters(PowerDomainOblivionMarkOfFate)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeAcid, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 20, (7, 1), (14, 2), (18, 3))
                            .Build(),
                        EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionHindered))
                    .Build())
            // required as in a feature set
            .AddCustomSubFeatures(ClassHolder.Inventor, ModifyPowerVisibility.Hidden)
            .AddToDB();

        // Mixture

        var mixturePowers = new FeatureDefinition[] { powerCorrosion, powerMisery, powerAffliction, powerViscosity };

        var featureSetMixture = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Mixture")
            .SetGuiPresentation(Category.Feature)
            .SetFeatureSet(powerMixture)
            .AddFeatureSet(mixturePowers)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerMixture, true, mixturePowers.OfType<FeatureDefinitionPower>());

        // LEVEL 05

        // Vitriolic Infusion

        var conditionInfusionMark = ConditionDefinitionBuilder
            .Create($"Condition{Name}InfusionMark")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialInterruptions(
                ConditionInterruption.Attacks,
                ConditionInterruption.CastSpellExecuted,
                (ConditionInterruption)ExtraConditionInterruption.SpendPowerExecuted,
                ConditionInterruption.UsePowerExecuted)
            .AddToDB();

        // kept name for backward compatibility
        var powerDamageInfusion = FeatureDefinitionPowerBuilder
            .Create($"AdditionalDamage{Name}Infusion")
            .SetGuiPresentation($"FeatureSet{Name}Infusion", Category.Feature, hidden: true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetBonusMode(AddBonusMode.Proficiency)
                            .SetDamageForm(DamageTypeAcid)
                            .Build())
                    .SetImpactEffectParameters(PowerDragonbornBreathWeaponBlack)
                    .Build())
            .AddToDB();

        powerDamageInfusion.AddCustomSubFeatures(
            new CustomBehaviorVitriolicInfusion(powerDamageInfusion, conditionInfusionMark));

        var featureSetVitriolicInfusion = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}Infusion")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(powerDamageInfusion, DamageAffinityAcidResistance)
            .AddToDB();

        // LEVEL 09

        // Vitriolic Arsenal - Refund Mixture

        var powerRefundMixture = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}RefundMixture")
            .SetGuiPresentation(Category.Feature, PowerDomainInsightForeknowledge)
            .SetUsesFixed(ActivationTime.Action)
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
            .SetSpecialDuration(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
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
            .AddCustomSubFeatures(new ModifyDamageAffinityArsenal())
            .AddToDB();

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

        // MAIN

        powerMixture.AddCustomSubFeatures(
            new ModifyEffectDescriptionMixture(
                conditionArsenal, ConditionDefinitions.ConditionIncapacitated, mixturePowers));

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.InventorVitriolist, 256))
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
    // Mixtures - add shocked at 9 and paralyzed at 15
    //

    private sealed class ModifyEffectDescriptionMixture(
        ConditionDefinition conditionArsenal,
        ConditionDefinition conditionParagon,
        params FeatureDefinition[] mixturePowers) : IModifyEffectDescription
    {
        private readonly EffectForm _effectConditionArsenal = EffectFormBuilder.ConditionForm(conditionArsenal);

        private readonly EffectForm _effectConditionParagon = EffectFormBuilder
            .Create()
            .HasSavingThrow(EffectSavingThrowType.Negates)
            .SetConditionForm(conditionParagon, ConditionForm.ConditionOperation.Add)
            .Build();

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

            // Arsenal - add shocked at 9
            if (levels >= 9 && mixturePowers.Contains(definition))
            {
                effectDescription.EffectForms.TryAdd(_effectConditionArsenal);
            }

            // Paragon - add paralyzed at 15
            if (levels >= 15 && mixturePowers.Contains(definition))
            {
                effectDescription.EffectForms.TryAdd(_effectConditionParagon);
            }

            return effectDescription;
        }
    }

    //
    // Refund Mixture
    //

    private sealed class CustomBehaviorRefundMixture(FeatureDefinitionPower powerMixture)
        : IPowerOrSpellFinishedByMe
    {
        public IEnumerator OnPowerOrSpellFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var battleManager = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (!battleManager)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var atLeastOneSpellSlotAvailable = false;
            var spellRepertoire = rulesetCharacter.GetClassSpellRepertoire(InventorClass.Class);

            for (var spellLevel = 1; spellLevel <= spellRepertoire!.MaxSpellLevelOfSpellCastingLevel; spellLevel++)
            {
                spellRepertoire.GetSlotsNumber(spellLevel, out var remaining, out _);

                if (remaining <= 0)
                {
                    continue;
                }

                atLeastOneSpellSlotAvailable = true;

                break;
            }

            if (!atLeastOneSpellSlotAvailable)
            {
                yield break;
            }

            var reactionParams = new CharacterActionParams(actingCharacter, Id.SpendSpellSlot)
            {
                ActionModifiers = { new ActionModifier() }
            };

            yield return battleManager.PrepareAndReactWithSpellUsingSpellSlot(
                actingCharacter, spellRepertoire, "RefundMixture", reactionParams);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            var slotUsed = reactionParams.IntParameter;
            var usablePower = PowerProvider.Get(powerMixture, rulesetCharacter);

            rulesetCharacter.UpdateUsageForPowerPool(-slotUsed, usablePower);
        }
    }

    //
    // Arsenal - bypass acid resistance / change acid immunity to acid resistance
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

    //
    // Vitriolic Infusion
    //

    private sealed class CustomBehaviorVitriolicInfusion(
        FeatureDefinitionPower powerVitriolicInfusion,
        ConditionDefinition conditionVitriolicInfusionMark)
        : IMagicEffectInitiatedByMe, IPhysicalAttackInitiatedByMe, IMagicEffectFinishedByMe, IPhysicalAttackFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(
            CharacterAction action,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            if (action.ActionParams.RulesetEffect.SourceDefinition == powerVitriolicInfusion)
            {
                yield break;
            }

            var damagedTargets = new List<GameLocationCharacter>();

            foreach (var target in targets)
            {
                var rulesetTarget = target.RulesetActor;

                rulesetTarget.DamageReceived -= DamageReceivedHandler;

                if (rulesetTarget.HasConditionOfCategoryAndType(
                        AttributeDefinitions.TagEffect, conditionVitriolicInfusionMark.Name))
                {
                    damagedTargets.Add(target);
                }
            }

            if (damagedTargets.Count > 0)
            {
                InflictDamage(attacker, damagedTargets);
            }
        }

        public IEnumerator OnMagicEffectInitiatedByMe(
            CharacterAction action,
            RulesetEffect activeEffect,
            GameLocationCharacter attacker,
            List<GameLocationCharacter> targets)
        {
            foreach (var target in targets)
            {
                target.RulesetActor.DamageReceived += DamageReceivedHandler;
            }

            yield break;
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetDefender = defender.RulesetActor;

            rulesetDefender.DamageReceived -= DamageReceivedHandler;

            if (rulesetDefender.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionVitriolicInfusionMark.Name))
            {
                InflictDamage(attacker, [defender]);
            }

            yield break;
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
        {
            defender.RulesetActor.DamageReceived += DamageReceivedHandler;

            yield break;
        }

        private void DamageReceivedHandler(
            RulesetActor rulesetDefender,
            int damage,
            string receivedDamageType,
            ulong sourceGuid,
            RollInfo rollInfo)
        {
            if (receivedDamageType != DamageTypeAcid)
            {
                return;
            }

            var rulesetAttacker = EffectHelpers.GetCharacterByGuid(sourceGuid);

            if (rulesetAttacker == null ||
                rulesetAttacker.HasConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, conditionVitriolicInfusionMark.Name))
            {
                return;
            }

            rulesetDefender.InflictCondition(
                conditionVitriolicInfusionMark.Name,
                DurationType.Round,
                0,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                rulesetAttacker.guid,
                rulesetAttacker.CurrentFaction.Name,
                1,
                conditionVitriolicInfusionMark.Name,
                0,
                0,
                0);
        }

        private void InflictDamage(GameLocationCharacter attacker, List<GameLocationCharacter> targets)
        {
            var rulesetAttacker = attacker.RulesetCharacter;
            var usablePower = PowerProvider.Get(powerVitriolicInfusion, rulesetAttacker);

            // vitriolic infusion damage is a use at will power
            attacker.MyExecuteActionSpendPower(usablePower, [.. targets]);
        }
    }
}
