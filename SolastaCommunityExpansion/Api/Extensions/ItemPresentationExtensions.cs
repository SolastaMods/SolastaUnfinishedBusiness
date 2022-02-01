using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using TA.AI;
using TA;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using  static  ActionDefinitions ;
using  static  TA . AI . DecisionPackageDefinition ;
using  static  TA . AI . DecisionDefinition ;
using  static  RuleDefinitions ;
using  static  BanterDefinitions ;
using  static  Gui ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(ItemPresentation))]
    public static partial class ItemPresentationExtensions
    {
        public static T SetArmorAddressableName<T>(this T entity, System.String value)
            where T : ItemPresentation
        {
            entity.SetField("armorAddressableName", value);
            return entity;
        }

        public static T SetAssetReference<T>(this T entity, UnityEngine.AddressableAssets.AssetReference value)
            where T : ItemPresentation
        {
            entity.SetField("assetReference", value);
            return entity;
        }

        public static T SetCrownVariationMask<T>(this T entity, System.Int32 value)
            where T : ItemPresentation
        {
            entity.SetField("crownVariationMask", value);
            return entity;
        }

        public static T SetCustomArmorMaterial<T>(this T entity, System.String value)
            where T : ItemPresentation
        {
            entity.SetField("customArmorMaterial", value);
            return entity;
        }

        public static T SetFemaleBodyPartBehaviours<T>(this T entity, GraphicsCharacterDefinitions.BodyPartBehaviour[] value)
            where T : ItemPresentation
        {
            entity.SetField("femaleBodyPartBehaviours", value);
            return entity;
        }

        public static T SetHasCrownVariationMask<T>(this T entity, System.Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("hasCrownVariationMask", value);
            return entity;
        }

        public static T SetMaleBodyPartBehaviours<T>(this T entity, GraphicsCharacterDefinitions.BodyPartBehaviour[] value)
            where T : ItemPresentation
        {
            entity.SetField("maleBodyPartBehaviours", value);
            return entity;
        }

        public static T SetOverrideSubtype<T>(this T entity, System.String value)
            where T : ItemPresentation
        {
            entity.SetField("overrideSubtype", value);
            return entity;
        }

        public static T SetSameBehavioursForMaleAndFemale<T>(this T entity, System.Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("sameBehavioursForMaleAndFemale", value);
            return entity;
        }

        public static T SetScaleFactorWhileWielded<T>(this T entity, System.Single value)
            where T : ItemPresentation
        {
            entity.SetField("scaleFactorWhileWielded", value);
            return entity;
        }

        public static T SetSerializedVersion<T>(this T entity, System.Int32 value)
            where T : ItemPresentation
        {
            entity.SetField("serializedVersion", value);
            return entity;
        }

        public static T SetUnidentifiedDescription<T>(this T entity, System.String value)
            where T : ItemPresentation
        {
            entity.SetField("unidentifiedDescription", value);
            return entity;
        }

        public static T SetUnidentifiedTitle<T>(this T entity, System.String value)
            where T : ItemPresentation
        {
            entity.SetField("unidentifiedTitle", value);
            return entity;
        }

        public static T SetUseArmorAddressableName<T>(this T entity, System.Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("useArmorAddressableName", value);
            return entity;
        }

        public static T SetUseCustomArmorMaterial<T>(this T entity, System.Boolean value)
            where T : ItemPresentation
        {
            entity.SetField("useCustomArmorMaterial", value);
            return entity;
        }
    }
}