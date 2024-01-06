using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomDefinitions;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using TA;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.CustomBuilders;

internal static class GambitsBuilders
{
    private static readonly LimitEffectInstances GambitLimiter = new("Gambit", _ => 1);

    internal static FeatureDefinitionPower GambitPool { get; } = FeatureDefinitionPowerBuilder
        .Create("PowerPoolTacticianGambit")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(IsPowerPool.Marker, HasModifiedUses.Marker)
        // force to zero here and add 4 on same level for better integration with tactician adept feat
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 0)
        .AddToDB();

    internal static FeatureDefinitionCustomInvocationPool Learn2Gambit { get; } =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitLearn2")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit, 2)
            .AddToDB();

    // kept name for backward compatibility
    internal static FeatureDefinitionCustomInvocationPool Learn3Gambit { get; } =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitLearn4")
            .SetGuiPresentation(Category.Feature)
            //adding base pool here instead of the pool power to make it properly work on pre-existing characters and not interfere with new feat
            .AddCustomSubFeatures(InitialPool.Instance)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit, 3)
            .AddToDB();

    internal static void BuildGambits()
    {
        #region Helpers

        var gambitDieDamage = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageGambitDie")
            .SetGuiPresentationNoContent(true)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetNotificationTag("GambitDie")
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionName = MartialTactician.MarkDamagedByGambit
                })
            .SetFrequencyLimit(FeatureLimitedUsage.None)
            .AddCustomSubFeatures(
                (DamageDieProvider)((character, _) => GetGambitDieSize(character)),
                ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
            .AddToDB();

        var gambitDieDamageMelee = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageGambitDieMelee")
            .SetGuiPresentationNoContent(true)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetNotificationTag("GambitDie")
            .SetConditionOperations(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionName = MartialTactician.MarkDamagedByGambit
                })
            .SetFrequencyLimit(FeatureLimitedUsage.None)
            .AddCustomSubFeatures(
                (DamageDieProvider)((character, _) => GetGambitDieSize(character)),
                ValidatorsRestrictedContext.IsMeleeOrUnarmedAttack)
            .AddToDB();

        var conditionGambitDieDamage = ConditionDefinitionBuilder
            .Create("ConditionGambitDieDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(gambitDieDamage)
            .SetSpecialInterruptions(
                ConditionInterruption.AnyBattleTurnEnd,
                (ConditionInterruption)ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
            .AddToDB();

        var hasGambitDice =
            new ValidatorsValidatePowerUse(character => character.GetRemainingPowerCharges(GambitPool) > 0);

        #endregion

        #region Distracted Strike (former Blind)

        var name = "GambitBlind";
        var sprite = Sprites.GetSprite(name, Resources.GambitBlind, 128);

        var combatAffinityDistracted = FeatureDefinitionCombatAffinityBuilder
            .Create($"CombatAffinity{name}")
            .SetGuiPresentation($"Condition{name}Distracted", Category.Condition, Gui.NoLocalization)
            .SetAttackOnMeAdvantage(AdvantageType.Advantage)
            .SetSituationalContext((SituationalContext)ExtraSituationalContext.IsNotSourceOfCondition)
            .AddToDB();

        var conditionDistracted = ConditionDefinitionBuilder
            .Create($"Condition{name}Distracted")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionDistracted)
            .SetConditionType(ConditionType.Detrimental)
            .SetFeatures(combatAffinityDistracted)
            .SetSpecialInterruptions(ExtraConditionInterruption.AttackedNotBySource)
            .AddToDB();

        combatAffinityDistracted.requiredCondition = conditionDistracted;

        var reactionPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetSharedPool(ActivationTime.OnAttackHitAuto, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionDistracted))
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(ForcePowerUseInSpendPowerAction.Marker);

        var power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                .SetFeatures(gambitDieDamage, reactionPower)
                                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Knockdown (former Trip Attack)

        name = "GambitKnockdown";
        sprite = Sprites.GetSprite(name, Resources.GambitKnockdown, 128);

        reactionPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetSharedPool(ActivationTime.OnAttackHitAuto, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.FallProne)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(
            ForcePowerUseInSpendPowerAction.Marker,
            new ModifyEffectDescriptionSavingThrow(reactionPower));

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                .SetFeatures(gambitDieDamage, reactionPower)
                                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Repel (former Pushing Attack)

        name = "GambitRepel";
        sprite = Sprites.GetSprite(name, Resources.GambitRepel, 128);

        reactionPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetSharedPool(ActivationTime.OnAttackHitAuto, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(false, AttributeDefinitions.Strength, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(MotionForm.MotionType.PushFromOrigin, 3)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(
            ForcePowerUseInSpendPowerAction.Marker,
            new ModifyEffectDescriptionSavingThrow(reactionPower));

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{name}")
                                    .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                    .SetPossessive()
                                    .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                    .SetFeatures(gambitDieDamage, reactionPower)
                                    .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                    .AddToDB(),
                                ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Threaten (former Menacing Attack)

        name = "GambitThreaten";
        sprite = Sprites.GetSprite(name, Resources.GambitThreaten, 128);

        reactionPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetSharedPool(ActivationTime.OnAttackHitAuto, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(false, AttributeDefinitions.Wisdom, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(ConditionDefinitions.ConditionFrightened,
                                ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(
            ForcePowerUseInSpendPowerAction.Marker,
            new ModifyEffectDescriptionSavingThrow(reactionPower));

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                .SetFeatures(gambitDieDamage, reactionPower)
                                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Debilitate

        name = "GambitDebilitate";
        sprite = Sprites.GetSprite(name, Resources.GambitDebilitate, 128);

        reactionPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetSharedPool(ActivationTime.OnAttackHitAuto, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetSavingThrowData(false, AttributeDefinitions.Constitution, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(
                                ConditionDefinitionBuilder
                                    .Create($"Condition{name}")
                                    .SetGuiPresentation(Category.Condition,
                                        ConditionDefinitions.ConditionPatronHiveWeakeningPheromones)
                                    .SetConditionType(ConditionType.Detrimental)
                                    .SetFeatures(
                                        FeatureDefinitionAbilityCheckAffinitys.AbilityCheckAffinityConditionExhausted,
                                        FeatureDefinitionSavingThrowAffinitys
                                            .SavingThrowAffinityPatronHiveWeakeningPheromones)
                                    .CopyParticleReferences(ConditionDefinitions.ConditionPainful)
                                    .AddToDB(), ConditionForm.ConditionOperation.Add)
                            .HasSavingThrow(EffectSavingThrowType.Negates)
                            .Build())
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(
            ForcePowerUseInSpendPowerAction.Marker,
            new ModifyEffectDescriptionSavingThrow(reactionPower));

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}Trigger")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                .SetFeatures(gambitDieDamage, reactionPower)
                                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Provoke (former Goading)

        name = "GambitGoading";
        sprite = Sprites.GetSprite(name, Resources.GambitProvoke, 128);

        reactionPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetSharedPool(ActivationTime.OnAttackHitMeleeAuto, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round, 1, TurnOccurenceType.EndOfSourceTurn)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(CustomConditionsContext.Taunted))
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(
            ForcePowerUseInSpendPowerAction.Marker,
            new ModifyEffectDescriptionSavingThrow(reactionPower));

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                .SetFeatures(gambitDieDamageMelee, reactionPower)
                                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Coordinated Attack

        name = "GambitCoordinatedAttack";
        sprite = Sprites.GetSprite(name, Resources.GambitCoordinatedAttack, 128);

        var powerCoordinatedAttack = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Command")
            .SetGuiPresentation(Category.Feature, SpellDefinitions.MistyStep)
            .SetUsesFixed(ActivationTime.NoCost, RechargeRate.None)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(ConditionDefinitions.ConditionDisengaging))
                    .Build())
            .AddToDB();

        powerCoordinatedAttack.AddCustomSubFeatures(new CoordinatedAttack(powerCoordinatedAttack));

        reactionPower = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}React")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetSharedPool(ActivationTime.OnAttackHitAuto, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}React")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetFeatures(powerCoordinatedAttack)
                                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(ForcePowerUseInSpendPowerAction.Marker);

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionGambitDieDamage),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .SetFeatures(reactionPower)
                                .AddCustomSubFeatures(new AddUsablePowersFromCondition())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Overwhelming Attack

        name = "GambitOverwhelmingAttack";
        sprite = Sprites.GetSprite(name, Resources.GambitOverwhelmingAttack, 128);

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.NoCost, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.MeleeHit, 1, TargetType.IndividualsUnique, 2)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new UpgradeRangeBasedOnWeaponReach(power),
            new OverwhelmingAttack(power));

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Aimed Attack (former Feint)

        name = "GambitFeint";
        sprite = Sprites.GetSprite(name, Resources.GambitFeint, 128);

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetUsesFixed(ActivationTime.NoCost)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .SetSpecialInterruptions(
                                    ExtraConditionInterruption.UsesBonusAction,
                                    ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                .SetFeatures(gambitDieDamage)
                                .AddCustomSubFeatures(new Feint(GambitPool))
                                .AddToDB()))
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(new ValidatorsValidatePowerUse(ValidatorsCharacter.HasAvailableBonusAction));

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Reach (former Lunging)

        name = "GambitLunging";
        sprite = Sprites.GetSprite(name, Resources.GambitReach, 128);

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.NoCost, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(conditionGambitDieDamage),
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentation(name, Category.Feature, Sprites.ConditionGambit)
                                .SetPossessive()
                                .AddCustomSubFeatures(new IncreaseWeaponReach(1, ValidatorsWeapon.IsMelee))
                                .AddToDB()))
                    .Build())
            .AddToDB();

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Tactical Strike (former Urgent Orders)

        name = "GambitUrgent";
        sprite = Sprites.GetSprite(name, Resources.GambitUrgentOrders, 128);

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.NoCost, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Distance, 6, TargetType.IndividualsUnique, 2)
                    .SetDurationData(DurationType.Round)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder.ConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{name}")
                                .SetGuiPresentationNoContent(true)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetFeatures(gambitDieDamage)
                                .SetSpecialInterruptions(
                                    ConditionInterruption.Attacked,
                                    (ConditionInterruption)ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
                                .AddCustomSubFeatures(new ModifyAdditionalDamageFormTacticalStrike())
                                .AddToDB()))
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasMainAttackAvailable,
            new TacticalStrike(power));

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Elusive Movement (former Evasive Footwork)

        name = "GambitElusiveMovement";
        sprite = Sprites.GetSprite(name, Resources.GambitElusiveMovement, 128);

        var conditionElusiveMovement = ConditionDefinitionBuilder
            .Create($"Condition{name}")
            .SetGuiPresentation(name, Category.Feature, ConditionDefinitions.ConditionHeraldOfBattle)
            .SetPossessive()
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{name}")
                    .SetGuiPresentation(name, Category.Feature)
                    .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .SetAmountOrigin(ExtraOriginOfAmount.SourceGambitDieRoll)
            .AddToDB();

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.NoCost, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round)
                    .SetEffectForms(EffectFormBuilder.ConditionForm(conditionElusiveMovement))
                    .Build())
            .AddToDB();

        conditionElusiveMovement.AddCustomSubFeatures(new ElusiveMovement(power));

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Inspire (former Rally)

        name = "GambitRally";
        sprite = Sprites.GetSprite(name, Resources.GambitRally, 128);

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.BonusAction, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Minute, 1)
                    .ExcludeCaster()
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetTempHpForm()
                            .Build())
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(new Rally(power));

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Swift Throw (former Quick Toss)

        name = "GambitSwiftThrow";
        sprite = Sprites.GetSprite(name, Resources.GambitSwiftThrow, 128);

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.BonusAction, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Enemy, RangeType.RangeHit, 12, TargetType.IndividualsUnique)
                    .Build())
            .AddToDB();

        var concealedDagger = ItemDefinitionBuilder
            .Create(ItemDefinitions.Dagger, "ConcealedDagger")
            .SetOrUpdateGuiPresentation("Item/&ConcealedDaggerTitle",
                ItemDefinitions.Dagger.GuiPresentation.Description)
            .AddToDB();

        power.AddCustomSubFeatures(new SwiftThrow(concealedDagger, power));

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Switch (former Bait and Switch)

        name = "GambitSwitch";
        sprite = Sprites.GetSprite(name, Resources.GambitSwitch, 128);

        var good = ConditionDefinitionBuilder
            .Create($"Condition{name}Good")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBlessed)
            .SetAmountOrigin(ConditionDefinition.OriginOfAmount.Fixed)
            .SetFeatures(
                FeatureDefinitionAttributeModifierBuilder
                    .Create($"AttributeModifier{name}Good")
                    .SetGuiPresentation($"Condition{name}Good", Category.Condition)
                    .SetAddConditionAmount(AttributeDefinitions.ArmorClass)
                    .AddToDB())
            .AddCustomSubFeatures(RemoveConditionOnSourceTurnStart.Mark)
            .AddToDB();

        var bad = ConditionDefinitionBuilder
            .Create($"Condition{name}Bad")
            .SetGuiPresentation(Category.Condition, ConditionDefinitions.ConditionBranded)
            .SetConditionType(ConditionType.Detrimental)
            .AddToDB();

        var self = ConditionDefinitionBuilder
            .Create($"Condition{name}Self")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{name}Self")
                    .SetGuiPresentationNoContent(true)
                    .SetAttackOfOpportunityImmunity(true)
                    .SetSituationalContext(SituationalContext.SourceHasCondition, bad)
                    .AddToDB())
            .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
            .AddToDB();

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(PowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.BonusAction, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Touch, 1, TargetType.IndividualsUnique)
                    .ExcludeCaster()
                    .SetSavingThrowData(true, AttributeDefinitions.Dexterity, false,
                        EffectDifficultyClassComputation.AbilityScoreAndProficiency)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetMotionForm(ExtraMotionType.CustomSwap, 1)
                            .Build())
                    .SetParticleEffectParameters(SpellDefinitions.Haste)
                    .Build())
            .AddToDB();

        power.AddCustomSubFeatures(
            ValidatorsValidatePowerUse.HasTacticalMovesAvailable,
            new Switch(power, good, bad, self),
            new ModifyEffectDescriptionSavingThrow(power));

        BuildFeatureInvocation(name, sprite, power);

        #endregion

        #region Counter Attack (former Riposte)

        name = "GambitRiposte";
        sprite = Sprites.GetSprite(name, Resources.GambitCounterAttack, 128);

        var feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .AddCustomSubFeatures(new Retaliate(GambitPool, conditionGambitDieDamage, true))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Return Fire

        name = "GambitReturnFire";
        sprite = Sprites.GetSprite(name, Resources.GambitReturnFire, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .AddCustomSubFeatures(new Retaliate(GambitPool, conditionGambitDieDamage, false))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Readied Strike (former Brace)

        name = "GambitBrace";
        sprite = Sprites.GetSprite(name, Resources.GambitBrace, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .AddCustomSubFeatures(new Brace(GambitPool, conditionGambitDieDamage))
            .AddToDB();

        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Accurate Attack (former Precise)

        name = "GambitPrecise";
        sprite = Sprites.GetSprite(name, Resources.GambitPrecision, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .AddToDB();

        feature.AddCustomSubFeatures(new Precise(GambitPool, feature));


        BuildFeatureInvocation(name, sprite, feature);

        #endregion

        #region Block (former Parry)

        name = "GambitParry";
        sprite = Sprites.GetSprite(name, Resources.GambitParry, 128);

        feature = FeatureDefinitionBuilder
            .Create($"Feature{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .AddToDB();

        feature.AddCustomSubFeatures(new Parry(GambitPool, feature));


        BuildFeatureInvocation(name, sprite, feature);

        #endregion
    }

    private sealed class ModifyEffectDescriptionSavingThrow : IModifyEffectDescription
    {
        private readonly FeatureDefinitionPower _baseDefinition;

        public ModifyEffectDescriptionSavingThrow(FeatureDefinitionPower baseDefinition)
        {
            _baseDefinition = baseDefinition;
        }

        public bool IsValid(
            BaseDefinition definition,
            RulesetCharacter character,
            EffectDescription effectDescription)
        {
            return definition == _baseDefinition;
        }

        public EffectDescription GetEffectDescription(
            BaseDefinition definition,
            EffectDescription effectDescription,
            RulesetCharacter character,
            RulesetEffect rulesetEffect)
        {
            var proficiencyBonus = character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var strength = character.TryGetAttributeValue(AttributeDefinitions.Strength);
            var dexterity = character.TryGetAttributeValue(AttributeDefinitions.Dexterity);
            var strDC = ComputeAbilityScoreBasedDC(strength, proficiencyBonus);
            var dexDC = ComputeAbilityScoreBasedDC(dexterity, proficiencyBonus);
            var saveDC = Math.Max(strDC, dexDC);

            if (rulesetEffect is RulesetEffectPower rulesetEffectPower)
            {
                rulesetEffectPower.usablePower.saveDC = saveDC;
            }

            return effectDescription;
        }
    }

    private sealed class InitialPool : IModifyPowerPoolAmount
    {
        private InitialPool()
        {
        }

        public static IModifyPowerPoolAmount Instance { get; } = new InitialPool();
        public FeatureDefinitionPower PowerPool => GambitPool;

        public int PoolChangeAmount(RulesetCharacter character)
        {
            return 4;
        }
    }

    //
    // used to change the gambit die size when ally attacks
    //
    private sealed class ModifyAdditionalDamageFormTacticalStrike : IModifyAdditionalDamageForm
    {
        public DamageForm AdditionalDamageForm(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            IAdditionalDamageProvider provider,
            DamageForm damageForm)
        {
            if (provider.NotificationTag != "GambitDie")
            {
                return damageForm;
            }

            var usableCondition = attacker.RulesetCharacter.AllConditions
                .FirstOrDefault(x => x.ConditionDefinition.Name == "ConditionGambitUrgent");

            if (usableCondition == null)
            {
                return damageForm;
            }

            var sourceCharacter = EffectHelpers.GetCharacterByGuid(usableCondition.SourceGuid);

            if (sourceCharacter == null)
            {
                return damageForm;
            }

            var dieType = GetGambitDieSize(sourceCharacter);

            damageForm.dieType = dieType;

            return damageForm;
        }
    }

    //
    // Rally
    //
    private sealed class Rally : IMagicEffectInitiatedByMe
    {
        private readonly FeatureDefinitionPower _powerRallyActivate;

        public Rally(FeatureDefinitionPower powerRallyActivate)
        {
            _powerRallyActivate = powerRallyActivate;
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var character = action.ActingCharacter.RulesetCharacter;
            var intelligence = character.TryGetAttributeValue(AttributeDefinitions.Intelligence);
            var wisdom = character.TryGetAttributeValue(AttributeDefinitions.Wisdom);
            var charisma = character.TryGetAttributeValue(AttributeDefinitions.Charisma);
            var modifier = Math.Max(Math.Max(
                    AttributeDefinitions.ComputeAbilityScoreModifier(intelligence),
                    AttributeDefinitions.ComputeAbilityScoreModifier(wisdom)),
                AttributeDefinitions.ComputeAbilityScoreModifier(charisma));
            var dieType = GetGambitDieSize(character);
            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);
            var bonusHitPoints = modifier + dieRoll;
            var target = action.ActionParams.TargetCharacters[0];

            character.ShowDieRoll(dieType, dieRoll, title: _powerRallyActivate.GuiPresentation.Title);
            character.LogCharacterUsedPower(_powerRallyActivate, "Feedback/&GambitGrantTempHP", true,
                (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                (ConsoleStyleDuplet.ParameterType.Positive, bonusHitPoints.ToString()));

            EffectHelpers.StartVisualEffect(target, target, FeatureDefinitionPowers.PowerOathOfJugementPurgeCorruption,
                EffectHelpers.EffectType.Caster);

            action.ActionParams.RulesetEffect.EffectDescription.EffectForms[0]
                .TemporaryHitPointsForm.BonusHitPoints = bonusHitPoints;

            yield break;
        }
    }

    //
    // Swift Throw
    //
    private sealed class SwiftThrow : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe, IModifyAttackActionModifier
    {
        private const int DaggerCloseRange = 4;
        private readonly ItemDefinition _concealedDagger;
        private readonly FeatureDefinitionPower _powerSwiftThrow;

        public SwiftThrow(ItemDefinition concealedDagger, FeatureDefinitionPower powerSwiftThrow)
        {
            _concealedDagger = concealedDagger;
            _powerSwiftThrow = powerSwiftThrow;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.RulesetEffect.EffectDescription.RangeType = RangeType.RangeHit;

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var attackMode = actingCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                yield break;
            }

            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();
            var target = action.ActionParams.TargetCharacters[0];
            var dieType = GetGambitDieSize(actingCharacter.RulesetCharacter);
            var strength = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Strength);
            var dexterity = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Dexterity);
            var bonus = Math.Max(
                AttributeDefinitions.ComputeAbilityScoreModifier(strength),
                AttributeDefinitions.ComputeAbilityScoreModifier(dexterity));

            attackModeCopy.Copy(attackMode);
            attackModeCopy.ActionType = ActionDefinitions.ActionType.NoCost;
            attackModeCopy.SourceDefinition = _concealedDagger;
            attackModeCopy.EffectDescription = _concealedDagger.WeaponDescription.EffectDescription;
            attackModeCopy.AttackTags.SetRange(_concealedDagger.WeaponDescription.WeaponTags);
            attackModeCopy.closeRange = DaggerCloseRange;
            attackModeCopy.maxRange = 12;
            attackModeCopy.thrown = true;
            attackModeCopy.ranged = true;
            attackModeCopy.EffectDescription.EffectForms.RemoveAll(x =>
                x.FormType == EffectForm.EffectFormType.Damage);
            attackModeCopy.EffectDescription.EffectForms.AddRange(
                EffectFormBuilder.DamageForm(DamageTypePiercing, 1, DieType.D4),
                EffectFormBuilder.DamageForm(DamageTypePiercing, 1, dieType, bonus));

            Attack(actingCharacter, target, attackModeCopy, action.ActionParams.ActionModifiers[0]);
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.RulesetEffect.EffectDescription.RangeType = RangeType.Distance;

            yield break;
        }

        public void OnAttackComputeModifier(
            RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (effectName != _powerSwiftThrow.Name)
            {
                return;
            }

            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                return;
            }

            var glcMyself = GameLocationCharacter.GetFromActor(myself);
            var glcDefender = GameLocationCharacter.GetFromActor(defender);

            if (!gameLocationBattleService.IsWithinXCells(glcMyself, glcDefender, DaggerCloseRange))
            {
                attackModifier.AttackAdvantageTrends.Add(
                    new TrendInfo(-1, FeatureSourceType.Equipment, "Tooltip/&ProximityLongRangeTitle", null));
            }
        }
    }

    //
    // Tactical Strike
    //
    private sealed class TacticalStrike :
        IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe, IFilterTargetingCharacter
    {
        private readonly FeatureDefinitionPower _powerSelectEnemy;

        public TacticalStrike(FeatureDefinitionPower powerSelectEnemy)
        {
            _powerSelectEnemy = powerSelectEnemy;
        }

        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != _powerSelectEnemy)
            {
                return true;
            }

            var selectedTargets = __instance.SelectionService.SelectedTargets;

            //
            // only allow allies that can react
            //
            if (selectedTargets.Empty())
            {
                if (target.Side == Side.Enemy || target.CanReact())
                {
                    return true;
                }

                __instance.actionModifier.FailureFlags.Add("Tooltip/&AllyMustBeAbleToReact");

                return false;
            }

            if (target.Side != Side.Enemy && !target.CanReact())
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&AllyMustBeAbleToReact");

                return false;
            }

            //
            // there is one selected creature already so ensure we don't allow same side pick
            //

            var selectedTarget = selectedTargets[0];

            if (selectedTarget.Side != Side.Enemy && target.Side != Side.Enemy)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&AlreadySelectedAnAlly");

                return false;
            }

            if (selectedTarget.Side == Side.Enemy && target.Side == Side.Enemy)
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&AlreadySelectedAnEnemy");

                return false;
            }

            //
            // finally check if attack from ally to enemy is possible
            //

            var ally = selectedTarget;
            var enemy = target;

            if (selectedTarget.Side == Side.Enemy)
            {
                ally = target;
                enemy = selectedTarget;
            }

            var attackMode = ally.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            // ReSharper disable once InvertIf
            if (attackMode == null || !IsValidAttack(__instance, attackMode, ally, enemy))
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&MustBeAbleToAttackTarget");

                return false;
            }

            return true;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is not { IsBattleInProgress: true })
            {
                yield break;
            }

            var targetCharacters = action.ActionParams.TargetCharacters;
            var ally = targetCharacters[0];
            var target = targetCharacters[1];

            // issue ally attack
            var attackMode = ally.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var attackModifier = new ActionModifier();
            var attackModeCopy = RulesetAttackMode.AttackModesPool.Get();

            attackModeCopy.Copy(attackMode);
            attackModeCopy.ActionType = ActionDefinitions.ActionType.Reaction;

            Attack(ally, target, attackModeCopy, attackModifier, ActionDefinitions.Id.AttackOpportunity);

            BurnOneMainAttack(actingCharacter);
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var targetCharacters = action.ActionParams.TargetCharacters;

            if (targetCharacters[0].Side == Side.Enemy)
            {
                (targetCharacters[0], targetCharacters[1]) = (targetCharacters[1], targetCharacters[0]);
            }

            var target = targetCharacters[1];

            EffectHelpers.StartVisualEffect(target, target, FeatureDefinitionPowers.PowerKnightLeadership,
                EffectHelpers.EffectType.Caster);

            yield break;
        }

        private static bool IsValidAttack(
            CursorLocationSelectTarget __instance,
            RulesetAttackMode attackMode,
            GameLocationCharacter selectedCharacter,
            GameLocationCharacter targetedCharacter)
        {
            __instance.predictivePosition = selectedCharacter.LocationPosition;

            var attackParams1 = new BattleDefinitions.AttackEvaluationParams();

            attackParams1.FillForPhysicalReachAttack(selectedCharacter, __instance.predictivePosition, attackMode,
                targetedCharacter, targetedCharacter.LocationPosition, __instance.actionModifier);

            if (__instance.BattleService.CanAttack(attackParams1))
            {
                return true;
            }

            var attackParams2 = new BattleDefinitions.AttackEvaluationParams();

            attackParams2.FillForPhysicalRangeAttack(selectedCharacter, __instance.predictivePosition, attackMode,
                targetedCharacter, targetedCharacter.LocationPosition, __instance.actionModifier);

            return __instance.BattleService.CanAttack(attackParams2);
        }
    }

    //
    // Feint
    //
    private sealed class Feint :
        IModifyAttackActionModifier, IPhysicalAttackInitiatedByMe, IPhysicalAttackFinishedByMe
    {
        private const string ConditionGambitFeint = "ConditionGambitFeint";
        private readonly FeatureDefinitionPower _pool;

        public Feint(FeatureDefinitionPower pool)
        {
            _pool = pool;
        }

        public void OnAttackComputeModifier(RulesetCharacter myself,
            RulesetCharacter defender,
            BattleDefinitions.AttackProximity attackProximity,
            RulesetAttackMode attackMode,
            string effectName,
            ref ActionModifier attackModifier)
        {
            if (attackMode?.SourceDefinition is not ItemDefinition ||
                attackMode.ActionType == ActionDefinitions.ActionType.Bonus ||
                !ValidatorsCharacter.HasAvailableBonusAction(myself))
            {
                return;
            }

            attackModifier.attackAdvantageTrends.Add(
                new TrendInfo(1, FeatureSourceType.Condition, ConditionGambitFeint, null));
        }

        public IEnumerator OnPhysicalAttackFinishedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (attackMode?.SourceDefinition is not ItemDefinition ||
                !ValidatorsCharacter.HasAvailableBonusAction(rulesetCharacter))
            {
                yield break;
            }

            attacker.CurrentActionRankByType[ActionDefinitions.ActionType.Bonus]++;
            rulesetCharacter.UpdateUsageForPower(_pool, 1);
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager __instance,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackerAttackMode)
        {
            if (action.ActionType != ActionDefinitions.ActionType.Bonus)
            {
                yield break;
            }

            attacker.RulesetCharacter.RemoveAllConditionsOfCategoryAndType(
                AttributeDefinitions.TagEffect, ConditionGambitFeint);
        }
    }

    //
    // Retaliate
    //
    private sealed class Retaliate : IPhysicalAttackFinishedOnMe
    {
        private readonly ConditionDefinition _condition;
        private readonly bool _melee;
        private readonly FeatureDefinitionPower _pool;

        public Retaliate(FeatureDefinitionPower pool, ConditionDefinition condition, bool melee)
        {
            _condition = condition;
            _melee = melee;
            _pool = pool;
        }

        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackerAttackMode,
            RollOutcome attackRollOutcome,
            int damageAmount)
        {
            //trigger only on a miss
            if (attackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            //do not trigger on my own turn, so won't retaliate on AoO
            if (Gui.Battle?.ActiveContenderIgnoringLegendary == defender)
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (!defender.CanReact() ||
                rulesetEnemy is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (defender.RulesetCharacter.GetRemainingPowerCharges(_pool) <= 0)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (!_melee && battle.IsWithin1Cell(defender, attacker))
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = _melee
                ? defender.GetFirstMeleeModeThatCanAttack(attacker)
                : defender.GetFirstRangedModeThatCanAttack(attacker);

            if (retaliationMode == null)
            {
                yield break;
            }

            retaliationMode.AddAttackTagAsNeeded(AttacksOfOpportunity.NotAoOTag);
            retaliationMode.AddAttackTagAsNeeded(MartialTactician.TacticalAwareness);

            var reactionParams = new CharacterActionParams(defender, ActionDefinitions.Id.AttackOpportunity);

            reactionParams.TargetCharacters.Add(attacker);
            reactionParams.ActionModifiers.Add(retaliationModifier);
            reactionParams.AttackMode = retaliationMode;

            var rulesetCharacter = defender.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                _condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                _condition.Name,
                0,
                0,
                0);

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var tag = _melee ? "GambitRiposte" : "GambitReturnFire";
            var reactionRequest = new ReactionRequestReactionAttack(tag, reactionParams)
            {
                Resource = new ReactionResourcePowerPool(_pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);
        }
    }

    //
    // Switch
    //
    private sealed class Switch : IFilterTargetingCharacter, IMagicEffectFinishedByMe
    {
        private readonly ConditionDefinition _good, _bad, _self;
        private readonly FeatureDefinitionPower _powerSwitchActivate;

        public Switch(
            FeatureDefinitionPower powerSwitchActivate,
            ConditionDefinition good,
            ConditionDefinition bad,
            ConditionDefinition self)
        {
            _powerSwitchActivate = powerSwitchActivate;
            _good = good;
            _bad = bad;
            _self = self;
        }

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != _powerSwitchActivate)
            {
                return true;
            }

            var actingCharacter = __instance.ActionParams.ActingCharacter;

            // ReSharper disable once InvertIf
            if (actingCharacter.RulesetCharacter.HasAnyConditionOfTypeOrSubType(
                    ConditionIncapacitated, ConditionParalyzed, ConditionRestrained) ||
                (target.Side != Side.Enemy &&
                 target.RulesetCharacter.HasAnyConditionOfTypeOrSubType(
                     ConditionIncapacitated, ConditionParalyzed, ConditionRestrained)))
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&SelfOrTargetCannotAct");

                return false;
            }

            return true;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            var actingCharacter = action.ActingCharacter;
            var caster = actingCharacter.RulesetCharacter;
            var target = action.ActionParams.TargetCharacters[0].RulesetCharacter;

            // consume one tactical move
            actingCharacter.UsedTacticalMoves++;

            if (caster.IsOppositeSide(target.Side))
            {
                target.InflictCondition(
                    _bad.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    caster.Guid,
                    caster.CurrentFaction.Name,
                    1,
                    _bad.Name,
                    0,
                    0,
                    0);

                caster.InflictCondition(
                    _self.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.StartOfTurn,
                    AttributeDefinitions.TagEffect,
                    target.Guid,
                    target.CurrentFaction.Name,
                    1,
                    _self.Name,
                    0,
                    0,
                    0);

                yield break;
            }

            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;
            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (battle is not { IsBattleInProgress: true } || manager == null)
            {
                yield break;
            }

            var reactionParams =
                new CharacterActionParams(actingCharacter, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter = "Reaction/&CustomReactionGambitSwitchDescription"
                };

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("GambitSwitch", reactionParams);

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(actingCharacter, manager, previousReactionCount);

            var dieType = GetGambitDieSize(caster);
            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            caster.ShowDieRoll(dieType, dieRoll, title: _good.GuiPresentation.Title);

            var finalTarget = !reactionParams.ReactionValidated ? caster : target;

            finalTarget.InflictCondition(
                _good.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.EndOfSourceTurn,
                AttributeDefinitions.TagEffect,
                caster.Guid,
                caster.CurrentFaction.Name,
                1,
                _good.Name,
                dieRoll,
                0,
                0);

            caster.LogCharacterUsedPower(_powerSwitchActivate, "Feedback/&GambitSwitchACIncrease", true,
                (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                (ConsoleStyleDuplet.ParameterType.Player, finalTarget.Name),
                (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()));
        }
    }

    //
    // Brace
    //
    private sealed class Brace : CanMakeAoOOnReachEntered
    {
        private readonly ConditionDefinition _condition;
        private readonly FeatureDefinitionPower _pool;

        public Brace(FeatureDefinitionPower pool, ConditionDefinition condition)
        {
            _pool = pool;
            _condition = condition;
            ValidateAttacker = character => character.GetRemainingPowerCharges(pool) > 0;
            BeforeReaction = HandleBeforeReaction;
            AfterReaction = HandleAfterReaction;
        }

        private IEnumerator HandleBeforeReaction(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationBattleManager battleManager,
            GameLocationActionManager actionManager,
            ReactionRequest request)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            rulesetCharacter.InflictCondition(
                _condition.Name,
                DurationType.Round,
                1,
                TurnOccurenceType.StartOfTurn,
                AttributeDefinitions.TagEffect,
                rulesetCharacter.guid,
                rulesetCharacter.CurrentFaction.Name,
                1,
                _condition.Name,
                0,
                0,
                0);

            yield break;
        }

        private IEnumerator HandleAfterReaction(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationBattleManager battleManager,
            GameLocationActionManager actionManager,
            ReactionRequest request)
        {
            var character = attacker.RulesetCharacter;
            var reactionParams = request.reactionParams;

            if (reactionParams.ReactionValidated)
            {
                character.UpdateUsageForPower(_pool, 1);
            }

            yield break;
        }

        protected override ReactionRequest MakeReactionRequest(GameLocationCharacter attacker,
            GameLocationCharacter defender, RulesetAttackMode attackMode, ActionModifier attackModifier)
        {
            return new ReactionRequestReactionAttack("GambitBrace", new CharacterActionParams(
                attacker,
                ActionDefinitions.Id.AttackOpportunity,
                attackMode,
                defender,
                attackModifier)) { Resource = new ReactionResourcePowerPool(_pool, Sprites.GambitResourceIcon) };
        }
    }

    //
    // Precise
    //
    private sealed class Precise : ITryAlterOutcomePhysicalAttack
    {
        private const string Format = "Reaction/&CustomReactionGambitPreciseDescription";
        private const string Line = "Feedback/&GambitPreciseToHitRoll";
        private readonly FeatureDefinition _feature;
        private readonly FeatureDefinitionPower _pool;

        public Precise(FeatureDefinitionPower pool, FeatureDefinition feature)
        {
            _pool = pool;
            _feature = feature;
        }

        public IEnumerator OnAttackTryAlterOutcome(GameLocationBattleManager battle, CharacterAction action,
            GameLocationCharacter me, GameLocationCharacter target, ActionModifier attackModifier)
        {
            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var character = me.RulesetCharacter;

            if (character.GetRemainingPowerCharges(_pool) <= 0)
            {
                yield break;
            }

            var dieType = GetGambitDieSize(character);
            var max = DiceMaxValue[(int)dieType];
            var delta = Math.Abs(action.AttackSuccessDelta);

            if (max < delta)
            {
                yield break;
            }

            var guiMe = new GuiCharacter(me);
            var guiTarget = new GuiCharacter(target);

            var description = Gui.Format(Format, guiMe.Name, guiTarget.Name, delta.ToString(),
                Gui.FormatDieTitle(dieType));
            var reactionParams =
                new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter = description
                };

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("GambitPrecise", reactionParams)
            {
                Resource = new ReactionResourcePowerPool(_pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            character.UpdateUsageForPower(_pool, 1);

            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            var hitTrends = attackModifier.AttacktoHitTrends;

            hitTrends?.Add(new TrendInfo(dieRoll, FeatureSourceType.Power, _pool.Name, null)
            {
                dieType = dieType, dieFlag = TrendInfoDieFlag.None
            });

            action.AttackSuccessDelta += dieRoll;
            attackModifier.attackRollModifier += dieRoll;

            var success = action.AttackSuccessDelta >= 0;

            if (success)
            {
                action.AttackRollOutcome = RollOutcome.Success;
            }

            character.ShowDieRoll(dieType, dieRoll,
                title: _feature.GuiPresentation.Title,
                outcome: success ? RollOutcome.Success : RollOutcome.Failure,
                displayOutcome: true
            );

            character.LogCharacterUsedFeature(_feature, Line,
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString())
                ]);
        }
    }

    //
    // Parry
    //
    private sealed class Parry : IAttackBeforeHitConfirmedOnMe
    {
        private const string Line = "Feedback/&GambitParryDamageReduction";
        private readonly FeatureDefinition _feature;
        private readonly FeatureDefinitionPower _pool;

        public Parry(FeatureDefinitionPower pool, FeatureDefinition feature)
        {
            _pool = pool;
            _feature = feature;
        }

        public IEnumerator OnAttackBeforeHitConfirmedOnMe(GameLocationBattleManager battle,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode,
            bool rangedAttack,
            AdvantageType advantageType,
            List<EffectForm> actualEffectForms,
            RulesetEffect rulesetEffect,
            bool firstTarget,
            bool criticalHit)
        {
            if (rangedAttack)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (manager == null)
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (!me.CanReact() || rulesetEnemy is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            var character = me.RulesetCharacter;

            if (character.GetRemainingPowerCharges(_pool) <= 0)
            {
                yield break;
            }

            var dieType = GetGambitDieSize(character);
            var guiMe = new GuiCharacter(me);
            var guiTarget = new GuiCharacter(attacker);

            var reactionParams =
                new CharacterActionParams(me, (ActionDefinitions.Id)ExtraActionId.DoNothingReaction)
                {
                    StringParameter = "CustomReactionGambitParryDescription"
                        .Formatted(Category.Reaction, guiMe.Name, guiTarget.Name, Gui.FormatDieTitle(dieType))
                };

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("GambitParry", reactionParams)
            {
                Resource = new ReactionResourcePowerPool(_pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            character.UpdateUsageForPower(_pool, 1);

            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            var pb = 2 * character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var reduction = dieRoll + pb;

            attackModifier.damageRollReduction += reduction;

            character.ShowDieRoll(dieType, dieRoll,
                title: _feature.GuiPresentation.Title,
                displayModifier: true, modifier: pb);

            character.LogCharacterUsedFeature(_feature, Line,
                extra:
                [
                    (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                    (ConsoleStyleDuplet.ParameterType.Positive, reduction.ToString())
                ]);
        }
    }

    //
    // Coordinated Attack
    //
    private sealed class CoordinatedAttack :
        IFilterTargetingCharacter, ISelectPositionAfterCharacter, IFilterTargetingPosition, IMagicEffectFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerCoordinatedAttack;

        public CoordinatedAttack(FeatureDefinitionPower powerCoordinatedAttack)
        {
            _powerCoordinatedAttack = powerCoordinatedAttack;
        }

        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != _powerCoordinatedAttack)
            {
                return true;
            }

            if (!target.CanReact())
            {
                __instance.actionModifier.FailureFlags.Add("Tooltip/&AllyMustBeAbleToReact");

                return false;
            }

            if (!target.RulesetCharacter.HasAnyConditionOfTypeOrSubType(
                    ConditionIncapacitated, ConditionParalyzed, ConditionRestrained))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Tooltip/&SelfOrTargetCannotAct");

            return false;
        }

        public void EnumerateValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition,
            List<int3> validPositions)
        {
            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;

            if (!actingCharacter.UsedSpecialFeatures.TryGetValue("SelectedCharacter", out var targetGuid))
            {
                return;
            }

            var gameLocationPositioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var targetRulesetCharacter = EffectHelpers.GetCharacterByGuid((ulong)targetGuid);
            var targetCharacter = GameLocationCharacter.GetFromActor(targetRulesetCharacter);
            var halfMaxTacticalMoves = (targetCharacter.MaxTacticalMoves + 1) / 2;
            var boxInt = new BoxInt(
                targetCharacter.LocationPosition,
                new int3(-halfMaxTacticalMoves, -halfMaxTacticalMoves, -halfMaxTacticalMoves),
                new int3(halfMaxTacticalMoves, halfMaxTacticalMoves, halfMaxTacticalMoves));

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (gameLocationPositioningService.CanPlaceCharacter(
                        targetCharacter, position, CellHelpers.PlacementMode.Station) &&
                    gameLocationPositioningService.CanCharacterStayAtPosition_Floor(
                        targetCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    validPositions.Add(position);
                }
            }
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.activeEffect.EffectDescription.rangeParameter = 6;

            if (!action.ActingCharacter.UsedSpecialFeatures.TryGetValue("SelectedCharacter", out var targetGuid))
            {
                yield break;
            }

            var actingCharacter = action.ActingCharacter;
            var targetRulesetCharacter = EffectHelpers.GetCharacterByGuid((ulong)targetGuid);
            var targetCharacter = GameLocationCharacter.GetFromActor(targetRulesetCharacter);
            var targetPosition = action.ActionParams.Positions[0];
            var actionService = ServiceRepository.GetService<IGameLocationActionService>();
            var actionParams =
                new CharacterActionParams(targetCharacter, ActionDefinitions.Id.TacticalMove)
                {
                    Positions = { targetPosition }
                };

            targetCharacter.UsedTacticalMoves = 0;
            targetRulesetCharacter.InflictCondition(
                ConditionDisengaging,
                DurationType.Round,
                0,
                TurnOccurenceType.StartOfTurn,
                // all disengaging in game is set under TagCombat (why?)
                AttributeDefinitions.TagCombat,
                targetRulesetCharacter.Guid,
                targetRulesetCharacter.CurrentFaction.Name,
                1,
                ConditionDisengaging,
                0,
                0,
                0);

            EffectHelpers.StartVisualEffect(actingCharacter, targetCharacter,
                FeatureDefinitionPowers.PowerDomainSunHeraldOfTheSun, EffectHelpers.EffectType.Effect);

            targetCharacter.CurrentActionRankByType[ActionDefinitions.ActionType.Reaction]++;
            targetCharacter.UsedSpecialFeatures.TryAdd("MoverNotInTurn", 0);
            actionService.ExecuteAction(actionParams, null, false);
        }
    }

    //
    // Elusive Movement
    //
    private sealed class ElusiveMovement : IOnConditionAddedOrRemoved
    {
        private readonly FeatureDefinitionPower _powerElusiveMovement;

        public ElusiveMovement(FeatureDefinitionPower powerElusiveMovement)
        {
            _powerElusiveMovement = powerElusiveMovement;
        }

        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var dieType = GetGambitDieSize(target);
            var dieRoll = rulesetCondition.Amount;

            target.LogCharacterUsedPower(_powerElusiveMovement, "Feedback/&GambitElusiveMovementACIncrease", true,
                (ConsoleStyleDuplet.ParameterType.AbilityInfo, Gui.FormatDieTitle(dieType)),
                (ConsoleStyleDuplet.ParameterType.Positive, dieRoll.ToString()));
        }

        public void OnConditionRemoved(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            // empty
        }
    }

    //
    // Overwhelming Attack
    //
    private sealed class OverwhelmingAttack :
        IFilterTargetingCharacter, IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe
    {
        private readonly FeatureDefinitionPower _powerOverwhelmingAttack;

        public OverwhelmingAttack(FeatureDefinitionPower powerOverwhelmingAttack)
        {
            _powerOverwhelmingAttack = powerOverwhelmingAttack;
        }

        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != _powerOverwhelmingAttack)
            {
                return true;
            }

            var selectedTargets = __instance.SelectionService.SelectedTargets;

            if (selectedTargets.Count == 0)
            {
                return true;
            }

            var firstTarget = selectedTargets[0];
            var gameLocationBattleService = ServiceRepository.GetService<IGameLocationBattleService>();

            if (gameLocationBattleService is { IsBattleInProgress: true } &&
                gameLocationBattleService.IsWithin1Cell(firstTarget, target))
            {
                return true;
            }

            __instance.actionModifier.FailureFlags.Add("Tooltip/&AllyMustBeAdjacentToFirstTarget");

            return false;
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.RulesetEffect.EffectDescription.RangeType = RangeType.MeleeHit;

            var actingCharacter = action.ActingCharacter;
            var rulesetCharacter = actingCharacter.RulesetCharacter;

            var attackMode = actingCharacter.FindActionAttackMode(ActionDefinitions.Id.AttackMain);

            if (attackMode == null)
            {
                yield break;
            }

            var attackModeCopy1 = RulesetAttackMode.AttackModesPool.Get();
            var target1 = action.ActionParams.TargetCharacters[0];

            attackModeCopy1.Copy(attackMode);
            attackModeCopy1.ActionType = ActionDefinitions.ActionType.NoCost;

            Attack(actingCharacter, target1, attackModeCopy1, action.ActionParams.ActionModifiers[0]);

            var attackModeCopy2 = RulesetAttackMode.AttackModesPool.Get();
            var target2 = action.ActionParams.TargetCharacters[1];
            var dieType = GetGambitDieSize(actingCharacter.RulesetCharacter);
            var strength = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Strength);
            var dexterity = rulesetCharacter.TryGetAttributeValue(AttributeDefinitions.Dexterity);
            var bonus = Math.Max(
                AttributeDefinitions.ComputeAbilityScoreModifier(strength),
                AttributeDefinitions.ComputeAbilityScoreModifier(dexterity));
            var firstDamageForm = attackMode.EffectDescription.FindFirstDamageForm();

            attackModeCopy2.Copy(attackMode);
            attackModeCopy2.ActionType = ActionDefinitions.ActionType.NoCost;
            attackModeCopy2.EffectDescription.EffectForms.RemoveAll(x =>
                x.FormType == EffectForm.EffectFormType.Damage);
            attackModeCopy2.EffectDescription.EffectForms.Add(
                EffectFormBuilder.DamageForm(firstDamageForm?.DamageType ?? DamageTypePiercing, 1, dieType, bonus));

            Attack(actingCharacter, target2, attackModeCopy2, action.ActionParams.ActionModifiers[0]);

            BurnOneMainAttack(actingCharacter);
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.RulesetEffect.EffectDescription.RangeType = RangeType.Distance;

            yield break;
        }
    }

    // supports displaying the gambit die type and remaining usages on action buttons
    internal sealed class GambitActionDiceBox : IActionItemDiceBox
    {
        private GambitActionDiceBox()
        {
        }

        public static IActionItemDiceBox Instance { get; } = new GambitActionDiceBox();

        public (DieType type, int number, string format) GetDiceInfo(RulesetCharacter character)
        {
            return (GetGambitDieSize(character), character.GetRemainingPowerUses(GambitPool),
                "Screen/&GambitDieDescription");
        }
    }

    #region Helpers

    private static void BuildFeatureInvocation(string name, AssetReferenceSprite sprite, FeatureDefinition feature)
    {
        CustomInvocationDefinitionBuilder
            .Create($"CustomInvocation{name}")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetPoolType(InvocationPoolTypeCustom.Pools.Gambit)
            .SetGrantedFeature(feature)
            .SetRequirements(0)
            .AddToDB();
    }

    internal static DieType GetGambitDieSize(RulesetCharacter character)
    {
        var level = character.GetSubclassLevel(CharacterClassDefinitions.Fighter, MartialTactician.Name);

        return level switch
        {
            >= 18 => DieType.D12,
            >= 10 => DieType.D10,
            >= 3 => DieType.D8,
            _ => DieType.D6
        };
    }

    private static void BurnOneMainAttack(GameLocationCharacter actingCharacter)
    {
        var rulesetCharacter = actingCharacter.RulesetCharacter;

        // burn one main attack
        actingCharacter.UsedMainAttacks++;
        rulesetCharacter.ExecutedAttacks++;
        rulesetCharacter.RefreshAttackModes();

        var maxAttacksNumber = rulesetCharacter.AttackModes
            .Where(x => x.ActionType == ActionDefinitions.ActionType.Main)
            .Max(x => x.AttacksNumber);

        if (maxAttacksNumber - actingCharacter.UsedMainAttacks > 0)
        {
            return;
        }

        actingCharacter.CurrentActionRankByType[ActionDefinitions.ActionType.Main]++;
        actingCharacter.UsedMainAttacks = 0;
    }

    private static void Attack(
        GameLocationCharacter actingCharacter,
        GameLocationCharacter target,
        RulesetAttackMode attackMode,
        ActionModifier attackModifier,
        ActionDefinitions.Id actionId = ActionDefinitions.Id.AttackFree)
    {
        var actionService = ServiceRepository.GetService<IGameLocationActionService>();
        var attackActionParams =
            new CharacterActionParams(actingCharacter, actionId)
            {
                AttackMode = attackMode, TargetCharacters = { target }, ActionModifiers = { attackModifier }
            };

        actionService.ExecuteAction(attackActionParams, null, true);
    }

    #endregion
}
