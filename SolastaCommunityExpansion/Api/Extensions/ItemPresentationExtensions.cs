using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using SolastaModApi.Infrastructure;
using UnityEngine.AddressableAssets;

namespace SolastaModApi.Extensions
{
    /// <summary>
    ///     This helper extensions class was automatically generated.
    ///     If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ItemPresentation))]
    [GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static class ItemPresentationExtensions
    {
        public static T AddItemFlags<T>(this T entity, params ItemFlagDefinition[] value)
            where T : ItemPresentation
        {
            AddItemFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddItemFlags<T>(this T entity, IEnumerable<ItemFlagDefinition> value)
            where T : ItemPresentation
        {
            entity.ItemFlags.AddRange(value);
            return entity;
        }

        public static T ClearItemFlags<T>(this T entity)
            where T : ItemPresentation
        {
            entity.ItemFlags.Clear();
            return entity;
        }

        public static ItemPresentation Copy(this ItemPresentation entity)
        {
            return new ItemPresentation(entity);
        }

        public static T SetArmorAddressableName<T>(this T entity, String value)
            where T : ItemPresentation
        {
            entity.SetField("armorAddressableName", value);
            return entity;
        }

        public static T SetAssetReference<T>(this T entity, AssetReference value)
            where T : ItemPresentation
        {
            entity.SetField("assetReference", value);
            return entity;
        }

        public static T SetCrownVariationMask<T>(this T entity, Int32 value)
            where T : ItemPresentation
        {
            entity.SetField("crownVariationMask", value);
            return entity;
        }

        public static T SetCustomArmorMaterial<T>(this T entity, String value)
            where T : ItemPresentation
        {
            entity.SetField("customArmorMaterial", value);
            return entity;
        }

        public static T SetFemaleBodyPartBehaviours<T>(this T entity,
            GraphicsCharacterDefinitions.BodyPartBehaviour[] value)
            where T : ItemPresentation
        {
            entity.SetField("femaleBodyPartBehaviours", value);
            return entity;
        }

        public static T SetHasCrownVariationMask<T>(this T entity, Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("hasCrownVariationMask", value);
            return entity;
        }

        public static T SetItemFlags<T>(this T entity, params ItemFlagDefinition[] value)
            where T : ItemPresentation
        {
            SetItemFlags(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetItemFlags<T>(this T entity, IEnumerable<ItemFlagDefinition> value)
            where T : ItemPresentation
        {
            entity.ItemFlags.SetRange(value);
            return entity;
        }

        public static T SetMaleBodyPartBehaviours<T>(this T entity,
            GraphicsCharacterDefinitions.BodyPartBehaviour[] value)
            where T : ItemPresentation
        {
            entity.SetField("maleBodyPartBehaviours", value);
            return entity;
        }

        public static T SetOverrideSubtype<T>(this T entity, String value)
            where T : ItemPresentation
        {
            entity.SetField("overrideSubtype", value);
            return entity;
        }

        public static T SetSameBehavioursForMaleAndFemale<T>(this T entity, Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("sameBehavioursForMaleAndFemale", value);
            return entity;
        }

        public static T SetScaleFactorWhileWielded<T>(this T entity, Single value)
            where T : ItemPresentation
        {
            entity.SetField("scaleFactorWhileWielded", value);
            return entity;
        }

        public static T SetSerializedVersion<T>(this T entity, Int32 value)
            where T : ItemPresentation
        {
            entity.SetField("serializedVersion", value);
            return entity;
        }

        public static T SetUnidentifiedDescription<T>(this T entity, String value)
            where T : ItemPresentation
        {
            entity.SetField("unidentifiedDescription", value);
            return entity;
        }

        public static T SetUnidentifiedTitle<T>(this T entity, String value)
            where T : ItemPresentation
        {
            entity.SetField("unidentifiedTitle", value);
            return entity;
        }

        public static T SetUseArmorAddressableName<T>(this T entity, Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("useArmorAddressableName", value);
            return entity;
        }

        public static T SetUseCustomArmorMaterial<T>(this T entity, Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("useCustomArmorMaterial", value);
            return entity;
        }
    }
}
