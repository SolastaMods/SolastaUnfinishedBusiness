using SolastaModApi.Infrastructure;
using AK.Wwise;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;
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
using  static  GadgetDefinitions ;
using  static  BestiaryDefinitions ;
using  static  CursorDefinitions ;
using  static  AnimationDefinitions ;
using  static  FeatureDefinitionAutoPreparedSpells ;
using  static  FeatureDefinitionCraftingAffinity ;
using  static  CharacterClassDefinition ;
using  static  CreditsGroupDefinition ;
using  static  SoundbanksDefinition ;
using  static  CampaignDefinition ;
using  static  GraphicsCharacterDefinitions ;
using  static  GameCampaignDefinitions ;
using  static  FeatureDefinitionAbilityCheckAffinity ;
using  static  TooltipDefinitions ;
using  static  BaseBlueprint ;
using  static  MorphotypeElementDefinition ;

namespace SolastaModApi.Extensions
{
    /// <summary>
    /// This helper extensions class was automatically generated.
    /// If you find a problem please report at https://github.com/SolastaMods/SolastaModApi/issues.
    /// </summary>
    [TargetType(typeof(EditableStateOutputDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EditableStateOutputDescriptionExtensions
    {
        public static T AddChildrenStates<T>(this T entity,  params  EditableStateDescription [ ]  value)
            where T : EditableStateOutputDescription
        {
            AddChildrenStates(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddChildrenStates<T>(this T entity, IEnumerable<EditableStateDescription> value)
            where T : EditableStateOutputDescription
        {
            entity.ChildrenStates.AddRange(value);
            return entity;
        }

        public static T AddChildrenStatesIndexes<T>(this T entity,  params  System . Int32 [ ]  value)
            where T : EditableStateOutputDescription
        {
            AddChildrenStatesIndexes(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddChildrenStatesIndexes<T>(this T entity, IEnumerable<System.Int32> value)
            where T : EditableStateOutputDescription
        {
            entity.ChildrenStatesIndexes.AddRange(value);
            return entity;
        }

        public static T ClearChildrenStates<T>(this T entity)
            where T : EditableStateOutputDescription
        {
            entity.ChildrenStates.Clear();
            return entity;
        }

        public static T ClearChildrenStatesIndexes<T>(this T entity)
            where T : EditableStateOutputDescription
        {
            entity.ChildrenStatesIndexes.Clear();
            return entity;
        }

        public static System.Collections.Generic.List<System.Int32> GetChildrenStateIndexes<T>(this T entity)
            where T : EditableStateOutputDescription
        {
            return entity.GetField<System.Collections.Generic.List<System.Int32>>("childrenStateIndexes");
        }

        public static T SetChildrenStates<T>(this T entity,  params  EditableStateDescription [ ]  value)
            where T : EditableStateOutputDescription
        {
            SetChildrenStates(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetChildrenStates<T>(this T entity, IEnumerable<EditableStateDescription> value)
            where T : EditableStateOutputDescription
        {
            entity.ChildrenStates.SetRange(value);
            return entity;
        }

        public static T SetChildrenStatesIndexes<T>(this T entity,  params  System . Int32 [ ]  value)
            where T : EditableStateOutputDescription
        {
            SetChildrenStatesIndexes(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetChildrenStatesIndexes<T>(this T entity, IEnumerable<System.Int32> value)
            where T : EditableStateOutputDescription
        {
            entity.ChildrenStatesIndexes.SetRange(value);
            return entity;
        }
    }
}