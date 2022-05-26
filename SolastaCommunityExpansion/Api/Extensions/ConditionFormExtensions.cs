using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ConditionForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ConditionFormExtensions
    {
        public static T AddConditionsList<T>(this T entity, params ConditionDefinition[] value)
            where T : ConditionForm
        {
            AddConditionsList(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddConditionsList<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : ConditionForm
        {
            entity.ConditionsList.AddRange(value);
            return entity;
        }

        public static T ClearConditionsList<T>(this T entity)
            where T : ConditionForm
        {
            entity.ConditionsList.Clear();
            return entity;
        }

        public static ConditionForm Copy(this ConditionForm entity)
        {
            var copy = new ConditionForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetApplyToSelf<T>(this T entity, System.Boolean value)
            where T : ConditionForm
        {
            entity.SetField("applyToSelf", value);
            return entity;
        }

        public static T SetConditionDefinition<T>(this T entity, ConditionDefinition value)
            where T : ConditionForm
        {
            entity.ConditionDefinition = value;
            return entity;
        }

        public static T SetConditionDefinitionName<T>(this T entity, System.String value)
            where T : ConditionForm
        {
            entity.SetField("conditionDefinitionName", value);
            return entity;
        }

        public static T SetConditionsList<T>(this T entity, params ConditionDefinition[] value)
            where T : ConditionForm
        {
            SetConditionsList(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetConditionsList<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : ConditionForm
        {
            entity.ConditionsList.SetRange(value);
            return entity;
        }

        public static T SetForceOnSelf<T>(this T entity, System.Boolean value)
            where T : ConditionForm
        {
            entity.SetField("forceOnSelf", value);
            return entity;
        }

        public static T SetOperation<T>(this T entity, ConditionForm.ConditionOperation value)
            where T : ConditionForm
        {
            entity.Operation = value;
            return entity;
        }
    }
}