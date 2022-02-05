using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Text;
using System.CodeDom.Compiler;
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
    [TargetType(typeof(CharacterInteractionDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class CharacterInteractionDefinitionExtensions
    {
        public static T SetActionId<T>(this T entity, ActionDefinitions.Id value)
            where T : CharacterInteractionDefinition
        {
            entity.SetField("actionId", value);
            return entity;
        }

        public static T SetAnimation<T>(this T entity, AnimationDefinitions.ManipulationType value)
            where T : CharacterInteractionDefinition
        {
            entity.SetField("animation", value);
            return entity;
        }

        public static T SetCursorType<T>(this T entity, CursorDefinitions.CursorType value)
            where T : CharacterInteractionDefinition
        {
            entity.SetField("cursorType", value);
            return entity;
        }
    }
}