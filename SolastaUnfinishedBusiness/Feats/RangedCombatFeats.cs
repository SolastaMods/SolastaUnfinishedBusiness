using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.GameExtensions;
using SolastaUnfinishedBusiness.Api.LanguageExtensions;
using SolastaUnfinishedBusiness.Behaviors;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Interfaces;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using SolastaUnfinishedBusiness.Subclasses;
using SolastaUnfinishedBusiness.Validators;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RangedCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featBowMastery = BuildBowMastery();
        var featDeadEye = BuildDeadEye();
        var featRangedExpert = BuildRangedExpert();
        var featZenArcher = BuildZenArcher();

        // kept for backward compatibility
        _ = BuildCrossbowMastery(featBowMastery);

        feats.AddRange(
            featBowMastery, featDeadEye, featRangedExpert, featZenArcher);

        GroupFeats.FeatGroupRangedCombat.AddFeats(
            featBowMastery,
            featDeadEye,
            featRangedExpert,
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

        var isLongOrShortBowOrCrossbow = ValidatorsWeapon.IsOfWeaponType(
            LongbowType, ShortbowType, HeavyCrossbowType, LightCrossbowType, CustomWeaponsContext.HandXbowWeaponType);

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create($"Feature{NAME}")
                    .SetGuiPresentationNoContent(true)
                    .AddCustomSubFeatures(
                        new ValidateContextInsteadOfRestrictedProperty((_, _, character, _, _, mode, _) =>
                            (OperationType.Set, isLongOrShortBowOrCrossbow(mode, null, character))),
                        new AddExtraRangedAttack(
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(LongbowType),
                            ValidatorsCharacter.HasUsedWeaponType(LongbowType)),
                        new AddExtraRangedAttack(
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(ShortbowType),
                            ValidatorsCharacter.HasUsedWeaponType(ShortbowType)),
                        new AddExtraRangedAttack(
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsWeapon.IsOfWeaponType(HeavyCrossbowType),
                            ValidatorsCharacter.HasUsedWeaponType(HeavyCrossbowType)),
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

    private static FeatDefinition BuildCrossbowMastery(FeatDefinition featBowMastery)
    {
        const string NAME = "FeatCrossbowMastery";

        var feat = FeatDefinitionBuilder
            .Create(featBowMastery, NAME)
            .AddToDB();

        feat.GuiPresentation.hidden = true;

        return feat;
    }

    private static FeatDefinition BuildDeadEye()
    {
        const string Name = "FeatDeadeye";

        var concentrationProvider = new StopPowerConcentrationProvider(
            "Deadeye",
            "Tooltip/&DeadeyeConcentration",
            Sprites.GetSprite(Name, Resources.DeadeyeConcentrationIcon, 64, 64));

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
                IgnoreInvisibilityInterruptionCheck.Marker,
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
                    .SetEffectForms(
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddCustomSubFeatures(IgnoreInvisibilityInterruptionCheck.Marker)
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
}
