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
    [TargetType(typeof(BaseDefinition))]
    public static partial class BaseDefinitionExtensions
    {
        public static T SetCachedName<T>(this T entity, System.String value)
            where T : BaseDefinition
        {
            entity.SetField("cachedName", value);
            return entity;
        }

        public static T SetContentCopyright<T>(this T entity, BaseDefinition.Copyright value)
            where T : BaseDefinition
        {
            entity.SetField("contentCopyright", value);
            return entity;
        }

        public static T SetContentPack<T>(this T entity, GamingPlatformDefinitions.ContentPack value)
            where T : BaseDefinition
        {
            entity.SetField("contentPack", value);
            return entity;
        }

        public static T SetGuid<T>(this T entity, System.String value)
            where T : BaseDefinition
        {
            entity.SetField("guid", value);
            return entity;
        }

        public static T SetGuiPresentation<T>(this T entity, GuiPresentation value)
            where T : BaseDefinition
        {
            entity.GuiPresentation = value;
            return entity;
        }
    }
}