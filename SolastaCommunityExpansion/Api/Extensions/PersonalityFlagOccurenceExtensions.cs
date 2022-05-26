using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(PersonalityFlagOccurence))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class PersonalityFlagOccurenceExtensions
    {
        public static PersonalityFlagOccurence Copy(this PersonalityFlagOccurence entity)
        {
            return new PersonalityFlagOccurence(entity);
        }

        public static T SetPersonalityFlag<T>(this T entity, System.String value)
            where T : PersonalityFlagOccurence
        {
            entity.SetField("personalityFlag", value);
            return entity;
        }

        public static T SetWeight<T>(this T entity, System.Int32 value)
            where T : PersonalityFlagOccurence
        {
            entity.SetField("weight", value);
            return entity;
        }
    }
}
