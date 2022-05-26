using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(MotionForm))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class MotionFormExtensions
    {
        public static MotionForm Copy(this MotionForm entity)
        {
            var copy = new MotionForm();
            copy.Copy(entity);
            return copy;
        }

        public static T SetDistance<T>(this T entity, System.Int32 value)
            where T : MotionForm
        {
            entity.SetField("distance", value);
            return entity;
        }

        public static T SetType<T>(this T entity, MotionForm.MotionType value)
            where T : MotionForm
        {
            entity.SetField("type", value);
            return entity;
        }
    }
}
