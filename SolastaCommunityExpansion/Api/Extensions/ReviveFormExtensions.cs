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
    [TargetType(typeof(ReviveForm)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class ReviveFormExtensions
    {
        public static T AddRemovedConditions<T>(this T entity, params ConditionDefinition[] value)
            where T : ReviveForm
        {
            AddRemovedConditions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddRemovedConditions<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : ReviveForm
        {
            entity.RemovedConditions.AddRange(value);
            return entity;
        }

        public static T ClearRemovedConditions<T>(this T entity)
            where T : ReviveForm
        {
            entity.RemovedConditions.Clear();
            return entity;
        }

        public static ReviveForm Copy(this ReviveForm entity)
        {
            var copy = new ReviveForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetMaxSecondsSinceDeath<T>(this T entity, System.Int32 value)
            where T : ReviveForm
        {
            entity.SetField("maxSecondsSinceDeath", value);
            return entity;
        }

        public static T SetRemovedConditions<T>(this T entity, params ConditionDefinition[] value)
            where T : ReviveForm
        {
            SetRemovedConditions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetRemovedConditions<T>(this T entity, IEnumerable<ConditionDefinition> value)
            where T : ReviveForm
        {
            entity.RemovedConditions.SetRange(value);
            return entity;
        }

        public static T SetReviveHitPoints<T>(this T entity, RuleDefinitions.ReviveHitPoints value)
            where T : ReviveForm
        {
            entity.SetField("reviveHitPoints", value);
            return entity;
        }
    }
}