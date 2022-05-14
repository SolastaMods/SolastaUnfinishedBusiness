using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static CharacterClassDefinition;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(HeroEquipmentOption)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class HeroEquipmentOptionExtensions
    {
        public static T SetDefaultChoice<T>(this T entity, System.String value)
            where T : HeroEquipmentOption
        {
            entity.SetField("defaultChoice", value);
            return entity;
        }

        public static T SetItemDefinition<T>(this T entity, ItemDefinition value)
            where T : HeroEquipmentOption
        {
            entity.SetField("itemReference", value);
            return entity;
        }

        public static T SetNumber<T>(this T entity, System.Int32 value)
            where T : HeroEquipmentOption
        {
            entity.SetField("number", value);
            return entity;
        }

        public static T SetOptionType<T>(this T entity, System.String value)
            where T : HeroEquipmentOption
        {
            entity.SetField("optionType", value);
            return entity;
        }
    }
}