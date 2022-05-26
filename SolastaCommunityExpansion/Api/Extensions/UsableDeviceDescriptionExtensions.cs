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
    [TargetType(typeof(UsableDeviceDescription))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class UsableDeviceDescriptionExtensions
    {
        public static T AddDeviceFunctions<T>(this T entity, params DeviceFunctionDescription[] value)
            where T : UsableDeviceDescription
        {
            AddDeviceFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddDeviceFunctions<T>(this T entity, IEnumerable<DeviceFunctionDescription> value)
            where T : UsableDeviceDescription
        {
            entity.DeviceFunctions.AddRange(value);
            return entity;
        }

        public static T AddUsableDeviceTags<T>(this T entity, params System.String[] value)
            where T : UsableDeviceDescription
        {
            AddUsableDeviceTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddUsableDeviceTags<T>(this T entity, IEnumerable<System.String> value)
            where T : UsableDeviceDescription
        {
            entity.UsableDeviceTags.AddRange(value);
            return entity;
        }

        public static T ClearDeviceFunctions<T>(this T entity)
            where T : UsableDeviceDescription
        {
            entity.DeviceFunctions.Clear();
            return entity;
        }

        public static T ClearUsableDeviceTags<T>(this T entity)
            where T : UsableDeviceDescription
        {
            entity.UsableDeviceTags.Clear();
            return entity;
        }

        public static UsableDeviceDescription Copy(this UsableDeviceDescription entity)
        {
            return new UsableDeviceDescription(entity);
        }

        public static T SetChargesCapital<T>(this T entity, EquipmentDefinitions.ItemChargesCapital value)
            where T : UsableDeviceDescription
        {
            entity.SetField("chargesCapital", value);
            return entity;
        }

        public static T SetChargesCapitalBonus<T>(this T entity, System.Int32 value)
            where T : UsableDeviceDescription
        {
            entity.SetField("chargesCapitalBonus", value);
            return entity;
        }

        public static T SetChargesCapitalDie<T>(this T entity, DieType value)
            where T : UsableDeviceDescription
        {
            entity.SetField("chargesCapitalDie", value);
            return entity;
        }

        public static T SetChargesCapitalNumber<T>(this T entity, System.Int32 value)
            where T : UsableDeviceDescription
        {
            entity.SetField("chargesCapitalNumber", value);
            return entity;
        }

        public static T SetDeviceFunctions<T>(this T entity, params DeviceFunctionDescription[] value)
            where T : UsableDeviceDescription
        {
            SetDeviceFunctions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetDeviceFunctions<T>(this T entity, IEnumerable<DeviceFunctionDescription> value)
            where T : UsableDeviceDescription
        {
            entity.DeviceFunctions.SetRange(value);
            return entity;
        }

        public static T SetMagicAttackBonus<T>(this T entity, System.Int32 value)
            where T : UsableDeviceDescription
        {
            entity.SetField("magicAttackBonus", value);
            return entity;
        }

        public static T SetOnUseParticle<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : UsableDeviceDescription
        {
            entity.SetField("onUseParticle", value);
            return entity;
        }

        public static T SetOutOfChargesConsequence<T>(this T entity, EquipmentDefinitions.ItemOutOfCharges value)
            where T : UsableDeviceDescription
        {
            entity.SetField("outOfChargesConsequence", value);
            return entity;
        }

        public static T SetRechargeBonus<T>(this T entity, System.Int32 value)
            where T : UsableDeviceDescription
        {
            entity.SetField("rechargeBonus", value);
            return entity;
        }

        public static T SetRechargeDie<T>(this T entity, DieType value)
            where T : UsableDeviceDescription
        {
            entity.SetField("rechargeDie", value);
            return entity;
        }

        public static T SetRechargeNumber<T>(this T entity, System.Int32 value)
            where T : UsableDeviceDescription
        {
            entity.SetField("rechargeNumber", value);
            return entity;
        }

        public static T SetRechargeRate<T>(this T entity, RechargeRate value)
            where T : UsableDeviceDescription
        {
            entity.SetField("rechargeRate", value);
            return entity;
        }

        public static T SetSaveDC<T>(this T entity, System.Int32 value)
            where T : UsableDeviceDescription
        {
            entity.SetField("saveDC", value);
            return entity;
        }

        public static T SetUsableDeviceTags<T>(this T entity, params System.String[] value)
            where T : UsableDeviceDescription
        {
            SetUsableDeviceTags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetUsableDeviceTags<T>(this T entity, IEnumerable<System.String> value)
            where T : UsableDeviceDescription
        {
            entity.UsableDeviceTags.SetRange(value);
            return entity;
        }

        public static T SetUsage<T>(this T entity, EquipmentDefinitions.ItemUsage value)
            where T : UsableDeviceDescription
        {
            entity.SetField("usage", value);
            return entity;
        }
    }
}
