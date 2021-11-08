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
    class DualFlurryFeatBuilder : BaseDefinitionBuilder<FeatDefinition>
    {
        const string DualFlurryFeatName = "DualFlurryFeat";
        private static readonly string DualFlurryFeatNameGuid = GuidHelper.Create(Core.FP_GUID, DualFlurryFeatName).ToString();

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

        public static FeatDefinition DualFlurryFeat = CreateAndAddToDB(DualFlurryFeatName, DualFlurryFeatNameGuid);

        public static void AddToFeatList()
        {
            var DualFlurryFeat = DualFlurryFeatBuilder.DualFlurryFeat;
        }

        private static FeatureDefinition buildFeatureDualFlurry()
        {
            var cond_apply = ConditionDualFlurryApplyBuilder.GetOrAdd();
            var cond_grant = ConditionDualFlurryGrantBuilder.GetOrAdd();

            return Helpers.FeatureBuilder<PrereqApplyConditionOnDamageDone>.createFeature
            (
                "FeatureDualFlurry",
                GuidHelper.Create(Core.FP_GUID, "FeatureDualFlurry").ToString(),
                "Feature/&DualFlurryTitle",
                "Feature/&DualFlurryDescription",
                null,
                a =>
                {
                    a.initialize_applicator
                    (
                        true,
                        cond_apply,
                        cond_grant,
                        1,
                        RuleDefinitions.DurationType.Turn,
                        RuleDefinitions.TurnOccurenceType.EndOfTurn,
                        true, false, false
                    );
                }
            );
        }
    }

    internal class ConditionDualFlurryApplyBuilder : BaseDefinitionBuilder<ConditionDefinition>
    {
        protected ConditionDualFlurryApplyBuilder(string name, string guid) : base(DatabaseHelper.ConditionDefinitions.ConditionSurged, name, guid)
        {
            Definition.GuiPresentation.Title = "Condition/&ConditionDualFlurryApplyTitle";
            Definition.GuiPresentation.Description = "Condition/&ConditionDualFlurryApplyDescription";

            Definition.SetAllowMultipleInstances(false);
            Definition.SetDurationParameter(1);
            Definition.SetDurationType(RuleDefinitions.DurationType.Turn);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetPossessive(true);
            Definition.SetSilentWhenAdded(true);
            Definition.SetSilentWhenRemoved(true);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
            Definition.Features.Clear();
        }

        public static ConditionDefinition CreateAndAddToDB()
            => new ConditionDualFlurryApplyBuilder("ConditionDualFlurryApply", GuidHelper.Create(Core.FP_GUID, "ConditionDualFlurryApply").ToString()).AddToDB();

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionDualFlurryApply", GuidHelper.Create(Core.FP_GUID, "ConditionDualFlurryApply").ToString()) ?? CreateAndAddToDB();
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
            Definition.SetDurationParameter(1);
            Definition.SetDurationType(RuleDefinitions.DurationType.Turn);
            Definition.SetTurnOccurence(RuleDefinitions.TurnOccurenceType.EndOfTurn);
            Definition.SetPossessive(true);
            Definition.SetSilentWhenAdded(false);
            Definition.SetSilentWhenRemoved(false);
            Definition.SetConditionType(RuleDefinitions.ConditionType.Beneficial);
            Definition.Features.Clear();
            Definition.Features.Add(buildAdditionalActionDualFlurry());
        }

        public static ConditionDefinition CreateAndAddToDB()
            => new ConditionDualFlurryGrantBuilder("ConditionDualFlurryGrant", GuidHelper.Create(Core.FP_GUID, "ConditionDualFlurryGrant").ToString()).AddToDB();

        public static ConditionDefinition GetOrAdd()
        {
            var db = DatabaseRepository.GetDatabase<ConditionDefinition>();
            return db.TryGetElement("ConditionDualFlurryGrant", GuidHelper.Create(Core.FP_GUID, "ConditionDualFlurryGrant").ToString()) ?? CreateAndAddToDB();
        }

        private static FeatureDefinition buildAdditionalActionDualFlurry()
        {
            return Helpers.CopyFeatureBuilder<FeatureDefinitionAdditionalAction>.createFeatureCopy
            (
                "AdditionalActionDualFlurry",
                GuidHelper.Create(Core.FP_GUID, "AdditionalActionDualFlurry").ToString(),
                "Feature/&AdditionalActionDualFlurryTitle",
                "Feature/&AdditionalActionDualFlurryDescription",
                DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionSurgedMain.GuiPresentation.SpriteReference,
                DatabaseHelper.FeatureDefinitionAdditionalActions.AdditionalActionSurgedMain,
                a =>
                {
                    a.SetActionType(ActionDefinitions.ActionType.Bonus);
                    a.AuthorizedActions.Clear();
                    a.ForbiddenActions.Clear();
                    a.RestrictedActions.Clear();
                    a.RestrictedActions.Add(DatabaseHelper.ActionDefinitions.AttackOff.Id);
                }
            );
        }
    }
}