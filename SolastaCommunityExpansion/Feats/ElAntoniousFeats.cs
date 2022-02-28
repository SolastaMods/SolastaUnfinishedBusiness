using System;
using System.Collections.Generic;
using SolastaCommunityExpansion.Builders;
using SolastaCommunityExpansion.Builders.Features;
using SolastaModApi;
using SolastaModApi.Extensions;
using static SolastaModApi.DatabaseHelper.FeatureDefinitionAdditionalActions;

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

    internal sealed class DualFlurryFeatBuilder : FeatDefinitionBuilder
    {
        public static readonly Guid DualFlurryGuid = new("03C523EB-91B9-4F1B-A697-804D1BC2D6DD");
        private const string DualFlurryFeatName = "DualFlurryFeat";
        private static readonly string DualFlurryFeatNameGuid = GuidHelper.Create(DualFlurryGuid, DualFlurryFeatName).ToString();

        private DualFlurryFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.Ambidextrous, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&DualFlurryTitle";
            Definition.GuiPresentation.Description = "Feat/&DualFlurryDescription";

            Definition.Features.Clear();
            Definition.Features.Add(BuildFeatureDualFlurry());

            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        private static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new DualFlurryFeatBuilder(name, guid).AddToDB();
        }

        public static readonly FeatDefinition DualFlurryFeat = CreateAndAddToDB(DualFlurryFeatName, DualFlurryFeatNameGuid);

        private static FeatureDefinition BuildFeatureDualFlurry()
        {
            return FeatureDefinitionOnAttackHitEffectBuilder
                .Create("FeatureDualFlurry", DualFlurryGuid)
                .SetGuiPresentation("DualFlurry", Category.Feature)
                .SetOnAttackHitDelegate(OnAttackHit)
                .AddToDB();
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

    internal sealed class ConditionDualFlurryApplyBuilder : ConditionDefinitionBuilder
    {
        private ConditionDualFlurryApplyBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionSurged, name, guid)
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

        private static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionDualFlurryApplyBuilder("ConditionDualFlurryApply", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryApply").ToString()).AddToDB();
        }

        // TODO: eliminate
        internal static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionDualFlurryApply", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryApply").ToString()) ?? CreateAndAddToDB();
        }
    }

    internal sealed class ConditionDualFlurryGrantBuilder : ConditionDefinitionBuilder
    {
        private ConditionDualFlurryGrantBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionSurged, name, guid)
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

        private static ConditionDefinition CreateAndAddToDB()
        {
            return new ConditionDualFlurryGrantBuilder("ConditionDualFlurryGrant", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryGrant").ToString()).AddToDB();
        }

        // TODO: eliminate
        internal static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionDualFlurryGrant", GuidHelper.Create(DualFlurryFeatBuilder.DualFlurryGuid, "ConditionDualFlurryGrant").ToString()) ?? CreateAndAddToDB();
        }

        private static FeatureDefinition BuildAdditionalActionDualFlurry()
        {
            return FeatureDefinitionAdditionalActionBuilder
                .Create(AdditionalActionSurgedMain, "AdditionalActionDualFlurry", DualFlurryFeatBuilder.DualFlurryGuid)
                .SetGuiPresentation(Category.Feature, AdditionalActionSurgedMain.GuiPresentation.SpriteReference)
                .SetActionType(ActionDefinitions.ActionType.Bonus)
                .SetRestrictedActions(ActionDefinitions.Id.AttackOff)
                .AddToDB();
        }
    }

    internal sealed class TorchbearerFeatBuilder : FeatDefinitionBuilder
    {
        private static readonly Guid TorchbearerGuid = new("03C523EB-91B9-4F1B-A697-804D1BC2D6DD");
        private const string TorchbearerFeatName = "TorchbearerFeat";
        private static readonly string TorchbearerFeatNameGuid = GuidHelper.Create(TorchbearerGuid, TorchbearerFeatName).ToString();

        private TorchbearerFeatBuilder(string name, string guid) : base(DatabaseHelper.FeatDefinitions.Ambidextrous, name, guid)
        {
            Definition.GuiPresentation.Title = "Feat/&TorchbearerTitle";
            Definition.GuiPresentation.Description = "Feat/&TorchbearerDescription";

            Definition.Features.Clear();
            Definition.Features.Add(BuildFeatureTorchbearer());

            Definition.SetMinimalAbilityScorePrerequisite(false);
        }

        private static FeatDefinition CreateAndAddToDB(string name, string guid)
        {
            return new TorchbearerFeatBuilder(name, guid).AddToDB();
        }

        public static readonly FeatDefinition TorchbearerFeat = CreateAndAddToDB(TorchbearerFeatName, TorchbearerFeatNameGuid);

        private static FeatureDefinition BuildFeatureTorchbearer()
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

            return FeatureDefinitionConditionalPowerBuilder
                .Create("PowerTorchbearer", TorchbearerGuid)
                .SetGuiPresentation(Category.Feature)
                .SetActivation(RuleDefinitions.ActivationTime.BonusAction, 0)
                .SetEffectDescription(burn_description)
                .SetUsesFixed(1)
                .SetRechargeRate(RuleDefinitions.RechargeRate.AtWill)
                .SetShowCasting(false)
                .SetIsActive(IsActive).AddToDB();
        }

        private static bool IsActive(RulesetCharacterHero hero)
        {
            if (hero == null)
            {
                return false;
            }
            RulesetItem off_item = hero.CharacterInventory.InventorySlotsByName[EquipmentDefinitions.SlotTypeOffHand].EquipedItem;

            return off_item != null && off_item.ItemDefinition != null && off_item.ItemDefinition.IsLightSourceItem;
        }
    }
}
