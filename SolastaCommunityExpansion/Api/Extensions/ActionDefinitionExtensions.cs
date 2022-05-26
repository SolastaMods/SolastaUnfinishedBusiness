using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ActionDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class ActionDefinitionExtensions
    {
        public static T SetAbilityScore<T>(this T entity, System.String value)
            where T : ActionDefinition
        {
            entity.SetField("abilityScore", value);
            return entity;
        }

        public static T SetActionScope<T>(this T entity, ActionDefinitions.ActionScope value)
            where T : ActionDefinition
        {
            entity.SetField("actionScope", value);
            return entity;
        }

        public static T SetActionType<T>(this T entity, ActionDefinitions.ActionType value)
            where T : ActionDefinition
        {
            entity.SetField("actionType", value);
            return entity;
        }

        public static T SetActivatedPower<T>(this T entity, FeatureDefinitionPower value)
            where T : ActionDefinition
        {
            entity.SetField("activatedPower", value);
            return entity;
        }

        public static T SetAddedConditionName<T>(this T entity, System.String value)
            where T : ActionDefinition
        {
            entity.SetField("addedConditionName", value);
            return entity;
        }

        public static T SetCanTriggerBattle<T>(this T entity, System.Boolean value)
            where T : ActionDefinition
        {
            entity.SetField("canTriggerBattle", value);
            return entity;
        }

        public static T SetClassNameOverride<T>(this T entity, System.String value)
            where T : ActionDefinition
        {
            entity.SetField("classNameOverride", value);
            return entity;
        }

        public static T SetDieType<T>(this T entity, DieType value)
            where T : ActionDefinition
        {
            entity.SetField("dieType", value);
            return entity;
        }

        public static T SetDisplayPowerTooltip<T>(this T entity, System.Boolean value)
            where T : ActionDefinition
        {
            entity.SetField("displayPowerTooltip", value);
            return entity;
        }

        public static T SetFeedbackOnHoverType<T>(this T entity, ActionDefinitions.FeedbackOnHoverType value)
            where T : ActionDefinition
        {
            entity.SetField("feedbackOnHoverType", value);
            return entity;
        }

        public static T SetFocusCameraOnAction<T>(this T entity, System.Boolean value)
            where T : ActionDefinition
        {
            entity.SetField("focusCameraOnAction", value);
            return entity;
        }

        public static T SetFormType<T>(this T entity, ActionDefinitions.ActionFormType value)
            where T : ActionDefinition
        {
            entity.SetField("formType", value);
            return entity;
        }

        public static T SetId<T>(this T entity, ActionDefinitions.Id value)
            where T : ActionDefinition
        {
            entity.SetField("id", value);
            return entity;
        }

        public static T SetIterativeTargeting<T>(this T entity, System.Boolean value)
            where T : ActionDefinition
        {
            entity.SetField("iterativeTargeting", value);
            return entity;
        }

        public static T SetMatchingCondition<T>(this T entity, System.String value)
            where T : ActionDefinition
        {
            entity.SetField("matchingCondition", value);
            return entity;
        }

        public static T SetMaxCells<T>(this T entity, System.Int32 value)
            where T : ActionDefinition
        {
            entity.SetField("maxCells", value);
            return entity;
        }

        public static T SetPairedActionId<T>(this T entity, ActionDefinitions.Id value)
            where T : ActionDefinition
        {
            entity.SetField("pairedActionId", value);
            return entity;
        }

        public static T SetParameter<T>(this T entity, ActionDefinitions.ActionParameter value)
            where T : ActionDefinition
        {
            entity.SetField("parameter", value);
            return entity;
        }

        public static T SetParticlePrefab<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : ActionDefinition
        {
            entity.SetField("particlePrefab", value);
            return entity;
        }

        public static T SetPreventsSerialization<T>(this T entity, System.Boolean value)
            where T : ActionDefinition
        {
            entity.SetField("preventsSerialization", value);
            return entity;
        }

        public static T SetRemovedConditionName<T>(this T entity, System.String value)
            where T : ActionDefinition
        {
            entity.SetField("removedConditionName", value);
            return entity;
        }

        public static T SetRequiresAuthorization<T>(this T entity, System.Boolean value)
            where T : ActionDefinition
        {
            entity.SetField("requiresAuthorization", value);
            return entity;
        }

        public static T SetSoundEvent<T>(this T entity, AK.Wwise.Event value)
            where T : ActionDefinition
        {
            entity.SetField("soundEvent", value);
            return entity;
        }

        public static T SetStealthBreakerBehavior<T>(this T entity, ActionDefinitions.StealthBreakerBehavior value)
            where T : ActionDefinition
        {
            entity.SetField("stealthBreakerBehavior", value);
            return entity;
        }

        public static T SetTargetParameter<T>(this T entity, System.Int32 value)
            where T : ActionDefinition
        {
            entity.SetField("targetParameter", value);
            return entity;
        }

        public static T SetTargetType<T>(this T entity, TargetType value)
            where T : ActionDefinition
        {
            entity.SetField("targetType", value);
            return entity;
        }

        public static T SetUsesPerTurn<T>(this T entity, System.Int32 value)
            where T : ActionDefinition
        {
            entity.SetField("usesPerTurn", value);
            return entity;
        }
    }
}
