using System;
using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(EffectParticleParameters))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class EffectParticleParametersExtensions
    {
        public static EffectParticleParameters Copy(this EffectParticleParameters entity)
        {
            var copy = new EffectParticleParameters();
            copy.Copy(entity);
            return copy;
        }

        public static T SetActiveEffectCellEndParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("activeEffectCellEndParticleReference", value);
            return entity;
        }

        public static T SetActiveEffectCellParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("activeEffectCellParticleReference", value);
            return entity;
        }

        public static T SetActiveEffectCellStartParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("activeEffectCellStartParticleReference", value);
            return entity;
        }

        public static T SetActiveEffectImpactParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("activeEffectImpactParticleReference", value);
            return entity;
        }

        public static T SetActiveEffectSurfaceEndParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("activeEffectSurfaceEndParticleReference", value);
            return entity;
        }

        public static T SetActiveEffectSurfaceParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("activeEffectSurfaceParticleReference", value);
            return entity;
        }

        public static T SetActiveEffectSurfaceStartParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("activeEffectSurfaceStartParticleReference", value);
            return entity;
        }

        public static T SetApplyEmissionColorOnWeapons<T>(this T entity, Boolean value)
            where T : EffectParticleParameters
        {
            entity.SetField("applyEmissionColorOnWeapons", value);
            return entity;
        }

        public static T SetBeforeImpactParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("beforeImpactParticleReference", value);
            return entity;
        }

        public static T SetCasterParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("casterParticleReference", value);
            return entity;
        }

        public static T SetCasterQuickSpellParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("casterQuickSpellParticleReference", value);
            return entity;
        }

        public static T SetCasterSelfParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("casterSelfParticleReference", value);
            return entity;
        }

        public static T SetConditionEndParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("conditionEndParticleReference", value);
            return entity;
        }

        public static T SetConditionParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("conditionParticleReference", value);
            return entity;
        }

        public static T SetConditionStartParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("conditionStartParticleReference", value);
            return entity;
        }

        public static T SetEffectParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("effectParticleReference", value);
            return entity;
        }

        public static T SetEffectSubTargetParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("effectSubTargetParticleReference", value);
            return entity;
        }

        public static T SetEmissionColor<T>(this T entity, Color value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissionColor", value);
            return entity;
        }

        public static T SetEmissionColorFadeInDuration<T>(this T entity, Single value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissionColorFadeInDuration", value);
            return entity;
        }

        public static T SetEmissionColorFadeOutDuration<T>(this T entity, Single value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissionColorFadeOutDuration", value);
            return entity;
        }

        public static T SetEmissiveBorderCellEndParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissiveBorderCellEndParticleReference", value);
            return entity;
        }

        public static T SetEmissiveBorderCellParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissiveBorderCellParticleReference", value);
            return entity;
        }

        public static T SetEmissiveBorderCellStartParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissiveBorderCellStartParticleReference", value);
            return entity;
        }

        public static T SetEmissiveBorderSurfaceEndParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissiveBorderSurfaceEndParticleReference", value);
            return entity;
        }

        public static T SetEmissiveBorderSurfaceParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissiveBorderSurfaceParticleReference", value);
            return entity;
        }

        public static T SetEmissiveBorderSurfaceStartParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("emissiveBorderSurfaceStartParticleReference", value);
            return entity;
        }

        public static T SetImpactParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("impactParticleReference", value);
            return entity;
        }

        public static T SetTargetParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("targetParticleReference", value);
            return entity;
        }

        public static T SetZoneParticleReference<T>(this T entity, AssetReference value)
            where T : EffectParticleParameters
        {
            entity.SetField("zoneParticleReference", value);
            return entity;
        }
    }
}
