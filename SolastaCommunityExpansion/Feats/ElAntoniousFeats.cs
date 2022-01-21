using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.CustomFeatureDefinitions;
using SolastaModApi;
using SolastaModApi.Extensions;

namespace SolastaCommunityExpansion.Feats
{
    internal static class ElAntoniousFeats
    {
        public static void CreateFeats(List<FeatDefinition> feats)
        {
            feats.Add(DualFlurryFeatBuilder.DualFlurryFeat);
            feats.Add(TorchbearerFeatBuilder.TorchbearerFeat);
        }
    }

    internal class DualFlurryFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        public static readonly Guid DualFlurryGuid = new Guid("03C523EB-91B9-4F1B-A697-804D1BC2D6DD");
        private const string DualFlurryFeatName = "DualFlurryFeat";
        private static readonly string DualFlurryFeatNameGuid = GuidHelper.Create(DualFlurryGuid, DualFlurryFeatName).ToString();

        protected DualFlurryFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.Ambidextrous, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&DualFlurryTitle";
            Definition.GuiPresentation.Description = "Feat/&DualFlurryDescription";

            Definition.Features.Clear();
            Definition.Features.Add(buildFeatureDualFlurry());

            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        public static FeatDefinition CreateAndAddToDB(string name, string guid)
            => new DualFlurryFeatBuilder(name, guid).AddToDB();

        public static readonly FeatDefinition DualFlurryFeat = CreateAndAddToDB(DualFlurryFeatName, DualFlurryFeatNameGuid);

        private static FeatureDefinition buildFeatureDualFlurry()
        {
            FeatureDefinitionOnAttackHitEffectBuilder builder = new FeatureDefinitionOnAttackHitEffectBuilder(
                "FeatureDualFlurry", GuidHelper.Create(DualFlurryGuid, "FeatureDualFlurry").ToString(),
                OnAttackHit, new GuiPresentationBuilder("Feature/&DualFlurryDescription", "Feature/&DualFlurryTitle").Build());

            return builder.AddToDB();
        }

        private static void OnAttackHit(GameLocationCharacter attacker,
                GameLocationCharacter defender, ActionModifier attackModifier, RulesetAttackMode attackMode,
                bool rangedAttack, RuleDefinitions.AdvantageType advantageType, List<EffectForm> actualEffectForms,
                RulesetEffect rulesetEffect, bool criticalHit, bool firstTarget)
        {
            // Note the game code currently always passes attackMode = null for magic attacks,
            // if that changes this will need to be updated.
            if (rangedAttack || attackMode == null)
            {
                return;
            }

            var condition = attacker.RulesetCharacter.HasConditionOfType(ConditionDualFlurryApplyBuilder.GetOrAdd().Name) ?
                ConditionDualFlurryGrantBuilder.GetOrAdd() : ConditionDualFlurryApplyBuilder.GetOrAdd();

            RulesetCondition active_condition = RulesetCondition.CreateActiveCondition(attacker.RulesetCharacter.Guid,
                                                                                       condition, RuleDefinitions.DurationType.Round, 0,
                                                                                       RuleDefinitions.TurnOccurenceType.EndOfTurn,
                                                                                       attacker.RulesetCharacter.Guid,
                                                                                       attacker.RulesetCharacter.CurrentFaction.Name);
            attacker.RulesetCharacter.AddConditionOfCategory("10Combat", active_condition, true);
        }
    }

    internal class ConditionDualFlurryApplyBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        protected ConditionDualFlurryApplyBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionSurged, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionDualFlurryApplyTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionDualFlurryApplyDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(0);
            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetPossessive(true);
            Definition.SetSilentWhenAdded(true);
            Definition.SetSilentWhenRemoved(true);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
            Definition.Features.Clear();
        }

        public static ConditionDefinition CreateAndAddToDB()
            => new ConditionDualFlurryApplyBuilder("ConditionDualFlurryApply", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryApply").ToString()).AddToDB();

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionDualFlurryApply", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryApply").ToString()) ?? CreateAndAddToDB();
        }
    }

    internal class ConditionDualFlurryGrantBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        protected ConditionDualFlurryGrantBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionSurged, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionDualFlurryGrantTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionDualFlurryGrantDescription";
            Definition.GuiPresentation.SetHidden(true);

            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(0);
            Definition.SetDurationType(RuleDefinitions.DurationType.Round);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetPossessive(true);
            Definition.SetSilentWhenAdded(false);
            Definition.SetSilentWhenRemoved(false);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
            Definition.Features.Clear();
            Definition.Features.Add(BuildAdditionalActionDualFlurry());
        }

        public static ConditionDefinition CreateAndAddToDB()
            => new ConditionDualFlurryGrantBuilder("ConditionDualFlurryGrant", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryGrant").ToString()).AddToDB();

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionDualFlurryGrant", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryGrant").ToString()) ?? CreateAndAddToDB();
        }
        private static FeatureDefinition BuildAdditionalActionDualFlurry()
        {
            GuiPresentationBuilder guiBuilder = new GuiPresentationBuilder("Feature/&AdditionalActionDualFlurryDescription",
                "Feature/&AdditionalActionDualFlurryTitle")
                .SetSpriteReference(DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionSurgedMain.GuiPresentation.SpriteReference);
            FeatureDefinitionAdditionalActionBuilder flurryBuilder = new FeatureDefinitionAdditionalActionBuilder(
                DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionSurgedMain, "AdditionalActionDualFlurry",
                GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "AdditionalActionDualFlurry").ToString())
                .SetGuiPresentation(guiBuilder.Build())
                .SetActionType(ActionDefinitions.ActionType.Bonus)
                .SetAuthorizedActions(new List<ActionDefinitions.Id>())
                .SetForbiddenActions(new List<ActionDefinitions.Id>())
                .SetRestrictedActions(new List<ActionDefinitions.Id>() { ActionDefinitions.Id.AttackOff });
            return flurryBuilder.AddToDB();
        }
    }

    internal class TorchbearerFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        private static readonly Guid TorchbearerGuid = new Guid("03C523EB-91B9-4F1B-A697-804D1BC2D6DD");
        private const string TorchbearerFeatName = "TorchbearerFeat";
        private static readonly string TorchbearerFeatNameGuid = GuidHelper.Create(TorchbearerGuid, TorchbearerFeatName).ToString();

        protected TorchbearerFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.Ambidextrous, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&TorchbearerTitle";
            Definition.GuiPresentation.Description = "Feat/&TorchbearerDescription";

            Definition.Features.Clear();
            Definition.Features.Add(buildFeatureTorchbearer());

            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        public static FeatDefinition CreateAndAddToDB(string name, string guid)
            => new TorchbearerFeatBuilder(name, guid).AddToDB();

        public static readonly FeatDefinition TorchbearerFeat = CreateAndAddToDB(TorchbearerFeatName, TorchbearerFeatNameGuid);

        private static FeatureDefinition buildFeatureTorchbearer()
        {
            var burn_effect = new EffectForm();
            burn_effect.SetFormType(EffectForm.EffectFormType.Condition);
            burn_effect.ConditionForm = new ConditionForm
            {
                Operation = ConditionForm.ConditionOperation.Add,
                ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionOnFire1D4
            };

            var burn_description = new EffectDescription();
            burn_description.Copy(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription);
            burn_description.SetCreatedByCharacter(true);
            burn_description.SetTargetSide(RuleDefinitions.Side.Enemy);
            burn_description.SetTargetType(RuleDefinitions.TargetType.Individuals);
            burn_description.SetTargetParameter(1);
            burn_description.SetRangeType(RuleDefinitions.RangeType.Touch);
            burn_description.SetDurationType(RuleDefinitions.DurationType.Round);
            burn_description.SetDurationParameter(3);
            burn_description.SetCanBePlacedOnCharacter(false);
            burn_description.SetHasSavingThrow(true);
            burn_description.SetSavingThrowAbility(AttributeDefinitions.Dexterity);
            burn_description.SetSavingThrowDifficultyAbility(AttributeDefinitions.Dexterity);
            burn_description.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            burn_description.SetSpeedType(RuleDefinitions.SpeedType.Instant);

            burn_description.EffectForms.Clear();
            burn_description.EffectForms.Add(burn_effect);

            FeatureDefinitionConditionalPowerBuilder powerFeature = new FeatureDefinitionConditionalPowerBuilder("PowerTorchbearer",
                GuidHelper.Create(TorchbearerGuid, "PowerTorchbearer").ToString(),
                new GuiPresentationBuilder("Feature/&PowerTorchbearerDescription", "Feature/&PowerTorchbearerTitle").Build());
            powerFeature.SetActivation(RuleDefinitions.ActivationTime.BonusAction, 0);
            powerFeature.SetEffect(burn_description);
            powerFeature.SetUsesFixed(1);
            powerFeature.SetRecharge(RuleDefinitions.RechargeRate.AtWill);
            powerFeature.SetShowCasting(false);
            powerFeature.SetIsActive(IsActive);

            return powerFeature.AddToDB();
        }

        private static bool IsActive(RulesetCharacterHero hero)
        {
            if (hero == null)
            {
                return false;
            }
            RulesetItem off_item = hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem;

            return (off_item != null && off_item.ItemDefinition != null && off_item.ItemDefinition.IsLightSourceItem);
        }
    }
}
