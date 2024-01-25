using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomGenericBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.CustomValidators;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using UnityEngine.AddressableAssets;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatureDefinitionMovementAffinitys;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RangedCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featBowMastery = BuildBowMastery();
        var featCrossbowMastery = BuildCrossbowMastery();
        var featDeadEye = BuildDeadEye();
        var featRangedExpert = BuildRangedExpert();
        var featSteadyAim = BuildSteadyAim();
        var featZenArcher = BuildZenArcher();

        feats.AddRange(
            featBowMastery, featCrossbowMastery, featDeadEye, featRangedExpert, featSteadyAim, featZenArcher);

        GroupFeats.FeatGroupRangedCombat.AddFeats(
            featBowMastery,
            featCrossbowMastery,
            featDeadEye,
            featRangedExpert,
            featSteadyAim,
            featZenArcher);
    }

    private static FeatDefinition BuildZenArcher()
    {
        const string Name = "ZenArcher";

        var attackModifier = FeatureDefinitionAttackModifierBuilder
            .Create($"Feature{Name}")
            .SetGuiPresentation(Name, Category.FightingStyle)
            .AddCustomSubFeatures(
                new CanUseAttribute(
                    AttributeDefinitions.Wisdom,
                    ValidatorsWeapon.IsOfWeaponType(
                        LongbowType,
                        ShortbowType)))
            .AddToDB();

        // kept for backward compatibility
        _ = FightingStyleBuilder
            .Create(Name)
            .SetGuiPresentation(Category.FightingStyle, DatabaseHelper.FightingStyleDefinitions.Archery)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Maraike,
                attackModifier)
            .AddToDB();

        return FeatDefinitionBuilder
            .Create($"Feat{Name}")
            .SetGuiPresentation(Name, Category.FightingStyle)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Maraike,
                attackModifier)
            .AddToDB();
    }

    private static FeatDefinition BuildBowMastery()
    {
        const string NAME = "FeatBowMastery";

        var isLongOrShortbow = ValidatorsWeapon.IsOfWeaponType(LongbowType, ShortbowType);

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create($"Custom{NAME}")
                    .SetGuiPresentation(NAME, Category.Feat)
                    .SetDamageRollModifier(1)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .AddCustomSubFeatures(
                        new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
                            (OperationType.Set, isLongOrShortbow(mode, null, character))),
                        new CanUseAttribute(
                            AttributeDefinitions.Strength,
                            ValidatorsWeapon.IsOfWeaponType(LongbowType)),
                        new AddExtraRangedAttack(
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(ShortbowType),
                            ValidatorsCharacter.HasUsedWeaponType(ShortbowType)))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildCrossbowMastery()
    {
        const string NAME = "FeatCrossbowMastery";

        var isCrossbow = ValidatorsWeapon.IsOfWeaponType(
            HeavyCrossbowType, LightCrossbowType, CustomWeaponsContext.HandXbowWeaponType);

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create($"Custom{NAME}")
                    .SetGuiPresentation(NAME, Category.Feat)
                    .SetDamageRollModifier(1)
                    .SetRequiredProperty(RestrictedContextRequiredProperty.RangeWeapon)
                    .AddCustomSubFeatures(
                        new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
                            (OperationType.Set, isCrossbow(mode, null, character))),
                        new CanUseAttribute(
                            AttributeDefinitions.Strength,
                            ValidatorsWeapon.IsOfWeaponType(HeavyCrossbowType)),
                        new AddExtraRangedAttack(
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(LightCrossbowType),
                            ValidatorsCharacter.HasUsedWeaponType(LightCrossbowType)),
                        new AddExtraRangedAttack(
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(CustomWeaponsContext.HandXbowWeaponType),
                            ValidatorsCharacter.HasUsedWeaponType(CustomWeaponsContext.HandXbowWeaponType)))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildDeadEye()
    {
        const string Name = "FeatDeadeye";

        var concentrationProvider = new StopPowerConcentrationProvider(
            "Deadeye",
            "Tooltip/&DeadeyeConcentration",
            Sprites.GetSprite("DeadeyeConcentrationIcon", Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionDeadeye = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .AddToDB();

        var powerDeadeye = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                Sprites.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Permanent)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(
                IgnoreInterruptionCheck.Marker,
                new ValidatorsValidatePowerUse(ValidatorsCharacter.HasNoneOfConditions(conditionDeadeye.Name)))
            .AddToDB();

        var powerTurnOffDeadeye = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TurnOff")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
            .SetShowCasting(false)
            .SetEffectDescription(
                EffectDescriptionBuilder
                    .Create()
                    .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                    .SetDurationData(DurationType.Round, 1)
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(IgnoreInterruptionCheck.Marker)
            .AddToDB();

        var featDeadeye = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerDeadeye,
                powerTurnOffDeadeye,
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}")
                    .SetGuiPresentation(Name, Category.Feat, Gui.NoLocalization)
                    .SetIgnoreCover()
                    .AddCustomSubFeatures(new BumpWeaponWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
            .AddToDB();

        concentrationProvider.StopPower = powerTurnOffDeadeye;
        conditionDeadeye.AddCustomSubFeatures(
            concentrationProvider, new ModifyWeaponAttackModeFeatDeadeye(featDeadeye));

        return featDeadeye;
    }

    private static FeatDefinition BuildRangedExpert()
    {
        const string NAME = "FeatRangedExpert";

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create(DatabaseHelper.FeatureDefinitionAttackModifiers.AttackModifierFightingStyleTwoWeapon,
                        $"AttackModifier{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        ValidatorsCharacter.HasOffhandWeaponType(
                            CustomWeaponsContext.HandXbowWeaponType, CustomWeaponsContext.LightningLauncherType),
                        new RemoveRangedAttackInMeleeDisadvantage(),
                        new InnovationArmor.AddLauncherAttack(ActionDefinitions.ActionType.Bonus,
                            InnovationArmor.InInfiltratorMode,
                            ValidatorsCharacter.HasAttacked),
                        new AddExtraRangedAttack(
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(CustomWeaponsContext.HandXbowWeaponType),
                            ValidatorsCharacter.HasAttacked))
                    .AddToDB())
            .AddToDB();
    }

    //
    // HELPERS
    //

    private sealed class ModifyWeaponAttackModeFeatDeadeye(
        // ReSharper disable once SuggestBaseTypeForParameterInConstructor
        FeatDefinition featDefinition) : IModifyWeaponAttackMode
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsOfWeaponType(DartType)(attackMode, null, null) &&
                attackMode is not { ranged: true })
            {
                return;
            }

            var damage = attackMode?.EffectDescription.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            const int TO_HIT = -5;
            const int TO_DAMAGE = +10;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT, FeatureSourceType.Feat, featDefinition.Name,
                featDefinition));

            damage.BonusDamage += TO_DAMAGE;
            damage.DamageBonusTrends.Add(new TrendInfo(TO_DAMAGE, FeatureSourceType.Feat, featDefinition.Name,
                featDefinition));
        }
    }

    #region Steady Aim

    private const string FeatSteadyAim = "FeatSteadyAim";

    internal static readonly FeatureDefinitionPower PowerFeatSteadyAim = FeatureDefinitionPowerBuilder
        .Create($"Power{FeatSteadyAim}")
        .SetGuiPresentation(Category.Feature, Sprites.GetSprite(FeatSteadyAim, Resources.PowerSteadyAim, 256, 128))
        .SetUsesFixed(ActivationTime.BonusAction)
        .SetEffectDescription(
            EffectDescriptionBuilder
                .Create()
                .SetDurationData(DurationType.Round, 0, TurnOccurenceType.StartOfTurn)
                .SetTargetingData(Side.Ally, RangeType.Self, 0, TargetType.Self)
                .SetEffectForms(
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{FeatSteadyAim}Advantage")
                                .SetGuiPresentation(Category.Condition,
                                    DatabaseHelper.ConditionDefinitions.ConditionGuided)
                                .SetPossessive()
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetSpecialInterruptions(ConditionInterruption.Attacks,
                                    ConditionInterruption.AnyBattleTurnEnd)
                                .AddFeatures(
                                    FeatureDefinitionCombatAffinityBuilder
                                        .Create($"CombatAffinity{FeatSteadyAim}")
                                        .SetGuiPresentation(FeatSteadyAim, Category.Feat, Gui.NoLocalization)
                                        .SetMyAttackAdvantage(AdvantageType.Advantage)
                                        .AddToDB())
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build(),
                    EffectFormBuilder
                        .Create()
                        .SetConditionForm(
                            ConditionDefinitionBuilder
                                .Create($"Condition{FeatSteadyAim}Restrained")
                                .SetGuiPresentation(Category.Condition)
                                .SetSilent(Silent.WhenAddedOrRemoved)
                                .SetSpecialInterruptions(ConditionInterruption.AnyBattleTurnEnd)
                                .AddFeatures(MovementAffinityConditionRestrained)
                                .AddToDB(),
                            ConditionForm.ConditionOperation.Add)
                        .Build())
                .SetParticleEffectParameters(DatabaseHelper.FeatureDefinitionPowers.PowerFunctionWandFearCommand)
                .Build())
        .AddCustomSubFeatures(
            new ValidatorsValidatePowerUse(character =>
            {
                var gameLocationCharacter = GameLocationCharacter.GetFromActor(character);

                return gameLocationCharacter == null || gameLocationCharacter.UsedTacticalMoves == 0;
            }))
        .AddToDB();

    private static FeatDefinition BuildSteadyAim()
    {
        PowerFeatSteadyAim.EffectDescription.EffectParticleParameters.impactParticleReference = new AssetReference();

        return FeatDefinitionBuilder
            .Create(FeatSteadyAim)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                DatabaseHelper.FeatureDefinitionAttributeModifiers.AttributeModifierCreed_Of_Misaye,
                PowerFeatSteadyAim)
            .AddToDB();
    }

    #endregion
}
