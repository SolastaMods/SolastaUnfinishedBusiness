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
    [TargetType(typeof(SpellListDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class SpellListDefinitionExtensions
    {
        public static T AddSpellsByLevel<T>(this T entity, params SpellListDefinition.SpellsByLevelDuplet[] value)
            where T : SpellListDefinition
        {
            AddSpellsByLevel(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSpellsByLevel<T>(this T entity, IEnumerable<SpellListDefinition.SpellsByLevelDuplet> value)
            where T : SpellListDefinition
        {
            entity.SpellsByLevel.AddRange(value);
            return entity;
        }

        public static T ClearSpellsByLevel<T>(this T entity)
            where T : SpellListDefinition
        {
            entity.SpellsByLevel.Clear();
            return entity;
        }

        public static T SetHasCantrips<T>(this T entity, System.Boolean value)
            where T : SpellListDefinition
        {
            entity.SetField("hasCantrips", value);
            return entity;
        }

        public static T SetMaxSpellLevel<T>(this T entity, System.Int32 value)
            where T : SpellListDefinition
        {
            entity.SetField("maxSpellLevel", value);
            return entity;
        }

        public static T SetSpellsByLevel<T>(this T entity, params SpellListDefinition.SpellsByLevelDuplet[] value)
            where T : SpellListDefinition
        {
            SetSpellsByLevel(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSpellsByLevel<T>(this T entity, IEnumerable<SpellListDefinition.SpellsByLevelDuplet> value)
            where T : SpellListDefinition
        {
            entity.SpellsByLevel.SetRange(value);
            return entity;
        }
    }
}