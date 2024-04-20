using System.Collections;
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
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;
using static SolastaUnfinishedBusiness.Subclasses.CommonBuilders;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class InnovationVivisectionist : AbstractSubclass
{
    private const string Name = "InnovationVivisectionist";

    public InnovationVivisectionist()
    {
        //
        // MAIN
        //

        // LEVEL 03

        // Auto Prepared Spells

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InventorVivisectionist")
            .AddPreparedSpellGroup(3, Bless, InflictWounds)
            .AddPreparedSpellGroup(5, EnhanceAbility, LesserRestoration)
            .AddPreparedSpellGroup(9, RemoveCurse, Revivify)
            .AddPreparedSpellGroup(13, DeathWard, IdentifyCreatures)
            .AddPreparedSpellGroup(17, Contagion, RaiseDead)
            .AddToDB();

        // Medical Accuracy

        var additionalDamageMedicalAccuracy = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}MedicalAccuracy")
            .SetGuiPresentation(Category.Feature)
            .SetNotificationTag("MedicalAccuracy")
            .SetDamageDice(DieType.D6, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 4, 3)
            .SetRequiredProperty(RestrictedContextRequiredProperty.FinesseOrRangeWeapon)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.AdvantageOrNearbyAlly)
            .SetFrequencyLimit(FeatureLimitedUsage.OncePerTurn)
            .AddToDB();

        // Emergency Surgery

        var powerEmergencySurgery = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EmergencySurgery")
            .SetGuiPresentation(Category.Feature, PowerDomainInsightForeknowledge)
            .SetUsesProficiencyBonus(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Touch, 0, TargetType.IndividualsUnique)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetHealingForm(
                                HealingComputation.Dice, 0, DieType.D6, 1, false, HealingCap.MaximumHitPoints)
                            .Build())
                    .Build())
            .AddToDB();

        powerEmergencySurgery.AddCustomSubFeatures(new ModifyEffectDescriptionEmergencySurgery(powerEmergencySurgery));

        // LEVEL 05

        // Extra Attack

        // Emergency Cure

        var powerEmergencyCure = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}EmergencyCure")
            .SetGuiPresentation(Category.Feature, PowerOathOfJugementPurgeCorruption)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .AddToDB();

        var powerEmergencyCureLesserRestoration = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}EmergencyCureLesserRestoration")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerEmergencyCure)
            .SetEffectDescription(LesserRestoration.EffectDescription)
            .AddToDB();

        var powerEmergencyCureRemoveCurse = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{Name}EmergencyCureRemoveCurse")
            .SetGuiPresentation(Category.Feature)
            .SetSharedPool(ActivationTime.BonusAction, powerEmergencyCure)
            .SetEffectDescription(RemoveCurse.EffectDescription)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerEmergencyCure, false,
            powerEmergencyCureLesserRestoration, powerEmergencyCureRemoveCurse);

        // LEVEL 09

        // Stable Surgery

        var dieRollModifierStableSurgery = FeatureDefinitionDieRollModifierBuilder
            .Create($"DieRollModifier{Name}StableSurgery")
            .SetGuiPresentation(Category.Feature)
            .SetModifiers(
                RollContext.HealValueRoll,
                0,
                2,
                0,
                $"Feature/&DieRollModifier{Name}StableSurgeryReroll")
            .AddToDB();

        // Organ Donation

        var powerOrganDonation = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}OrganDonation")
            .SetGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost, RechargeRate.ShortRest)
            .AddToDB();

        powerOrganDonation.AddCustomSubFeatures(
            ModifyPowerVisibility.Hidden,
            new OnReducedToZeroHpByMeOrganDonation(powerOrganDonation, powerEmergencySurgery, powerEmergencyCure));

        // LEVEL 15

        // Master Emergency Surgery

        var powerMasterEmergencySurgery = FeatureDefinitionPowerBuilder
            .Create(powerEmergencySurgery, $"Power{Name}MasterEmergencySurgery")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.BonusAction)
            .SetOverriddenPower(powerEmergencySurgery)
            .AddToDB();

        powerMasterEmergencySurgery.AddCustomSubFeatures(
            new ModifyEffectDescriptionEmergencySurgery(powerMasterEmergencySurgery));

        // Master Emergency Cure

        var powerMasterEmergencyCure = FeatureDefinitionPowerBuilder
            .Create(powerEmergencyCure, $"Power{Name}MasterEmergencyCure")
            .SetOrUpdateGuiPresentation(Category.Feature)
            .SetUsesProficiencyBonus(ActivationTime.NoCost)
            .SetOverriddenPower(powerEmergencyCure)
            .AddToDB();

        var powerMasterEmergencyCureLesserRestoration = FeatureDefinitionPowerSharedPoolBuilder
            .Create(powerEmergencyCureLesserRestoration, $"Power{Name}MasterEmergencyCureLesserRestoration")
            .SetSharedPool(ActivationTime.NoCost, powerMasterEmergencyCure)
            .AddToDB();

        var powerMasterEmergencyCureRemoveCurse = FeatureDefinitionPowerSharedPoolBuilder
            .Create(powerEmergencyCureRemoveCurse, $"Power{Name}MasterEmergencyCureRemoveCurse")
            .SetSharedPool(ActivationTime.NoCost, powerMasterEmergencyCure)
            .AddToDB();

        PowerBundle.RegisterPowerBundle(powerMasterEmergencyCure, false,
            powerMasterEmergencyCureLesserRestoration, powerMasterEmergencyCureRemoveCurse);

        // MAIN

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.InventorVivisectionist, 256))
            .AddFeaturesAtLevel(3,
                autoPreparedSpells,
                additionalDamageMedicalAccuracy,
                powerEmergencySurgery)
            .AddFeaturesAtLevel(5,
                PowerCasterFightingWarMagic,
                powerEmergencyCure)
            .AddFeaturesAtLevel(9,
                dieRollModifierStableSurgery,
                powerOrganDonation)
            .AddFeaturesAtLevel(15,
                powerMasterEmergencySurgery,
                powerMasterEmergencyCure)
            .AddToDB();
    }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override CharacterClassDefinition Klass => InventorClass.Class;
    internal override FeatureDefinitionSubclassChoice SubclassChoice => InventorClass.SubclassChoice;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    private sealed class ModifyEffectDescriptionEmergencySurgery(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatureDefinitionPower baseDefinition)
        : IModifyEffectDescription
    {
        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == baseDefinition;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var intelligence = character.TryGetAttributeValue(AttributeDefinitions.Intelligence);
            var intelligenceModifier = AttributeDefinitions.ComputeAbilityScoreModifier(intelligence);
            var levels = character.GetClassLevel(InventorClass.Class);
            var diceNumber = levels switch
            {
                >= 19 => 5,
                >= 15 => 4,
                >= 11 => 3,
                >= 7 => 2,
                _ => 1
            };

            var healingForm = effectDescription.EffectForms[0].HealingForm;

            if (healingForm == null)
            {
                return effectDescription;
            }

            healingForm.diceNumber = diceNumber;
            healingForm.bonusHealing = intelligenceModifier;

            return effectDescription;
        }
    }

    private class OnReducedToZeroHpByMeOrganDonation(
        FeatureDefinitionPower powerOrganDonation,
        FeatureDefinitionPower powerEmergencySurgery,
        FeatureDefinitionPower powerEmergencyCure)
        : IOnReducedToZeroHpByMe
    {
        public IEnumerator HandleReducedToZeroHpByMe(
            GameLocationCharacter attacker,
            GameLocationCharacter downedCreature,
            RulesetAttackMode attackMode,
            RulesetEffect activeEffect)
        {
            if (ServiceRepository.GetService<IGameLocationBattleService>() is not GameLocationBattleManager
                {
                    IsBattleInProgress: true
                } battleManager)
            {
                yield break;
            }

            var rulesetAttacker = attacker.RulesetCharacter;

            if (rulesetAttacker.GetRemainingPowerUses(powerOrganDonation) == 0)
            {
                yield break;
            }

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var implementationManager =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(powerOrganDonation, rulesetAttacker);
            var reactionParams = new CharacterActionParams(attacker, ActionDefinitions.Id.SpendPower)
            {
                StringParameter = "OrganDonation",
                RulesetEffect = implementationManager
                    .MyInstantiateEffectPower(rulesetAttacker, usablePower, false),
                UsablePower = usablePower
            };
            var count = actionService.PendingReactionRequestGroups.Count;

            actionService.ReactToSpendPower(reactionParams);

            yield return battleManager.WaitForReactions(attacker, actionService, count);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetAttacker.UsePower(usablePower);

            rulesetAttacker.LogCharacterUsedPower(powerOrganDonation);

            var usablePowerEmergencyCure = PowerProvider.Get(powerEmergencyCure, rulesetAttacker);

            rulesetAttacker.RepayPowerUse(usablePowerEmergencyCure);

            var usablePowerEmergencySurgery = PowerProvider.Get(powerEmergencySurgery, rulesetAttacker);

            rulesetAttacker.RepayPowerUse(usablePowerEmergencySurgery);
        }
    }
}
