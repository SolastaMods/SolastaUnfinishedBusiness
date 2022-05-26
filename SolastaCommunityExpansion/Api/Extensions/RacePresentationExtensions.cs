using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using TA;
using UnityEngine;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(RacePresentation))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class RacePresentationExtensions
    {
        public static T AddFemaleFaceShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddFemaleFaceShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFemaleFaceShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.FemaleFaceShapeOptions.AddRange(value);
            return entity;
        }

        public static T AddFemaleHairShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddFemaleHairShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFemaleHairShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.FemaleHairShapeOptions.AddRange(value);
            return entity;
        }

        public static T AddFemaleNameOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddFemaleNameOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddFemaleNameOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.FemaleNameOptions.AddRange(value);
            return entity;
        }

        public static T AddMaleBeardShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddMaleBeardShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMaleBeardShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleBeardShapeOptions.AddRange(value);
            return entity;
        }

        public static T AddMaleFaceShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddMaleFaceShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMaleFaceShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleFaceShapeOptions.AddRange(value);
            return entity;
        }

        public static T AddMaleHairShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddMaleHairShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMaleHairShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleHairShapeOptions.AddRange(value);
            return entity;
        }

        public static T AddMaleNameOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddMaleNameOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddMaleNameOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleNameOptions.AddRange(value);
            return entity;
        }

        public static T AddOriginOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddOriginOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddOriginOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.OriginOptions.AddRange(value);
            return entity;
        }

        public static T AddSurNameOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            AddSurNameOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddSurNameOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.SurNameOptions.AddRange(value);
            return entity;
        }

        public static T ClearFemaleFaceShapeOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.FemaleFaceShapeOptions.Clear();
            return entity;
        }

        public static T ClearFemaleHairShapeOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.FemaleHairShapeOptions.Clear();
            return entity;
        }

        public static T ClearFemaleNameOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.FemaleNameOptions.Clear();
            return entity;
        }

        public static T ClearMaleBeardShapeOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.MaleBeardShapeOptions.Clear();
            return entity;
        }

        public static T ClearMaleFaceShapeOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.MaleFaceShapeOptions.Clear();
            return entity;
        }

        public static T ClearMaleHairShapeOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.MaleHairShapeOptions.Clear();
            return entity;
        }

        public static T ClearMaleNameOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.MaleNameOptions.Clear();
            return entity;
        }

        public static T ClearOriginOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.OriginOptions.Clear();
            return entity;
        }

        public static T ClearSurNameOptions<T>(this T entity)
            where T : RacePresentation
        {
            entity.SurNameOptions.Clear();
            return entity;
        }

        public static T SetBodyAssetPrefix<T>(this T entity, String value)
            where T : RacePresentation
        {
            entity.SetField("bodyAssetPrefix", value);
            return entity;
        }

        public static T SetCanModifyMusculature<T>(this T entity, Boolean value)
            where T : RacePresentation
        {
            entity.SetField("canModifyMusculature", value);
            return entity;
        }

        public static T SetEquipmentLayoutPath<T>(this T entity, String value)
            where T : RacePresentation
        {
            entity.SetField("equipmentLayoutPath", value);
            return entity;
        }

        public static T SetFemaleFaceShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetFemaleFaceShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFemaleFaceShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.FemaleFaceShapeOptions.SetRange(value);
            return entity;
        }

        public static T SetFemaleHairShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetFemaleHairShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFemaleHairShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.FemaleHairShapeOptions.SetRange(value);
            return entity;
        }

        public static T SetFemaleNameOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetFemaleNameOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetFemaleNameOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.FemaleNameOptions.SetRange(value);
            return entity;
        }

        public static T SetFemaleVoiceDefinition<T>(this T entity, String value)
            where T : RacePresentation
        {
            entity.SetField("femaleVoiceDefinition", value);
            return entity;
        }

        public static T SetHasSurName<T>(this T entity, Boolean value)
            where T : RacePresentation
        {
            entity.SetField("hasSurName", value);
            return entity;
        }

        public static T SetMaleBeardShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetMaleBeardShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMaleBeardShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleBeardShapeOptions.SetRange(value);
            return entity;
        }

        public static T SetMaleFaceShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetMaleFaceShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMaleFaceShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleFaceShapeOptions.SetRange(value);
            return entity;
        }

        public static T SetMaleHairShapeOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetMaleHairShapeOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMaleHairShapeOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleHairShapeOptions.SetRange(value);
            return entity;
        }

        public static T SetMaleNameOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetMaleNameOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetMaleNameOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.MaleNameOptions.SetRange(value);
            return entity;
        }

        public static T SetMaleVoiceDefinition<T>(this T entity, String value)
            where T : RacePresentation
        {
            entity.SetField("maleVoiceDefinition", value);
            return entity;
        }

        public static T SetMorphotypeAssetPrefix<T>(this T entity, String value)
            where T : RacePresentation
        {
            entity.SetField("morphotypeAssetPrefix", value);
            return entity;
        }

        public static T SetNeedBeard<T>(this T entity, Boolean value)
            where T : RacePresentation
        {
            entity.SetField("needBeard", value);
            return entity;
        }

        public static T SetOriginOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetOriginOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetOriginOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.OriginOptions.SetRange(value);
            return entity;
        }

        public static T SetPortraitShieldOffset<T>(this T entity, Vector3 value)
            where T : RacePresentation
        {
            entity.SetField("portraitShieldOffset", value);
            return entity;
        }

        public static T SetPreferedHairColors<T>(this T entity, RangedInt value)
            where T : RacePresentation
        {
            entity.SetField("preferedHairColors", value);
            return entity;
        }

        public static T SetPreferedSkinColors<T>(this T entity, RangedInt value)
            where T : RacePresentation
        {
            entity.SetField("preferedSkinColors", value);
            return entity;
        }

        public static T SetSurNameOptions<T>(this T entity, params String[] value)
            where T : RacePresentation
        {
            SetSurNameOptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetSurNameOptions<T>(this T entity, IEnumerable<String> value)
            where T : RacePresentation
        {
            entity.SurNameOptions.SetRange(value);
            return entity;
        }

        public static T SetSurNameTitle<T>(this T entity, String value)
            where T : RacePresentation
        {
            entity.SetField("surNameTitle", value);
            return entity;
        }
    }
}
