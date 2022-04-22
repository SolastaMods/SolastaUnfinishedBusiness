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
    [TargetType(typeof(EditableGraphDefinition)), GeneratedCode("Community Expansion Extension Generator", "1.0.0")]
    public static partial class EditableGraphDefinitionExtensions
    {
        public static T AddAllStateDescriptions<T>(this T entity,  params  EditableStateDescription [ ]  value)
            where T : EditableGraphDefinition
        {
            AddAllStateDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T AddAllStateDescriptions<T>(this T entity, IEnumerable<EditableStateDescription> value)
            where T : EditableGraphDefinition
        {
            entity.AllStateDescriptions.AddRange(value);
            return entity;
        }

        public static T ClearAllStateDescriptions<T>(this T entity)
            where T : EditableGraphDefinition
        {
            entity.AllStateDescriptions.Clear();
            return entity;
        }

        public static T SetAllStateDescriptions<T>(this T entity,  params  EditableStateDescription [ ]  value)
            where T : EditableGraphDefinition
        {
            SetAllStateDescriptions(entity, value.AsEnumerable());
            return entity;
        }

        public static T SetAllStateDescriptions<T>(this T entity, IEnumerable<EditableStateDescription> value)
            where T : EditableGraphDefinition
        {
            entity.AllStateDescriptions.SetRange(value);
            return entity;
        }

        public static T SetRootState<T>(this T entity, EditableStateDescription value)
            where T : EditableGraphDefinition
        {
            entity.RootState = value;
            return entity;
        }

        public static T SetRootStateIndex<T>(this T entity, System.Int32 value)
            where T : EditableGraphDefinition
        {
            entity.RootStateIndex = value;
            return entity;
        }

        public static T SetStartPosition<T>(this T entity, UnityEngine.Vector2 value)
            where T : EditableGraphDefinition
        {
            entity.StartPosition = value;
            return entity;
        }
    }
}