using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api;
using SolastaUnfinishedBusiness.Api.Extensions;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.FeatDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper.WeaponTypeDefinitions;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RangedCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featBowMastery = BuildBowMastery();
        var featDeadEye = BuildDeadEye();
        var featRangedExpert = BuildRangedExpert();

        feats.AddRange(featDeadEye, featRangedExpert, featBowMastery);

        GroupFeats.MakeGroup("FeatGroupRangedCombat", null,
            GroupFeats.FeatGroupPiercer,
            TakeAim,
            DiscretionOfTheCoedymwarth,
            UncannyAccuracy,
            featBowMastery,
            featDeadEye,
            featRangedExpert);
    }

    private static FeatDefinition BuildBowMastery()
    {
        const string NAME = "FeatBowMastery";

        var validWeapon = ValidatorsWeapon.IsOfWeaponType(LongbowType, ShortbowType);

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                FeatureDefinitionAttackModifierBuilder
                    .Create($"Custom{NAME}")
                    .SetGuiPresentation(NAME, Category.Feat)
                    .SetDamageRollModifier(1)
                    .SetCustomSubFeatures(
                        new AfterAttackEffectBowMastery(),
                        new RestrictedContextValidator((_, _, character, _, _, mode, _) =>
                            (OperationType.Set, validWeapon(mode, null, character))),
                        new CanUseAttributeForWeapon(AttributeDefinitions.Strength,
                            ValidatorsWeapon.IsOfWeaponType(LongbowType)),
                        new AddExtraRangedAttack(
                            ValidatorsWeapon.IsOfWeaponType(ShortbowType),
                            ActionDefinitions.ActionType.Bonus,
                            ValidatorsCharacter.HasUsedSpecialFeature(nameof(ShortbowType))))
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

        var modifyAttackModeForWeapon = FeatureDefinitionBuilder
            .Create($"ModifyAttackModeForWeapon{Name}")
            .SetGuiPresentationNoContent(true)
            .AddToDB();

        var conditionDeadeye = ConditionDefinitionBuilder
            .Create($"Condition{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                DatabaseHelper.ConditionDefinitions.ConditionHeraldOfBattle)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(modifyAttackModeForWeapon)
            .AddToDB();

        var powerDeadeye = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}")
            .SetGuiPresentation(Name, Category.Feat,
                Sprites.GetSprite("DeadeyeIcon", Resources.DeadeyeIcon, 128, 64))
            .SetUsesFixed(ActivationTime.NoCost)
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
            .SetCustomSubFeatures(
                new ValidatorsPowerUse(ValidatorsCharacter.HasNoneOfConditions(conditionDeadeye.Name)))
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerDeadeye);

        var powerTurnOffDeadeye = FeatureDefinitionPowerBuilder
            .Create($"Power{Name}TurnOff")
            .SetGuiPresentationNoContent(true)
            .SetUsesFixed(ActivationTime.NoCost)
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
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffDeadeye);

        var featDeadeye = FeatDefinitionBuilder
            .Create(Name)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(
                powerDeadeye,
                powerTurnOffDeadeye,
                FeatureDefinitionCombatAffinityBuilder
                    .Create($"CombatAffinity{Name}")
                    .SetGuiPresentation(Name, Category.Feat)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
            .AddToDB();

        concentrationProvider.StopPower = powerTurnOffDeadeye;
        modifyAttackModeForWeapon
            .SetCustomSubFeatures(
                concentrationProvider,
                new ModifyAttackModeForWeaponFeatDeadeye(featDeadeye));

        return featDeadeye;
    }

    private static FeatDefinition BuildRangedExpert()
    {
        const string NAME = "FeatRangedExpert";

        return FeatDefinitionBuilder
            .Create(NAME)
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create($"Feature{NAME}")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new RangedAttackInMeleeDisadvantageRemover(),
                    new AddExtraRangedAttack(ValidatorsWeapon.IsOfWeaponType(CustomWeaponsContext.HandXbowWeaponType),
                        ActionDefinitions.ActionType.Bonus, ValidatorsCharacter.HasAttacked))
                .AddToDB())
            .AddToDB();
    }

    //
    // HELPERS
    //

    private sealed class AfterAttackEffectBowMastery : IAfterAttackEffect
    {
        public void AfterOnAttackHit(
            GameLocationCharacter attacker,
            GameLocationCharacter defender,
            RollOutcome outcome,
            CharacterActionParams actionParams,
            RulesetAttackMode attackMode,
            ActionModifier attackModifier)
        {
            if (actionParams.ActionDefinition != DatabaseHelper.ActionDefinitions.AttackMain ||
                !ValidatorsWeapon.IsOfWeaponType(ShortbowType)(attackMode, null, null))
            {
                return;
            }

            attacker.UsedSpecialFeatures.Add(nameof(ShortbowType), 1);
        }
    }

    private sealed class ModifyAttackModeForWeaponFeatDeadeye : IModifyAttackModeForWeapon
    {
        private readonly FeatDefinition _featDefinition;

        public ModifyAttackModeForWeaponFeatDeadeye(FeatDefinition featDefinition)
        {
            _featDefinition = featDefinition;
        }

        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            if (!ValidatorsWeapon.IsOfWeaponType(LongbowType, ShortbowType, HeavyCrossbowType, LightCrossbowType,
                    CustomWeaponsContext.HandXbowWeaponType)(attackMode, null, null))
            {
                return;
            }

            var damage = attackMode?.EffectDescription?.FindFirstDamageForm();

            if (damage == null)
            {
                return;
            }

            const int TO_HIT = -5;
            const int TO_DAMAGE = +10;

            attackMode.ToHitBonus += TO_HIT;
            attackMode.ToHitBonusTrends.Add(new TrendInfo(TO_HIT, FeatureSourceType.Feat, _featDefinition.Name,
                _featDefinition));

            damage.BonusDamage += TO_DAMAGE;
            damage.DamageBonusTrends.Add(new TrendInfo(TO_DAMAGE, FeatureSourceType.Feat, _featDefinition.Name,
                _featDefinition));
        }
    }
}
