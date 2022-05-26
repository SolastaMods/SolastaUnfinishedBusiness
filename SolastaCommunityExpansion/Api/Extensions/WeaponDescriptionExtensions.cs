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
    [TargetType(typeof(WeaponDescription))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class WeaponDescriptionExtensions
    {
        public static T AddWeaponTags<T>(this T entity, params System.String[] value)
            where T : WeaponDescription
        {
            AddWeaponTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddWeaponTags<T>(this T entity, IEnumerable<System.String> value)
            where T : WeaponDescription
        {
            entity.WeaponTags.AddRange(value);
            return entity;
        }

        public static T ClearWeaponTags<T>(this T entity)
            where T : WeaponDescription
        {
            entity.WeaponTags.Clear();
            return entity;
        }

        public static WeaponDescription Copy(this WeaponDescription entity)
        {
            return new WeaponDescription(entity);
        }

        public static T SetAmmunitionType<T>(this T entity, System.String value)
            where T : WeaponDescription
        {
            entity.SetField("ammunitionType", value);
            return entity;
        }

        public static T SetCloseRange<T>(this T entity, System.Int32 value)
            where T : WeaponDescription
        {
            entity.SetField("closeRange", value);
            return entity;
        }

        public static T SetEffectDescription<T>(this T entity, EffectDescription value)
            where T : WeaponDescription
        {
            entity.EffectDescription = value;
            return entity;
        }

        public static T SetMaxRange<T>(this T entity, System.Int32 value)
            where T : WeaponDescription
        {
            entity.SetField("maxRange", value);
            return entity;
        }

        public static T SetReachRange<T>(this T entity, System.Int32 value)
            where T : WeaponDescription
        {
            entity.SetField("reachRange", value);
            return entity;
        }

        public static T SetWeaponTags<T>(this T entity, params System.String[] value)
            where T : WeaponDescription
        {
            SetWeaponTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetWeaponTags<T>(this T entity, IEnumerable<System.String> value)
            where T : WeaponDescription
        {
            entity.WeaponTags.SetRange(value);
            return entity;
        }

        public static T SetWeaponType<T>(this T entity, System.String value)
            where T : WeaponDescription
        {
            entity.SetField("weaponType", value);
            return entity;
        }
    }
}
