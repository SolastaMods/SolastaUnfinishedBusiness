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
    [TargetType(typeof(EditableStateDescription)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EditableStateDescriptionExtensions
    {
        public static T AddOutputDescriptions<T>(this T entity,  params  EditableStateOutputDescription [ ]  value)
            where T : EditableStateDescription
        {
            AddOutputDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddOutputDescriptions<T>(this T entity, IEnumerable<EditableStateOutputDescription> value)
            where T : EditableStateDescription
        {
            entity.OutputDescriptions.AddRange(value);
            return entity;
        }

        public static T ClearOutputDescriptions<T>(this T entity)
            where T : EditableStateDescription
        {
            entity.OutputDescriptions.Clear();
            return entity;
        }

        public static T SetChildrenStates<T>(this T entity, EditableStateDescription[] value)
            where T : EditableStateDescription
        {
            entity.SetField("childrenStates", value);
            return entity;
        }

        public static T SetChildrenStatesIndexes<T>(this T entity, System.Int32[] value)
            where T : EditableStateDescription
        {
            entity.SetField("childrenStatesIndexes", value);
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

        public static T SetOutputDescriptions<T>(this T entity,  params  EditableStateOutputDescription [ ]  value)
            where T : EditableStateDescription
        {
            SetOutputDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetOutputDescriptions<T>(this T entity, IEnumerable<EditableStateOutputDescription> value)
            where T : EditableStateDescription
        {
            entity.OutputDescriptions.SetRange(value);
            return entity;
        }
    }
}