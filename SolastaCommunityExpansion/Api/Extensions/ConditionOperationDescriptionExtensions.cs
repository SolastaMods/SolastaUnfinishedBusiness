using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ConditionOperationDescription))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class ConditionOperationDescriptionExtensions
    {
        public static T SetCanSaveToCancel<T>(this T entity, Boolean value)
            where T : ConditionOperationDescription
        {
            entity.SetField("canSaveToCancel", value);
            return entity;
        }

        public static T SetConditionDefinition<T>(this T entity, ConditionDefinition value)
            where T : ConditionOperationDescription
        {
            entity.ConditionDefinition = value;
            return entity;
        }

        public static T SetConditionName<T>(this T entity, String value)
            where T : ConditionOperationDescription
        {
            entity.SetField("conditionName", value);
            return entity;
        }

        public static T SetHasSavingThrow<T>(this T entity, Boolean value)
            where T : ConditionOperationDescription
        {
            entity.SetField("hasSavingThrow", value);
            return entity;
        }

        public static T SetOperation<T>(this T entity, ConditionOperationDescription.ConditionOperation value)
            where T : ConditionOperationDescription
        {
            entity.Operation = value;
            return entity;
        }

        public static T SetSaveAffinity<T>(this T entity, EffectSavingThrowType value)
            where T : ConditionOperationDescription
        {
            entity.SetField("saveAffinity", value);
            return entity;
        }

        public static T SetSaveOccurence<T>(this T entity, TurnOccurenceType value)
            where T : ConditionOperationDescription
        {
            entity.SetField("saveOccurence", value);
            return entity;
        }
    }
}
