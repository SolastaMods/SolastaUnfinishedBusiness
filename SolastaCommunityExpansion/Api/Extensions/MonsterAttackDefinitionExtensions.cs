using System.CodeDom.Compiler;
using SolastaModApi.Infrastructure;
using static RuleDefinitions;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(MonsterAttackDefinition))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class MonsterAttackDefinitionExtensions
    {
        public static MonsterAttackDefinition Copy(this MonsterAttackDefinition entity)
        {
            var copy = new MonsterAttackDefinition();
            copy.Copy(entity);
            return copy;
        }

        public static T SetActionType<T>(this T entity, ActionDefinitions.ActionType value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("actionType", value);
            return entity;
        }

        public static T SetAfterChargeOnly<T>(this T entity, System.Boolean value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("afterChargeOnly", value);
            return entity;
        }

        public static T SetAnimationTag<T>(this T entity, System.String value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("animationTag", value);
            return entity;
        }

        public static T SetChargeEndParticle<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("chargeEndParticle", value);
            return entity;
        }

        public static T SetChargeLoopParticle<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("chargeLoopParticle", value);
            return entity;
        }

        public static T SetChargePrepareParticle<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("chargePrepareParticle", value);
            return entity;
        }

        public static T SetChargeStartParticle<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("chargeStartParticle", value);
            return entity;
        }

        public static T SetCloseRange<T>(this T entity, System.Int32 value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("closeRange", value);
            return entity;
        }

        public static T SetDisplayedInEditor<T>(this T entity, System.Boolean value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("displayedInEditor", value);
            return entity;
        }

        public static T SetEffectDescription<T>(this T entity, EffectDescription value)
            where T : MonsterAttackDefinition
        {
            entity.EffectDescription = value;
            return entity;
        }

        public static T SetItemDefinitionMainHand<T>(this T entity, ItemDefinition value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("itemDefinitionMainHand", value);
            return entity;
        }

        public static T SetItemDefinitionOffHand<T>(this T entity, ItemDefinition value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("itemDefinitionOffHand", value);
            return entity;
        }

        public static T SetLimitedUse<T>(this T entity, System.Boolean value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("limitedUse", value);
            return entity;
        }

        public static T SetMagical<T>(this T entity, System.Boolean value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("magical", value);
            return entity;
        }

        public static T SetMaxRange<T>(this T entity, System.Int32 value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("maxRange", value);
            return entity;
        }

        public static T SetMaxUses<T>(this T entity, System.Int32 value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("maxUses", value);
            return entity;
        }

        public static T SetMethodName<T>(this T entity, System.String value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("methodName", value);
            return entity;
        }

        public static T SetProjectile<T>(this T entity, System.String value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("projectile", value);
            return entity;
        }

        public static T SetProjectileBone<T>(this T entity, AnimationDefinitions.BoneType value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("projectileBone", value);
            return entity;
        }

        public static T SetProximity<T>(this T entity, AttackProximity value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("proximity", value);
            return entity;
        }

        public static T SetReachRange<T>(this T entity, System.Int32 value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("reachRange", value);
            return entity;
        }

        public static T SetSoundEffectOnHitDescription<T>(this T entity, SoundEffectOnHitDescription value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("soundEffectOnHitDescription", value);
            return entity;
        }

        public static T SetToHitBonus<T>(this T entity, System.Int32 value)
            where T : MonsterAttackDefinition
        {
            entity.ToHitBonus = value;
            return entity;
        }

        public static T SetUseAnimationTag<T>(this T entity, System.Boolean value)
            where T : MonsterAttackDefinition
        {
            entity.SetField("useAnimationTag", value);
            return entity;
        }
    }
}
