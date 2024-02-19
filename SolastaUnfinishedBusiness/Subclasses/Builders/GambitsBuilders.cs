using System;
using System.Collections;
using System.Collections.Generic;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.Helpers;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Behaviors.Specific;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Validators;
using TA;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Subclasses.Builders;

internal static class GambitsBuilders
{
    private static readonly LimitEffectInstances GambitLimiter = new("Gambit", _ => 1);

    internal static FeatureDefinitionPower GambitPool { get; } = FeatureDefinitionPowerBuilder
        .Create("PowerPoolTacticianGambit")
        .SetGuiPresentation(Category.Feature)
        .AddCustomSubFeatures(IsModifyPowerPool.Marker, HasModifiedUses.Marker)
        // force to zero here and add 4 on same level for better integration with tactician adept feat
        .SetUsesFixed(ActivationTime.NoCost, RechargeRate.ShortRest, 1, 0)
        .AddToDB();

    internal static FeatureDefinitionCustomInvocationPool Learn1Gambit { get; } =
        CustomInvocationPoolDefinitionBuilder
            .Create("InvocationPoolGambitLearn1")
            .SetGuiPresentation(Category.Feature)
            .Setup(InvocationPoolTypeCustom.Pools.Gambit)
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
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionName = MartialTactician.MarkDamagedByGambit
                })
            .SetFrequencyLimit(FeatureLimitedUsage.None)
            .AddCustomSubFeatures(
                ModifyAdditionalDamageFormGambitDieSize.Marker,
                ValidatorsRestrictedContext.IsWeaponOrUnarmedAttack)
            .AddToDB();

        var gambitDieDamageMelee = FeatureDefinitionAdditionalDamageBuilder
            .Create("AdditionalDamageGambitDieMelee")
            .SetGuiPresentationNoContent(true)
            .SetDamageDice(DieType.D6, 1)
            .SetAdditionalDamageType(AdditionalDamageType.SameAsBaseDamage)
            .SetNotificationTag("GambitDie")
            .AddConditionOperation(
                new ConditionOperationDescription
                {
                    operation = ConditionOperationDescription.ConditionOperation.Add,
                    conditionName = MartialTactician.MarkDamagedByGambit
                })
            .SetFrequencyLimit(FeatureLimitedUsage.None)
            .AddCustomSubFeatures(
                ModifyAdditionalDamageFormGambitDieSize.Marker,
                ValidatorsRestrictedContext.IsMeleeOrUnarmedAttack)
            .AddToDB();

        var conditionGambitDieDamage = ConditionDefinitionBuilder
            .Create("ConditionGambitDieDamage")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(gambitDieDamage)
            .SetSpecialInterruptions(ExtraConditionInterruption.AttacksWithWeaponOrUnarmed)
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
            .SetSituationalContext(ExtraSituationalContext.IsNotConditionSource)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
                                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
                                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
                                    .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
                                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
                                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
                    .SetDurationData(DurationType.Round, 1, (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
                                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
                    .ExcludeCaster()
                    .SetTargetingData(Side.Ally, RangeType.Distance, 6, TargetType.IndividualsUnique)
                    .SetDurationData(DurationType.Round)
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
                                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
                                .AddToDB()))
                    .Build())
            .AddToDB();

        reactionPower.AddCustomSubFeatures(ForcePowerUseInSpendPowerAction.Marker);

        power = FeatureDefinitionPowerBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
                                .AddCustomSubFeatures(AddUsablePowersFromCondition.Marker)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, GambitLimiter, hasGambitDice)
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
            new UpgradeSpellRangeBasedOnWeaponReach(power),
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, hasGambitDice)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, hasGambitDice)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, hasGambitDice)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, hasGambitDice)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, hasGambitDice)
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
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, hasGambitDice)
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
            .AddToDB();

        power = FeatureDefinitionPowerSharedPoolBuilder
            .Create($"Power{name}Activate")
            .SetGuiPresentation(name, Category.Feature, sprite)
            .SetShowCasting(false)
            .AddCustomSubFeatures(ModifyPowerFromInvocation.Marker, hasGambitDice)
            .SetUniqueInstance()
            .SetSharedPool(ActivationTime.BonusAction, GambitPool)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.All, RangeType.Touch, 0, TargetType.IndividualsUnique)
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

    private sealed class ModifyEffectDescriptionSavingThrow(
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

    private sealed class ModifyAdditionalDamageFormGambitDieSize : IModifyAdditionalDamageForm
    {
        public static readonly ModifyAdditionalDamageFormGambitDieSize Marker = new();

        public DamageForm AdditionalDamageForm(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            FeatureDefinitionAdditionalDamage featureDefinitionAdditionalDamage,
            DamageForm damageForm)
        {
            var rulesetAttacker = attacker.RulesetCharacter;

            if (!rulesetAttacker.TryGetConditionOfCategoryAndType(
                    AttributeDefinitions.TagEffect, "ConditionGambitUrgent", out var activeCondition))
            {
                damageForm.DieType = GetGambitDieSize(attacker.RulesetCharacter);

                return damageForm;
            }

            var rulesetSource = EffectHelpers.GetCharacterByGuid(activeCondition.SourceGuid);

            if (rulesetSource == null)
            {
                return damageForm;
            }

            damageForm.dieType = GetGambitDieSize(rulesetSource);

            return damageForm;
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
    // Rally
    //
    private sealed class Rally(FeatureDefinitionPower powerRallyActivate) : IMagicEffectInitiatedByMe
    {
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

            character.ShowDieRoll(dieType, dieRoll, title: powerRallyActivate.GuiPresentation.Title);
            character.LogCharacterUsedPower(powerRallyActivate, "Feedback/&GambitGrantTempHP", true,
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
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class SwiftThrow(ItemDefinition concealedDagger, FeatureDefinitionPower powerSwiftThrow)
        : IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe, IModifyAttackActionModifier
    {
        private const int DaggerCloseRange = 4;

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
            attackModeCopy.SourceDefinition = concealedDagger;
            attackModeCopy.EffectDescription = concealedDagger.WeaponDescription.EffectDescription;
            attackModeCopy.AttackTags.SetRange(concealedDagger.WeaponDescription.WeaponTags);
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
            if (effectName != powerSwiftThrow.Name)
            {
                return;
            }

            var glcMyself = GameLocationCharacter.GetFromActor(myself);
            var glcDefender = GameLocationCharacter.GetFromActor(defender);

            if (glcMyself != null &&
                glcDefender != null &&
                !glcMyself.IsWithinRange(glcDefender, DaggerCloseRange))
            {
                attackModifier.AttackAdvantageTrends.Add(
                    new TrendInfo(-1, FeatureSourceType.Equipment, "Tooltip/&ProximityLongRangeTitle", null));
            }
        }
    }

    //
    // Tactical Strike
    //
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class TacticalStrike(FeatureDefinitionPower powerSelectEnemy) :
        IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe, IFilterTargetingCharacter
    {
        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != powerSelectEnemy)
            {
                return true;
            }

            var selectedTargets = __instance.SelectionService.SelectedTargets;

            //
            // only allow allies that can react
            //
            if (selectedTargets.Count == 0)
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

            actingCharacter.BurnOneMainAttack();
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
    private sealed class Feint(FeatureDefinitionPower pool) :
        IModifyAttackActionModifier, IPhysicalAttackInitiatedByMe, IPhysicalAttackFinishedByMe
    {
        private const string ConditionGambitFeint = "ConditionGambitFeint";

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
            RollOutcome rollOutcome,
            int damageAmount)
        {
            var rulesetCharacter = attacker.RulesetCharacter;

            if (attackMode?.SourceDefinition is not ItemDefinition ||
                !ValidatorsCharacter.HasAvailableBonusAction(rulesetCharacter))
            {
                yield break;
            }

            attacker.CurrentActionRankByType[ActionDefinitions.ActionType.Bonus]++;
            rulesetCharacter.UpdateUsageForPower(pool, 1);
        }

        public IEnumerator OnPhysicalAttackInitiatedByMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            ActionModifier attackModifier,
            RulesetAttackMode attackMode)
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
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class Retaliate(FeatureDefinitionPower pool, ConditionDefinition condition, bool melee)
        : IPhysicalAttackFinishedOnMe
    {
        public IEnumerator OnPhysicalAttackFinishedOnMe(
            GameLocationBattleManager battleManager,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RulesetAttackMode attackMode,
            RollOutcome rollOutcome,
            int damageAmount)
        {
            //trigger only on a miss
            if (rollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            //do not trigger on my own turn, so won't retaliate on AoO
            if (defender.IsMyTurn())
            {
                yield break;
            }

            var rulesetEnemy = attacker.RulesetCharacter;

            if (!defender.CanReact() ||
                rulesetEnemy is not { IsDeadOrDyingOrUnconscious: false })
            {
                yield break;
            }

            if (defender.RulesetCharacter.GetRemainingPowerCharges(pool) <= 0)
            {
                yield break;
            }

            var manager = ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;
            var battle = ServiceRepository.GetService<IGameLocationBattleService>() as GameLocationBattleManager;

            if (manager == null || battle is not { IsBattleInProgress: true })
            {
                yield break;
            }

            if (!melee && defender.IsWithinRange(attacker, 1))
            {
                yield break;
            }

            var (retaliationMode, retaliationModifier) = melee
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

            var previousReactionCount = manager.PendingReactionRequestGroups.Count;
            var tag = melee ? "GambitRiposte" : "GambitReturnFire";
            var reactionRequest = new ReactionRequestReactionAttack(tag, reactionParams)
            {
                Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, manager, previousReactionCount);
        }
    }

    //
    // Switch
    //
    private sealed class Switch(
        FeatureDefinitionPower powerSwitchActivate,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition good,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition bad,
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        ConditionDefinition self)
        : IFilterTargetingCharacter, IMagicEffectFinishedByMe
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != powerSwitchActivate)
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
                    bad.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfSourceTurn,
                    AttributeDefinitions.TagEffect,
                    caster.Guid,
                    caster.CurrentFaction.Name,
                    1,
                    bad.Name,
                    0,
                    0,
                    0);

                caster.InflictCondition(
                    self.Name,
                    DurationType.Round,
                    0,
                    TurnOccurenceType.EndOfTurn,
                    AttributeDefinitions.TagEffect,
                    target.Guid,
                    target.CurrentFaction.Name,
                    1,
                    self.Name,
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

            caster.ShowDieRoll(dieType, dieRoll, title: good.GuiPresentation.Title);

            var finalTarget = !reactionParams.ReactionValidated ? caster : target;

            finalTarget.InflictCondition(
                good.Name,
                DurationType.Round,
                1,
                (TurnOccurenceType)ExtraTurnOccurenceType.StartOfSourceTurn,
                AttributeDefinitions.TagEffect,
                caster.Guid,
                caster.CurrentFaction.Name,
                1,
                good.Name,
                dieRoll,
                0,
                0);

            caster.LogCharacterUsedPower(powerSwitchActivate, "Feedback/&GambitSwitchACIncrease", true,
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
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class Precise(FeatureDefinitionPower pool, FeatureDefinition feature)
        : ITryAlterOutcomeAttack
    {
        private const string Format = "Reaction/&CustomReactionGambitPreciseDescription";
        private const string Line = "Feedback/&GambitPreciseToHitRoll";

        public IEnumerator OnTryAlterOutcomeAttack(
            GameLocationBattleManager battle,
            CharacterAction action,
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            GameLocationCharacter helper,
            ActionModifier attackModifier)
        {
            var gameLocationActionManager =
                ServiceRepository.GetService<IGameLocationActionService>() as GameLocationActionManager;

            if (gameLocationActionManager == null)
            {
                yield break;
            }

            if (action.AttackRollOutcome is not (RollOutcome.Failure or RollOutcome.CriticalFailure))
            {
                yield break;
            }

            if (attacker != helper)
            {
                yield break;
            }

            var rulesetCharacter = attacker.RulesetCharacter;

            if (rulesetCharacter is not { IsDeadOrDyingOrUnconscious: false } ||
                !rulesetCharacter.CanUsePower(pool) ||
                !attacker.CanPerceiveTarget(defender))
            {
                yield break;
            }

            var dieType = GetGambitDieSize(rulesetCharacter);
            var max = DiceMaxValue[(int)dieType];
            var delta = Math.Abs(action.AttackSuccessDelta);

            if (max < delta)
            {
                yield break;
            }

            var guiAttacker = new GuiCharacter(attacker);
            var guiDefender = new GuiCharacter(defender);

            var reactionParams =
                new CharacterActionParams(attacker, (ActionDefinitions.Id)ExtraActionId.DoNothingFree)
                {
                    StringParameter = Gui.Format(
                        Format, guiAttacker.Name, guiDefender.Name, delta.ToString(), Gui.FormatDieTitle(dieType))
                };

            var previousReactionCount = gameLocationActionManager.PendingReactionRequestGroups.Count;
            var reactionRequest = new ReactionRequestCustom("GambitPrecise", reactionParams)
            {
                Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon)
            };

            gameLocationActionManager.AddInterruptRequest(reactionRequest);

            yield return battle.WaitForReactions(attacker, gameLocationActionManager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            rulesetCharacter.UpdateUsageForPower(pool, 1);

            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);
            var hitTrends = attackModifier.AttacktoHitTrends;

            hitTrends?.Add(new TrendInfo(dieRoll, FeatureSourceType.Power, pool.Name, null)
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

            rulesetCharacter.ShowDieRoll(
                dieType,
                dieRoll,
                title: feature.GuiPresentation.Title,
                outcome: success ? RollOutcome.Success : RollOutcome.Failure,
                displayOutcome: true
            );

            rulesetCharacter.LogCharacterUsedFeature(
                feature,
                Line,
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
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class Parry(FeatureDefinitionPower pool, FeatureDefinition feature) : IAttackBeforeHitConfirmedOnMe
    {
        private const string Line = "Feedback/&GambitParryDamageReduction";

        public IEnumerator OnAttackBeforeHitConfirmedOnMe(GameLocationBattleManager battleManager,
            GameLocationCharacter attacker,
            GameLocationCharacter me,
            ActionModifier actionModifier,
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

            if (character.GetRemainingPowerCharges(pool) <= 0)
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
                Resource = new ReactionResourcePowerPool(pool, Sprites.GambitResourceIcon)
            };

            manager.AddInterruptRequest(reactionRequest);

            yield return battleManager.WaitForReactions(me, manager, previousReactionCount);

            if (!reactionParams.ReactionValidated)
            {
                yield break;
            }

            character.UpdateUsageForPower(pool, 1);

            var dieRoll = RollDie(dieType, AdvantageType.None, out _, out _);

            var pb = 2 * character.TryGetAttributeValue(AttributeDefinitions.ProficiencyBonus);
            var reduction = dieRoll + pb;

            actionModifier.damageRollReduction += reduction;

            character.ShowDieRoll(dieType, dieRoll,
                title: feature.GuiPresentation.Title,
                displayModifier: true, modifier: pb);

            character.LogCharacterUsedFeature(feature, Line,
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
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class CoordinatedAttack(FeatureDefinitionPower powerCoordinatedAttack) :
        IFilterTargetingCharacter, ISelectPositionAfterCharacter, IFilterTargetingPosition, IMagicEffectFinishedByMe
    {
        public bool EnforceFullSelection => false;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != powerCoordinatedAttack)
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

        public IEnumerator ComputeValidPositions(CursorLocationSelectPosition cursorLocationSelectPosition)
        {
            cursorLocationSelectPosition.validPositionsCache.Clear();

            var actingCharacter = cursorLocationSelectPosition.ActionParams.ActingCharacter;
            var targetCharacter = cursorLocationSelectPosition.ActionParams.TargetCharacters[0];
            var positioningService = ServiceRepository.GetService<IGameLocationPositioningService>();
            var visibilityService =
                ServiceRepository.GetService<IGameLocationVisibilityService>() as GameLocationVisibilityManager;

            var halfMaxTacticalMoves = (targetCharacter.MaxTacticalMoves + 1) / 2;
            var boxInt = new BoxInt(targetCharacter.LocationPosition, int3.zero, int3.zero);

            boxInt.Inflate(halfMaxTacticalMoves, 0, halfMaxTacticalMoves);

            foreach (var position in boxInt.EnumerateAllPositionsWithin())
            {
                if (!visibilityService.MyIsCellPerceivedByCharacter(position, actingCharacter) ||
                    !positioningService.CanPlaceCharacter(
                        actingCharacter, position, CellHelpers.PlacementMode.Station) ||
                    !positioningService.CanCharacterStayAtPosition_Floor(
                        actingCharacter, position, onlyCheckCellsWithRealGround: true))
                {
                    continue;
                }

                cursorLocationSelectPosition.validPositionsCache.Add(position);

                if (cursorLocationSelectPosition.stopwatch.Elapsed.TotalMilliseconds > 0.5)
                {
                    yield return null;
                }
            }
        }

        public IEnumerator OnMagicEffectFinishedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.activeEffect.EffectDescription.rangeParameter = 6;

            var actingCharacter = action.ActingCharacter;
            var targetCharacter = action.ActionParams.TargetCharacters[0];
            var targetRulesetCharacter = targetCharacter.RulesetCharacter;
            var targetPosition = action.ActionParams.Positions[0];
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
                TurnOccurenceType.EndOfTurn,
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

            ServiceRepository.GetService<IGameLocationActionService>()?
                .ExecuteAction(actionParams, null, true);

            yield break;
        }

        public int PositionRange => 12;
    }

    //
    // Elusive Movement
    //
    private sealed class ElusiveMovement(FeatureDefinitionPower powerElusiveMovement) : IOnConditionAddedOrRemoved
    {
        public void OnConditionAdded(RulesetCharacter target, RulesetCondition rulesetCondition)
        {
            var dieType = GetGambitDieSize(target);
            var dieRoll = rulesetCondition.Amount;

            target.LogCharacterUsedPower(powerElusiveMovement, "Feedback/&GambitElusiveMovementACIncrease", true,
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
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    private sealed class OverwhelmingAttack(FeatureDefinitionPower powerOverwhelmingAttack) :
        IFilterTargetingCharacter, IMagicEffectInitiatedByMe, IMagicEffectFinishedByMe
    {
        public bool EnforceFullSelection => true;

        public bool IsValid(CursorLocationSelectTarget __instance, GameLocationCharacter target)
        {
            if (__instance.actionParams.RulesetEffect is not RulesetEffectPower rulesetEffectPower ||
                rulesetEffectPower.PowerDefinition != powerOverwhelmingAttack)
            {
                return true;
            }

            var selectedTargets = __instance.SelectionService.SelectedTargets;

            if (selectedTargets.Count == 0)
            {
                return true;
            }

            var firstTarget = selectedTargets[0];

            if (firstTarget.IsWithinRange(target, 1))
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

            actingCharacter.BurnOneMainAttack();
        }

        public IEnumerator OnMagicEffectInitiatedByMe(CharacterActionMagicEffect action, BaseDefinition baseDefinition)
        {
            action.ActionParams.RulesetEffect.EffectDescription.RangeType = RangeType.Distance;

            yield break;
        }
    }

    // supports displaying the gambit die type and remaining usages on action buttons
    internal interface IActionItemDiceBox
    {
        (DieType type, int number, string format) GetDiceInfo(RulesetCharacter character);
    }

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
