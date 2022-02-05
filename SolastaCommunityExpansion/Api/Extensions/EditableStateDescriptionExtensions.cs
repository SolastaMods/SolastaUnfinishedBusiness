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
    [TargetType(typeof(EditableStateDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EditableStateDescriptionExtensions
    {
        [Obsolete("Use content of OutputDescriptions instead")]
        public static T SetChildrenStates<T>(this T entity, EditableStateDescription[] value)
            where T : EditableStateDescription
        {
            entity.ChildrenStates = value;
            return entity;
        }

        [Obsolete("Use content of OutputDescriptions instead")]
        public static T SetChildrenStatesIndexes<T>(this T entity, System.Int32[] value)
            where T : EditableStateDescription
        {
            entity.ChildrenStatesIndexes = value;
            return entity;
        }

        public static T SetEditionColor<T>(this T entity, UnityEngine.Color value)
            where T : EditableStateDescription
        {
            entity.EditionColor = value;
            return entity;
        }

        public static T SetEditionPosition<T>(this T entity, UnityEngine.Vector2 value)
            where T : EditableStateDescription
        {
            entity.EditionPosition = value;
            return entity;
        }

        public static T SetIndex<T>(this T entity, System.Int32 value)
            where T : EditableStateDescription
        {
            entity.SetField("<Index>k__BackingField", value);
            return entity;
        }
    }
}
