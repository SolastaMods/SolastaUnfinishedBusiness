using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolastaModApi;
using SolastaModApi.Extensions;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;
using TA;

namespace SolastaCommunityExpansion.Feats
{
    class TorchbearerFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        const string TorchbearerFeatName = "TorchbearerFeat";
        private static readonly string TorchbearerFeatNameGuid = GuidHelper.Create(Core.FP_GUID, TorchbearerFeatName).ToString();

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
            var light_restrict = new HasLightSourceRestriction();

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
            burn_description.SetSavingThrowAbility(Helpers.Stats.Dexterity);
            burn_description.SetSavingThrowDifficultyAbility(Helpers.Stats.Dexterity);
            burn_description.SetDifficultyClassComputation(RuleDefinitions.EffectDifficultyClassComputation.AbilityScoreAndProficiency);
            burn_description.SetSpeedType(RuleDefinitions.SpeedType.Instant);

            burn_description.EffectForms.Clear();
            burn_description.EffectForms.Add(burn_effect);

            return Helpers.FeatureBuilder<NewFeatureDefinitions.PowerWithRestrictions>.createFeature
            (
                "PowerTorchbearer",
                GuidHelper.Create(Core.FP_GUID, "PowerTorchbearer").ToString(),
                "Feature/&PowerTorchbearerTitle",
                "Feature/&PowerTorchbearerDescription",
                DatabaseHelper.FeatureDefinitionPowers.PowerDomainElementalFireBurst.GuiPresentation.SpriteReference,
                a =>
                {
                    a.SetActivationTime(RuleDefinitions.ActivationTime.BonusAction);
                    a.SetCostPerUse(0);
                    a.SetEffectDescription(burn_description);
                    a.SetFixedUsesPerRecharge(1);
                    a.SetUsesDetermination(RuleDefinitions.UsesDetermination.Fixed);
                    a.SetRechargeRate(RuleDefinitions.RechargeRate.AtWill);
                    a.SetShowCasting(false);
                    a.SetUniqueInstance(false);
                    a.restrictions = new List<NewFeatureDefinitions.IRestriction>()
                    {
                        light_restrict
                    };
                }
            );
        }
    }
}