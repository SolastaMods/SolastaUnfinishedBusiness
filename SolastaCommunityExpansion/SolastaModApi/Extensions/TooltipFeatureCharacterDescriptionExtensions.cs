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
    [TargetType(typeof(TooltipFeatureCharacterDescription))]
    public static partial class TooltipFeatureCharacterDescriptionExtensions
    {
        public static T SetActionLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureCharacterDescription
        {
            entity.SetField("actionLabel", value);
            return entity;
        }

        public static T SetCoverFullImage<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureCharacterDescription
        {
            entity.SetField("coverFullImage", value);
            return entity;
        }

        public static T SetCoverGroup<T>(this T entity, UnityEngine.RectTransform value)
            where T : TooltipFeatureCharacterDescription
        {
            entity.SetField("coverGroup", value);
            return entity;
        }

        public static T SetCoverHalfImage<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureCharacterDescription
        {
            entity.SetField("coverHalfImage", value);
            return entity;
        }

        public static T SetCoverLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureCharacterDescription
        {
            entity.SetField("coverLabel", value);
            return entity;
        }

        public static T SetCoverThreeQuarterImage<T>(this T entity, UnityEngine.UI.Image value)
            where T : TooltipFeatureCharacterDescription
        {
            entity.SetField("coverThreeQuarterImage", value);
            return entity;
        }

        public static T SetHeaderLabel<T>(this T entity, GuiLabel value)
            where T : TooltipFeatureCharacterDescription
        {
            entity.SetField("headerLabel", value);
            return entity;
        }
    }
}