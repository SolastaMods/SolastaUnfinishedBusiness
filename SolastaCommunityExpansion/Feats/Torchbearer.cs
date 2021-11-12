using System;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaCommunityExpansion.CustomFeatureDefinitions;

namespace SolastaCommunityExpansion.Feats
{
    class TorchbearerFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        private static Guid TorchbearerGuid = new Guid("03C523EB-91B9-4F1B-A697-804D1BC2D6DD");
        const string TorchbearerFeatName = "TorchbearerFeat";
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

        public static FeatDefinition TorchbearerFeat = CreateAndAddToDB(TorchbearerFeatName, TorchbearerFeatNameGuid);

        public static void AddToFeatList()
        {
            var TorchbearerFeat = TorchbearerFeatBuilder.TorchbearerFeat;
        }

        private static FeatureDefinition buildFeatureTorchbearer()
        {

            var burn_effect = new EffectForm();
            burn_effect.SetFormType(EffectForm.EffectFormType.Condition);
            burn_effect.ConditionForm = new ConditionForm();
            burn_effect.ConditionForm.Operation = ConditionForm.ConditionOperation.Add;
            burn_effect.ConditionForm.ConditionDefinition = DatabaseHelper.ConditionDefinitions.ConditionOnFire1D4;

            var burn_description = new EffectDescription();
            burn_description.Copy(DatabaseHelper.SpellDefinitions.Fireball.EffectDescription);
            burn_description.SetCreatedByCharacter(true);
            burn_description.SetTargetSide(RuleDefinitions.Side.Enemy);
            burn_description.SetTargetType(RuleDefinitions.TargetType.Individuals);
            burn_description.SetTargetParameter(1);
            burn_description.SetRangeType(RuleDefinitions.RangeType.Touch);
            burn_description.SetDurationType(RuleDefinitions.DurationType.Turn);
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