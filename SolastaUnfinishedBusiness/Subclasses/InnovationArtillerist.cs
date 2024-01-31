using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
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
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static FeatureDefinitionAttributeModifier;
using static ActionDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionConditionAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionDamageAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMagicAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionPowers;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionSenses;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.MonsterDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.SpellDefinitions;

namespace SolastaUnfinishedBusiness.Subclasses;

[UsedImplicitly]
public sealed class InnovationArtillerist : AbstractSubclass
{
    private const string Name = "InnovationArtillerist";
    private const string CreatureTag = "EldritchCannon";
    private const string ConditionCommandCannon = $"Condition{Name}Command";
    private const string PowerSummonCannon = $"Power{Name}Summon";
    private const string EldritchCannon = "EldritchCannon";
    private const string ExplosiveCannon = "ExplosiveCannon";
    private const string FortifiedPosition = "FortifiedPosition";
    private const string Flamethrower = "Flamethrower";
    private const string ForceBallista = "ForceBallista";
    private const string Protector = "Protector";
    private const string ArcaneFirearm = "ArcaneFirearm";

    private static readonly LimitEffectInstances CannonLimiter =
        new(CreatureTag, character => character.GetClassLevel(InventorClass.Class) < 15 ? 1 : 2);

    public InnovationArtillerist()
    {
        #region COMMON

        var eldritchCannonSprite = Sprites.GetSprite(EldritchCannon, Resources.PowerEldritchCannon, 256, 128);

        // Cannon Powers

        var powerFlamethrower = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{Flamethrower}")
            .SetGuiPresentation(Category.Feature, FlameStrike)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(FlameStrike)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Cone, 3)
                    .ExcludeCaster()
                    .SetSavingThrowData(
                        false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.FixedValue, AttributeDefinitions.Intelligence, 15)
                    .SetParticleEffectParameters(FlameStrike)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeFire, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 1, 6, 3)
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetAlterationForm(AlterationForm.Type.LightUp)
                            .Build())
                    .Build())
            .AddToDB();

        var powerForceBallista = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{ForceBallista}")
            .SetGuiPresentation(Category.Feature, EldritchBlast)
            .SetUsesFixed(ActivationTime.Action)
            .SetUseSpellAttack()
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(EldritchBlast)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.All, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .SetParticleEffectParameters(EldritchBlast)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetDamageForm(DamageTypeForce, 2, DieType.D8)
                            .SetDiceAdvancement(LevelSourceType.ClassLevel, 0, 1, 6, 3)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 1)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(InventorModifyAdditionalDamageClassLevelHolder.Marker)
            .AddToDB();

        var powerProtector = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{Protector}")
            .SetGuiPresentation(Category.Feature, MassCureWounds)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(MassCureWounds)
                    .SetDurationData(DurationType.Minute, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Sphere, 2)
                    .SetParticleEffectParameters(MassCureWounds)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm(5, DieType.D8, 1)
                            .Build())
                    .Build())
            .AddToDB();

        // Actions Medium Cannon

        _ = ActionDefinitionBuilder
            .Create($"Action{Name}{Flamethrower}")
            .SetGuiPresentation($"Power{Name}{Flamethrower}", Category.Feature, powerFlamethrower)
            .OverrideClassName("UsePower")
            .SetStealthBreakerBehavior(StealthBreakerBehavior.RollIfTargets)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CannonFlamethrower)
            .SetActionType(ActionType.Main)
            .SetActivatedPower(powerFlamethrower)
            .SetFormType(ActionFormType.Large)
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create($"Action{Name}{ForceBallista}")
            .SetGuiPresentation($"Power{Name}{ForceBallista}", Category.Feature, powerForceBallista)
            .OverrideClassName("UsePower")
            .SetStealthBreakerBehavior(StealthBreakerBehavior.RollIfTargets)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CannonForceBallista)
            .SetActionType(ActionType.Main)
            .SetActivatedPower(powerForceBallista)
            .SetFormType(ActionFormType.Large)
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create($"Action{Name}{Protector}")
            .SetGuiPresentation($"Power{Name}{Protector}", Category.Feature, powerProtector)
            .OverrideClassName("UsePower")
            .SetStealthBreakerBehavior(StealthBreakerBehavior.RollIfTargets)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CannonProtector)
            .SetActionType(ActionType.Main)
            .SetActivatedPower(powerProtector)
            .SetFormType(ActionFormType.Large)
            .AddToDB();

        // Actions Tiny Cannon

        _ = ActionDefinitionBuilder
            .Create($"Action{Name}{Flamethrower}Tiny")
            .SetGuiPresentation($"Power{Name}{Flamethrower}", Category.Feature, powerFlamethrower)
            .OverrideClassName("UsePower")
            .SetStealthBreakerBehavior(StealthBreakerBehavior.RollIfTargets)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CannonFlamethrowerBonus)
            .SetActionType(ActionType.Bonus)
            .SetActivatedPower(powerFlamethrower)
            .SetFormType(ActionFormType.Large)
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create($"Action{Name}{ForceBallista}Tiny")
            .SetGuiPresentation($"Power{Name}{ForceBallista}", Category.Feature, powerForceBallista)
            .OverrideClassName("UsePower")
            .SetStealthBreakerBehavior(StealthBreakerBehavior.RollIfTargets)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CannonForceBallistaBonus)
            .SetActionType(ActionType.Bonus)
            .SetActivatedPower(powerForceBallista)
            .SetFormType(ActionFormType.Large)
            .AddToDB();

        _ = ActionDefinitionBuilder
            .Create($"Action{Name}{Protector}Tiny")
            .SetGuiPresentation($"Power{Name}{Protector}", Category.Feature, powerProtector)
            .OverrideClassName("UsePower")
            .SetStealthBreakerBehavior(StealthBreakerBehavior.RollIfTargets)
            .RequiresAuthorization()
            .SetActionId(ExtraActionId.CannonProtectorBonus)
            .SetActionType(ActionType.Bonus)
            .SetActivatedPower(powerProtector)
            .SetFormType(ActionFormType.Large)
            .AddToDB();

        #endregion

        #region LEVEL 03

        // LEVEL 03

        // Auto Prepared Spell

        var autoPreparedSpells = FeatureDefinitionAutoPreparedSpellsBuilder
            .Create($"AutoPreparedSpells{Name}")
            .SetGuiPresentation(Category.Feature)
            .SetSpellcastingClass(InventorClass.Class)
            .SetAutoTag("InventorArtillerist")
            .AddPreparedSpellGroup(3, Shield, Thunderwave)
            .AddPreparedSpellGroup(5, ScorchingRay, Shatter)
            .AddPreparedSpellGroup(9, Fireball, WindWall)
            .AddPreparedSpellGroup(13, IceStorm, WallOfFire)
            .AddPreparedSpellGroup(17, ConeOfCold, WallOfForce)
            .AddToDB();

        // Summoning Affinities

        var hpBonus = FeatureDefinitionAttributeModifierBuilder
            .Create($"AttributeModifier{Name}{EldritchCannon}HitPoints")
            .SetGuiPresentationNoContent(true)
            .SetModifier(AttributeModifierOperation.AddConditionAmount, AttributeDefinitions.HitPoints)
            .AddToDB();

        var summoningAffinityEldritchCannon = FeatureDefinitionSummoningAffinityBuilder
            .Create($"SummoningAffinity{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetRequiredMonsterTag(CreatureTag)
            .SetAddedConditions(
                // required for dice advancement on cannon powers
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}{EldritchCannon}CopyCharacterLevel")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCopyAttributeFromSummoner,
                        AttributeDefinitions.CharacterLevel)
                    .AddToDB(),
                ConditionDefinitionBuilder
                    .Create($"Condition{Name}{EldritchCannon}HitPoints")
                    .SetGuiPresentationNoContent(true)
                    .SetSilent(Silent.WhenAddedOrRemoved)
                    .SetAmountOrigin(ExtraOriginOfAmount.SourceCharacterLevel)
                    .SetFeatures(hpBonus, hpBonus, hpBonus, hpBonus, hpBonus)
                    .AddToDB())
            .AddToDB();

        // Command Cannon

        var conditionEldritchCannonCommand = ConditionDefinitionBuilder
            .Create(ConditionCommandCannon)
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetSpecialDuration(DurationType.Round, 1, TurnOccurenceType.StartOfTurn)
            .AddToDB();

        var powerEldritchCannonCommand = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{EldritchCannon}Command")
            .SetGuiPresentation(Category.Feature, Command)
            .SetUsesFixed(ActivationTime.BonusAction)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionEldritchCannonCommand, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new ShowInCombatWhenHasCannon())
            .AddToDB();

        powerEldritchCannonCommand.AddCustomSubFeatures(
            new ApplyOnTurnEnd(conditionEldritchCannonCommand, powerEldritchCannonCommand));

        // Dismiss Cannon

        var powerEldritchCannonDismiss = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{EldritchCannon}Dismiss")
            .SetGuiPresentation(Category.Feature, PowerSorcererManaPainterTap)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetRestrictedCreatureFamilies(InventorClass.InventorConstructFamily)
                    .SetParticleEffectParameters(Counterspell)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetCounterForm(CounterForm.CounterType.DismissCreature, 0, 0, false, false)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(new ShowWhenHasCannon())
            .AddToDB();

        // Refund Cannon

        var powerEldritchCannonRefund = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{EldritchCannon}Refund")
            .SetGuiPresentation(Category.Feature, PowerDomainInsightForeknowledge)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .Build())
            .AddCustomSubFeatures(new CustomBehaviorRefundCannon())
            .AddToDB();

        // Eldritch Cannon

        var powerEldritchCannonPool = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{EldritchCannon}Activate")
            .SetGuiPresentation(Category.Feature, eldritchCannonSprite)
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest)
            // only for UI reasons
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.All, RangeType.Distance, 1, TargetType.Position)
                    .Build())
            .AddToDB();

        var powerFlamethrower03 = BuildFlamethrowerPower(powerEldritchCannonPool, 3);
        var powerForceBallista03 = BuildForceBallistaPower(powerEldritchCannonPool, 3);
        var powerProtector03 = BuildProtectorPower(powerEldritchCannonPool, 3);
        var powerTinyFlamethrower03 = BuildTinyFlamethrowerPower(powerEldritchCannonPool, 3);
        var powerTinyForceBallista03 = BuildTinyForceBallistaPower(powerEldritchCannonPool, 3);
        var powerTinyProtector03 = BuildTinyProtectorPower(powerEldritchCannonPool, 3);

        var featureSetEldritchCannon = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{EldritchCannon}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                summoningAffinityEldritchCannon,
                powerEldritchCannonCommand,
                powerEldritchCannonDismiss,
                powerEldritchCannonRefund,
                powerEldritchCannonPool,
                powerFlamethrower03,
                powerForceBallista03,
                powerProtector03,
                powerTinyFlamethrower03,
                powerTinyForceBallista03,
                powerTinyProtector03)
            .AddToDB();

        #endregion

        #region LEVEL 05

        // LEVEL 05

        // Arcane Firearm

        const string ARCANE_FIREARM = $"FeatureSet{Name}{ArcaneFirearm}";

        var additionalDamageArcaneFirearm = FeatureDefinitionAdditionalDamageBuilder
            .Create($"AdditionalDamage{Name}{ArcaneFirearm}")
            .SetGuiPresentationNoContent(true)
            .SetNotificationTag(ArcaneFirearm)
            .SetDamageDice(DieType.D8, 1)
            .SetAdvancement(AdditionalDamageAdvancement.ClassLevel, 1, 1, 10, 5)
            .SetRequiredProperty(RestrictedContextRequiredProperty.SpellWithAttackRoll)
            .SetTriggerCondition(AdditionalDamageTriggerCondition.SpellDamagesTarget)
            .AddToDB();

        var featureSetArcaneFirearm = FeatureDefinitionFeatureSetBuilder
            .Create(ARCANE_FIREARM)
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                additionalDamageArcaneFirearm,
                MagicAffinitySpellBladeIntoTheFray)
            .AddToDB();

        #endregion

        #region LEVEL 09

        // Eldritch Detonation

        const string ELDRITCH_DETONATION = $"Power{Name}{EldritchCannon}Detonate";

        var powerDetonateSelf = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}{EldritchCannon}DetonateSelf")
            .SetGuiPresentation(ELDRITCH_DETONATION, Category.Feature, Fireball)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create(Fireball)
                    .SetDurationData(DurationType.Instantaneous)
                    .SetTargetingData(Side.All, RangeType.Self, 0, TargetType.Sphere, 4)
                    .SetParticleEffectParameters(Fireball)
                    .SetSavingThrowData(false, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.FixedValue, AttributeDefinitions.Wisdom, 17)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .HasSavingThrow(EffectSavingThrowType.HalfDamage)
                            .SetDamageForm(DamageTypeForce, 3, DieType.D8)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(
                    ConditionFlamethrower.Name, ConditionForceBallista.Name, ConditionProtector.Name)))
            .AddToDB();

        var powerDetonate = FeatureDefinitionPowerBuilder
            .Create(ELDRITCH_DETONATION)
            .SetGuiPresentation(Category.Feature, Fireball)
            .SetUsesFixed(ActivationTime.Action)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 12, TargetType.IndividualsUnique)
                    .SetTargetFiltering(TargetFilteringMethod.CharacterOnly)
                    .SetRestrictedCreatureFamilies(InventorClass.InventorConstructFamily)
                    .SetParticleEffectParameters(Counterspell)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetCounterForm(CounterForm.CounterType.DismissCreature, 0, 0, false, false)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(
                new ShowWhenHasCannon(),
                new MagicEffectFinishedByMeEldritchDetonation(powerDetonateSelf))
            .AddToDB();

        // Explosive Cannon

        var powerExplosiveCannonPool = FeatureDefinitionPowerBuilder
            .Create(powerEldritchCannonPool, $"Power{Name}{ExplosiveCannon}Activate")
            .SetOverriddenPower(powerEldritchCannonPool)
            .AddToDB();

        var powerFlamethrower09 = BuildFlamethrowerPower(powerExplosiveCannonPool, 9);
        var powerForceBallista09 = BuildForceBallistaPower(powerExplosiveCannonPool, 9);
        var powerProtector09 = BuildProtectorPower(powerExplosiveCannonPool, 9);
        var powerTinyFlamethrower09 = BuildTinyFlamethrowerPower(powerExplosiveCannonPool, 9);
        var powerTinyForceBallista09 = BuildTinyForceBallistaPower(powerExplosiveCannonPool, 9);
        var powerTinyProtector09 = BuildTinyProtectorPower(powerExplosiveCannonPool, 9);

        var featureSetExplosiveCannon = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{ExplosiveCannon}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerExplosiveCannonPool,
                powerDetonate,
                powerDetonateSelf,
                powerFlamethrower09,
                powerForceBallista09,
                powerProtector09,
                powerTinyFlamethrower09,
                powerTinyForceBallista09,
                powerTinyProtector09)
            .AddToDB();

        #endregion

        #region LEVEL 15

        // Aura of Half-Cover

        var combatAffinityFortifiedPosition = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{Name}{FortifiedPosition}")
            .SetGuiPresentation(powerEldritchCannonPool.Name, Category.Feature, Gui.NoLocalization)
            .SetPermanentCover(CoverType.Half)
            .AddToDB();

        var conditionFortifiedPosition = ConditionDefinitionBuilder
            .Create($"Condition{Name}{FortifiedPosition}")
            .SetGuiPresentation(Category.Condition, DatabaseHelper.ConditionDefinitions.ConditionBlessed)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(combatAffinityFortifiedPosition)
            .AddToDB();

        var powerFortifiedPosition = FeatureDefinitionPowerBuilder
            .Create(PowerPaladinAuraOfProtection, $"Power{Name}{FortifiedPosition}Aura")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        // keep it simple and ensure it'll follow any changes from Aura of Protection
        powerFortifiedPosition.EffectDescription.EffectForms[0] = EffectFormBuilder
            .Create()
            .SetConditionForm(conditionFortifiedPosition, ConditionForm.ConditionOperation.Add)
            .Build();

        var powerFortifiedPositionTiny = FeatureDefinitionPowerBuilder
            .Create(powerFortifiedPosition, $"Power{Name}{FortifiedPosition}AuraTiny")
            .SetGuiPresentationNoContent(true)
            .AddCustomSubFeatures(
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAnyOfConditions(
                    ConditionFlamethrower.Name, ConditionForceBallista.Name, ConditionProtector.Name)))
            .AddToDB();

        // Fortified Position

        var powerFortifiedPositionPool = FeatureDefinitionPowerBuilder
            .Create(powerEldritchCannonPool, $"Power{Name}{FortifiedPosition}Activate")
            .SetUsesFixed(ActivationTime.Action, RechargeRate.LongRest, 1, 2)
            .SetOverriddenPower(powerExplosiveCannonPool)
            .AddToDB();

        var powerFlamethrower15 = BuildFlamethrowerPower(powerFortifiedPositionPool, 15, powerFortifiedPosition);
        var powerForceBallista15 = BuildForceBallistaPower(powerFortifiedPositionPool, 15, powerFortifiedPosition);
        var powerProtector15 = BuildProtectorPower(powerFortifiedPositionPool, 15, powerFortifiedPosition);
        var powerTinyFlamethrower15 = BuildTinyFlamethrowerPower(powerFortifiedPositionPool, 15);
        var powerTinyForceBallista15 = BuildTinyForceBallistaPower(powerFortifiedPositionPool, 15);
        var powerTinyProtector15 = BuildTinyProtectorPower(powerFortifiedPositionPool, 15);

        var featureSetFortifiedPosition = FeatureDefinitionFeatureSetBuilder
            .Create($"FeatureSet{Name}{FortifiedPosition}")
            .SetGuiPresentation(Category.Feature)
            .AddFeatureSet(
                powerFortifiedPositionPool,
                powerFortifiedPositionTiny,
                powerFlamethrower15,
                powerForceBallista15,
                powerProtector15,
                powerTinyFlamethrower15,
                powerTinyForceBallista15,
                powerTinyProtector15)
            .AddToDB();

        #endregion

        #region MAIN

        //
        // MAIN
        //

        PowerBundle.RegisterPowerBundle(powerEldritchCannonPool, true,
            powerFlamethrower03, powerForceBallista03, powerProtector03,
            powerTinyFlamethrower03, powerTinyForceBallista03, powerTinyProtector03);

        PowerBundle.RegisterPowerBundle(powerExplosiveCannonPool, true,
            powerFlamethrower09, powerForceBallista09, powerProtector09,
            powerTinyFlamethrower09, powerTinyForceBallista09, powerTinyProtector09);

        PowerBundle.RegisterPowerBundle(powerFortifiedPositionPool, true,
            powerFlamethrower15, powerForceBallista15, powerProtector15,
            powerTinyFlamethrower15, powerTinyForceBallista15, powerTinyProtector15);

        Subclass = CharacterSubclassDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Subclass, Sprites.GetSprite(Name, Resources.InventorArtillerist, 256))
            .AddFeaturesAtLevel(3, autoPreparedSpells, featureSetEldritchCannon)
            .AddFeaturesAtLevel(5, featureSetArcaneFirearm)
            .AddFeaturesAtLevel(9, featureSetExplosiveCannon)
            .AddFeaturesAtLevel(15, featureSetFortifiedPosition)
            .AddToDB();

        #endregion
    }

    internal override CharacterSubclassDefinition Subclass { get; }
    internal override CharacterClassDefinition Klass => InventorClass.Class;
    internal override FeatureDefinitionSubclassChoice SubclassChoice => InventorClass.SubclassChoice;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    internal override DeityDefinition DeityDefinition { get; }

    #region REFUND CANNON

    private class CustomBehaviorRefundCannon : IValidatePowerUse, IMagicEffectFinishedByMe
    {
        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var rulesetCharacter = action.ActingCharacter.RulesetCharacter;

            foreach (var rulesetPower in rulesetCharacter.UsablePowers
                         .Where(x =>
                             x.PowerDefinition.Name.StartsWith($"Power{Name}") &&
                             x.PowerDefinition.Name.EndsWith("Activate")))
            {
                rulesetCharacter.RepayPowerUse(rulesetPower);
            }

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

            var hasUsedPowerActivate = character.UsablePowers
                .Any(x => x.RemainingUses == 0 &&
                          x.PowerDefinition.Name.StartsWith($"Power{Name}") &&
                          x.PowerDefinition.Name.EndsWith("Activate"));

            var hasSpellSlotsAvailable = spellRepertoire.GetLowestAvailableSlotLevel() > 0;

            return hasUsedPowerActivate && hasSpellSlotsAvailable;
        }
    }

    #endregion

    #region COMMON BLUEPRINTS

    private static readonly FeatureDefinitionActionAffinity ActionAffinityEldritchCannon =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetForbiddenActions(
                Id.AttackMain, Id.AttackOff, Id.AttackFree, Id.AttackReadied, Id.AttackOpportunity, Id.Ready,
                Id.PowerMain, Id.PowerBonus, Id.PowerReaction, Id.SpendPower, Id.Shove, Id.ShoveBonus, Id.ShoveFree)
            .AddCustomSubFeatures(new SummonerHasConditionOrKOd())
            .AddToDB();

    private static readonly FeatureDefinitionConditionAffinity ConditionAffinityEldritchCannon =
        FeatureDefinitionConditionAffinityBuilder
            .Create($"ConditionAffinity{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetConditionAffinityType(ConditionAffinityType.Immunity)
            .SetConditionType(DatabaseHelper.ConditionDefinitions.ConditionSurprised)
            .AddCustomSubFeatures(ForceInitiativeToSummoner.Mark)
            .AddToDB();

    private static readonly FeatureDefinitionMoveMode MoveModeEldritchCannon =
        FeatureDefinitionMoveModeBuilder
            .Create($"MoveMode{Name}{EldritchCannon}")
            .SetGuiPresentationNoContent(true)
            .SetMode(MoveMode.Walk, 3)
            .AddToDB();

    // Action Affinities Medium Cannon

    private static readonly FeatureDefinitionActionAffinity ActionAffinityFlamethrower =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{Flamethrower}")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CannonFlamethrower)
            .AddToDB();

    private static readonly ConditionDefinition ConditionFlamethrower = ConditionDefinitionBuilder
        .Create($"Condition{Name}{Flamethrower}")
        .SetGuiPresentation($"Power{Name}{Flamethrower}", Category.Feature)
        .SetPossessive()
        .SetFeatures(ActionAffinityFlamethrower)
        .AddToDB();

    private static readonly FeatureDefinitionActionAffinity ActionAffinityForceBallista =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{ForceBallista}")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CannonForceBallista)
            .AddToDB();

    private static readonly ConditionDefinition ConditionForceBallista = ConditionDefinitionBuilder
        .Create($"Condition{Name}{ForceBallista}")
        .SetGuiPresentation($"Power{Name}{ForceBallista}", Category.Feature)
        .SetPossessive()
        .SetFeatures(ActionAffinityForceBallista)
        .AddToDB();

    private static readonly FeatureDefinitionActionAffinity ActionAffinityProtector =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{Protector}")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CannonProtector)
            .AddToDB();

    private static readonly ConditionDefinition ConditionProtector = ConditionDefinitionBuilder
        .Create($"Condition{Name}{Protector}")
        .SetGuiPresentation($"Power{Name}{Protector}", Category.Feature)
        .SetPossessive()
        .SetFeatures(ActionAffinityProtector)
        .AddToDB();

    // Action Affinity Tiny Cannon

    private static readonly FeatureDefinitionActionAffinity ActionAffinityFlamethrowerTiny =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{Flamethrower}Tiny")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CannonFlamethrowerBonus)
            .AddToDB();

    private static readonly ConditionDefinition ConditionFlamethrowerTiny = ConditionDefinitionBuilder
        .Create($"Condition{Name}{Flamethrower}Tiny")
        .SetGuiPresentation($"Power{Name}{Flamethrower}", Category.Feature)
        .SetPossessive()
        .SetFeatures(ActionAffinityFlamethrowerTiny)
        .AddToDB();

    private static readonly FeatureDefinitionActionAffinity ActionAffinityForceBallistaTiny =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{ForceBallista}Tiny")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CannonForceBallistaBonus)
            .AddToDB();

    private static readonly ConditionDefinition ConditionForceBallistaTiny = ConditionDefinitionBuilder
        .Create($"Condition{Name}{ForceBallista}Tiny")
        .SetGuiPresentation($"Power{Name}{ForceBallista}", Category.Feature)
        .SetPossessive()
        .SetFeatures(ActionAffinityForceBallistaTiny)
        .AddToDB();

    private static readonly FeatureDefinitionActionAffinity ActionAffinityProtectorTiny =
        FeatureDefinitionActionAffinityBuilder
            .Create($"ActionAffinity{Name}{Protector}Tiny")
            .SetGuiPresentationNoContent(true)
            .SetAuthorizedActions((Id)ExtraActionId.CannonProtectorBonus)
            .AddToDB();

    private static readonly ConditionDefinition ConditionProtectorTiny = ConditionDefinitionBuilder
        .Create($"Condition{Name}{Protector}Tiny")
        .SetGuiPresentation($"Power{Name}{Protector}", Category.Feature)
        .SetPossessive()
        .SetFeatures(ActionAffinityProtectorTiny)
        .AddToDB();

    #endregion

    #region SMALL CANNON POWER

    private static FeatureDefinitionPowerSharedPool BuildFlamethrowerPower(FeatureDefinitionPower sharedPoolPower,
        int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var additionalFeatures = monsterAdditionalFeatures.ToList();

        additionalFeatures.Add(ActionAffinityFlamethrower);

        return BuildEldritchCannonPower(Flamethrower, sharedPoolPower, Fire_Spider, level, additionalFeatures);
    }

    private static FeatureDefinitionPowerSharedPool BuildForceBallistaPower(FeatureDefinitionPower sharedPoolPower,
        int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var additionalFeatures = monsterAdditionalFeatures.ToList();

        additionalFeatures.Add(ActionAffinityForceBallista);

        return BuildEldritchCannonPower(ForceBallista, sharedPoolPower, PhaseSpider, level, additionalFeatures);
    }

    private static FeatureDefinitionPowerSharedPool BuildProtectorPower(FeatureDefinitionPower sharedPoolPower,
        int level,
        params FeatureDefinition[] monsterAdditionalFeatures)
    {
        var additionalFeatures = monsterAdditionalFeatures.ToList();

        additionalFeatures.Add(ActionAffinityProtector);

        return BuildEldritchCannonPower(Protector, sharedPoolPower, SpectralSpider, level, additionalFeatures);
    }

    private static FeatureDefinitionPowerSharedPool BuildEldritchCannonPower(
        string powerName,
        FeatureDefinitionPower sharedPoolPower,
        MonsterDefinition monsterDefinition,
        int level,
        IEnumerable<FeatureDefinition> monsterAdditionalFeatures)
    {
        var name = PowerSummonCannon + powerName;
        var monster = BuildEldritchCannonMonster(powerName, monsterDefinition, level, monsterAdditionalFeatures);

        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(name + level)
            .SetGuiPresentation($"Power{Name}{powerName}", Category.Feature, hidden: true)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Distance, 1, TargetType.Position)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetSummonCreatureForm(1, monster.Name)
                            .Build())
                    .SetParticleEffectParameters(ConjureGoblinoids)
                    .Build())
            .SetUniqueInstance()
            .AddCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always, CannonLimiter)
            .AddToDB();

        return power;
    }

    private static MonsterDefinition BuildEldritchCannonMonster(
        string cannonName,
        MonsterDefinition monsterDefinition,
        int level,
        IEnumerable<FeatureDefinition> monsterAdditionalFeatures)
    {
        var monsterName = $"{Name}{cannonName}{level}";
        var presentationName = $"Power{Name}{cannonName}";

        var monster = MonsterDefinitionBuilder
            .Create(monsterDefinition, monsterName)
            .SetOrUpdateGuiPresentation(presentationName, Category.Feature)
            .SetSizeDefinition(DatabaseHelper.CharacterSizeDefinitions.Small)
            .SetMonsterPresentation(
                MonsterPresentationBuilder.Create()
                    .SetAllPrefab(monsterDefinition.MonsterPresentation)
                    .SetPhantom()
                    .SetModelScale(0.1f)
                    .SetHasMonsterPortraitBackground(true)
                    .SetCanGeneratePortrait(true)
                    .Build())
            .SetStandardHitPoints(1)
            .SetHeight(2)
            .NoExperienceGain()
            .SetArmorClass(18)
            .SetChallengeRating(0)
            .SetHitDice(DieType.D8, 1)
            .SetAbilityScores(10, 10, 10, 10, 10, 10)
            .SetDefaultFaction(DatabaseHelper.FactionDefinitions.Party)
            .SetCharacterFamily(InventorClass.InventorConstructFamily)
            .SetCreatureTags(CreatureTag)
            .SetBestiaryEntry(BestiaryDefinitions.BestiaryEntry.None)
            .SetFullyControlledWhenAllied(true)
            .SetDungeonMakerPresence(MonsterDefinition.DungeonMaker.None)
            .ClearAttackIterations()
            .SetFeatures(
                ActionAffinityEldritchCannon,
                ConditionAffinityEldritchCannon,
                MoveModeEldritchCannon,
                ConditionAffinityPoisonImmunity,
                DamageAffinityPoisonImmunity,
                DamageAffinityPsychicImmunity,
                SenseNormalVision,
                MovementAffinityNoSpecialMoves,
                MovementAffinitySpiderClimb)
            .AddFeatures(monsterAdditionalFeatures.ToArray())
            .AddToDB();

        monster.guiPresentation.description = GuiPresentationBuilder.EmptyString;

        return monster;
    }

    #endregion

    #region TINY CANNON POWER

    private static FeatureDefinitionPowerSharedPool BuildTinyFlamethrowerPower(FeatureDefinitionPower sharedPoolPower,
        int level)
    {
        return BuildTinyEldritchCannonPower(Flamethrower, sharedPoolPower, level, ConditionFlamethrowerTiny);
    }

    private static FeatureDefinitionPowerSharedPool BuildTinyForceBallistaPower(FeatureDefinitionPower sharedPoolPower,
        int level)
    {
        return BuildTinyEldritchCannonPower(ForceBallista, sharedPoolPower, level, ConditionForceBallistaTiny);
    }

    private static FeatureDefinitionPowerSharedPool BuildTinyProtectorPower(FeatureDefinitionPower sharedPoolPower,
        int level)
    {
        return BuildTinyEldritchCannonPower(Protector, sharedPoolPower, level, ConditionProtectorTiny);
    }

    private static FeatureDefinitionPowerSharedPool BuildTinyEldritchCannonPower(
        string powerName, FeatureDefinitionPower sharedPoolPower, int level, ConditionDefinition conditionDefinition)
    {
        var name = PowerSummonCannon + powerName + "Tiny";

        var power = FeatureDefinitionPowerSharedPoolBuilder
            .Create(name + level)
            .SetGuiPresentation(name, Category.Feature,
                $"Feature/&PowerInnovationArtillerist{powerName}Description", hidden: true)
            .SetSharedPool(ActivationTime.Action, sharedPoolPower)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetDurationData(DurationType.Hour, 1)
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDefinition, ConditionForm.ConditionOperation.Add, true)
                            .Build())
                    .SetParticleEffectParameters(ConjureGoblinoids)
                    .Build())
            .SetUniqueInstance()
            .AddCustomSubFeatures(SkipEffectRemovalOnLocationChange.Always, CannonLimiter)
            .AddToDB();

        return power;
    }

    #endregion

    #region COMMAND CANNON

    private static bool HasCannon(RulesetActor character)
    {
        var gameLocationCharacterService = ServiceRepository.GetService<IGameLocationCharacterService>();

        return gameLocationCharacterService != null &&
               gameLocationCharacterService.AllValidEntities
                   .ToList()
                   .Where(x => x.Side == character.Side)
                   .SelectMany(x => x.RulesetCharacter.AllConditions
                       .Where(y => y.ConditionDefinition ==
                                   DatabaseHelper.ConditionDefinitions.ConditionConjuredCreature))
                   .Any(x => x.sourceGuid == character.guid);
    }

    private class ShowInCombatWhenHasCannon : IValidatePowerUse
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            return Gui.Battle != null && HasCannon(character);
        }
    }

    private class SummonerHasConditionOrKOd : IValidateDefinitionApplication, ICharacterTurnStartListener
    {
        public void OnCharacterTurnStarted(GameLocationCharacter locationCharacter)
        {
            // if commanded allow anything
            if (IsCommanded(locationCharacter.RulesetCharacter))
            {
                return;
            }

            // if not commanded it cannot move
            locationCharacter.usedTacticalMoves = locationCharacter.MaxTacticalMoves;

            // or use powers so force the dodge action
            ServiceRepository.GetService<ICommandService>()
                ?.ExecuteAction(new CharacterActionParams(locationCharacter, Id.Dodge), null, false);
        }

        public bool IsValid(BaseDefinition definition, RulesetCharacter character)
        {
            //Apply limits if not commanded
            return !IsCommanded(character);
        }

        private static bool IsCommanded(RulesetCharacter character)
        {
            //can act freely outside of battle
            if (Gui.Battle == null)
            {
                return true;
            }

            var summoner = character.GetMySummoner()?.RulesetCharacter;

            //shouldn't happen, but consider being commanded in this case
            if (summoner == null)
            {
                return true;
            }

            //can act if summoner is KO
            return summoner.IsUnconscious ||
                   //can act if summoner commanded
                   summoner.HasConditionOfType(ConditionCommandCannon);
        }
    }

    private class ApplyOnTurnEnd(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition condition,
        FeatureDefinitionPower power)
        : ICharacterTurnEndListener
    {
        public void OnCharacterTurnEnded(GameLocationCharacter locationCharacter)
        {
            var status = locationCharacter.GetActionStatus(Id.PowerBonus, ActionScope.Battle);

            if (status != ActionStatus.Available || !HasCannon(locationCharacter.RulesetCharacter))
            {
                return;
            }

            var rulesetCharacter = locationCharacter.RulesetCharacter;

            rulesetCharacter.LogCharacterUsedPower(power);
            rulesetCharacter.InflictCondition(
                condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                condition.Name,
                0,
                0,
                0);
        }
    }

    #endregion

    #region DISMISS / DETONATE CANNON

    private class ShowWhenHasCannon : IValidatePowerUse
    {
        public bool CanUsePower(RulesetCharacter character, FeatureDefinitionPower featureDefinitionPower)
        {
            return HasCannon(character);
        }
    }

    private sealed class MagicEffectFinishedByMeEldritchDetonation : IMagicEffectFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerEldritchDetonation;

        internal MagicEffectFinishedByMeEldritchDetonation(FeatureDefinitionPower powerEldritchDetonation)
        {
            _powerEldritchDetonation = powerEldritchDetonation;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition power)
        {
            var gameLocationTargetingService = ServiceRepository.GetService<IGameLocationTargetingService>();

            if (gameLocationTargetingService == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;
            var selectedTarget = action.ActionParams.TargetCharacters[0];
            var targets = new List<GameLocationCharacter>();

            var implementationManagerService =
                ServiceRepository.GetService<IRulesetImplementationService>() as RulesetImplementationManager;

            var usablePower = PowerProvider.Get(_powerEldritchDetonation, rulesetCharacter);
            var effectPower = implementationManagerService
                //CHECK: no need for AddAsActivePowerToSource
                .MyInstantiateEffectPower(rulesetCharacter, usablePower, false);

            gameLocationTargetingService.CollectTargetsInLineOfSightWithinDistance(
                selectedTarget, effectPower.EffectDescription, targets, []);

            var actionParams = new CharacterActionParams(actingCharacter, Id.SpendPower)
            {
                RulesetEffect = effectPower, UsablePower = usablePower, targetCharacters = targets
            };

            var actionService = ServiceRepository.GetService<IGameLocationActionService>();

            actionService.ExecuteAction(actionParams, null, false);
        }
    }

    #endregion
}
