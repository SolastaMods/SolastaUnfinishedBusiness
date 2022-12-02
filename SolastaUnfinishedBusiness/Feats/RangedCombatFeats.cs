using System.Collections.Generic;
using JetBrains.Annotations;
using SolastaUnfinishedBusiness.Api.Infrastructure;
using SolastaUnfinishedBusiness.Builders;
using SolastaUnfinishedBusiness.Builders.Features;
using SolastaUnfinishedBusiness.CustomBehaviors;
using SolastaUnfinishedBusiness.CustomInterfaces;
using SolastaUnfinishedBusiness.CustomUI;
using SolastaUnfinishedBusiness.Models;
using SolastaUnfinishedBusiness.Properties;
using static RuleDefinitions;
using static SolastaUnfinishedBusiness.Api.DatabaseHelper;

namespace SolastaUnfinishedBusiness.Feats;

internal static class RangedCombatFeats
{
    internal static void CreateFeats([NotNull] List<FeatDefinition> feats)
    {
        var featDeadEye = BuildDeadEye();
        var featRangedExpert = BuildRangedExpert();

        feats.AddRange(featDeadEye, featRangedExpert);

        GroupFeats.MakeGroup("FeatGroupRangedCombat", null,
            FeatDefinitions.TakeAim,
            FeatDefinitions.UncannyAccuracy,
            featDeadEye,
            featRangedExpert);
    }

    private static FeatDefinition BuildDeadEye()
    {
        const string FEAT_NAME = "FeatDeadeye";

        var conditionDeadeye = ConditionDefinitionBuilder
            .Create("ConditionDeadeye")
            .SetGuiPresentation(FEAT_NAME, Category.Feat)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("ModifyAttackModeForWeaponFeatDeadeye")
                    .SetGuiPresentation(FEAT_NAME, Category.Feat)
                    .SetCustomSubFeatures(new ModifyDeadeyeAttackPower())
                    .AddToDB())
            .AddToDB();

        var concentrationProvider = new StopPowerConcentrationProvider(
            "Deadeye",
            "Tooltip/&DeadeyeConcentration",
            Sprites.GetSprite("DeadeyeConcentrationIcon", Resources.DeadeyeConcentrationIcon, 64, 64));

        var conditionDeadeyeTrigger = ConditionDefinitionBuilder
            .Create("ConditionDeadeyeTrigger")
            .SetGuiPresentationNoContent(true)
            .SetSilent(Silent.WhenAddedOrRemoved)
            .SetFeatures(
                FeatureDefinitionBuilder
                    .Create("TriggerFeatureDeadeye")
                    .SetGuiPresentationNoContent(true)
                    .SetCustomSubFeatures(concentrationProvider)
                    .AddToDB())
            .AddToDB();

        var powerDeadeye = FeatureDefinitionPowerBuilder
            .Create("PowerDeadeye")
            .SetGuiPresentation(FEAT_NAME, Category.Feat,
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
                            .SetConditionForm(conditionDeadeyeTrigger, ConditionForm.ConditionOperation.Add)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Add)
                            .Build())
                    .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerDeadeye);

        var powerTurnOffDeadeye = FeatureDefinitionPowerBuilder
            .Create("PowerTurnOffDeadeye")
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
                            .SetConditionForm(conditionDeadeyeTrigger, ConditionForm.ConditionOperation.Remove)
                            .Build(),
                        EffectFormBuilder
                            .Create()
                            .SetConditionForm(conditionDeadeye, ConditionForm.ConditionOperation.Remove)
                            .Build())
                    .Build())
            .AddToDB();

        Global.PowersThatIgnoreInterruptions.Add(powerTurnOffDeadeye);
        concentrationProvider.StopPower = powerTurnOffDeadeye;

        return FeatDefinitionBuilder
            .Create(FEAT_NAME)
            .SetGuiPresentation(Category.Feat,
                Gui.Format("Feat/&FeatDeadeyeDescription", Main.Settings.DeadEyeAndPowerAttackBaseValue.ToString()))
            .SetFeatures(
                powerDeadeye,
                powerTurnOffDeadeye,
                FeatureDefinitionCombatAffinityBuilder
                    .Create("CombatAffinityDeadeyeIgnoreDefender")
                    .SetGuiPresentation(FEAT_NAME, Category.Feat)
                    .SetIgnoreCover()
                    .SetCustomSubFeatures(new BumpWeaponAttackRangeToMax(ValidatorsWeapon.AlwaysValid))
                    .AddToDB())
            .AddToDB();
    }

    private static FeatDefinition BuildRangedExpert()
    {
        return FeatDefinitionBuilder
            .Create("FeatRangedExpert")
            .SetGuiPresentation(Category.Feat)
            .SetFeatures(FeatureDefinitionBuilder
                .Create("FeatureFeatRangedExpert")
                .SetGuiPresentationNoContent(true)
                .SetCustomSubFeatures(
                    new RangedAttackInMeleeDisadvantageRemover(),
                    new AddExtraRangedAttack(IsOneHandedRanged, ActionDefinitions.ActionType.Bonus,
                        ValidatorsCharacter.HasAttacked))
                .AddToDB())
            .AddToDB();
    }

    //
    // HELPERS
    //

    private static bool IsOneHandedRanged(RulesetAttackMode mode, RulesetItem weapon, RulesetCharacter character)
    {
        return ValidatorsWeapon.IsRanged(weapon) && ValidatorsWeapon.IsOneHanded(weapon);
    }

    private sealed class ModifyDeadeyeAttackPower : IModifyAttackModeForWeapon
    {
        public void ModifyAttackMode(RulesetCharacter character, [CanBeNull] RulesetAttackMode attackMode)
        {
            if (attackMode is not { Reach: false, Ranged: true })
            {
                return;
            }

            SrdAndHouseRulesContext.ModifyAttackModeAndDamage(character, "Feat/&FeatDeadEyeTitle", attackMode);
        }
    }
}
