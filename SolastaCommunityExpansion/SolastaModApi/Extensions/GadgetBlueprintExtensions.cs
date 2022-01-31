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
    [TargetType(typeof(GadgetBlueprint))]
    public static partial class GadgetBlueprintExtensions
    {
        public static T SetCampaignOnly<T>(this T entity, System.Boolean value)
            where T : GadgetBlueprint
        {
            entity.SetField("campaignOnly", value);
            return entity;
        }

        public static T SetCanBeActivated<T>(this T entity, System.Boolean value)
            where T : GadgetBlueprint
        {
            entity.SetField("canBeActivated", value);
            return entity;
        }

        public static T SetCustomizableDimensions<T>(this T entity, System.Boolean value)
            where T : GadgetBlueprint
        {
            entity.SetField("customizableDimensions", value);
            return entity;
        }

        public static T SetMaxCustomizableDimensions<T>(this T entity, UnityEngine.Vector2Int value)
            where T : GadgetBlueprint
        {
            entity.SetField("maxCustomizableDimensions", value);
            return entity;
        }

        public static T SetMinCustomizableDimensions<T>(this T entity, UnityEngine.Vector2Int value)
            where T : GadgetBlueprint
        {
            entity.SetField("minCustomizableDimensions", value);
            return entity;
        }
    }
}